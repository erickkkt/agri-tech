
using Farm.Business.Services;
using Farm.Domain.Repositories.Interfaces;
using Moq;

namespace Farm.TestBusiness
{
    public class FarmServiceTest
    {
        [Fact]
        public async Task CreateFarm_ShouldReturnId_WhenCreated()
        {
            var repoMock = new Mock<IFarmRepository>();
            var farm = new Domain.Entities.Farm { Id = Guid.NewGuid(), Name = "Test Farm" };
            repoMock.Setup(r => r.CreateAsync(farm)).ReturnsAsync(farm);
            var service = new FarmService(repoMock.Object);
            var result = await service.CreateFarm(farm);
            Assert.Equal(farm.Id, result);
        }

        [Fact]
        public async Task GetFarm_ShouldReturnFarm()
        {
            var repoMock = new Mock<IFarmRepository>();
            var farm = new Domain.Entities.Farm { Id = Guid.NewGuid(), Name = "Farm2" };
            repoMock.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.Farm, bool>>>())).ReturnsAsync(farm);
            var service = new FarmService(repoMock.Object);
            var result = await service.GetFarm(farm.Id);
            Assert.NotNull(result);
            Assert.Equal("Farm2", result.Name);
        }

        [Fact]
        public async Task UpdateFarm_ShouldReturnUpdatedFarm()
        {
            var repoMock = new Mock<IFarmRepository>();
            var farm = new Domain.Entities.Farm { Id = Guid.NewGuid(), Name = "Farm3" };
            repoMock.Setup(r => r.UpdateAsync(farm)).ReturnsAsync(farm);
            var service = new FarmService(repoMock.Object);
            var result = await service.UpdateFarm(farm);
            Assert.Equal(farm, result);
        }

        [Fact]
        public async Task CountTotalRecords_ShouldReturnCount()
        {
            var repoMock = new Mock<IFarmRepository>();
            repoMock.Setup(r => r.CountTotalRecordsAsync()).ReturnsAsync(3);
            var service = new FarmService(repoMock.Object);
            var result = await service.CountTotalRecords();
            Assert.Equal(3, result);
        }
    }
}
