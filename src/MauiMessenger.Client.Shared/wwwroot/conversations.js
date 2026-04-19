window.messengerConversations = {
  scrollToBottom(element) {
    if (!element) {
      return;
    }

    element.scrollTop = element.scrollHeight;
  }
};
