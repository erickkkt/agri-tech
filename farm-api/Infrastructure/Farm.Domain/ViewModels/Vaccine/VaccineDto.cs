using Farm.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace Farm.Domain.ViewModels.Vaccine
{
    public class VaccineDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public Species? RecommendedSpecies { get; set; }
        public int IntervalDays { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateVaccineDto
    {
        [Required, StringLength(250)] public string Name { get; set; }
        [StringLength(250)] public string Manufacturer { get; set; }
        public Species? RecommendedSpecies { get; set; }
        [Range(0, 3650)] public int IntervalDays { get; set; }
        [StringLength(2000)] public string Description { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateVaccineDto : CreateVaccineDto
    {
        [Required] public Guid Id { get; set; }
    }

    public class VaccineScheduleDto
    {
        public Guid Id { get; set; }
        public Guid AnimalId { get; set; }
        public string AnimalCode { get; set; }
        public string AnimalName { get; set; }
        public Guid VaccineId { get; set; }
        public string VaccineName { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime? AdministeredDate { get; set; }
        public string AdministeredBy { get; set; }
        public VaccineStatus Status { get; set; }
        public string Notes { get; set; }
    }

    public class CreateVaccineScheduleDto
    {
        [Required] public Guid AnimalId { get; set; }
        [Required] public Guid VaccineId { get; set; }
        [Required] public DateTime ScheduledDate { get; set; }
        [StringLength(2000)] public string Notes { get; set; }
    }

    public class AdministerVaccineDto
    {
        [Required] public Guid ScheduleId { get; set; }
        [Required] public DateTime AdministeredDate { get; set; }
        [Required, StringLength(250)] public string AdministeredBy { get; set; }
        [StringLength(2000)] public string Notes { get; set; }
    }
}
