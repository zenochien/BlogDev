using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Oauth.Controllers
{
    public class SecretController : Controller
    {
        [Authorize]
        public string Index()
        {
            return "thử xem";
        }
    }
}
