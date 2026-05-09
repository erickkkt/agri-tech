
namespace Farm.Domain.ViewModels.User
{
    public class UserProfile
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime ActionDateTime { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}
