using Farm.Domain.ViewModels.User;

namespace Farm.Api.Extensions
{
    public static class ContextExtensions
    {
        public static UserProfile GetUserProfile(this HttpContext httpContext)
        {
            if (httpContext == null) return default(UserProfile);

            var userId = httpContext.User?.Claims?.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier");
            var userName = httpContext.User?.Claims?.FirstOrDefault(c => c.Type == "name");
            var emails = httpContext.User?.Claims?.FirstOrDefault(c => c.Type == "emails" || c.Type == "preferred_username");

            var exp = httpContext.User?.Claims?.FirstOrDefault(c => c.Type == "exp")?.Value;
            var expDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local)
                                    .AddSeconds(Convert.ToInt32(exp));

            Guid.TryParse(userId?.Value, out var userGuid);
            var userProfile = new UserProfile
            {
                Id = userGuid,
                Username = userName?.Value,
                Email = emails?.Value,
                ActionDateTime = DateTime.Now,
                ExpirationTime = expDate
            };

            return userProfile;
        }
    }
}
