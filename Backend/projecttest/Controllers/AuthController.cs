using BCrypt.Net;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using projecttest.Data;
using projecttest.Dtos;
using projecttest.Models;
using projecttest.Services;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;

namespace projecttest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly IUrlSignerService _urlSignerService;

        public AuthController(AppDbContext context, IConfiguration configuration, IEmailService emailService, ITokenService tokenService, IUrlSignerService urlSignerService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
            _tokenService = tokenService;
            _urlSignerService = urlSignerService;
        }

        [HttpPost("login")]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            string identifier = req.Identifier.Trim();

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Username == identifier ||
                u.Email == identifier ||
                u.PhoneNumber == identifier);

            if (user == null)
            {
                return Unauthorized(new { message = "Kombinasi User dan Password salah" });
            }

            if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Kombinasi User dan Password salah" });
            }
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            var phoneRegex = new Regex(@"^\+?[0-9]{10,15}$");

            if (emailRegex.IsMatch(identifier))
            {
                if (!user.EmailVerified)
                {
                    return StatusCode(403, new
                    {
                        message = "Email belum diverifikasi. Silakan cek email Anda.",
                        needVerification = true,
                        verificationType = "email"
                    });
                }
            }
            else if (phoneRegex.IsMatch(identifier))
            {
                if (!user.PhoneVerified)
                {
                    return StatusCode(403, new
                    {
                        message = "Nomor HP belum diverifikasi.",
                        needVerification = true,
                        verificationType = "phone"
                    });
                }
            }
            else
            {
                if (!user.EmailVerified && !user.PhoneVerified)
                {
                    return StatusCode(403, new
                    {
                        message = "Akun belum diverifikasi. Silakan verifikasi Email atau No HP Anda.",
                        needVerification = true,
                        verificationType = "generic"
                    });
                }
            }

            var jwtToken = _tokenService.CreateToken(user);

            return Ok(new LoginResponse(
                jwtToken,
                user.Username,
                user.Email,
                user.PhoneNumber
            ));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            var sanitizer = new HtmlSanitizer();
            string safeUsername = sanitizer.Sanitize(req.Username);
            string safeEmail = sanitizer.Sanitize(req.Email);
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string signature = _urlSignerService.GenerateSignature(req.Email, timestamp);
            string safeSig = System.Net.WebUtility.UrlEncode(signature);
            string verificationLink = $"http://localhost:8080/verify?identifier={req.Email}&type=email&ts={timestamp}&sig={safeSig}";

            if (await _context.Users.AnyAsync(u => u.Username == safeUsername))
                return BadRequest(new { message = "Username sudah digunakan" });

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(req.Password);
            var otp = new Random().Next(100000, 999999).ToString();

            var newUser = new UsersModels
            {
                Username = safeUsername,
                Email = safeEmail,
                PhoneNumber = req.PhoneNumber,
                PasswordHash = passwordHash,
                EmailVerified = false,
                PhoneVerified = false,
                VerificationToken = otp,
                TokenExpiry = DateTime.UtcNow.AddMinutes(10)
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            string emailBody = $@"
        <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
            <h2 style='color: #667eea;'>Verifikasi Akun ProjectTest</h2>
            <p>Halo <strong>{req.Username}</strong>,</p>
            <p>Terima kasih telah mendaftar. Gunakan kode OTP di bawah ini untuk memverifikasi akun Anda:</p>
            
            <h1 style='background: #f4f4f4; padding: 10px; text-align: center; letter-spacing: 5px; border-radius: 5px;'>{otp}</h1>
            
            <p>Atau klik tombol di bawah ini untuk langsung verifikasi (Berlaku 30 menit):</p>
            <div style='text-align: center; margin: 20px 0;'>
                <a href='{verificationLink}' style='background-color: #667eea; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; font-weight: bold;'>Verifikasi Sekarang</a>
            </div>

            <p style='color: #888; font-size: 12px;'>Kode ini berlaku selama 10 menit.</p>
        </div>";

            await _emailService.SendEmailAsync(req.Email, "Kode Verifikasi OTP", emailBody);

            return Ok(new { message = "Registrasi berhasil! Cek Email untuk verifikasi." });
        }

        [HttpPost("verify-otp")]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest req)
        {
            string identifier = req.Identifier.Trim();

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email == identifier ||
                u.PhoneNumber == identifier);

            if (user == null) return BadRequest(new { message = "User tidak ditemukan" });

            if (user.VerificationToken != req.OtpCode)
                return BadRequest(new { message = "Kode OTP Salah!" });

            if (user.TokenExpiry < DateTime.UtcNow)
                return BadRequest(new { message = "Kode OTP Kadaluarsa. Silakan request ulang." });

            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            var phoneRegex = new Regex(@"^\+?[0-9]{10,15}$");

            if (emailRegex.IsMatch(identifier))
            {
                if (user.Email != identifier) return BadRequest(new { message = "Format Email tidak cocok dengan data." });

                if (user.EmailVerified) return BadRequest(new { message = "Email ini sudah terverifikasi." });

                user.EmailVerified = true; // HANYA UPDATE EMAIL
            }
            else if (phoneRegex.IsMatch(identifier))
            {
                if (user.PhoneNumber != identifier) return BadRequest(new { message = "No HP tidak cocok dengan data." });

                if (user.PhoneVerified) return BadRequest(new { message = "No HP ini sudah terverifikasi." });

                user.PhoneVerified = true; // HANYA UPDATE HP
            }
            else
            {
                return BadRequest(new { message = "Format Identifier tidak valid (Bukan Email atau No HP)." });
            }

            user.VerificationToken = null;
            user.TokenExpiry = null;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Verifikasi Berhasil! Silakan Login." });
        }

        [HttpPost("resend-otp")]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<IActionResult> ResendOtp([FromBody] LoginRequest req)
        {
            string identifier = req.Identifier.Trim();

            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email == identifier ||
                u.PhoneNumber == identifier);

            if (user == null) return BadRequest(new { message = "User tidak ditemukan" });

            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            var phoneRegex = new Regex(@"^\+?[0-9]{10,15}$");

            string targetType = "";

            if (emailRegex.IsMatch(identifier))
            {
                if (user.EmailVerified) return BadRequest(new { message = "Email sudah terverifikasi." });
                targetType = "email";
            }
            else if (phoneRegex.IsMatch(identifier))
            {
                if (user.PhoneVerified) return BadRequest(new { message = "No HP sudah terverifikasi." });
                targetType = "phone";
            }
            else
            {
                return BadRequest(new { message = "Format tidak valid. Masukkan Email atau No HP yang benar." });
            }

            var otp = new Random().Next(100000, 999999).ToString();
            user.VerificationToken = otp;
            user.TokenExpiry = DateTime.UtcNow.AddMinutes(10);

            await _context.SaveChangesAsync();

            if (targetType == "email")
            {
                // Kirim ke EMAIL
                string emailBody = $"<h1>Request OTP Baru</h1><p>Kode: <b>{otp}</b></p>";
                await _emailService.SendEmailAsync(user.Email, "Kode Verifikasi Baru", emailBody);
                return Ok(new { message = "Kode OTP baru dikirim ke Email." });
            }
            else
            {
                Console.WriteLine($"[SMS GATEWAY] Kirim ke {user.PhoneNumber}: {otp}");

                return Ok(new
                {
                    message = "Kode OTP baru dikirim ke No Handphone.",
                    debugOtp = otp
                });
            }
        }

        [HttpPost("verify-signed")]
        public async Task<IActionResult> VerifySigned([FromBody] VerifySignedRequest req)
        {
            if (!_urlSignerService.ValidateSignature(req.Identifier, req.Timestamp, req.Signature))
            {
                return BadRequest(new { message = "Link Verifikasi Invalid atau Sudah Kadaluarsa (Expired)." });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == req.Identifier);
            if (user == null) return BadRequest(new { message = "Users Tidak Ditemukan" });
            if (user.VerificationToken != req.OtpCode) return BadRequest(new { message = "Kode OTP Salah!" });

            user.EmailVerified = true;
            user.VerificationToken = null;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Verifikasi Berhasil via Link Aman" });
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var username = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null) return NotFound();

            return Ok(new
            {
                user.Username,
                user.Email,
                user.PhoneNumber,
                user.EmailVerified,
                user.PhoneVerified
            });
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req)
        {
            if (req.NewPassword != req.ConfirmNewPassword)
                return BadRequest(new {message = "Konfirmasi password baru tidak cocok"});

            var username = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null) return Unauthorized();

            if (!BCrypt.Net.BCrypt.Verify(req.OldPassword, user.PasswordHash))
                return BadRequest(new { message = "Password Lama Salah" });
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.NewPassword);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Passwod telah berhasil diubah" });
        }

        [HttpGet("check-session")]
        [Authorize]
        public IActionResult CheckSession()
        {
            return Ok(new
            {
                message = "Token Valid! Anda terautentikasi.",
                user = User.Identity?.Name
            });
        }
    }
}
