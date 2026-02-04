using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Radzen;
using TodoSeUsa.BlazorServer.Components.Account;
using TodoSeUsa.BlazorServer.UI.Notifications;
using TodoSeUsa.Infrastructure.Data;

namespace TodoSeUsa.BlazorServer;

public static class DependencyInjection
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddCascadingAuthenticationState();

        builder.Services.AddScoped<IdentityUserAccessor>();
        builder.Services.AddScoped<IdentityRedirectManager>();
        builder.Services.AddScoped<AuthenticationStateProvider,
            IdentityRevalidatingAuthenticationStateProvider>();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        })
        .AddIdentityCookies();

        builder.Services.AddScoped<SimpleNotifications>();

        builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, options =>
        {
            options.LoginPath = "/Cuenta/Ingresar";
            options.LogoutPath = "/Cuenta/CerrarSesion";
            options.AccessDeniedPath = "/Cuenta/AccesoDenegado";
        });

        builder.Services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.Lockout.AllowedForNewUsers = true;

            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

            options.User.RequireUniqueEmail = true;
        })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddRadzenComponents();

        builder.Services.AddRadzenCookieThemeService(options =>
        {
            options.Name = "TodoSeUsaApplicationTheme";
            options.Duration = TimeSpan.FromDays(365);
        });
    }
}