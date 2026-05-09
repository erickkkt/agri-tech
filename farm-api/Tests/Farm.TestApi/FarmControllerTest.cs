
using AutoMapper;
using Farm.Api.Controllers;
using Farm.Business.Services.Interfaces;
using Farm.Domain.ViewModels.Farm;
using Farm.Domain.ViewModels.Paging;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Farm.TestApi
{
    public class FarmControllerTest
    {
        [Fact]
        public async Task GetFarms_ShouldReturnOkWithPagedFarms()
        {
            var serviceMock = new Mock<IFarmService>();
            var mapperMock = new Mock<IMapper>();
            var userServiceMock = new Mock<IUserService>();
            
            serviceMock.Setup(s => s.CountTotalRecords()).ReturnsAsync(1);
            serviceMock.Setup(s => s.GetFarms("Name", "asc", 0, 10)).ReturnsAsync(new List<Farm.Domain.Entities.Farm> { new Farm.Domain.Entities.Farm { Name = "Test Farm" } });
            mapperMock.Setup(m => m.Map<List<FarmDetailsDto>>(It.IsAny<List<Farm.Domain.Entities.Farm>>())).Returns(new List<FarmDetailsDto> { new FarmDetailsDto { Name = "Test Farm" } });
            
            var controller = new FarmController(serviceMock.Object, mapperMock.Object, userServiceMock.Object);
            var result = await controller.GetFarms("Name", "asc", 0, 10);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<PaginationResponseDto<FarmDetailsDto>>(okResult.Value);
            
            Assert.Single(dto.Items);
            Assert.Equal("Test Farm", dto.Items.First().Name);
        }

        [Fact]
        public async Task GetFarms_ShouldReturnEmpty_WhenNoFarms()
        {
            var serviceMock = new Mock<IFarmService>();
            var mapperMock = new Mock<IMapper>();
            var userServiceMock = new Mock<IUserService>();

            serviceMock.Setup(s => s.CountTotalRecords()).ReturnsAsync(0);
            serviceMock.Setup(s => s.GetFarms("Name", "asc", 0, 10)).ReturnsAsync(new List<Farm.Domain.Entities.Farm>());
            mapperMock.Setup(m => m.Map<List<FarmDetailsDto>>(It.IsAny<List<Farm.Domain.Entities.Farm>>())).Returns(new List<FarmDetailsDto>());

            var controller = new FarmController(serviceMock.Object, mapperMock.Object, userServiceMock.Object);
            var result = await controller.GetFarms("Name", "asc", 0, 10);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<PaginationResponseDto<FarmDetailsDto>>(okResult.Value);

            Assert.Empty(dto.Items);
            Assert.Equal(0, dto.Total);
        }

        [Fact]
        public async Task CreateFarm_ShouldReturnOk_WhenModelIsValid()
        {
            var serviceMock = new Mock<IFarmService>();
            var mapperMock = new Mock<IMapper>();
            var userServiceMock = new Mock<IUserService>();

            var farmDto = new CreateFarmDto { Name = "Farm1" };
            var farm = new Farm.Domain.Entities.Farm { Id = Guid.NewGuid(), Name = "Farm1" };
            mapperMock.Setup(m => m.Map<Farm.Domain.Entities.Farm>(farmDto)).Returns(farm);
            serviceMock.Setup(s => s.CreateFarm(farm)).ReturnsAsync(farm.Id);
            var controller = new FarmController(serviceMock.Object, mapperMock.Object, userServiceMock.Object);

            var result = await controller.CreateFarm(farmDto);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(farm.Id, okResult.Value);
        }

        [Fact]
        public async Task CreateFarm_ShouldReturnBadRequest_WhenModelIsInvalid()
        {
            var serviceMock = new Mock<IFarmService>();
            var mapperMock = new Mock<IMapper>();
            var userServiceMock = new Mock<IUserService>();

            var farmDto = new CreateFarmDto { Name = null };
            var controller = new FarmController(serviceMock.Object, mapperMock.Object, userServiceMock.Object);

            controller.ModelState.AddModelError("Name", "Required");
            var result = await controller.CreateFarm(farmDto);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(farmDto, badRequest.Value);
        }
    }
}
