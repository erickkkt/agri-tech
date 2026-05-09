using Farm.Business.Services.Interfaces;
using Farm.Domain.Entities;
using Farm.Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Farm.Business.Services
{
    public class DiseaseService : IDiseaseService
    {
        private readonly IDiseaseRecordRepository _diseaseRepo;
        private readonly ITreatmentRepository _treatmentRepo;

        public DiseaseService(IDiseaseRecordRepository diseaseRepo, ITreatmentRepository treatmentRepo)
        {
            _diseaseRepo = diseaseRepo;
            _treatmentRepo = treatmentRepo;
        }

        public async Task<IReadOnlyCollection<DiseaseRecord>> GetByAnimal(Guid animalId)
        {
            return await _diseaseRepo.QueryMany(d => d.AnimalId == animalId)
                .Include(d => d.Animal)
                .OrderByDescending(d => d.DiagnosedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<DiseaseRecord> GetById(Guid id) => _diseaseRepo.GetAsync(d => d.Id == id);

        public async Task<Guid> Create(DiseaseRecord record)
        {
            record.CreatedAt = DateTime.UtcNow;
            var created = await _diseaseRepo.CreateAsync(record);
            return created?.Id ?? Guid.Empty;
        }

        public Task<DiseaseRecord> Update(DiseaseRecord record) => _diseaseRepo.UpdateAsync(record);

        public async Task<Guid> CreateTreatment(Treatment treatment)
        {
            treatment.CreatedAt = DateTime.UtcNow;
            var created = await _treatmentRepo.CreateAsync(treatment);
            return created?.Id ?? Guid.Empty;
        }

        public async Task<IReadOnlyCollection<Treatment>> GetTreatmentsByAnimal(Guid animalId)
        {
            return await _treatmentRepo.QueryMany(t => t.AnimalId == animalId)
                .Include(t => t.DiseaseRecord)
                .OrderByDescending(t => t.StartDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<Treatment>> GetTreatmentsByDisease(Guid diseaseId)
        {
            return await _treatmentRepo.QueryMany(t => t.DiseaseRecordId == diseaseId)
                .OrderByDescending(t => t.StartDate)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
