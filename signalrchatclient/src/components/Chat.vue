<template>
  <div class="chat-container">
    <header class="chat-header">
      <div class="header-content">
        <h1>Real-time Chat</h1>
        <div class="connection-status" :class="{ connected: connectionEstablished }">
          {{ connectionEstablished ? 'Connected' : 'Connecting...' }}
        </div>
      </div>
    </header>

    <div class="chat-layout">
      <main class="chat-main">
        <div class="message-container" ref="messageContainer">
          <div v-if="messages.length === 0" class="no-messages">
            <div class="empty-state">
              <svg xmlns="http://www.w3.org/2000/svg" width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" class="empty-icon">
                <path d="M21 15a2 2 0 0 1-2 2H7l-4 4V5a2 2 0 0 1 2-2h14a2 2 0 0 1 2 2z"></path>
              </svg>
              <p>No messages yet. Start the conversation!</p>
            </div>
          </div>
          <transition-group name="message-transition">
            <div 
              v-for="(msg, index) in messages" 
              :key="index" 
              class="message-item"
              :class="{'my-message': msg.user === user}"
            >
              <div class="message-avatar">
                {{ msg.user.charAt(0).toUpperCase() }}
              </div>
              <div class="message-content">
                <div class="message-user">{{ msg.user }}</div>
                <div class="message-text">{{ msg.message }}</div>
                <div class="message-time">{{ formatTime(new Date()) }}</div>
              </div>
            </div>
          </transition-group>
        </div>

        <div class="message-input">
          <div v-if="!hasJoined" class="user-setup">
            <div class="welcome-message">Welcome! Please enter your name to join the chat.</div>
            <div class="input-group">
              <input 
                v-model="user" 
                placeholder="Enter your name" 
                @keyup.enter="joinChat"
                class="user-input"
              />
              <button @click="joinChat" class="join-button">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <path d="M15 3h4a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2h-4"></path>
                  <polyline points="10 17 15 12 10 7"></polyline>
                  <line x1="15" y1="12" x2="3" y2="12"></line>
                </svg>
                Join Chat
              </button>
            </div>
          </div>
          <div v-else class="message-form">
            <input 
              v-model="message" 
              placeholder="Type a message..." 
              @keyup.enter="sendMessage"
              class="message-text-input"
            />
            <button @click="sendMessage" class="send-button">
              <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                <line x1="22" y1="2" x2="11" y2="13"></line>
                <polygon points="22 2 15 22 11 13 2 9 22 2"></polygon>
              </svg>
              Send
            </button>
          </div>
        </div>
      </main>

      <aside class="chat-sidebar">
        <div class="sidebar-header">
          <h3>Active Users</h3>
          <span class="user-count">{{ connectedUsers.length }}</span>
        </div>
        <ul class="user-list">
          <transition-group name="list-transition">
            <li v-for="connectedUser in connectedUsers" :key="connectedUser" class="user-item" :title="connectedUser">
              <div class="user-avatar">{{ connectedUser.charAt(0).toUpperCase() }}</div>
              <span class="user-name">{{ truncateUsername(connectedUser) }}</span>
              <span v-if="connectedUser === user" class="user-status">(You)</span>
            </li>
          </transition-group>
        </ul>
      </aside>
    </div>
  </div>
</template>
  
<script>
import * as signalR from "@microsoft/signalr";
import axios from "axios";

export default {
  name: "ChatVue",
  data() {
    return {
      user: "",
      message: "",
      messages: [],
      connectedUsers: [],
      hubConnection: null,
      hasJoined: false,
      connectionEstablished: false,
      maxUsernameLength: 12, // Maximum length for displayed username
    };
  },

  mounted() {
    this.initializeSignalR();
    this.refreshConnectedUsers();
  },

  updated() {
    this.scrollToBottom();
  },

  methods: {
    initializeSignalR() {
      this.hubConnection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:5001/chatHub")
        .configureLogging(signalR.LogLevel.Information)
        .withAutomaticReconnect()
        .build();

      this.hubConnection.start().then(() => {
        console.log("SignalR connection established");
        this.connectionEstablished = true;
        this.registerSignalREventHandlers();
      }).catch(err => {
        console.error("Connection failed: ", err);
        this.connectionEstablished = false;
      });

      this.hubConnection.onreconnecting(() => {
        console.log("Reconnecting...");
        this.connectionEstablished = false;
      });

      this.hubConnection.onreconnected(() => {
        console.log("Reconnected!");
        this.connectionEstablished = true;
      });
    },

    registerSignalREventHandlers() {
      this.hubConnection.on("ReceiveMessage", (user, message) => {
        this.messages.push({ user, message });
      }); 
    },

    refreshConnectedUsers() {
      axios
        .get("https://localhost:5001/api/chat/users", {
          headers: { "Cache-Control": "no-cache" }
        })
        .then((response) => {
          this.connectedUsers = response.data;
        })
        .catch((error) => {
          console.error("Error fetching connected users:", error);
        });
    },

    joinChat() {
      if (this.user.trim().length >= 2) {
        this.hasJoined = true;
        this.refreshConnectedUsers();
      } else if (this.user.trim().length > 0) {
        alert("Username must be at least 2 characters long");
      }
    },

    sendMessage() {
      if (this.user && this.message.trim()) {
        this.hubConnection
          .invoke("SendMessage", this.user, this.message)
          .then(() => {
            console.log("Message sent");
            this.message = "";
            this.refreshConnectedUsers();
          })
          .catch((error) => {
            console.error("Error sending message:", error);
          });
      }
    },

    scrollToBottom() {
      if (this.$refs.messageContainer) {
        const container = this.$refs.messageContainer;
        container.scrollTop = container.scrollHeight;
      }
    },

    formatTime(date) {
      return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    },

    truncateUsername(username) {
      if (username.length <= this.maxUsernameLength) {
        return username;
      }
      return username.substring(0, this.maxUsernameLength) + '...';
    },
  },
};
</script>
  
