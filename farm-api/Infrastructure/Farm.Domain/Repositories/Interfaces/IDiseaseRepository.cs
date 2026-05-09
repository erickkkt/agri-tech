using Farm.Domain.Entities;

namespace Farm.Domain.Repositories.Interfaces
{
    public interface IDiseaseRecordRepository : IGenericRepository<DiseaseRecord> { }

    public interface ITreatmentRepository : IGenericRepository<Treatment> { }
}
