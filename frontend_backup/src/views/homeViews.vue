<template>
  <div class="home-container">
    <!-- Header Section -->
    <header class="dashboard-header">
      <div class="user-info">
        <img :src="userImage" alt="Profile" class="avatar" />
        <div>
          <h2>Welcome, {{ userFirstName }}!</h2>
          <small class="role-text">Authorized User</small>
        </div>
      </div>
      <button @click="handleLogout" class="btn-logout">Logout</button>
    </header>

    <!-- Content Grid Section -->
    <main class="content-area">
      <h3>🛒 Exclusive Products (Protected Data)</h3>
      
      <!-- Loading State -->
      <div v-if="loading" class="loading-state">
        Loading data from server...
      </div>

      <!-- Error State -->
      <div v-else-if="error" class="error-state">
        {{ error }}
      </div>

      <!-- Grid Data -->
      <div v-else class="product-grid">
        <div v-for="product in products" :key="product.id" class="product-card">
          <div class="img-wrapper">
            <img :src="product.thumbnail" :alt="product.title" loading="lazy" />
            <span class="badge">{{ product.category }}</span>
          </div>
          <div class="card-body">
            <h4>{{ product.title }}</h4>
            <p class="desc">{{ product.description.substring(0, 50) }}...</p>
            <div class="card-footer">
              <span class="price">${{ product.price }}</span>
              <span class="rating">⭐ {{ product.rating }}</span>
            </div>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/authStores' // Sesuaikan path store kamu
import { authAPI } from '@/services/apiServices'

const router = useRouter()
const authStore = useAuthStore()

// State Data
const products = ref([])
const loading = ref(true)
const error = ref('')

// Computed User Data (Ambil dari Store)
const userFirstName = computed(() => authStore.user?.username || 'Guest')
const userImage = computed(() => authStore.user?.image || 'https://via.placeholder.com/50')

// Logout Function
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
      // Opsional: Otomatis logout jika token basi
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
/* Layout Utama */
.home-container {
  min-height: 100vh;
  background-color: #f3f4f6;
  padding-bottom: 40px;
}

/* Header Styles */
.dashboard-header {
  background: white;
  padding: 15px 30px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  box-shadow: 0 2px 10px rgba(0,0,0,0.05);
  margin-bottom: 30px;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 15px;
}

.avatar {
  width: 50px;
  height: 50px;
  border-radius: 50%;
  border: 2px solid #667eea;
}

.role-text {
  color: #888;
  font-size: 12px;
  text-transform: uppercase;
  letter-spacing: 1px;
}

.btn-logout {
  background: #ef4444;
  color: white;
  border: none;
  padding: 8px 20px;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 600;
  transition: 0.2s;
}

.btn-logout:hover {
  background: #dc2626;
}

/* Content Area */
.content-area {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
}

h3 {
  margin-bottom: 20px;
  color: #374151;
}

/* Grid Layout */
.product-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 25px;
}

/* Card Styles */
.product-card {
  background: white;
  border-radius: 12px;
  overflow: hidden;
  box-shadow: 0 4px 6px rgba(0,0,0,0.05);
  transition: transform 0.2s;
}

.product-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 10px 15px rgba(0,0,0,0.1);
}

.img-wrapper {
  position: relative;
  height: 180px;
  background: #eee;
}

.img-wrapper img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.badge {
  position: absolute;
  top: 10px;
  right: 10px;
  background: rgba(0,0,0,0.6);
  color: white;
  padding: 4px 10px;
  border-radius: 20px;
  font-size: 11px;
  text-transform: capitalize;
}

.card-body {
  padding: 15px;
}

.card-body h4 {
  margin: 0 0 10px;
  font-size: 16px;
  color: #1f2937;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.desc {
  font-size: 13px;
  color: #6b7280;
  margin-bottom: 15px;
}

.card-footer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-weight: bold;
}

.price {
  color: #667eea;
  font-size: 18px;
}

.rating {
  font-size: 13px;
  color: #fbbf24;
}

/* States */
.loading-state, .error-state {
  text-align: center;
  padding: 40px;
  font-size: 18px;
  color: #666;
  background: white;
  border-radius: 8px;
}
.error-state {
  color: #ef4444;
  border: 1px solid #fee2e2;
}
</style>