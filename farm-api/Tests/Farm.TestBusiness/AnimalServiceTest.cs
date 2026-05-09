
using Farm.Business.Services;
using Farm.Domain.Entities;
using Farm.Domain.Repositories.Interfaces;
using Moq;

namespace Farm.TestBusiness
{
    public class AnimalServiceTest
    {
        [Fact]
        public async Task CreateAnimal_ShouldReturnId_WhenCreated()
        {
            var repoMock = new Mock<IAnimalRepository>();
            var animal = new Animal { Id = Guid.NewGuid(), Name = "Lion", Code = "A001" };
            repoMock.Setup(r => r.CreateAsync(animal)).ReturnsAsync(animal);
            var service = new AnimalService(repoMock.Object);
            var result = await service.CreateAnimal(animal);
            Assert.Equal(animal.Id, result);
        }

        [Fact]
        public async Task GetAnimal_ShouldReturnAnimal()
        {
            var repoMock = new Mock<IAnimalRepository>();
            var animal = new Animal { Id = Guid.NewGuid(), Name = "Tiger", Code = "A002" };
            repoMock.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Animal, bool>>>())).ReturnsAsync(animal);
            var service = new AnimalService(repoMock.Object);
            var result = await service.GetAnimal(animal.Id);
            Assert.NotNull(result);
            Assert.Equal("Tiger", result.Name);
        }

        [Fact]
        public async Task UpdateAnimal_ShouldReturnUpdatedAnimal()
        {
            var repoMock = new Mock<IAnimalRepository>();
            var animal = new Animal { Id = Guid.NewGuid(), Name = "Bear", Code = "A003" };
            repoMock.Setup(r => r.UpdateAsync(animal)).ReturnsAsync(animal);
            var service = new AnimalService(repoMock.Object);
            var result = await service.UpdateAnimal(animal);
            Assert.Equal(animal, result);
        }

        [Fact]
        public async Task CountTotalRecords_ShouldReturnCount()
        {
            var repoMock = new Mock<IAnimalRepository>();
            repoMock.Setup(r => r.CountTotalRecordsAsync()).ReturnsAsync(5);
            var service = new AnimalService(repoMock.Object);
            var result = await service.CountTotalRecords();
            Assert.Equal(5, result);
        }

    }
}
