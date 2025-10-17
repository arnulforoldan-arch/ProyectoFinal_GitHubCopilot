// Funciones auxiliares para la aplicación Blazor
window.blazorHelpers = {
    // Función para copiar al portapapeles
    copyToClipboard: function (text) {
        navigator.clipboard.writeText(text)
            .then(() => {
                console.log('Texto copiado al portapapeles');
                return true;
            })
            .catch(err => {
                console.error('Error al copiar texto: ', err);
                return false;
            });
    }
};