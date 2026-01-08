<template>
  <div class="home-container">
    <!-- Header Section -->
    <header class="dashboard-header">
      <div class="user-info">
        <!-- Gambar Profile -->
        <img :src="userImage" alt="Profile" class="avatar" />
        <div>
          <h2>Welcome, {{ userFirstName }}!</h2>
          <small class="role-text">AUTHORIZED USER</small>
        </div>
      </div>

      <!-- Action Buttons -->
      <div class="header-actions">
        <!-- TOMBOL BARU: PROFILE -->
        <router-link to="/profile" class="btn btn-profile">
            👤 Profile
        </router-link>
        
        <button @click="handleLogout" class="btn-logout">
          Logout
        </button>
      </div>
    </header>

    <!-- Content Grid Section -->
    <main class="content-area">
      <h3>🛒 Exclusive Products (Protected Data)</h3>
      <!-- ... (Sisa kode grid biarkan sama) ... -->
      <div v-if="loading" class="loading-state">Loading data...</div>
      <div v-else class="product-grid">
          <!-- Placeholder Grid jika API produk belum ada -->
          <p>Data produk akan muncul di sini.</p>
      </div>
    </main>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/authStores'
import { authAPI } from '@/services/apiServices'

const router = useRouter()
const authStore = useAuthStore()

// State Data
const products = ref([])
const loading = ref(true)
const error = ref('')


// Computed User Data (Ambil dari Store)
const userFirstName = computed(() => authStore.user?.username || 'Guest')
const userImage = computed(() => {
  const name = authStore.user?.username || 'User';
  return `https://ui-avatars.com/api/?name=${name}&background=667eea&color=fff&bold=true`;
})

const handleLogout = () => {
  authStore.clearAuth()
  router.push('/login')
}

// Fetch Data Function
const fetchData = async () => {
  try {
    loading.value = true
    const response = await authAPI.checkSession()
    products.value = response.products
  } catch (err) {
    console.error("Fetch Error:", err)
    
    // Cek Authorization
    if (err.response && (err.response.status === 401 || err.response.status === 403)) {
      error.value = "Session expired or Unauthorized. Please login again."
      setTimeout(() => handleLogout(), 2000) 
    } else {
      error.value = "Failed to load products."
    }
  } finally {
    loading.value = false
  }
}

// Jalankan saat halaman dibuka
onMounted(() => {
  fetchData()
})
</script>

<style scoped>
@import "@/assets/css/dashboard.css";
</style>