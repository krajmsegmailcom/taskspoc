using Task.Blazor.Components;
using MudBlazor.Services;
using Task.Blazor.Services;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7266/") });
// Add services to the container.
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();
builder.Services.AddHttpClient<TaskService>();
var app = builder.Build();


// Configure the HTTP request pipeline.
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseAntiforgery();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _user = new ClaimsPrincipal(new ClaimsIdentity());

    public void MarkUserAsAuthenticated(ClaimsPrincipal user)
    {
        _user = user;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public override global::System.Threading.Tasks.Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return global::System.Threading.Tasks.Task.FromResult(new AuthenticationState(_user));
    }
}