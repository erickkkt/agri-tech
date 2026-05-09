
using Farm.Domain.FarmDbContexts;
using Farm.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Farm.TestDomain
{
    public class FarmRepositoryTest
    {
        private FarmDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<FarmDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new FarmDbContext(options);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddFarm()
        {
            using var context = GetInMemoryDbContext();
            var repo = new FarmRepository(context);
            var farm = new Domain.Entities.Farm
            {
                Name = "Test Farm",
                OwnerName = "Owner",
                Description = "A test farm",
                Location = "Test Location",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            var result = await repo.CreateAsync(farm);
            Assert.NotNull(result);
            Assert.Equal("Test Farm", result.Name);
            Assert.Single(context.Farms);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnFarm()
        {
            using var context = GetInMemoryDbContext();
            var repo = new FarmRepository(context);
            var farm = new Domain.Entities.Farm
            {
                Name = "Farm2",
                OwnerName = "Owner2",
                Description = "Second farm",
                Location = "Location2",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            await repo.CreateAsync(farm);
            var found = await repo.GetAsync(f => f.Name == "Farm2");
            Assert.NotNull(found);
            Assert.Equal("Farm2", found.Name);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyFarm()
        {
            using var context = GetInMemoryDbContext();
            var repo = new FarmRepository(context);
            var farm = new Domain.Entities.Farm
            {
                Name = "Farm3",
                OwnerName = "Owner3",
                Description = "Third farm",
                Location = "Location3",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            var created = await repo.CreateAsync(farm);
            created.Name = "Updated Farm3";
            var updated = await repo.UpdateAsync(created);
            Assert.Equal("Updated Farm3", updated.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveFarm()
        {
            using var context = GetInMemoryDbContext();
            var repo = new FarmRepository(context);
            var farm = new Domain.Entities.Farm
            {
                Name = "Farm4",
                OwnerName = "Owner4",
                Description = "Fourth farm",
                Location = "Location4",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            var created = await repo.CreateAsync(farm);
            var deleted = await repo.DeleteAsync(created);
            Assert.True(deleted);
            Assert.Empty(context.Farms);
        }

        [Fact]
        public async Task CountTotalRecordsAsync_ShouldReturnCorrectCount()
        {
            using var context = GetInMemoryDbContext();
            var repo = new FarmRepository(context);
            await repo.CreateAsync(new Domain.Entities.Farm
            {
                Name = "Farm5",
                OwnerName = "Owner5",
                Description = "Fifth farm",
                Location = "Location5",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            });
            await repo.CreateAsync(new Domain.Entities.Farm
            {
                Name = "Farm6",
                OwnerName = "Owner6",
                Description = "Sixth farm",
                Location = "Location6",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            });
            var count = await repo.CountTotalRecordsAsync();
            Assert.Equal(2, count);
        }
    }
}
