using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace BillingSystem.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
           .AddCookie()
           .AddOpenIdConnect(options =>
           {
               var oidcConfig = builder.Configuration.GetSection("IdentityServer");

               options.Authority = oidcConfig["Authority"];
               options.ClientId = oidcConfig["ClientId"];
               options.ClientSecret = oidcConfig["ClientSecret"];
       

               options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
               options.ResponseType = OpenIdConnectResponseType.Code;

               options.Scope.Clear();
               options.Scope.Add("openid");
               options.Scope.Add("profile");
               options.Scope.Add("email");
               options.Scope.Add("offline_access");

               options.SaveTokens = true;
               options.GetClaimsFromUserInfoEndpoint = true;

               options.MapInboundClaims = false;
               options.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
               options.TokenValidationParameters.RoleClaimType = "roles";

               options.SignedOutCallbackPath = "/Logout";
           });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();


            app.UseAuthentication();
            // Authorization is applied for middleware after the UseAuthorization method
            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
