window.messengerAuth = {
  async login(apiBaseUrl, request) {
    return await this.sendJson(`${apiBaseUrl}/api/auth/login`, {
      method: "POST",
      credentials: "include",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(request)
    });
  },

  async logout(apiBaseUrl) {
    const response = await fetch(`${apiBaseUrl}/api/auth/logout`, {
      method: "POST",
      credentials: "include"
    });

    if (!response.ok) {
      const message = await response.text();
      throw new Error(message || "Logout failed.");
    }
  },

  async getCurrentUser(apiBaseUrl) {
    const response = await fetch(`${apiBaseUrl}/api/auth/me`, {
      method: "GET",
      credentials: "include"
    });

    if (response.status === 401) {
      return null;
    }

    if (!response.ok) {
      const message = await response.text();
      throw new Error(message || "Failed to load current user.");
    }

    return await response.json();
  },

  async sendJson(url, options) {
    const response = await fetch(url, options);

    if (response.status === 401) {
      throw new Error("Invalid username or password.");
    }

    if (!response.ok) {
      const message = await response.text();
      throw new Error(message || "Request failed.");
    }

    return await response.json();
  }
};

window.messengerApi = {
  async getUsers(apiBaseUrl) {
    return await this.fetchJson(`${apiBaseUrl}/api/users`, {
      method: "GET",
      credentials: "include"
    });
  },

  async createUser(apiBaseUrl, request) {
    return await this.fetchJson(`${apiBaseUrl}/api/users`, {
      method: "POST",
      credentials: "include",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(request)
    });
  },

  async getConversationsByUser(apiBaseUrl, userId) {
    const encodedUserId = encodeURIComponent(userId);
    return await this.fetchJson(`${apiBaseUrl}/api/conversations?userId=${encodedUserId}`, {
      method: "GET",
      credentials: "include"
    });
  },

  async createConversation(apiBaseUrl, request) {
    return await this.fetchJson(`${apiBaseUrl}/api/conversations`, {
      method: "POST",
      credentials: "include",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(request)
    });
  },

  async getMessagesByConversation(apiBaseUrl, conversationId) {
    const encodedConversationId = encodeURIComponent(conversationId);
    return await this.fetchJson(`${apiBaseUrl}/api/messages?conversationId=${encodedConversationId}`, {
      method: "GET",
      credentials: "include"
    });
  },

  async createMessage(apiBaseUrl, request) {
    return await this.fetchJson(`${apiBaseUrl}/api/messages`, {
      method: "POST",
      credentials: "include",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(request)
    });
  },

  async fetchJson(url, options) {
    const response = await fetch(url, options);

    if (response.status === 401) {
      throw new Error("Your session has expired. Please log in again.");
    }

    if (!response.ok) {
      const message = await response.text();
      throw new Error(message || "Request failed.");
    }

    if (response.status === 204) {
      return null;
    }

    return await response.json();
  }
};
