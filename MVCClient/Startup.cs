using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MVCClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(config =>
            {
                config.DefaultScheme = "Cookie";
                config.DefaultChallengeScheme = "oidc";
            }).AddCookie("Cookie") 
            .AddOpenIdConnect("oidc", config =>
            {
                config.Authority = "http://localhost:44365/";
                config.ClientId = "client_id_mvc";
                config.ClientSecret = "client_secret_mvc";
                config.SaveTokens = true;
                config.ResponseType = "code";
                config.SignedOutCallbackPath = "/Home/Index";

                // configure cookie claim mapping
                config.ClaimActions.DeleteClaim("amr");
                config.ClaimActions.DeleteClaim("s_hash");
                config.ClaimActions.MapUniqueJsonKey("RawCoding.Grandma", "rc.garndma");

                // two trips to load claims in to the cookie
                // but the id token is smaller !
                config.GetClaimsFromUserInfoEndpoint = true;

                // configure scope
                config.Scope.Clear();
                config.Scope.Add("openid");
                config.Scope.Add("rc.scope");
                config.Scope.Add("ApiOne");
                config.Scope.Add("ApiTwo");
                config.Scope.Add("offline_access");

            });

            services.AddHttpClient();

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
