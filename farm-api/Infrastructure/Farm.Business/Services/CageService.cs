using Farm.Business.Services.Interfaces;
using Farm.Domain.Entities;
using Farm.Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Farm.Business.Services
{
    public class CageService : ICageService
    {
        private readonly ICageRepository _cageRepository;

        public CageService(ICageRepository cageRepository)
        {
            _cageRepository = cageRepository;
        }

        public async Task<Guid> CreateCage(Cage cage)
        {
            var createdCage = await _cageRepository.CreateAsync(cage);

            if (createdCage != null)
                return createdCage.Id;

            return Guid.Empty;
        }

        public async Task<IReadOnlyCollection<Cage>> GetCages(string sortField, string sortDirection = "asc", int pageIndex = 0, int pageSize = 10)
        {
            var query = _cageRepository.QueryAll(new List<Expression<Func<Cage, object>>>
                {
                    exp=>exp.Farm
                }).AsNoTracking();

            if (sortDirection.ToLower().Equals("asc"))
            {
                switch (sortField)
                {
                    case "Name":
                        return await query.OrderBy(p => p.Name).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
                    case "Capacity":
                        return await query.OrderBy(p => p.Capacity).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
                    default:
                        return await query.OrderBy(p => p.Name).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
                }
                ;
            }
            else
            {
                switch (sortField)
                {
                    case "Name":
                        return await query.OrderByDescending(p => p.Name).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
                    case "Capacity":
                        return await query.OrderByDescending(p => p.Capacity).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
                    default:
                        return await query.OrderByDescending(p => p.Name).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
                }
                ;
            }
        }

        public async Task<int> CountTotalRecords()
        {
            return await _cageRepository.CountTotalRecordsAsync();
        }

        public async Task<Cage> GetCage(Guid cageId)
        {
            return await _cageRepository.GetAsync(x => x.Id == cageId);
        }

        public async Task<Cage> UpdateCage(Cage cage)
        {
            var result = await _cageRepository.UpdateAsync(cage);
            return result;
        }

        public async Task<IReadOnlyCollection<Cage>> GetActiveCages(Guid farmId)
        {
            var result = await _cageRepository.QueryAll()
                .Where(c => c.FarmId == farmId && c.IsActive)
                .AsNoTracking()
                .ToListAsync();
            return result;
        }
    }
}
