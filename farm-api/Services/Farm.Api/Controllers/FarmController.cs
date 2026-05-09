using AutoMapper;
using Farm.Business.Services.Interfaces;
using Farm.Domain.ViewModels.Farm;
using Farm.Domain.ViewModels.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Farm.Api.Controllers
{
    /// <summary>
    /// Farm Controller
    /// </summary>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/farms")]
    [Produces("application/json")]
    [ApiController]
    public class FarmController : FarmBaseController
    {
        private readonly IFarmService _farmService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Farm Controller class
        /// </summary>
        public FarmController(IFarmService farmService, IMapper mapper, IUserService userService) : base(userService)
        {
            _farmService = farmService;
            _mapper = mapper;
        }

        #region Admin     

        /// <summary>
        /// Get all active Farms
        /// </summary>
        /// <returns></returns>
        [HttpGet("active")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyCollection<FarmDetailsDto>>> GetActiveFarms()
        {
            var farms = await _farmService.GetActiveFarms();
            if (farms == null || !farms.Any())
            {
                return NotFound();
            }
            return Ok(_mapper.Map<List<FarmDetailsDto>>(farms));
        }

        /// <summary>
        /// Get Farms with paging and sort field
        /// </summary>
        /// <param name="sortField"></param>
        /// <param name="sortDirection"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("paging/{sortField}/{sortDirection}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<IReadOnlyCollection<FarmDetailsDto>>> GetFarms(string sortField, string sortDirection = "asc", int pageIndex = 0, int pageSize = 10)
        {
            PaginationResponseDto<FarmDetailsDto> responseDto = new PaginationResponseDto<FarmDetailsDto>();

            var countTotal = await _farmService.CountTotalRecords();
            var farms = await _farmService.GetFarms(sortField, sortDirection, pageIndex, pageSize);

            responseDto.Items = _mapper.Map<List<FarmDetailsDto>>(farms);
            responseDto.Total = countTotal;

            return Ok(responseDto);
        }

        /// <summary>
        /// Create a new Farm
        /// </summary>
        /// <param name="farmDto"></param>
        /// <returns>A single Farm id when successful</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CreateFarmDto), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Guid>> CreateFarm([FromBody] CreateFarmDto farmDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(farmDto);
            }

            var farm = _mapper.Map<Domain.Entities.Farm>(farmDto);
            var id = await _farmService.CreateFarm(farm);

            return Ok(id);
        }

        /// <summary>
        /// Update an existing Farm
        /// </summary>
        /// <param name="farmDto"></param>
        /// <returns>The updated Farm when successful</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CreateFarmDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<FarmDetailsDto>> UpdateFarm([FromBody] UpdateFarmDto farmDto)
        {
            if (farmDto == null)
            {
                return BadRequest(farmDto);
            }

            var farm = _mapper.Map<Domain.Entities.Farm>(farmDto);
            var updated = await _farmService.UpdateFarm(farm);

            return Ok(_mapper.Map<FarmDetailsDto>(updated));
        }
        #endregion
    }
}
