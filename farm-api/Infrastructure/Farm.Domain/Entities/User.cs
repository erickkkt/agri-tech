using Farm.Domain.Attibutes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farm.Domain.Entities
{
    [TrackAudit]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required, MaxLength(250)]
        public string UserName { get; set; }

        [Required, MaxLength(250)]
        public string EmailAddress { get; set; }

        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        public Guid? RoleId { get; set; }
        public Role Role { get; set; }

        public bool IsActive { get; set; }
        public Guid? ChangedByUserId { get; set; }
        public string ChangedByUserName { get; set; }
        public DateTime? ChangedAt { get; set; }
    }
}
