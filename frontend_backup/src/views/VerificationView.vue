<template>
  <div class="auth-container">
    <div class="auth-card">
      <div class="header">
        <h1>🔐 Verifikasi OTP</h1>
        <p>Verifikasi untuk: <strong>{{ identifier }}</strong></p>
      </div>

      <div class="form-group">
        <input 
          v-model="identifier" 
          type="text" 
          placeholder="Email atau No HP..." 
          class="std-input"
          :disabled="isLocked"
        />
      </div>

      <div class="resend-container">
         <button @click="requestOtp" :disabled="loading || countdown > 0" class="btn-resend">
           {{ countdown > 0 ? `Tunggu ${countdown}s` : 'Kirim Kode OTP Baru' }}
         </button>
      </div>

      <div class="form-group">
        <input 
          v-model="otpCode" 
          type="text" 
          maxlength="6" 
          placeholder="Masukan 6 digit kode" 
          class="otp-input"
        />
      </div>

      <div v-if="error" class="error-message">{{ error }}</div>
      <div v-if="success" class="success-message">{{ success }}</div>

      <button @click="submitOtp" :disabled="loading" class="btn-primary">
        {{ loading ? 'Memproses...' : 'Verifikasi Sekarang' }}
      </button>

      <p class="link-text" @click="backToLogin">Kembali ke Login</p>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import apiClient from '@/services/apiServices'

const route = useRoute()
const router = useRouter()

const identifier = ref('')
const otpCode = ref('')
const loading = ref(false)
const error = ref('')
const success = ref('')
const isLocked = ref(false)
const countdown = ref(0)

onMounted(() => {
  const queryId = route.query.identifier
  if (queryId) {
    identifier.value = queryId
    isLocked.value = true
  }
})

const requestOtp = async () => {
  if (!identifier.value) {
    error.value = "Identifier wajib diisi!"
    return
  }

  try {
    loading.value = true
    error.value = ''
    
    const response = await apiClient.post('/Auth/resend-otp', { identifier: identifier.value, password: "" })

    if (response.debugOtp) {
        alert("SMS SIMULASI: Kode OTP Anda adalah " + response.debugOtp)
        console.log("OTP Code:", response.debugOtp)
    }

    success.value = "Kode OTP telah dikirim!"
    startCountdown(60)

  } catch (err) {
    error.value = err.response?.data?.message || "Gagal mengirim OTP."
  } finally {
    loading.value = false
  }
}

const submitOtp = async () => {
  if (!identifier.value) {
    error.value = "Email wajib diisi!"
    return
  }
  if (otpCode.value.length < 6) {
    error.value = "Masukkan 6 digit kode OTP!"
    return
  }

  try {
    loading.value = true
    error.value = ''
    
    await apiClient.post('/Auth/verify-otp', {
      identifier: identifier.value.trim(), 
      otpCode: otpCode.value.trim()
    })

    success.value = "Verifikasi Berhasil! Mengalihkan..."
    setTimeout(() => router.push('/login'), 2000)

  } catch (err) {
    error.value = err.response?.data?.message || "Gagal verifikasi."
  } finally {
    loading.value = false
  }
}

const startCountdown = (seconds) => {
  countdown.value = seconds
  const interval = setInterval(() => {
    countdown.value--
    if (countdown.value <= 0) clearInterval(interval)
  }, 1000)
}

const backToLogin = () => router.push('/login')
</script>

<style scoped>
.resend-container {
  margin-bottom: 15px;
  text-align: right;
}
.btn-resend {
  background: none;
  border: none;
  color: #667eea;
  font-size: 13px;
  cursor: pointer;
  text-decoration: underline;
}
.btn-resend:disabled {
  color: #999;
  cursor: not-allowed;
  text-decoration: none;
}
.auth-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 20px;
}
.auth-card {
  background: white;
  padding: 40px;
  border-radius: 16px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
  width: 100%;
  max-width: 400px;
  text-align: center;
}
.header h1 { margin-bottom: 10px; color: #333; }
.form-group { margin-bottom: 15px; text-align: left; }
label { font-size: 14px; font-weight: bold; color: #555; display: block; margin-bottom: 5px; }

/* Input Biasa */
.std-input {
  width: 100%; padding: 10px; border: 1px solid #ddd; border-radius: 8px; font-size: 14px;
}

/* Input OTP Besar */
.otp-input {
  width: 100%; font-size: 24px; letter-spacing: 8px; text-align: center;
  padding: 10px; border: 2px solid #ddd; border-radius: 8px;
}
.otp-input:focus, .std-input:focus { border-color: #667eea; outline: none; }

.btn-primary {
  width: 100%; padding: 14px; background: #667eea; color: white;
  border: none; border-radius: 8px; font-weight: bold; cursor: pointer; margin-top: 10px;
}
.btn-primary:hover { background: #5a67d8; }
.btn-primary:disabled { background: #ccc; }

.error-message { color: #c53030; background: #fff5f5; padding: 10px; border-radius: 6px; margin-bottom: 15px; }
.success-message { color: #276749; background: #f0fff4; padding: 10px; border-radius: 6px; margin-bottom: 15px; }
.link-text { margin-top: 20px; cursor: pointer; color: #666; font-size: 14px; }
.link-text:hover { text-decoration: underline; color: #667eea; }
</style>