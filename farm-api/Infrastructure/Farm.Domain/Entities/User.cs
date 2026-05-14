using Farm.Domain.Attibutes;
using Farm.Domain.Enum;
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

        // --- End-user auth (Phase 4) ---

        /// <summary>
        /// PBKDF2 hash (Microsoft.AspNetCore.Identity.PasswordHasher) when AuthProvider = Local.
        /// Null when user signs in via an external provider (Google/Facebook).
        /// </summary>
        [MaxLength(500)]
        public string PasswordHash { get; set; }

        /// <summary>How this user authenticates. Defaults to Local for legacy / admin users.</summary>
        public AuthProvider AuthProvider { get; set; } = AuthProvider.Local;

        /// <summary>
        /// External account identifier (Google sub, Facebook user_id) when applicable. Null for Local.
        /// </summary>
        [MaxLength(250)]
        public string ExternalId { get; set; }

        /// <summary>Display name shown in marketplace / forum. Falls back to UserName if empty.</summary>
        [MaxLength(250)]
        public string DisplayName { get; set; }
    }
}
