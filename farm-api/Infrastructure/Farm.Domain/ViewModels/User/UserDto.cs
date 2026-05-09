
namespace Farm.Domain.ViewModels.User
{
    public class UserDto
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }

        public string PhoneNumber { get; set; }

        public Guid RoleId { get; set; }

        public string Role { get; set; }
    }
}
