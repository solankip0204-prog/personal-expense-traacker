using System.Security.Claims;

namespace PersonalExpenseTracker.Helpers
{
    // Small helper to pull the logged-in user's/admin's id from the ClaimsPrincipal.
    public static class CurrentUser
    {
        public static int GetUserId(ClaimsPrincipal principal)
        {
            var claim = principal.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }
    }
}
