$(document).ready(function () {
    const chatbotToggleButton = $('#chatbot-toggle-button');
    const chatbotPane = $('#chatbot-pane');
    const chatbotCloseBtn = $('#chatbot-close-btn');
    const chatbotMessages = $('#chatbot-messages');
    const chatbotInput = $('#chatbot-input');
    const chatbotSendBtn = $('#chatbot-send-btn');

    // --- Chatbot UI Toggle Logic ---
    chatbotToggleButton.on('click', function () {
        chatbotPane.toggle();
        if (chatbotPane.is(':visible')) {
            chatbotInput.focus(); // Focus input when opening
            chatbotMessages.scrollTop(chatbotMessages[0].scrollHeight); // Scroll to bottom
        }
    });

    chatbotCloseBtn.on('click', function () {
        chatbotPane.hide();
    });

    // --- Chatbot Messaging Logic ---
    function appendMessage(sender, message, isHtml = false) {
        const messageDiv = $('<div/>').addClass('chatbot-message').addClass(sender);
        if (isHtml) {
            messageDiv.html(message); // Use .html() to render HTML content
        } else {
            messageDiv.text(message); // Use .text() for plain text to prevent XSS
        }
        chatbotMessages.append(messageDiv);
        chatbotMessages.scrollTop(chatbotMessages[0].scrollHeight); // Scroll to bottom
    }

    // Initial bot message
    appendMessage('bot', 'Hello! How can I help you find products today?');

    chatbotSendBtn.on('click', sendMessage);
    chatbotInput.on('keypress', function (e) {
        if (e.which === 13) { // Enter key
            sendMessage();
        }
    });

    async function sendMessage() {
        const userMessage = chatbotInput.val().trim();
        if (userMessage === '') return;

        appendMessage('user', userMessage);
        chatbotInput.val('');
        chatbotSendBtn.prop('disabled', true).text('Thinking...');

        try {
            const response = await $.ajax({
                url: '/components/chatbot?handler=SemanticSearch', // Matches the handler in Chatbot.cshtml.cs
                method: 'GET',
                data: { text: userMessage }
            });

            if (response && response.length > 0) {
                let botResponseHtml = 'Here are some products that might be relevant:<br>';
                response.forEach(product => {
                    const productLink = `/Product/${product.id}`; // Adjust this URL to your actual product details page
                    botResponseHtml += `- <strong><a href="${productLink}" target="_blank">${product.name}</a></strong> (Category: ${product.category || 'N/A'})<br>`;
                    botResponseHtml += `  Author: ${product.author ? product.author.substring(0, 100) : 'No author'}<br>`;
                });
                appendMessage('bot', botResponseHtml, true); // Pass true for isHtml
            } else {
                appendMessage('bot', 'I could not find any relevant products for your query. Please try rephrasing.');
            }
        } catch (error) {
            console.error("Chatbot API error:", error);
            appendMessage('bot', 'Oops! Something went wrong. Please try again later.');
        } finally {
            chatbotSendBtn.prop('disabled', false).text('Send');
            chatbotInput.focus(); // Keep focus on input after sending
        }
    }
});