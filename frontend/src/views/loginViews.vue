<template>
  <div class="auth-container">
    <div class="auth-card">
      <h1>Login</h1>
      <form @submit.prevent="handleLogin">
        <div class="form-group">
          <label for="username">Username</label>
          <input
            id="username"
            v-model="formData.username"
            type="text"
            placeholder="Enter username"
            required
          />
        </div>

        <div class="form-group">
          <label for="password">Password</label>
          <input
            id="password"
            v-model="formData.password"
            type="password"
            placeholder="Enter password"
            required
          />
        </div>

        <div class="form-group captcha-group">
          <label>Security Check</label>
          <div class="captcha-box">
            <div class="captcha-preview" @click="generateCaptcha">
              {{ captchaCode }}
            </div>
            <button type="button" @click="generateCaptcha" class="btn-refresh" title="Refresh Captcha">
              🔄
            </button>
          </div>
          <input
            v-model="captchaInput"
            type="text"
            placeholder="Type the code above"
            class="captcha-input"
            required
          />
        </div>
        <div v-if="error" class="error-message">
          {{ error }}
        </div>

        <!-- Login Button -->
        <button type="submit" :disabled="loading" class="btn-primary">
          {{ loading ? 'Loading...' : 'Login' }}
        </button>
      </form>

      <p class="link-text">
        Don't have an account?
        <router-link to="/register">Register here</router-link>
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
  border-radius: 12px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.1);
  width: 100%;
  max-width: 400px;
}
h1 {
  margin-bottom: 30px;
  color: #333;
  text-align: center;
}
.form-group {
  margin-bottom: 20px;
}
label {
  display: block;
  margin-bottom: 8px;
  color: #555;
  font-weight: 500;
}
input {
  width: 100%;
  padding: 12px;
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  font-size: 14px;
  transition: border-color 0.3s;
}
input:focus {
  outline: none;
  border-color: #667eea;
}

.captcha-box {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 10px;
}

.captcha-preview {
  flex: 1;
  background-color: #eee;
  padding: 10px;
  text-align: center;
  font-size: 24px;
  font-weight: bold;
  font-family: 'Courier New', Courier, monospace;
  letter-spacing: 5px;
  color: #444;
  border-radius: 6px;
  background-image: linear-gradient(45deg, #f3f3f3 25%, #e8e8e8 25%, #e8e8e8 50%, #f3f3f3 50%, #f3f3f3 75%, #e8e8e8 75%, #e8e8e8 100%); /* Pattern garis halus */
  background-size: 10px 10px;
  user-select: none;
  cursor: pointer;
}

.btn-refresh {
  background: none;
  border: 1px solid #ddd;
  padding: 10px;
  border-radius: 6px;
  cursor: pointer;
  font-size: 18px;
  transition: 0.2s;
}
.btn-refresh:hover {
  background: #f0f0f0;
}

.btn-primary {
  width: 100%;
  padding: 12px;
  background: #667eea;
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.3s;
}
.btn-primary:hover:not(:disabled) {
  background: #5568d3;
}
.btn-primary:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}
.error-message {
  background: #fee;
  color: #c33;
  padding: 12px;
  border-radius: 6px;
  margin-bottom: 20px;
  font-size: 14px;
}
.link-text {
  text-align: center;
  margin-top: 20px;
  color: #666;
}
.link-text a {
  color: #667eea;
  text-decoration: none;
  font-weight: 600;
}
.link-text a:hover {
  text-decoration: underline;
}
</style>