import axios from "axios";

const API_BASE_URL = 'http://localhost:5113/api';

const apiClient = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json'
    }
});

apiClient.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('token');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => Promise.reject(error)
);

apiClient.interceptors.response.use(
    (response) => {
        return response;
    },
    (error) => {
        if (error.response && error.response.status === 401) {
            console.warn("Token Invalid atau Expired! Logout otomatis...");
            
            localStorage.removeItem('token');
            localStorage.removeItem('user');

            window.location.href = '/login';
        }
        return Promise.reject(error);
    }
);

export const authAPI = {
    login: async (identifier, password) => {
        const response = await apiClient.post('/auth/login', { identifier, password });
        return response.data;
    },
    register: async (userData) => {
        const response = await apiClient.post('/auth/register', userData);
        return response.data;
    },
    getProducts: async () => {
        const response = await apiClient.get('/auth/products'); 
        return response.data;
    },
    checkSession: async () => {
        const response = await apiClient.get('/Auth/check-session'); 
        return response.data;
    },
    verify: async () => {
        const response = await apiClient.get('/auth/verify');
        return response.data;
    }
}

export default apiClient;