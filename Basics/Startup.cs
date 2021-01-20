using Basics.AuthrizationRequirements;
using Basics.CustomPolicyProvider;
using Basics.Transformer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using static Basics.Controllers.OperationsController;

namespace Basics
{
    public class Startup
    {

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("CookieAuth")
                .AddCookie("CookieAuth", config =>
                {
                    config.Cookie.Name = "Grandms.Cookie";
                    config.LoginPath = "/Home/Authenticate";
                });

            services.AddAuthorization(configure =>
            {
                configure.AddPolicy("Admin", policyBuilder => policyBuilder.RequireClaim(ClaimTypes.Role, "Admin"));
                configure.AddPolicy("Claim.DoB", policyBuilder =>
                {
                    policyBuilder.RequirementCustomClaim(ClaimTypes.DateOfBirth);
                });
            });

            services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, SucurityLevelHandler>();
            services.AddScoped<IAuthorizationHandler, CustomRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, CookieJarAuthorizationHandler>();
            services.AddScoped<IClaimsTransformation, ClaimsTransformation>();

            //services.AddControllersWithViews(config =>
            //{
            //    var defaultAuthBuilder = new AuthorizationPolicyBuilder();
            //    var defaultAuthPolicy = defaultAuthBuilder
            //        .RequireAuthenticatedUser()
            //        .Build();

            //    // global authorization filter
            //    //config.Filters.Add(new AuthorizeFilter(defaultAuthPolicy));
            //});

            //services.AddRazorPages()
            //    .AddRazorPagesOptions(config =>
            //    {
            //        config.Conventions.AuthorizePage("/Razor/Secured");
            //        config.Conventions.AuthorizePage("/Razor/Policy", "Admin");
            //        config.Conventions.AuthorizeFolder("/RazorSecured");
            //        config.Conventions.AllowAnonymousToPage("/RazorSecured/Anon");
            //    });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            // who are you?
            app.UseAuthentication();

            // are you allowed?
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }
    }
}
