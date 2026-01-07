![.NET](https://img.shields.io/badge/.NET%208-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Vue.js](https://img.shields.io/badge/Vue.js-35495E?style=for-the-badge&logo=vuedotjs&logoColor=4FC08D)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-Secure-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white)

# 🔐 Secure Auth System

**.NET 8 + Vue 3 + PostgreSQL + JWT**

Sistem autentikasi modern dengan keamanan tingkat enterprise, verifikasi multi-channel, dan arsitektur yang scalable.

---

## 🌟 Key Features

### 🔐 Backend Security (.NET 8)

- **Cryptographic JWT**  
  Token signing menggunakan **HMAC-SHA512** untuk mencegah token tampering.

- **Secure Password Hashing**  
  Password di-hash menggunakan **BCrypt** dengan auto-salt.

- **Rate Limiting**  
  Proteksi brute force & DDoS (**max 5 request/menit**).

- **Input Sanitization**  
  Menggunakan **HtmlSanitizer** untuk mencegah XSS.

- **Strict CORS Policy**  
  API hanya bisa diakses dari origin frontend.

- **Secure Configuration**  
  Credential DB & SMTP disimpan di **.NET User Secrets**.

- **HTTP Security Headers**
  - `X-Content-Type-Options`
  - `X-Frame-Options`
  - `X-XSS-Protection`

---

### 👤 User Management & Verification

- **Multi-Channel Login**
  - Username
  - Email
  - Phone Number

- **Mandatory Verification**
  - User belum terverifikasi → **403 Forbidden**
  - Auto-redirect ke halaman verifikasi

- **OTP Verification**
  - **Email OTP** via Gmail SMTP (MailKit)
  - **Phone OTP** (simulasi via Server Console)

- **OTP Security**
  - Expiry **10 menit**
  - Disimpan di database (anti replay attack)

---

### 💻 Frontend (Vue 3 + Vite)

- **State Management:** Pinia

- **Axios Interceptors**
  - Auto inject `Authorization: Bearer <token>`
  - Auto logout saat `401 Unauthorized`

- **Client-Side Security**
  - Custom CAPTCHA pada login
  - Content Security Policy (CSP)

- **UX Enhancements**
  - Auto-detect verification status
  - OTP resend countdown
  - Password visibility toggle

---

## 🏗️ Architecture

Mengikuti prinsip **Clean Architecture**:

- **Controllers** → HTTP request/response handling
- **Services** → Business logic (`EmailService`, `TokenService`)
- **DTOs** → API contract abstraction
- **Models** → EF Core entity mapping (PostgreSQL)

---

## 🚀 Getting Started

### Prerequisites

- .NET SDK 8.0
- Node.js & npm
- PostgreSQL

---

## 📦 Installation

### 1️⃣ Database Setup

Jalankan SQL berikut di PostgreSQL:

```sql
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    phone_number VARCHAR(20) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    email_verified BOOLEAN DEFAULT FALSE,
    phone_verified BOOLEAN DEFAULT FALSE,
    verification_token VARCHAR(6),
    token_expiry TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

---

### 2️⃣ Backend Setup (.NET 8)

Masuk ke folder backend lalu jalankan:

#### Initialize User Secrets

```bash
dotnet user-secrets init
```

#### Configure Secrets

```bash
# Database Connection
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=YOUR_DB;Username=postgres;Password=YOUR_PASSWORD"

# Email Settings
dotnet user-secrets set "EmailSettings:SmtpServer" "smtp.gmail.com"
dotnet user-secrets set "EmailSettings:SmtpPort" "587"
dotnet user-secrets set "EmailSettings:SenderEmail" "YOUR_EMAIL@gmail.com"
dotnet user-secrets set "EmailSettings:SenderPassword" "YOUR_APP_PASSWORD"

# JWT Settings
dotnet user-secrets set "JwtSettings:Key" "YOUR_SUPER_LONG_SECRET_KEY_MIN_64_CHARS"
dotnet user-secrets set "JwtSettings:Issuer" "MySecureApp_Server"
dotnet user-secrets set "JwtSettings:Audience" "MySecureApp_Client"
```

#### Run Server

```bash
dotnet restore
dotnet run
```

📍 **Backend running at:** `http://localhost:5113`

---

### 3️⃣ Frontend Setup (Vue 3)

Masuk ke folder frontend:

```bash
npm install
npm run serve
```

📍 **Frontend running at:** `http://localhost:8080`

---

## 📡 API Endpoints

| Method | Endpoint                    | Description                  | Auth Required |
|--------|----------------------------|------------------------------|---------------|
| POST   | `/api/Auth/register`       | Register user + send OTP     | ❌            |
| POST   | `/api/Auth/login`          | Login (Email/User/Phone)     | ❌            |
| POST   | `/api/Auth/verify-otp`     | Verify Email / Phone OTP     | ❌            |
| POST   | `/api/Auth/resend-otp`     | Request new OTP              | ❌            |
| GET    | `/api/Auth/check-session`  | Validate JWT session         | ✅            |

---

## 🔒 Security Best Practices

### Backend

1. **Never commit secrets** - Always use User Secrets or environment variables
2. **Use HTTPS** in production
3. **Enable rate limiting** for all public endpoints
4. **Validate all inputs** server-side
5. **Log security events** (failed logins, token validation errors)

### Frontend

1. **Store JWT in httpOnly cookies** (recommended) or secure localStorage
2. **Implement CSRF protection** if using cookies
3. **Sanitize user inputs** before rendering
4. **Use Content Security Policy** headers
5. **Implement proper error handling** without leaking sensitive info

---

## 📸 Screenshots


> <img width="416" height="398" alt="Screenshot 2026-01-07 145754" src="https://github.com/user-attachments/assets/dfb801c1-a319-4075-903a-1d6a809b0053" />



---

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

---

## 📄 License

This project is licensed under the MIT License.

---

## 📞 Support

Jika ada pertanyaan atau issues, silakan buat issue di repository ini.

---

**Built with ❤️ using .NET 8, Vue 3, and PostgreSQL**
