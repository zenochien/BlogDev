using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MVCClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Secret()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var idToken = await HttpContext.GetTokenAsync("id_token");
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");

            var cliaims = User.Claims.ToList();
            var _accessToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            var _idToken = new JwtSecurityTokenHandler().ReadJwtToken(idToken);

            var result = await GetSecret(accessToken);
            await RefreshAccessToken();
            return View();
        }

        private async Task<string> GetSecret(string accessToken)
        {
            var apiClient = _httpClientFactory.CreateClient();
            apiClient.SetBearerToken(accessToken);
            var response = await apiClient.GetAsync("http://localhost:44376/secret");
            var content = await response.Content.ReadAsStringAsync();

            return content;
        }

        public IActionResult Logout()
        {
            return SignOut("Cookie", "oidc");
        }

        private async Task RefreshAccessToken()
        {
            {
                var serverClient = _httpClientFactory.CreateClient();
                var discoveryDocument = await serverClient.GetDiscoveryDocumentAsync("http://localhost:44365/");

                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var idToken = await HttpContext.GetTokenAsync("id_token");
                var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
                var refreshTokenClient = _httpClientFactory.CreateClient();

                var tokenResponse = await refreshTokenClient.RequestRefreshTokenAsync(
                    new RefreshTokenRequest
                    {
                        Address = discoveryDocument.TokenEndpoint,
                        RefreshToken = refreshToken,
                        ClientId = "client_id_mvc",
                        ClientSecret = "client_secret_mvc"
                    });
                var authInfo = await HttpContext.AuthenticateAsync("Cookie");

                authInfo.Properties.UpdateTokenValue("access_token", tokenResponse.AccessToken);
                authInfo.Properties.UpdateTokenValue("id_token", tokenResponse.IdentityToken);
                authInfo.Properties.UpdateTokenValue("refresh_token", tokenResponse.RefreshToken);

                await HttpContext.SignInAsync("Cookie", authInfo.Principal, authInfo.Properties);
            }
        }
    }
}
