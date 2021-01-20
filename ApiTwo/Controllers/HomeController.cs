using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiTwo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [Route("/home")]
        public async Task<IActionResult> Index() 
        {
            var serverClient = _httpClientFactory.CreateClient();
            var discoveryDocument = await serverClient.GetDiscoveryDocumentAsync("https://localhost:44365/");
            var tokkenResponse = await serverClient.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {
                    Address = discoveryDocument.TokenEndpoint,
                    ClientId = "client_id",
                    ClientSecret = "client_secret",
                    Scope = "ApiOne",
                });
            var apiClient = _httpClientFactory.CreateClient();
            apiClient.SetBearerToken(tokkenResponse.AccessToken);
            var response = await apiClient.GetAsync("https://localhost:44376/secret");
            var content = await response.Content.ReadAsStringAsync();
            return Ok(new
            {
                access_Token = tokkenResponse.AccessToken,
                message = content,
            });
        }
    }
}
