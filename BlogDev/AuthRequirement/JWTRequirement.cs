using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Threading.Tasks;

namespace Oauth.AuthRequirement
{
    public class JWTRequirement : IAuthorizationRequirement { }

    public class JWTRequiremnetHandler: AuthorizationHandler<JWTRequirement>
    {
        private readonly HttpClient _httpClient;
        private readonly HttpContext _httpContext;

        public JWTRequiremnetHandler(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpContext = httpContextAccessor.HttpContext;
        }
                

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, JWTRequirement requirement)
        {
            if(_httpContext.Request.Headers.TryGetValue("Authorization",out var authHeader))
            {
                var accessToken = authHeader.ToString().Split(' ')[1];

                var response = await _httpClient.GetAsync($"https://localhost:44335/oauth/validate?access_token={accessToken}");

                if(response.StatusCode==System.Net.HttpStatusCode.OK)
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}
