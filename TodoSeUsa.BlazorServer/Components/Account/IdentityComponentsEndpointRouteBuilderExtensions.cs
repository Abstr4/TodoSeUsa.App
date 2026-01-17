using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json;
using TodoSeUsa.BlazorServer.Components.Account.Pages;
using TodoSeUsa.BlazorServer.Components.Account.Pages.Manage;
using TodoSeUsa.Infrastructure.Data;

namespace TodoSeUsa.BlazorServer.Components.Account;

internal static class IdentityComponentsEndpointRouteBuilderExtensions
{
    // These endpoints are required by the Identity Razor components defined in the /Components/Account/Pages directory of this project.
    public static IEndpointConventionBuilder MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var accountGroup = endpoints.MapGroup("/Account");

        accountGroup.MapPost("/PerformLogin", async (
        HttpContext context,
        [FromServices] SignInManager<ApplicationUser> signInManager,
        [FromForm] LoginInputModel input,
        [FromForm] string returnUrl) =>
        {
            var result = await signInManager.PasswordSignInAsync(input.Email, input.Password, input.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return TypedResults.LocalRedirect(returnUrl ?? "/");
            }
            else if (result.RequiresTwoFactor)
            {
                var query = new Dictionary<string, string?>
                {
                    ["returnUrl"] = returnUrl,
                    ["rememberMe"] = input.RememberMe.ToString()
                };
                var redirectUrl = QueryString.Create(query).ToUriComponent();
                return TypedResults.Redirect($"/Account/LoginWith2fa{redirectUrl}");
            }
            else if (result.IsLockedOut)
            {
                return TypedResults.Redirect("/Account/Lockout");
            }
            else
            {
                return TypedResults.Redirect($"/Account/Login?error=invalid&ReturnUrl={Uri.EscapeDataString(returnUrl ?? "/")}");
            }
        });


        accountGroup.MapPost("/Logout", async (
            ClaimsPrincipal user,
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromForm] string returnUrl) =>
        {
            await signInManager.SignOutAsync();
            return TypedResults.LocalRedirect($"~/{returnUrl}");
        });

        var manageGroup = accountGroup.MapGroup("/Manage").RequireAuthorization();


        var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var downloadLogger = loggerFactory.CreateLogger("DownloadPersonalData");

        return accountGroup;
    }

    internal sealed class LoginInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; } = false;
    }
}
