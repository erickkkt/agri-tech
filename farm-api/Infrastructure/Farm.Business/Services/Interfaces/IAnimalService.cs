
using Farm.Domain.Entities;

namespace Farm.Business.Services.Interfaces
{
    public interface IAnimalService
    {
        Task<Guid> CreateAnimal(Animal animal);
        Task<Animal> UpdateAnimal(Animal animal);
        Task<Animal> GetAnimal(Guid animal);
        Task<int> CountTotalRecords();
        Task<IReadOnlyCollection<Animal>> GetAnimals(string sortField, string sortDirection = "asc", int pageIndex = 0, int pageSize = 10);
    }
}
