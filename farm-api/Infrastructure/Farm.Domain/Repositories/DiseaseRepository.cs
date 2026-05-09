using Farm.Domain.Entities;
using Farm.Domain.FarmDbContexts;
using Farm.Domain.Repositories.Interfaces;

namespace Farm.Domain.Repositories
{
    public class DiseaseRecordRepository : GenericRepository<DiseaseRecord, FarmDbContext>, IDiseaseRecordRepository
    {
        public DiseaseRecordRepository(FarmDbContext context) : base(context) { }
    }

    public class TreatmentRepository : GenericRepository<Treatment, FarmDbContext>, ITreatmentRepository
    {
        public TreatmentRepository(FarmDbContext context) : base(context) { }
    }
}
