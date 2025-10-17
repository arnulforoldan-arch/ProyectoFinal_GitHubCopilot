window.themeHelper = {
    setTheme: function(theme) {
        try {
            const themeClass = theme === 'dark' ? 'theme-dark' : 'theme-light';
            document.body.className = themeClass;
            localStorage.setItem('theme', theme);
        } catch (error) {
            console.warn('No se pudo establecer el tema:', error);
        }
    },
    
    getSystemTheme: function() {
        try {
            return window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
        } catch (error) {
            console.warn('No se pudo detectar el tema del sistema:', error);
            return 'light';
        }
    },
    
    getSavedTheme: function() {
        try {
            return localStorage.getItem('theme');
        } catch (error) {
            console.warn('No se pudo obtener el tema guardado:', error);
            return null;
        }
    },
    
    initTheme: function() {
        try {
            const savedTheme = this.getSavedTheme();
            const theme = savedTheme || this.getSystemTheme();
            this.setTheme(theme);
            return theme === 'dark';
        } catch (error) {
            console.warn('No se pudo inicializar el tema:', error);
            this.setTheme('light');
            return false;
        }
    },
    
    // Función para esperar a que el DOM esté listo
    domReady: function(callback) {
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', callback);
        } else {
            callback();
        }
    }
};

// Inicializar tema cuando el DOM esté listo
window.themeHelper.domReady(function() {
    window.themeHelper.initTheme();
});