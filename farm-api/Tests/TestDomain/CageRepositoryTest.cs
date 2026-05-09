
using Microsoft.EntityFrameworkCore.InMemory;
using Farm.Domain.Entities;
using Farm.Domain.FarmDbContexts;
using Farm.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Farm.TestDomain
{
    public class CageRepositoryTest
    {
        private FarmDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<FarmDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new FarmDbContext(options);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddCage()
        {
            using var context = GetInMemoryDbContext();
            var repo = new CageRepository(context);
            var cage = new Cage { Name = "Test Cage", Description = "Test Cage Description", Capacity = 10, IsActive = true, CreatedAt = DateTime.UtcNow, FarmId = Guid.NewGuid() };
            var result = await repo.CreateAsync(cage);
            Assert.NotNull(result);
            Assert.Equal("Test Cage", result.Name);
            Assert.Single(context.Cages);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnCage()
        {
            using var context = GetInMemoryDbContext();
            var repo = new CageRepository(context);
            var cage = new Cage { Name = "Test Cage", Description = "Test Cage Description", Capacity = 10, IsActive = true, CreatedAt = DateTime.UtcNow, FarmId = Guid.NewGuid() };
            await repo.CreateAsync(cage);
            var found = await repo.GetAsync(c => c.Name == "Test Cage");
            Assert.NotNull(found);
            Assert.Equal("Test Cage", found.Name);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyCage()
        {
            using var context = GetInMemoryDbContext();
            var repo = new CageRepository(context);
            var cage = new Cage { Name = "Test Cage", Description = "Test Cage Description", Capacity = 10, IsActive = true, CreatedAt = DateTime.UtcNow, FarmId = Guid.NewGuid() };
            var created = await repo.CreateAsync(cage);
            created.Name = "Updated Cage";
            var updated = await repo.UpdateAsync(created);
            Assert.Equal("Updated Cage", updated.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveCage()
        {
            using var context = GetInMemoryDbContext();
            var repo = new CageRepository(context);
            var cage = new Cage { Name = "Test Cage", Description = "Test Cage Description", Capacity = 10, IsActive = true, CreatedAt = DateTime.UtcNow, FarmId = Guid.NewGuid() };
            var created = await repo.CreateAsync(cage);
            var deleted = await repo.DeleteAsync(created);
            Assert.True(deleted);
            Assert.Empty(context.Cages);
        }

        [Fact]
        public async Task CountTotalRecordsAsync_ShouldReturnCorrectCount()
        {
            using var context = GetInMemoryDbContext();
            var repo = new CageRepository(context);
            await repo.CreateAsync(new Cage { Name = "Cage1", Description = "Test Cage Description", Capacity = 5, IsActive = true, CreatedAt = DateTime.UtcNow, FarmId = Guid.NewGuid() });
            await repo.CreateAsync(new Cage { Name = "Cage2", Description = "Test Cage Description", Capacity = 8, IsActive = true, CreatedAt = DateTime.UtcNow, FarmId = Guid.NewGuid() });
            var count = await repo.CountTotalRecordsAsync();
            Assert.Equal(2, count);
        }
    }
}
