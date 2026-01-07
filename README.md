![.NET](https://img.shields.io/badge/.NET%208-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Vue.js](https://img.shields.io/badge/Vue.js-35495E?style=for-the-badge&logo=vuedotjs&logoColor=4FC08D)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-Secure-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white)

# 🔐 Secure Auth System  
**.NET 8 + Vue 3 + PostgreSQL + JWT**

---

## 🌟 Key Features

### 🔐 Backend Security (.NET 8)
- **Cryptographic JWT**  
  Token signing menggunakan **HMAC-SHA512** untuk mencegah token tampering.
- **Secure Password Hashing**  
  Password di-hash menggunakan **BCrypt** dengan auto-salt.
- **Rate Limiting**  
  Proteksi brute force & DDoS (**max 5 request / menit**).
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

- **Controllers** → HTTP request/response
- **Services** → Business logic (`EmailService`, `TokenService`)
- **DTOs** → API contract abstraction
- **Models** → EF Core entity (PostgreSQL)

---

## 🚀 Getting Started

### Prerequisites
- .NET SDK 8.0
- Node.js & npm
- PostgreSQL

---

## 1️⃣ Database Setup

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
2️⃣ Backend Setup (.NET 8)
Masuk ke folder backend lalu jalankan:

Initialize User Secrets
bash
Copy code
dotnet user-secrets init
Configure Secrets
bash
Copy code
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=YOUR_DB;Username=postgres;Password=YOUR_PASSWORD"

dotnet user-secrets set "EmailSettings:SmtpServer" "smtp.gmail.com"
dotnet user-secrets set "EmailSettings:SmtpPort" "587"
dotnet user-secrets set "EmailSettings:SenderEmail" "YOUR_EMAIL@gmail.com"
dotnet user-secrets set "EmailSettings:SenderPassword" "YOUR_APP_PASSWORD"

dotnet user-secrets set "JwtSettings:Key" "YOUR_SUPER_LONG_SECRET_KEY_MIN_64_CHARS"
dotnet user-secrets set "JwtSettings:Issuer" "MySecureApp_Server"
dotnet user-secrets set "JwtSettings:Audience" "MySecureApp_Client"
Run Server
bash
Copy code
dotnet restore
dotnet run
📍 Backend running at:
http://localhost:5113

3️⃣ Frontend Setup (Vue 3)
Masuk ke folder frontend:

bash
Copy code
npm install
npm run serve
📍 Frontend running at:
http://localhost:8080

📡 API Endpoints
Method	Endpoint	Description	Auth
POST	/api/Auth/register	Register user + send OTP	❌
POST	/api/Auth/login	Login (Email/User/Phone)	❌
POST	/api/Auth/verify-otp	Verify Email / Phone OTP	❌
POST	/api/Auth/resend-otp	Request new OTP	❌
GET	/api/Auth/check-session	Validate JWT session	✅

📸 Screenshots
(Tambahkan screenshot Login Page, OTP Email, dan Swagger UI di sini)
