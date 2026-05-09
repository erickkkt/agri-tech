using Farm.Domain.Attibutes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farm.Domain.Entities
{
    /// <summary>Community forum thread.</summary>
    [TrackAudit]
    public class ForumThread
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required, StringLength(250)]
        public string Title { get; set; }

        [StringLength(120)]
        public string Category { get; set; }

        public Guid AuthorUserId { get; set; }
        [StringLength(250)]
        public string AuthorUserName { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? LastReplyAt { get; set; }
        public int ViewCount { get; set; }
        public int PostCount { get; set; }

        public bool IsLocked { get; set; }
        public bool IsPinned { get; set; }

        public Guid? ChangedByUserId { get; set; }
        public string ChangedByUserName { get; set; }
        public DateTime? ChangedAt { get; set; }
    }

    /// <summary>Single post / reply within a forum thread.</summary>
    [TrackAudit]
    public class ForumPost
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid ThreadId { get; set; }
        [ForeignKey("ThreadId")]
        public virtual ForumThread Thread { get; set; }

        public Guid AuthorUserId { get; set; }
        [StringLength(250)]
        public string AuthorUserName { get; set; }

        public Guid? ParentPostId { get; set; }

        [Required, StringLength(8000)]
        public string Body { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? EditedAt { get; set; }

        public Guid? ChangedByUserId { get; set; }
        public string ChangedByUserName { get; set; }
        public DateTime? ChangedAt { get; set; }
    }
}
