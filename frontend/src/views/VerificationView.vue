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
const timestamp = ref(0)
const signature = ref('')
const identifier = ref('')
const otpCode = ref('')
const loading = ref(false)
const error = ref('')
const success = ref('')
const isLocked = ref(false)
const countdown = ref(0)

onMounted(() => {
  const q = route.query
  identifier.value = q.identifier || ''
  
  // Ambil parameter security
  if (q.ts && q.sig) {
    timestamp.value = q.ts
    signature.value = q.sig
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
  try {
    loading.value = true; error.value = ''
    
    if (timestamp.value && signature.value) {
        await apiClient.post('/Auth/verify-signed', {
            identifier: identifier.value,
            otpCode: otpCode.value,
            timestamp: parseInt(timestamp.value),
            signature: signature.value
        })
    } else {
        await apiClient.post('/Auth/verify-otp', {
            identifier: identifier.value.trim(), 
            otpCode: otpCode.value.trim()
        })
    }

    success.value = "Sukses! Mengalihkan..."
    setTimeout(() => router.push('/login'), 2000)
  } catch (err) {
    error.value = err.response?.data?.message || "Gagal."
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
@import "@/assets/css/auth.css";
</style>