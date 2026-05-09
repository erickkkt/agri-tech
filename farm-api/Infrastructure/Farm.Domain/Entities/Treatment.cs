using Farm.Domain.Attibutes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Farm.Domain.Entities
{
    /// <summary>
    /// A medication / treatment course applied to an animal,
    /// optionally tied to a DiseaseRecord.
    /// </summary>
    [TrackAudit]
    public class Treatment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid AnimalId { get; set; }
        [ForeignKey("AnimalId")]
        public virtual Animal Animal { get; set; }

        public Guid? DiseaseRecordId { get; set; }
        [ForeignKey("DiseaseRecordId")]
        public virtual DiseaseRecord DiseaseRecord { get; set; }

        [Required, StringLength(250)]
        public string Medication { get; set; }

        [StringLength(250)]
        public string Dosage { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [StringLength(250)]
        public string AdministeredBy { get; set; }

        [StringLength(2000)]
        public string Outcome { get; set; }

        public DateTime CreatedAt { get; set; }
        public Guid? ChangedByUserId { get; set; }
        public string ChangedByUserName { get; set; }
        public DateTime? ChangedAt { get; set; }
    }
}
