
using Farm.Business.Services;
using Farm.Domain.Entities;
using Farm.Domain.Repositories.Interfaces;
using Moq;

namespace Farm.TestBusiness
{
    public class CageServiceTest
    {
        [Fact]
        public async Task CreateCage_ShouldReturnId_WhenCreated()
        {
            var repoMock = new Mock<ICageRepository>();
            var cage = new Cage { Id = Guid.NewGuid(), Name = "Test Cage" };
            repoMock.Setup(r => r.CreateAsync(cage)).ReturnsAsync(cage);
            var service = new CageService(repoMock.Object);
            var result = await service.CreateCage(cage);
            Assert.Equal(cage.Id, result);
        }

        [Fact]
        public async Task GetCage_ShouldReturnCage()
        {
            var repoMock = new Mock<ICageRepository>();
            var cage = new Cage { Id = Guid.NewGuid(), Name = "Cage2" };
            repoMock.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Cage, bool>>>())).ReturnsAsync(cage);
            var service = new CageService(repoMock.Object);
            var result = await service.GetCage(cage.Id);
            Assert.NotNull(result);
            Assert.Equal("Cage2", result.Name);
        }

        [Fact]
        public async Task UpdateCage_ShouldReturnUpdatedCage()
        {
            var repoMock = new Mock<ICageRepository>();
            var cage = new Cage { Id = Guid.NewGuid(), Name = "Cage3" };
            repoMock.Setup(r => r.UpdateAsync(cage)).ReturnsAsync(cage);
            var service = new CageService(repoMock.Object);
            var result = await service.UpdateCage(cage);
            Assert.Equal(cage, result);
        }

        [Fact]
        public async Task CountTotalRecords_ShouldReturnCount()
        {
            var repoMock = new Mock<ICageRepository>();
            repoMock.Setup(r => r.CountTotalRecordsAsync()).ReturnsAsync(7);
            var service = new CageService(repoMock.Object);
            var result = await service.CountTotalRecords();
            Assert.Equal(7, result);
        }
    }
}
