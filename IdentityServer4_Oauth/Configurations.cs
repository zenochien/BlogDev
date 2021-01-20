using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer4_Oauth
{
    public class Configurations
    {
        public static IEnumerable<IdentityResource> GetIdentityResources() =>
            new List<IdentityResource>
             {
                new IdentityResources.OpenId(),
                new IdentityResource
                {
                    Name="rc.scope",
                    UserClaims={"re.grndma"}
                }
             };

        public static IEnumerable<ApiResource> GetApis() =>
            new List<ApiResource> {
                new ApiResource("ApiOne"),
                new ApiResource("ApiTwo", new string[] {"rc.api.garndma"}),
            };

        public static IEnumerable<Client> GetClients() =>
            new List<Client>
            {
                new Client
                {
                    ClientId="client_id",
                    ClientSecrets = {new Secret("client_secret".ToSha256())},
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes={"ApiOne"}
                },

                new Client
                {
                    ClientId="client_id_mvc",
                    ClientSecrets={new Secret("clinet_secret_mvc".ToSha256()) },
                    AllowedGrantTypes=GrantTypes.Code,
                    RequirePkce=true,

                    RedirectUris={ "https://localhost:44366/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:44366/Home/Index" },
                    AllowedScopes={"ApiOne", "ApiTwo", IdentityServerConstants.StandardScopes.OpenId, "rc.scope"},

                    AllowOfflineAccess=true,
                    RequireConsent=false,
                },

                new Client
                {
                    ClientId="client_id_js",
                    AllowedGrantTypes=GrantTypes.Code,
                    RequirePkce=true,
                    RequireClientSecret=false,

                    RedirectUris={ "https://localhost:44365/hoem/signin" },
                    PostLogoutRedirectUris={ "https://localhost:44365/home/index" },
                    AllowedCorsOrigins={ "https://localhost:44365" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,"ApiOne", "ApiTwo", "rc.scope",
                    },
                    AccessTokenLifetime=1,
                    AllowAccessTokensViaBrowser=true,
                    RequireConsent=false,
                },              

            };
    }
}