<style scoped>
.chat-container {
  background-color: var(--card-bg);
  border-radius: 16px;
  box-shadow: var(--card-shadow);
  overflow: hidden;
  height: calc(100vh - 40px);
  display: flex;
  flex-direction: column;
}

.chat-header {
  background-image: linear-gradient(to right, var(--gradient-start), var(--gradient-end));
  color: white;
  padding: 18px 24px;
}

.header-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.connection-status {
  font-size: 12px;
  border-radius: 999px;
  padding: 4px 12px;
  background-color: rgba(0, 0, 0, 0.1);
  display: flex;
  align-items: center;
}

.connection-status::before {
  content: '';
  display: inline-block;
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background-color: var(--danger-color);
  margin-right: 6px;
}

.connection-status.connected::before {
  background-color: var(--success-color);
}

.chat-header h1 {
  font-size: 1.5rem;
  font-weight: 700;
  margin: 0;
}

.chat-layout {
  display: flex;
  flex: 1;
  overflow: hidden;
}

.chat-main {
  flex: 1;
  display: flex;
  flex-direction: column;
  background-color: #f9fafb;
}

.message-container {
  flex: 1;
  overflow-y: auto;
  padding: 24px;
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: var(--text-muted);
  padding: 40px 0;
  animation: fadeIn 0.5s ease-out;
}

.empty-icon {
  color: #d1d5db;
  margin-bottom: 16px;
}

.no-messages {
  color: var(--text-muted);
  text-align: center;
  margin-top: 40px;
}

.message-item {
  display: flex;
  align-items: flex-start;
  gap: 12px;
  max-width: 80%;
  animation: slideUp 0.3s ease-out;
}

.my-message {
  align-self: flex-end;
  flex-direction: row-reverse;
}

.message-avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background-color: var(--primary-color);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  flex-shrink: 0;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.my-message .message-avatar {
  background-color: var(--secondary-color);
}

.message-content {
  background-color: white;
  border-radius: 16px;
  padding: 12px 18px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
  position: relative;
}

.my-message .message-content {
  background-image: linear-gradient(to right, var(--primary-light), var(--primary-color));
  color: white;
}

.message-user {
  font-weight: 600;
  font-size: 14px;
  margin-bottom: 4px;
}

.my-message .message-user {
  color: rgba(255, 255, 255, 0.9);
}

.message-text {
  word-break: break-word;
  line-height: 1.5;
}

.message-time {
  font-size: 11px;
  color: var(--text-muted);
  margin-top: 6px;
  text-align: right;
}

.my-message .message-time {
  color: rgba(255, 255, 255, 0.7);
}

.message-input {
  border-top: 1px solid var(--border-color);
  padding: 20px;
  background-color: white;
}

.welcome-message {
  text-align: center;
  margin-bottom: 16px;
  color: var(--text-color);
  font-weight: 500;
}

.user-setup, .message-form {
  display: flex;
  gap: 12px;
}

.input-group {
  display: flex;
  gap: 12px;
  width: 100%;
}

.user-input, .message-text-input {
  flex: 1;
}

.join-button {
  background-color: var(--secondary-color);
}

.join-button:hover {
  background-color: var(--secondary-dark);
}

.chat-sidebar {
  width: 280px;
  background-color: white;
  border-left: 1px solid var(--border-color);
  display: flex;
  flex-direction: column;
}

.sidebar-header {
  padding: 20px;
  border-bottom: 1px solid var(--border-color);
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.sidebar-header h3 {
  font-size: 16px;
  font-weight: 600;
  margin: 0;
  color: var(--text-color);
}

.user-count {
  background-image: linear-gradient(to right, var(--gradient-start), var(--gradient-end));
  color: white;
  font-size: 12px;
  font-weight: 600;
  border-radius: 999px;
  padding: 3px 10px;
}

.user-list {
  list-style: none;
  overflow-y: auto;
  padding: 16px;
}

.user-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 10px;
  border-radius: 12px;
  transition: background-color 0.2s;
  margin-bottom: 6px;
}

.user-item:hover {
  background-color: #f9fafb;
}

.user-avatar {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  background-color: var(--primary-color);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  font-size: 14px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.user-status {
  font-size: 12px;
  color: var(--text-muted);
  margin-left: auto;
}

.user-name {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  max-width: 150px; /* Limit width of username container */
}

/* Transitions */
.message-transition-enter-active, .message-transition-leave-active {
  transition: all 0.3s ease;
}
.message-transition-enter-from, .message-transition-leave-to {
  opacity: 0;
  transform: translateY(20px);
}

.list-transition-enter-active, .list-transition-leave-active {
  transition: all 0.3s ease;
}
.list-transition-enter-from, .list-transition-leave-to {
  opacity: 0;
  transform: translateX(20px);
}

@media (max-width: 768px) {
  .chat-layout {
    flex-direction: column;
  }
  
  .chat-sidebar {
    width: 100%;
    border-left: none;
    border-top: 1px solid var(--border-color);
    max-height: 200px;
  }
  
  .message-item {
    max-width: 90%;
  }

  .chat-header {
    padding: 14px 20px;
  }

  .chat-header h1 {
    font-size: 1.2rem;
  }

  .message-avatar {
    width: 32px;
    height: 32px;
    font-size: 12px;
  }
}
</style>
