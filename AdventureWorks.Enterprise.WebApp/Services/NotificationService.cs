using Microsoft.JSInterop;

namespace AdventureWorks.Enterprise.WebApp.Services
{
    /// <summary>
    /// Servicio para mostrar notificaciones en la interfaz mediante JavaScript.
    /// Permite mostrar mensajes de éxito, error, advertencia e información.
    /// </summary>
    public class NotificationService
    {
        private readonly IJSRuntime _jsRuntime;

        /// <summary>
        /// Inicializa el servicio de notificaciones con el runtime JS.
        /// </summary>
        /// <param name="jsRuntime">Instancia de <see cref="IJSRuntime"/> para invocaciones JS.</param>
        public NotificationService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        /// <summary>
        /// Muestra una notificación de éxito.
        /// </summary>
        public async Task ShowSuccessAsync(string message)
        {
            await _jsRuntime.InvokeVoidAsync("showNotification", "success", message);
        }

        /// <summary>
        /// Muestra una notificación de error.
        /// </summary>
        public async Task ShowErrorAsync(string message)
        {
            await _jsRuntime.InvokeVoidAsync("showNotification", "error", message);
        }

        /// <summary>
        /// Muestra una notificación de advertencia.
        /// </summary>
        public async Task ShowWarningAsync(string message)
        {
            await _jsRuntime.InvokeVoidAsync("showNotification", "warning", message);
        }

        /// <summary>
        /// Muestra una notificación informativa.
        /// </summary>
        public async Task ShowInfoAsync(string message)
        {
            await _jsRuntime.InvokeVoidAsync("showNotification", "info", message);
        }
    }
}