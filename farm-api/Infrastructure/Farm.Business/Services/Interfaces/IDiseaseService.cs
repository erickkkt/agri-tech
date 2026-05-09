using Farm.Domain.Entities;

namespace Farm.Business.Services.Interfaces
{
    public interface IDiseaseService
    {
        Task<IReadOnlyCollection<DiseaseRecord>> GetByAnimal(Guid animalId);
        Task<DiseaseRecord> GetById(Guid id);
        Task<Guid> Create(DiseaseRecord record);
        Task<DiseaseRecord> Update(DiseaseRecord record);

        Task<Guid> CreateTreatment(Treatment treatment);
        Task<IReadOnlyCollection<Treatment>> GetTreatmentsByAnimal(Guid animalId);
        Task<IReadOnlyCollection<Treatment>> GetTreatmentsByDisease(Guid diseaseId);
    }
}
