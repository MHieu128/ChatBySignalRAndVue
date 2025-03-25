<template>
  <div class="app-container">
    <ChatVue></ChatVue>
  </div>
</template>

<script>
import ChatVue from './components/Chat.vue'

export default {
  name: 'App',
  components: {
    ChatVue
  }
}
</script>

<style>
:root {
  --primary-color: #5b21b6;
  --primary-light: #7c3aed;
  --primary-dark: #4c1d95;
  --secondary-color: #059669;
  --secondary-light: #10b981;
  --secondary-dark: #047857;
  --text-color: #1f2937;
  --text-muted: #6b7280;
  --bg-color: #f3f4f6;
  --card-bg: #ffffff;
  --card-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05);
  --border-color: #e5e7eb;
  --gradient-start: #8b5cf6;
  --gradient-end: #6366f1;
  --danger-color: #ef4444;
  --success-color: #10b981;
}

* {
  box-sizing: border-box;
  margin: 0;
  padding: 0;
}

body {
  background-color: var(--bg-color);
  color: var(--text-color);
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  line-height: 1.5;
}

#app {
  display: flex;
  justify-content: center;
  min-height: 100vh;
  padding: 20px;
  background-image: linear-gradient(135deg, rgba(139, 92, 246, 0.05) 0%, rgba(99, 102, 241, 0.05) 100%);
}

.app-container {
  max-width: 1200px;
  width: 100%;
}

button {
  cursor: pointer;
  background-color: var(--primary-color);
  color: white;
  border: none;
  border-radius: 8px;
  padding: 10px 18px;
  font-weight: 600;
  font-size: 14px;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

button:hover {
  background-color: var(--primary-dark);
  transform: translateY(-1px);
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

button:active {
  transform: translateY(0);
}

input {
  padding: 12px 16px;
  border: 1px solid var(--border-color);
  border-radius: 8px;
  font-size: 14px;
  outline: none;
  transition: all 0.2s ease;
  background-color: #fff;
  color: var(--text-color);
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.05);
}

input::placeholder {
  color: #9ca3af;
}

input:focus {
  border-color: var(--primary-light);
  box-shadow: 0 0 0 3px rgba(124, 58, 237, 0.25);
}

/* Scrollbar styling */
::-webkit-scrollbar {
  width: 8px;
}

::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 10px;
}

::-webkit-scrollbar-thumb {
  background: #d1d5db;
  border-radius: 10px;
}

::-webkit-scrollbar-thumb:hover {
  background: #9ca3af;
}

/* Simple animations */
@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

@keyframes slideUp {
  from { transform: translateY(10px); opacity: 0; }
  to { transform: translateY(0); opacity: 1; }
}
</style>
