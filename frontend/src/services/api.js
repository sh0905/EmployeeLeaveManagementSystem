import axios from 'axios';
 
// Backend API base URL
const API_BASE_URL = 
 'http://localhost:5195/api';
// Create axios instance with default config
const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true, // For CORS with credentials
});
 
// API service object
const api = {
  /**
   * Employee applies for leave
   * @param {Object} leave - Leave application data
   * @param {string} leave.leaveType - Type of leave (VACATION, SICK, CASUAL)
   * @param {string} leave.startDate - Start date (YYYY-MM-DD)
   * @param {string} leave.endDate - End date (YYYY-MM-DD)
   * @param {number} leave.days - Number of days
   */
  applyLeave: async (leave) => {
    try {
      const response = await apiClient.post('/leave/apply', {
        leaveType: leave.leaveType,
        startDate: leave.startDate,
        endDate: leave.endDate,
        days: leave.days,
      });
      return response.data;
    } catch (error) {
      console.error('Error applying leave:', error);
      throw error;
    }
  },
 
  /**
   * Get employee's leave requests
   */
  getMyLeaves: async () => {
    try {
      const response = await apiClient.get('/leave/my-leaves');
      return response.data;
    } catch (error) {
      console.error('Error fetching my leaves:', error);
      throw error;
    }
  },
 
  /**
   * Manager views pending leave requests
   */
  getPendingLeaves: async () => {
    try {
      const response = await apiClient.get('/leave/pending');
      return response.data;
    } catch (error) {
      console.error('Error fetching pending leaves:', error);
      throw error;
    }
  },
 
  /**
   * Manager views all leave requests (complete history)
   */
  getAllLeaves: async () => {
    try {
      const response = await apiClient.get('/leave/all');
      return response.data;
    } catch (error) {
      console.error('Error fetching all leaves:', error);
      throw error;
    }
  },
 
  /**
   * Manager approves/rejects leave
   * @param {number} id - Leave request ID
   * @param {string} status - 'APPROVED' or 'REJECTED'
   * @param {string} comments - Optional manager comments
   */
  updateLeaveStatus: async (id, status, comments = '') => {
    try {
      const endpoint = status === 'APPROVED' 
        ? `/leave/${id}/approve` 
        : `/leave/${id}/reject`;
      const response = await apiClient.put(endpoint, {
        managerComments: comments,
      });
      return response.data;
    } catch (error) {
      console.error(`Error updating leave status:`, error);
      throw error;
    }
  },
 
  /**
   * Get employee leave balance
   */
  getLeaveBalance: async () => {
    try {
      const response = await apiClient.get('/leave/balance');
      return response.data;
    } catch (error) {
      console.error('Error fetching leave balance:', error);
      throw error;
    }
  },
};
 
// Add response interceptor for global error handling
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response) {
      // Server responded with error status
      console.error('API Error Response:', error.response.data);
      const message = error.response.data?.message || 'An error occurred';
      throw new Error(message);
    } else if (error.request) {
      // Request made but no response received
      console.error('API No Response:', error.request);
      throw new Error('Cannot connect to server. Please ensure the backend is running.');
    } else {
      // Error in request setup
      console.error('API Request Error:', error.message);
      throw error;
    }
  }
);
 
export default api;