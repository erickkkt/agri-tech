using Farm.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace Farm.Domain.ViewModels.Disease
{
    public class DiseaseRecordDto
    {
        public Guid Id { get; set; }
        public Guid AnimalId { get; set; }
        public string AnimalCode { get; set; }
        public string AnimalName { get; set; }
        public string DiseaseName { get; set; }
        public DateTime DiagnosedAt { get; set; }
        public string DiagnosedBy { get; set; }
        public DiseaseSeverity Severity { get; set; }
        public DiseaseStatus Status { get; set; }
        public DateTime? RecoveredAt { get; set; }
        public string Notes { get; set; }
    }

    public class CreateDiseaseRecordDto
    {
        [Required] public Guid AnimalId { get; set; }
        [Required, StringLength(250)] public string DiseaseName { get; set; }
        [Required] public DateTime DiagnosedAt { get; set; }
        [StringLength(250)] public string DiagnosedBy { get; set; }
        public DiseaseSeverity Severity { get; set; }
        public DiseaseStatus Status { get; set; }
        [StringLength(2000)] public string Notes { get; set; }
    }

    public class UpdateDiseaseRecordDto : CreateDiseaseRecordDto
    {
        [Required] public Guid Id { get; set; }
        public DateTime? RecoveredAt { get; set; }
    }

    public class TreatmentDto
    {
        public Guid Id { get; set; }
        public Guid AnimalId { get; set; }
        public Guid? DiseaseRecordId { get; set; }
        public string DiseaseName { get; set; }
        public string Medication { get; set; }
        public string Dosage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string AdministeredBy { get; set; }
        public string Outcome { get; set; }
    }

    public class CreateTreatmentDto
    {
        [Required] public Guid AnimalId { get; set; }
        public Guid? DiseaseRecordId { get; set; }
        [Required, StringLength(250)] public string Medication { get; set; }
        [StringLength(250)] public string Dosage { get; set; }
        [Required] public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [StringLength(250)] public string AdministeredBy { get; set; }
        [StringLength(2000)] public string Outcome { get; set; }
    }
}
