// Funciones auxiliares para la aplicaci�n Blazor
window.blazorHelpers = {
    // Funci�n para copiar al portapapeles
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