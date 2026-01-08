<template>
  <div class="auth-container">
    <div class="auth-card">
      <h1>Ganti Password</h1>
      <form @submit.prevent="handleChangePassword">
        <div class="form-group">
          <label>Password Lama</label>
          <input v-model="form.oldPassword" type="password" required />
        </div>
        <div class="form-group">
          <label>Password Baru</label>
          <input v-model="form.newPassword" type="password" required />
        </div>
        <div class="form-group">
          <label>Konfirmasi Password Baru</label>
          <input v-model="form.confirmNewPassword" type="password" required />
        </div>

        <div v-if="msg" :class="{'success': isSuccess, 'error': !isSuccess}">{{ msg }}</div>

        <button type="submit" :disabled="loading" class="btn-primary">Update Password</button>
      </form>
      <p class="link-text" @click="$router.push('/')">Kembali ke Home</p>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { authAPI } from '@/services/apiServices'

const form = ref({ oldPassword: '', newPassword: '', confirmNewPassword: '' })
const msg = ref('')
const isSuccess = ref(false)
const loading = ref(false)

const handleChangePassword = async () => {
  if (form.value.newPassword !== form.value.confirmNewPassword) {
    msg.value = "Password baru tidak sama!"; isSuccess.value = false; return;
  }
  
  try {
    loading.value = true
    await authAPI.changePassword(form.value)
    msg.value = "Password berhasil diubah!"; isSuccess.value = true;
    form.value = { oldPassword: '', newPassword: '', confirmNewPassword: '' }
  } catch (err) {
    msg.value = err.response?.data?.message || "Gagal mengubah password"; isSuccess.value = false;
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.auth-container { min-height: 100vh; display: flex; align-items: center; justify-content: center; background: #f3f4f6; }
.auth-card { background: white; padding: 40px; border-radius: 12px; width: 100%; max-width: 400px; }
.form-group { margin-bottom: 15px; }
input { width: 100%; padding: 10px; border: 1px solid #ddd; border-radius: 5px; }
.btn-primary { width: 100%; padding: 12px; background: #667eea; color: white; border: none; border-radius: 5px; cursor: pointer; }
.success { color: green; margin-bottom: 10px; }
.error { color: red; margin-bottom: 10px; }
.link-text { margin-top: 15px; text-align: center; cursor: pointer; color: #666; }
</style>