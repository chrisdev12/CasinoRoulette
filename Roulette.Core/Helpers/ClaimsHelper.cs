using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Roulette.Core.Helpers
{
    public class ClaimsHelper
    {
        public static string GetValue(HttpContext context, string claimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")
        {
            var identity = context.User.Identity as ClaimsIdentity;
            IEnumerable<Claim> claims = identity.Claims;

            return claims.Where(x => x.Type == claimType).FirstOrDefault().Value;
        }
    }
}
