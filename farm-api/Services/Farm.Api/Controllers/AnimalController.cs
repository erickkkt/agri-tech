using AutoMapper;
using Farm.Business.Services.Interfaces;
using Farm.Domain.ViewModels.Animal;
using Farm.Domain.ViewModels.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Farm.Api.Controllers
{
    /// <summary>
    /// Animal Controller
    /// </summary>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/animals")]
    [Produces("application/json")]
    [ApiController]
    public class AnimalController : FarmBaseController
    {
        private readonly IAnimalService _animalService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Animal Controller class
        /// </summary>
        public AnimalController(IAnimalService animalService, IMapper mapper, IUserService userService) : base(userService)
        {
            _animalService = animalService;
            _mapper = mapper;
        }

        #region Admin     

        /// <summary>
        /// Get Animals with paging and sort field
        /// </summary>
        /// <param name="sortField"></param>
        /// <param name="sortDirection"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("paging/{sortField}/{sortDirection}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<IReadOnlyCollection<AnimalDetailsDto>>> GetAnimals(string sortField, string sortDirection = "asc", int pageIndex = 0, int pageSize = 10)
        {
            PaginationResponseDto<AnimalDetailsDto> responseDto = new PaginationResponseDto<AnimalDetailsDto>();

            var countTotal = await _animalService.CountTotalRecords();
            var animals = await _animalService.GetAnimals(sortField, sortDirection, pageIndex, pageSize);

            responseDto.Items = _mapper.Map<List<AnimalDetailsDto>>(animals);
            responseDto.Total = countTotal;

            return Ok(responseDto);
        }

        /// <summary>
        /// Create a new Animal
        /// </summary>
        /// <param name="animalDto"></param>
        /// <returns>A single Animal id when successful</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CreateAnimalDto), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Guid>> CreateAnimal([FromBody] CreateAnimalDto animalDto)
        {
            var user = await AuthorizedUser;
            
            if (user == null)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(animalDto);
            }

            var animal = _mapper.Map<Domain.Entities.Animal>(animalDto);
            
            animal.ChangedByUserId = user.Id;
            animal.ChangedByUserName = user.UserName;
            
            var id = await _animalService.CreateAnimal(animal);

            return Ok(id);
        }

        /// <summary>
        /// Update an existing Animal
        /// </summary>
        /// <param name="animalDto"></param>
        /// <returns>The updated Animal when successful</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CreateAnimalDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AnimalDetailsDto>> UpdateAnimal([FromBody] UpdateAnimalDto animalDto)
        {
            var user = await AuthorizedUser;

            if (user == null)
            {
                return Unauthorized();
            }

            if (animalDto == null)
            {
                return BadRequest(animalDto);
            }

            var animal = _mapper.Map<Domain.Entities.Animal>(animalDto);

            animal.ChangedByUserId = user.Id;
            animal.ChangedByUserName = user.UserName;

            var updated = await _animalService.UpdateAnimal(animal);

            return Ok(_mapper.Map<AnimalDetailsDto>(updated));
        }
        #endregion
    }
}
