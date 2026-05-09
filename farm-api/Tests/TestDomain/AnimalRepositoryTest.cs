
using Farm.Domain.Entities;
using Farm.Domain.Enum;
using Farm.Domain.FarmDbContexts;
using Farm.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Farm.TestDomain
{
    public class AnimalRepositoryTest
    {
        private FarmDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<FarmDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new FarmDbContext(options);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddAnimal()
        {
            using var context = GetInMemoryDbContext();
            var repo = new AnimalRepository(context);
            var cage = new Cage { Name = "Cage1", Description = "Test Cage Description", Capacity = 5, IsActive = true, CreatedAt = DateTime.UtcNow, FarmId = Guid.NewGuid() };
            context.Cages.Add(cage);
            await context.SaveChangesAsync();

            var animal = new Animal
            {
                Code = "A001",
                Name = "Lion",
                Description = "A strong lion",
                Species = Species.Deer,
                Gender = Gender.Male,
                HealthStatus = HealthStatus.Healthy,
                DateOfBirth = DateTime.UtcNow.AddYears(-2),
                DateOfArrival = DateTime.UtcNow,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CageId = cage.Id,
                ChangedByUserName = "TestUser",
            };
            var result = await repo.CreateAsync(animal);
            Assert.NotNull(result);
            Assert.Equal("Lion", result.Name);
            Assert.Single(context.Animals);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnAnimal()
        {
            using var context = GetInMemoryDbContext();
            var repo = new AnimalRepository(context);
            var cage = new Cage { Name = "Cage2", Description = "Test Cage Description", Capacity = 10, IsActive = true, CreatedAt = DateTime.UtcNow, FarmId = Guid.NewGuid() };
            context.Cages.Add(cage);
            await context.SaveChangesAsync();

            var animal = new Animal
            {
                Code = "A002",
                Name = "Tiger",
                Description = "A fierce tiger",
                Species = Species.Cow,
                Gender = Gender.Female,
                HealthStatus = HealthStatus.Weak,
                DateOfBirth = DateTime.UtcNow.AddYears(-3),
                DateOfArrival = DateTime.UtcNow,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CageId = cage.Id,
                ChangedByUserName = "TestUser",
            };
            await repo.CreateAsync(animal);
            var found = await repo.GetAsync(a => a.Name == "Tiger");
            Assert.NotNull(found);
            Assert.Equal("Tiger", found.Name);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyAnimal()
        {
            using var context = GetInMemoryDbContext();
            var repo = new AnimalRepository(context);
            var cage = new Cage { Name = "Cage3", Description = "Test Cage Description", Capacity = 8, IsActive = true, CreatedAt = DateTime.UtcNow, FarmId = Guid.NewGuid() };
            context.Cages.Add(cage);
            await context.SaveChangesAsync();

            var animal = new Animal
            {
                Code = "A003",
                Name = "Bear",
                Description = "A strong bear",
                Species = Species.Cow,
                Gender = Gender.Male,
                HealthStatus = HealthStatus.Healthy,
                DateOfBirth = DateTime.UtcNow.AddYears(-4),
                DateOfArrival = DateTime.UtcNow,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CageId = cage.Id,
                ChangedByUserId = Guid.NewGuid(),
                ChangedByUserName = "TestUser",
            };
            var created = await repo.CreateAsync(animal);
            created.Name = "Polar Bear";
            var updated = await repo.UpdateAsync(created);
            Assert.Equal("Polar Bear", updated.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveAnimal()
        {
            using var context = GetInMemoryDbContext();
            var repo = new AnimalRepository(context);
            var cage = new Cage { Name = "Cage4", Description = "Test Cage Description", Capacity = 6, IsActive = true, CreatedAt = DateTime.UtcNow, FarmId = Guid.NewGuid() };
            context.Cages.Add(cage);
            await context.SaveChangesAsync();

            var animal = new Animal
            {
                Code = "A004",
                Name = "Wolf",
                Description = "A wild wolf",
                Species = Species.Sheep,
                Gender = Gender.Female,
                HealthStatus = HealthStatus.Healthy,
                DateOfBirth = DateTime.UtcNow.AddYears(-1),
                DateOfArrival = DateTime.UtcNow,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CageId = cage.Id,
                ChangedByUserName = "TestUser",
            };
            var created = await repo.CreateAsync(animal);
            var deleted = await repo.DeleteAsync(created);
            Assert.True(deleted);
            Assert.Empty(context.Animals);
        }

        [Fact]
        public async Task CountTotalRecordsAsync_ShouldReturnCorrectCount()
        {
            using var context = GetInMemoryDbContext();
            var repo = new AnimalRepository(context);
            var cage = new Cage { Name = "Cage5", Description = "Test Cage Description", Capacity = 12, IsActive = true, CreatedAt = DateTime.UtcNow, FarmId = Guid.NewGuid() };
            context.Cages.Add(cage);
            await context.SaveChangesAsync();

            await repo.CreateAsync(new Animal
            {
                Code = "A005",
                Name = "Fox",
                Description = "A cunning fox",
                Species = Species.Cow,
                Gender = Gender.Male,
                HealthStatus = HealthStatus.Healthy,
                DateOfBirth = DateTime.UtcNow.AddYears(-2),
                DateOfArrival = DateTime.UtcNow,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CageId = cage.Id,
                ChangedByUserName = "TestUser",
            });
            await repo.CreateAsync(new Animal
            {
                Code = "A006",
                Name = "Rabbit",
                Description = "A quick Rabbit",
                Species = Species.Cow,
                Gender = Gender.Female,
                HealthStatus = HealthStatus.Healthy,
                DateOfBirth = DateTime.UtcNow.AddYears(-1),
                DateOfArrival = DateTime.UtcNow,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CageId = cage.Id,
                ChangedByUserName = "TestUser",
            });
            var count = await repo.CountTotalRecordsAsync();
            Assert.Equal(2, count);
        }
    }
}
