<template>
  <div class="container">
    <div class="card profile-card">
      <!-- Header Profile -->
      <div class="profile-header">
        <img :src="avatarUrl" alt="Avatar" class="profile-avatar" />
        <div class="profile-title">
          <h2>{{ user.username || 'Loading...' }}</h2>
          <span class="badge badge-role">Authorized User</span>
        </div>
      </div>

      <div v-if="loading" class="loading-state">
        <span class="spinner"></span> Memuat data profil...
      </div>

      <div v-else class="profile-details">
        
        <!-- EMAIL SECTION -->
        <div class="detail-item">
          <div class="detail-label">
            <span class="icon">📧</span> Email Address
          </div>
          <div class="detail-content">
            <span class="value">{{ user.email }}</span>
            
            <!-- Status Badge -->
            <span v-if="user.emailVerified" class="status-badge verified">
              ✅ Verified
            </span>
            <button v-else @click="verifyNow('email', user.email)" class="btn-verify">
              ⚠️ Unverified - Verifikasi Sekarang
            </button>
          </div>
        </div>

        <!-- PHONE SECTION -->
        <div class="detail-item">
          <div class="detail-label">
            <span class="icon">📱</span> Phone Number
          </div>
          <div class="detail-content">
            <span class="value">{{ user.phoneNumber }}</span>

            <!-- Status Badge -->
            <span v-if="user.phoneVerified" class="status-badge verified">
              ✅ Verified
            </span>
            <button v-else @click="verifyNow('phone', user.phoneNumber)" class="btn-verify">
              ⚠️ Unverified - Verifikasi Sekarang
            </button>
          </div>
        </div>

        <hr class="divider" />

        <!-- ACTIONS FOOTER -->
        <div class="profile-actions">
          <button @click="$router.push('/change-password')" class="btn-secondary">
            🔒 Ganti Password
          </button>
          <button @click="logout" class="btn-danger">
            🚪 Logout
          </button>
        </div>

      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { authAPI } from '@/services/apiServices'
import { useAuthStore } from '@/stores/authStores'

const router = useRouter()
const authStore = useAuthStore()
const user = ref({})
const loading = ref(true)

// Generate Avatar Otomatis dari Username
const avatarUrl = computed(() => {
  const name = user.value.username || 'User';
  return `https://ui-avatars.com/api/?name=${name}&background=667eea&color=fff&size=128&bold=true`;
})

onMounted(async () => {
  try {
    loading.value = true
    // Request ke Backend
    user.value = await authAPI.getProfile()
  } catch (err) {
    console.error("Gagal load profile:", err)
    if(err.response?.status === 401) logout() // Auto logout jika token basi
  } finally {
    loading.value = false
  }
})

// Logic Redirect ke Halaman Verifikasi
const verifyNow = (type, identifier) => {
  // Redirect ke VerificationView dengan parameter yang sesuai
  router.push({
    path: '/verify',
    query: { 
      identifier: identifier,
      type: type // 'email' atau 'phone'
    }
  })
}

const logout = () => {
  if(confirm("Apakah Anda yakin ingin keluar?")) {
    authStore.clearAuth()
    router.push('/login')
  }
}
</script>

<style scoped>
@import "@/assets/css/dashboard.css";
</style>