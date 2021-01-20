using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Oauth.AuthRequirement
{
    public class CustomeAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public CustomeAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory loggerFactory, UrlEncoder encoder, ISystemClock systemClock)
            : base(options, loggerFactory, encoder, systemClock) { }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            return await Task.FromResult(AuthenticateResult.Fail("Failed Authentication"));
        }
    }
}
