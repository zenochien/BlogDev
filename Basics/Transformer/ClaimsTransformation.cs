using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Basics.Transformer
{
    public class ClaimsTransformation : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var hasFriendClaim = principal.Claims.Any(x => x.Type == "Friend");

            if(!hasFriendClaim)
            {
                ((ClaimsIdentity)principal.Identities).AddClaim(new Claim("friend", "bad"));
            }

            return Task.FromResult(principal);
        }
    }
}
