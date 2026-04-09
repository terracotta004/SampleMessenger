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
