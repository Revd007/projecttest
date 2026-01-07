
![.NET](https://img.shields.io/badge/.NET%208-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Vue.js](https://img.shields.io/badge/Vue.js-35495E?style=for-the-badge&logo=vuedotjs&logoColor=4FC08D)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-Secure-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white)

## 🌟 Key Features

### 🔐 Backend Security (.NET 8)
- **Cryptographic JWT:** Tokens are signed using **HMAC-SHA512** for maximum security against tampering.
- **Secure Password Hashing:** Implemented **BCrypt** with automatic salting.
- **Rate Limiting:** Protects against Brute Force & DDoS attacks (max 5 requests/minute).
- **Input Sanitization:** **HtmlSanitizer** integration to prevent XSS (Cross-Site Scripting) attacks via input fields.
- **Strict CORS:** API access is locked strictly to the frontend origin.
- **Secure Configuration:** Database credentials and SMTP secrets are stored in **.NET User Secrets** (not hardcoded).
- **HTTP Security Headers:** Anti-MIME Sniffing, Anti-Clickjacking (`X-Frame-Options`), and XSS Protection headers.

### 👤 User Management & Verification
- **Multi-Channel Login:** Login via **Username**, **Email**, or **Phone Number**.
- **Mandatory Verification:**
  - Unverified users are blocked (403 Forbidden) and auto-redirected to the verification page.
  - **Email OTP:** Integrated with **Gmail SMTP** (MailKit) to send HTML-formatted OTP codes.
  - **Phone OTP:** Logic ready (currently simulated via Server Console for cost-efficiency).
- **Token Management:** Database handles OTP expiry (10 minutes validity) to prevent replay attacks.

### 💻 Frontend (Vue 3 + Vite)
- **State Management:** **Pinia** for centralized auth state handling.
- **Axios Interceptors:** Automatic token injection (`Authorization: Bearer ...`) and global error handling (auto-logout on 401).
- **Client-Side Security:**
  - **Custom CAPTCHA** implementation on login.
  - **Content Security Policy (CSP)** meta tags implemented.
- **UX Improvements:** Auto-detection of verification status, countdown timers for OTP resend, and password visibility toggles.

---

## 🏗️ Architecture

The backend follows **Clean Architecture** principles to ensure maintainability:
- **Controllers:** Handle HTTP Requests/Responses.
- **Services:** Business logic separation (`EmailService`, `TokenService`).
- **DTOs:** Data Transfer Objects to decouple internal models from API contracts.
- **Models:** Entity Framework Core models mapping to PostgreSQL.

---

## 🚀 Getting Started

Follow these steps to run the project locally.

### Prerequisites
- .NET SDK 8.0
- Node.js & npm
- PostgreSQL Database

### 1. Database Setup
Execute the following SQL script in your PostgreSQL database:
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
);```

### 2. Backend Setup (.NET)
Navigate to the backend folder.
Configure Secrets: (Do not skip this, as appsettings.json does not contain sensitive keys).


