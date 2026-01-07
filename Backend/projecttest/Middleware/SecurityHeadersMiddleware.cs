namespace projecttest.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Anti-MIME Sniffing (Memaksa browser baca sesuai Content-Type)
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");

            // Anti-Clickjacking (Menolak website ditampilkan di iframe orang lain)
            context.Response.Headers.Add("X-Frame-Options", "DENY");

            // XSS Protection (Untuk browser lama)
            context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");

            // Menyembunyikan info server (Biar hacker gak tau kita pake Kestrel/IIS)
            context.Response.Headers.Remove("Server");
            context.Response.Headers.Remove("X-Powered-By");

            await _next(context);
        }
    }
}