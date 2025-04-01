function toggleChat() {
    const chatBox = document.getElementById('chatBox');
    chatBox.style.display = chatBox.style.display === 'flex' ? 'none' : 'flex';
}

function addMessage(content, isUser) {
    const messages = document.getElementById('chatMessages');
    const messageDiv = document.createElement('div');
    messageDiv.className = 'message ' + (isUser ? 'user-message' : 'bot-message');
    messageDiv.textContent = content;
    messages.appendChild(messageDiv);
    messages.scrollTop = messages.scrollHeight;
}

async function sendMessage() {
    const input = document.getElementById('chatInput');
    const message = input.value.trim();
    if (!message || message.length === 0) {
        addMessage("Vui lòng nhập tin nhắn!", false);
        return;
    }

    addMessage(message, true);
    input.value = '';

    try {
        const response = await fetch('/Gemini/Chat', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify(message)
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(`Server error: ${response.status} - ${errorText}`);
        }

        const data = await response.json();
        if (data.success) {
            addMessage(data.response, false);
        } else {
            addMessage('Lỗi: ' + data.error, false);
        }
    } catch (error) {
        addMessage('Lỗi: ' + error.message, false);
    }
}