using System.Security.Claims;

namespace Phase_07_Poc_01.Helper
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst("id")?.Value;
            if (userIdClaim is null)
                throw new UnauthorizedAccessException("User id not found in token");
            return int.Parse(userIdClaim);
        }
    }
}
