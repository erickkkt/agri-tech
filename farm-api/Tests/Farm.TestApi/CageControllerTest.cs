
using AutoMapper;
using Farm.Api.Controllers;
using Farm.Business.Services.Interfaces;
using Farm.Domain.Entities;
using Farm.Domain.ViewModels.Cage;
using Farm.Domain.ViewModels.Paging;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Farm.TestApi
{
    public class CageControllerTest
    {
        [Fact]
        public async Task GetCages_ShouldReturnOkWithPagedCages()
        {
            var serviceMock = new Mock<ICageService>();
            var mapperMock = new Mock<IMapper>();
            var userServiceMock = new Mock<IUserService>();

            serviceMock.Setup(s => s.CountTotalRecords()).ReturnsAsync(1);
            serviceMock.Setup(s => s.GetCages("Name", "asc", 0, 10)).ReturnsAsync(new List<Cage> { new Cage { Name = "Cage1" } });
            mapperMock.Setup(m => m.Map<List<CageDetailsDto>>(It.IsAny<List<Cage>>())).Returns(new List<CageDetailsDto> { new CageDetailsDto { Name = "Cage1" } });
            var controller = new CageController(serviceMock.Object, mapperMock.Object, userServiceMock.Object);

            var result = await controller.GetCages("Name", "asc", 0, 10);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<PaginationResponseDto<CageDetailsDto>>(okResult.Value);

            Assert.Single(dto.Items);
            Assert.Equal("Cage1", dto.Items.First().Name);
        }

        [Fact]
        public async Task GetCages_ShouldReturnEmpty_WhenNoCages()
        {
            var serviceMock = new Mock<ICageService>();
            var mapperMock = new Mock<IMapper>();
            var userServiceMock = new Mock<IUserService>();

            serviceMock.Setup(s => s.CountTotalRecords()).ReturnsAsync(0);
            serviceMock.Setup(s => s.GetCages("Name", "asc", 0, 10)).ReturnsAsync(new List<Cage>());
            mapperMock.Setup(m => m.Map<List<CageDetailsDto>>(It.IsAny<List<Cage>>())).Returns(new List<CageDetailsDto>());

            var controller = new CageController(serviceMock.Object, mapperMock.Object, userServiceMock.Object);
            var result = await controller.GetCages("Name", "asc", 0, 10);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var dto = Assert.IsType<PaginationResponseDto<CageDetailsDto>>(okResult.Value);

            Assert.Empty(dto.Items);
            Assert.Equal(0, dto.Total);
        }

        [Fact]
        public async Task CreateCage_ShouldReturnOk_WhenModelIsValid()
        {
            var serviceMock = new Mock<ICageService>();
            var mapperMock = new Mock<IMapper>();
            var userServiceMock = new Mock<IUserService>();

            var cageDto = new CreateCageDto { Name = "Cage1" };
            var cage = new Cage { Id = Guid.NewGuid(), Name = "Cage1" };
            mapperMock.Setup(m => m.Map<Cage>(cageDto)).Returns(cage);
            serviceMock.Setup(s => s.CreateCage(cage)).ReturnsAsync(cage.Id);
            var controller = new CageController(serviceMock.Object, mapperMock.Object, userServiceMock.Object);

            var result = await controller.CreateCage(cageDto);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(cage.Id, okResult.Value);
        }

        [Fact]
        public async Task CreateCage_ShouldReturnBadRequest_WhenModelIsInvalid()
        {
            var serviceMock = new Mock<ICageService>();
            var mapperMock = new Mock<IMapper>();
            var userServiceMock = new Mock<IUserService>();

            var cageDto = new CreateCageDto { Name = null };
            var controller = new CageController(serviceMock.Object, mapperMock.Object, userServiceMock.Object);

            controller.ModelState.AddModelError("Name", "Required");
            var result = await controller.CreateCage(cageDto);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);

            Assert.Equal(cageDto, badRequest.Value);
        }
    }
}
