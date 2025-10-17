using Microsoft.JSInterop;

namespace AdventureWorks.Enterprise.WebApp.Services;

/// <summary>
/// Servicio para gestionar el tema (claro/oscuro) de la aplicación Blazor.
/// Permite inicializar, alternar y persistir la preferencia de tema del usuario.
/// </summary>
public class ThemeService
{
    private readonly IJSRuntime _jsRuntime;
    private bool _isDarkTheme = false;
    private bool _isInitialized = false;
    
    /// <summary>
    /// Evento que se dispara cuando cambia el tema.
    /// El parámetro booleano indica si el tema actual es oscuro.
    /// </summary>
    public event Action<bool>? OnThemeChanged;

    /// <summary>
    /// Crea una instancia del servicio de temas.
    /// </summary>
    /// <param name="jsRuntime">Runtime de JavaScript para interoperabilidad.</param>
    public ThemeService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Indica si el tema actual es oscuro.
    /// </summary>
    public bool IsDarkTheme => _isDarkTheme;

    /// <summary>
    /// Indica si la inicialización del servicio se completó.
    /// </summary>
    public bool IsInitialized => _isInitialized;

    /// <summary>
    /// Inicializa el servicio intentando leer la preferencia desde localStorage o del sistema.
    /// Para prerenderizado, utiliza valores por defecto.
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            if (_jsRuntime is IJSInProcessRuntime)
            {
                await PerformInitializationAsync();
            }
            else
            {
                _isDarkTheme = false;
                _isInitialized = true;
            }
        }
        catch (InvalidOperationException)
        {
            _isDarkTheme = false;
            _isInitialized = true;
        }
    }

    /// <summary>
    /// Método auxiliar para inicializar después del primer renderizado si aún no se hizo.
    /// </summary>
    public async Task InitializeAfterRenderAsync()
    {
        if (!_isInitialized)
        {
            await PerformInitializationAsync();
        }
    }

    /// <summary>
    /// Realiza la lógica de inicialización: obtiene el tema guardado o detecta preferencia del sistema.
    /// </summary>
    private async Task PerformInitializationAsync()
    {
        try
        {
            // Obtener tema desde localStorage o usar preferencia del sistema
            var savedTheme = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "theme");
            
            if (string.IsNullOrEmpty(savedTheme))
            {
                // Verificar preferencia del sistema
                var prefersDark = await _jsRuntime.InvokeAsync<bool>("window.matchMedia('(prefers-color-scheme: dark)').matches");
                _isDarkTheme = prefersDark;
            }
            else
            {
                _isDarkTheme = savedTheme == "dark";
            }

            await ApplyThemeAsync();
            _isInitialized = true;
            OnThemeChanged?.Invoke(_isDarkTheme);
        }
        catch (JSException)
        {
            // Manejar caso donde JavaScript no está disponible aún
            _isDarkTheme = false;
            _isInitialized = true;
        }
    }

    /// <summary>
    /// Alterna el tema entre claro y oscuro y persiste el cambio.
    /// </summary>
    public async Task ToggleThemeAsync()
    {
        if (!_isInitialized)
        {
            await InitializeAfterRenderAsync();
        }

        _isDarkTheme = !_isDarkTheme;
        await ApplyThemeAsync();
        await SaveThemeAsync();
        OnThemeChanged?.Invoke(_isDarkTheme);
    }

    /// <summary>
    /// Aplica la clase CSS correspondiente al cuerpo del documento.
    /// </summary>
    private async Task ApplyThemeAsync()
    {
        try
        {
            var themeClass = _isDarkTheme ? "theme-dark" : "theme-light";
            await _jsRuntime.InvokeVoidAsync("eval", $"document.body.className = '{themeClass}'");
        }
        catch (JSException)
        {
            // Manejar caso donde JavaScript no está disponible aún
        }
        catch (InvalidOperationException)
        {
            // Manejar prerenderizado
        }
    }

    /// <summary>
    /// Guarda la preferencia de tema en localStorage.
    /// </summary>
    private async Task SaveThemeAsync()
    {
        try
        {
            var theme = _isDarkTheme ? "dark" : "light";
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "theme", theme);
        }
        catch (JSException)
        {
            // Manejar caso donde JavaScript no está disponible aún
        }
        catch (InvalidOperationException)
        {
            // Manejar prerenderizado
        }
    }
}