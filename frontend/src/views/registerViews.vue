<template>
  <div class="auth-container">
    <div class="auth-card">
      <div class="header">
        <h1>Register</h1>
        <p>Buat akun baru Anda</p>
      </div>
      
      <form @submit.prevent="handleRegister">
        
        <div class="form-group">
          <label>Username</label>
          <input v-model="formData.username" type="text" placeholder="Contoh: revian007" required />
        </div>

        <div class="form-group">
          <label>Email</label>
          <input v-model="formData.email" type="email" placeholder="email@anda.com" required />
        </div>

        <div class="form-group">
          <label>No. Handphone</label>
          <input v-model="formData.phoneNumber" type="text" placeholder="08xxxxxxxx" required />
        </div>

        <!-- Password dengan Icon Mata -->
        <div class="form-group">
          <label>Password</label>
          <div class="input-wrapper">
             <input 
               v-model="formData.password" 
               :type="showPassword ? 'text' : 'password'" 
               placeholder="Minimal 8 karakter" 
               required 
             />
             <span class="eye-icon" @click="showPassword = !showPassword">
               <!-- Icon SVG biar lebih rapi daripada emoji -->
               <svg v-if="showPassword" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path><circle cx="12" cy="12" r="3"></circle></svg>
               <svg v-else xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"></path><line x1="1" y1="1" x2="23" y2="23"></line></svg>
             </span>
          </div>
        </div>

        <!-- Konfirmasi Password -->
        <div class="form-group">
          <label>Ulangi Password</label>
          <input 
            v-model="confirmPassword" 
            type="password" 
            placeholder="Ketik ulang password" 
            required 
            :class="{'input-error': passwordMismatch}"
          />
          <small v-if="passwordMismatch" class="text-danger">Password tidak sama!</small>
        </div>

        <div v-if="error" class="error-message">{{ error }}</div>
        <div v-if="success" class="success-message">{{ success }}</div>

        <button type="submit" :disabled="loading" class="btn-primary">
          {{ loading ? 'Mendaftar...' : 'Register' }}
        </button>
      </form>

      <p class="link-text">
        Sudah punya akun? <router-link to="/login">Login di sini</router-link>
      </p>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { authAPI } from '@/services/apiServices'

const router = useRouter()
const loading = ref(false)
const error = ref('')
const success = ref('')
const showPassword = ref(false)

const formData = ref({
  username: '',
  email: '',
  phoneNumber: '',
  password: ''
})

const confirmPassword = ref('')

const passwordMismatch = computed(() => {
  return formData.value.password && confirmPassword.value && formData.value.password !== confirmPassword.value
})

const handleRegister = async () => {
  if (formData.value.password.length < 8) {
    error.value = "Password minimal 8 karakter!"
    return
  }
  if (formData.value.password !== confirmPassword.value) {
    error.value = "Konfirmasi password tidak cocok!"
    return
  }

  try {
    loading.value = true
    error.value = ''
    success.value = ''

    await authAPI.register(formData.value)

    success.value = "Registrasi Berhasil! Silakan cek OTP."
    setTimeout(() => router.push({path: '/verify', query: { identifier: formData.value.email}}), 2000)

  } catch (err) {
    console.error(err)
    error.value = err.response?.data?.message || 'Gagal terhubung ke server.'
  } finally {
    loading.value = false
  }
}
</script>

<!-- CUMA INI STYLE NYA SEKARANG -->
<style scoped>
@import "@/assets/css/auth.css";
</style>