<template>
  <div class="auth-container">
    <div class="auth-card">
      <h1>Login</h1>
      <form @submit.prevent="handleLogin">
        <div class="form-group">
          <label>Username</label>
          <input v-model="formData.username" type="text" placeholder="Enter username" required />
        </div>

        <div class="form-group">
          <label>Password</label>
          <input v-model="formData.password" type="password" placeholder="Enter password" required />
        </div>

        <!-- Captcha -->
        <div class="form-group captcha-group">
          <label>Security Check</label>
          <div class="captcha-box">
            <div class="captcha-preview" @click="generateCaptcha">{{ captchaCode }}</div>
            <button type="button" @click="generateCaptcha" class="btn-refresh" title="Refresh">🔄</button>
          </div>
          <input v-model="captchaInput" type="text" placeholder="Type the code above" required />
        </div>

        <div v-if="error" class="error-message">{{ error }}</div>

        <button type="submit" :disabled="loading" class="btn-primary">
          {{ loading ? 'Loading...' : 'Login' }}
        </button>
      </form>

      <p class="link-text">
        Don't have an account? <router-link to="/register">Register here</router-link>
      </p>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/authStores'
import { authAPI } from '@/services/apiServices'

const router = useRouter()
const authStore = useAuthStore()

const formData = ref({
  username: '',
  password: ''
})

const captchaCode = ref('')
const captchaInput = ref('')

const loading = ref(false)
const error = ref('')

const generateCaptcha = () => {
  const chars = 'ABCDEFGHJKLMNPQRSTUVWXYZ23456789'
  let result = ''
  for (let i = 0; i < 5; i++) {
    result += chars.charAt(Math.floor(Math.random() * chars.length))
  }
  captchaCode.value = result
}

onMounted(() => {
  generateCaptcha()
})

const handleLogin = async () => {
  try {
    error.value = ''
    if (captchaInput.value.toUpperCase() !== captchaCode.value) {
      error.value = "Captcha salah! Silakan coba lagi."
      generateCaptcha()
      captchaInput.value = ''
      return
    }

    loading.value = true
    
    const response = await authAPI.login(
      formData.value.username,
      formData.value.password
    )

    console.log("Isi Response API:", response)

    const token = response.accessToken || response.token; 

    if (!token) {
      throw new Error("Token tidak ditemukan di response API")
    }

    authStore.setAuth(token, { 
        username: response.username || formData.value.username,
        email: response.email,
        image: response.image 
    })
    
    router.push('/')

  } catch (err) {
    if (err.response && err.response.status === 403) {
        const data = err.response.data;
        
        if (data.needVerification) {
          router.push({
            path: '/verify',
            query: {
              type: data.verificationType,
              identifier: formData.value.identifier
            }
          });
          return;
        }
      }
    console.error(err)
    error.value = err.response?.data?.message || err.message || 'Login failed.'
    generateCaptcha()
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
@import "@/assets/css/auth.css";
</style>