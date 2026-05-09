using Farm.Business.Services.Interfaces;
using Farm.Domain.Entities;
using Farm.Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Farm.Business.Services
{
    public class AnimalService : IAnimalService
    {
        private readonly IAnimalRepository _animalRepository;

        public AnimalService(IAnimalRepository animalRepository)
        {
            _animalRepository = animalRepository;
        }

        public async Task<Guid> CreateAnimal(Animal Animal)
        {
            var createdAnimal = await _animalRepository.CreateAsync(Animal);

            if (createdAnimal != null)
                return createdAnimal.Id;

            return Guid.Empty;
        }

        public async Task<IReadOnlyCollection<Animal>> GetAnimals(string sortField, string sortDirection = "asc", int pageIndex = 0, int pageSize = 10)
        {
            var query = _animalRepository.QueryAll()
                .Include(x=>x.Cage)
                .ThenInclude(x=>x.Farm)
                .AsNoTracking();

            if (sortDirection.ToLower().Equals("asc"))
            {
                switch (sortField)
                {
                    case "Name":
                        return await query.OrderBy(p => p.Name).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
                    case "Code":
                        return await query.OrderBy(p => p.Code).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
                    default:
                        return await query.OrderBy(p => p.Code).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
                };
            }
            else
            {
                switch (sortField)
                {
                    case "Name":
                        return await query.OrderByDescending(p => p.Name).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
                    case "Code":
                        return await query.OrderByDescending(p => p.Code).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
                    default:
                        return await query.OrderByDescending(p => p.Code).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();
                };
            }
        }

        public async Task<int> CountTotalRecords()
        {
            return await _animalRepository.CountTotalRecordsAsync();
        }

        public async Task<Animal> GetAnimal(Guid AnimalId)
        {
            return await _animalRepository.GetAsync(x=> x.Id == AnimalId);
        }

        public async Task<Animal> UpdateAnimal(Animal Animal)
        {
            var result = await _animalRepository.UpdateAsync(Animal);
            return result;
        }
    }
}
