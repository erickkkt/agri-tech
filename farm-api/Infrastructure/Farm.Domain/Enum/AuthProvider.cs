using System.ComponentModel;

namespace Farm.Domain.Enum
{
    /// <summary>
    /// How a user authenticates. Local users have a password hash; external users
    /// have an ExternalId from the provider and no password.
    /// </summary>
    public enum AuthProvider
    {
        [Description("Local username + password")] Local = 0,
        [Description("Google OAuth")]              Google = 1,
        [Description("Facebook OAuth")]            Facebook = 2,
        /// <summary>Legacy users created via Azure AD B2C (farm-admin).</summary>
        [Description("Azure AD")]                  AzureAd = 3
    }
}
