
using AutoMapper;
using Farm.Api.Controllers;
using Farm.Business.Services.Interfaces;
using Farm.Domain.Entities;
using Farm.Domain.ViewModels.Animal;
using Farm.Domain.ViewModels.Paging;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Farm.TestApi
{
    public class AnimalControllerTest
    {
        [Fact]
        public async Task GetAnimals_ShouldReturnOkWithPagedAnimals()
        {
            var serviceMock = new Mock<IAnimalService>();
            var mapperMock = new Mock<IMapper>();
            var userServiceMock = new Mock<IUserService>();

            serviceMock.Setup(s => s.CountTotalRecords()).ReturnsAsync(1);
            serviceMock.Setup(s => s.GetAnimals("Name", "asc", 0, 10)).ReturnsAsync(new List<Animal> { new Animal { Name = "Lion" } });
            mapperMock.Setup(m => m.Map<List<AnimalDetailsDto>>(It.IsAny<List<Animal>>())).Returns(new List<AnimalDetailsDto> { new AnimalDetailsDto { Name = "Lion" } });
            
            var controller = new AnimalController(serviceMock.Object, mapperMock.Object, userServiceMock.Object);
            var result = await controller.GetAnimals("Name", "asc", 0, 10);
            
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<PaginationResponseDto<AnimalDetailsDto>>(okResult.Value);
            Assert.Single(dto.Items);
            Assert.Equal("Lion", dto.Items.First().Name);
        }

        [Fact]
        public async Task GetAnimals_ShouldReturnEmpty_WhenNoAnimals()
        {
            var serviceMock = new Mock<IAnimalService>();
            var mapperMock = new Mock<IMapper>();
            var userServiceMock = new Mock<IUserService>();

            serviceMock.Setup(s => s.CountTotalRecords()).ReturnsAsync(0);
            serviceMock.Setup(s => s.GetAnimals("Name", "asc", 0, 10)).ReturnsAsync(new List<Animal>());
            mapperMock.Setup(m => m.Map<List<AnimalDetailsDto>>(It.IsAny<List<Animal>>())).Returns(new List<AnimalDetailsDto>());
            
            var controller = new AnimalController(serviceMock.Object, mapperMock.Object, userServiceMock.Object);
            var result = await controller.GetAnimals("Name", "asc", 0, 10);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<PaginationResponseDto<AnimalDetailsDto>>(okResult.Value);
            Assert.Empty(dto.Items);
            Assert.Equal(0, dto.Total);
        }

        [Fact]
        public async Task CreateAnimal_ShouldReturnOk_WhenModelIsValid()
        {
            var serviceMock = new Mock<IAnimalService>();
            var mapperMock = new Mock<IMapper>();
            var userServiceMock = new Mock<IUserService>();
            var animalDto = new CreateAnimalDto { Name = "Lion" };
            var animal = new Animal { Id = Guid.NewGuid(), Name = "Lion" };

            mapperMock.Setup(m => m.Map<Animal>(animalDto)).Returns(animal);
            serviceMock.Setup(s => s.CreateAnimal(animal)).ReturnsAsync(animal.Id);

            var controller = new AnimalController(serviceMock.Object, mapperMock.Object, userServiceMock.Object);
            var result = await controller.CreateAnimal(animalDto);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(animal.Id, okResult.Value);
        }

        [Fact]
        public async Task CreateAnimal_ShouldReturnBadRequest_WhenModelIsInvalid()
        {
            var serviceMock = new Mock<IAnimalService>();
            var mapperMock = new Mock<IMapper>();
            var userServiceMock = new Mock<IUserService>();
            var animalDto = new CreateAnimalDto { Name = null };
            var controller = new AnimalController(serviceMock.Object, mapperMock.Object, userServiceMock.Object);

            controller.ModelState.AddModelError("Name", "Required");
            var result = await controller.CreateAnimal(animalDto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(animalDto, badRequest.Value);
        }
    }
}
