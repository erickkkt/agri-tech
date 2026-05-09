using Farm.Business.Services.Interfaces;
using Farm.Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Farm.Business.Services
{
    public class FarmService : IFarmService
    {
        private readonly IFarmRepository _farmRepository;

        public FarmService(IFarmRepository farmRepository)
        {
            _farmRepository = farmRepository;
        }

        public async Task<Guid> CreateFarm(Domain.Entities.Farm farm)
        {
            var createdFarm = await _farmRepository.CreateAsync(farm);

            if (createdFarm != null)
                return createdFarm.Id;

            return Guid.Empty;
        }

        public async Task<IReadOnlyCollection<Domain.Entities.Farm>> GetFarms(string sortField, string sortDirection = "asc", int pageIndex = 0, int pageSize = 10)
        {
            var query = _farmRepository.QueryAll().AsNoTracking();

            if (sortDirection.ToLower().Equals("asc"))
            {
                switch (sortField)
                {
                    case "Name":
                        return await query.OrderBy(p => p.Name).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
                    case "OwnerName":
                        return await query.OrderBy(p => p.OwnerName).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
                    case "Description":
                        return await query.OrderBy(p => p.Description).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
                    default:
                        return await query.OrderBy(p => p.Name).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
                };
            }
            else
            {
                switch (sortField)
                {
                    case "Name":
                        return await query.OrderByDescending(p => p.Name).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
                    case "OwnerName":
                        return await query.OrderByDescending(p => p.OwnerName).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
                    case "Description":
                        return await query.OrderByDescending(p => p.Description).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
                    default:
                        return await query.OrderByDescending(p => p.Name).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
                };
            }
        }

        public async Task<int> CountTotalRecords()
        {
            return await _farmRepository.CountTotalRecordsAsync();
        }

        public async Task<Domain.Entities.Farm> GetFarm(Guid farmId)
        {
            return await _farmRepository.GetAsync(x => x.Id == farmId);
        }

        public async Task<Domain.Entities.Farm> UpdateFarm(Domain.Entities.Farm farm)
        {
            var result = await _farmRepository.UpdateAsync(farm);
            return result;
        }

        public async Task<IReadOnlyCollection<Domain.Entities.Farm>> GetActiveFarms()
        {
            var query = _farmRepository.QueryMany(x=>x.IsActive == true).AsNoTracking();
            return await query.ToListAsync();
        }
    }
}
