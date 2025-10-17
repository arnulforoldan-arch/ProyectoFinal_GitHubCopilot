using AdventureWorks.Enterprise.WebApp.Components;
using AdventureWorks.Enterprise.WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register notification service
builder.Services.AddScoped<NotificationService>();

// Register theme service
builder.Services.AddScoped<ThemeService>();

// Read ApiKeySettings from configuration
var apiBaseUrl = builder.Configuration.GetSection("ApiKeySettings")["ApiBaseUrl"];
var apiKey = builder.Configuration.GetSection("ApiKeySettings")["ApiKey"];

// Register ApiService and configure HttpClient
builder.Services.AddHttpClient<ApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl ?? "http://localhost:7006/"); // fallback if not set
    client.DefaultRequestHeaders.Add("X-API-Key", apiKey ?? string.Empty);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
