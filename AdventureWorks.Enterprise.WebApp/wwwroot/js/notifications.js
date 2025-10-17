// Notification system for the application
window.showNotification = function (type, message) {
    // Create notification container if it doesn't exist
    let container = document.getElementById('notification-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'notification-container';
        container.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 10000;
            pointer-events: none;
        `;
        document.body.appendChild(container);
    }

    // Create notification element
    const notification = document.createElement('div');
    notification.style.cssText = `
        background: var(--notification-bg, #fff);
        border: 1px solid var(--notification-border, #ddd);
        border-radius: 8px;
        padding: 12px 16px;
        margin-bottom: 8px;
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
        max-width: 400px;
        opacity: 0;
        transform: translateX(100%);
        transition: all 0.3s ease-in-out;
        pointer-events: auto;
        display: flex;
        align-items: center;
        gap: 8px;
        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        font-size: 14px;
        line-height: 1.4;
    `;

    // Set type-specific styles and icons
    let iconClass = '';
    switch (type) {
        case 'success':
            notification.style.background = '#d4edda';
            notification.style.borderColor = '#c3e6cb';
            notification.style.color = '#155724';
            iconClass = 'fas fa-check-circle';
            break;
        case 'error':
            notification.style.background = '#f8d7da';
            notification.style.borderColor = '#f5c6cb';
            notification.style.color = '#721c24';
            iconClass = 'fas fa-exclamation-circle';
            break;
        case 'warning':
            notification.style.background = '#fff3cd';
            notification.style.borderColor = '#ffeaa7';
            notification.style.color = '#856404';
            iconClass = 'fas fa-exclamation-triangle';
            break;
        case 'info':
            notification.style.background = '#d1ecf1';
            notification.style.borderColor = '#bee5eb';
            notification.style.color = '#0c5460';
            iconClass = 'fas fa-info-circle';
            break;
    }

    // Create close button
    const closeBtn = document.createElement('span');
    closeBtn.innerHTML = '<i class="fas fa-times"></i>';
    closeBtn.style.cssText = `
        cursor: pointer;
        font-size: 16px;
        margin-left: auto;
        opacity: 0.7;
        padding: 4px;
        border-radius: 4px;
        transition: all 0.2s ease;
    `;
    closeBtn.onmouseover = () => {
        closeBtn.style.opacity = '1';
        closeBtn.style.backgroundColor = 'rgba(0, 0, 0, 0.1)';
    };
    closeBtn.onmouseout = () => {
        closeBtn.style.opacity = '0.7';
        closeBtn.style.backgroundColor = 'transparent';
    };

    // Set notification content with Font Awesome icon
    notification.innerHTML = `
        <i class="${iconClass}" style="font-size: 16px; margin-right: 4px; flex-shrink: 0;"></i>
        <span style="flex: 1;">${message}</span>
    `;
    notification.appendChild(closeBtn);

    // Add to container
    container.appendChild(notification);

    // Animate in
    setTimeout(() => {
        notification.style.opacity = '1';
        notification.style.transform = 'translateX(0)';
    }, 10);

    // Auto-remove after delay
    const autoRemoveTimeout = setTimeout(() => {
        removeNotification(notification);
    }, type === 'error' ? 7000 : 5000); // Errors stay longer

    // Manual close functionality
    const removeNotification = (element) => {
        clearTimeout(autoRemoveTimeout);
        element.style.opacity = '0';
        element.style.transform = 'translateX(100%)';
        setTimeout(() => {
            if (element.parentNode) {
                element.parentNode.removeChild(element);
            }
        }, 300);
    };

    closeBtn.onclick = () => removeNotification(notification);
    notification.onclick = () => removeNotification(notification);
};