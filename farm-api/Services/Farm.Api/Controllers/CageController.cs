using AutoMapper;
using Farm.Business.Services.Interfaces;
using Farm.Domain.ViewModels.Cage;
using Farm.Domain.ViewModels.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Farm.Api.Controllers
{
    /// <summary>
    /// Cage Controller
    /// </summary>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/cages")]
    [Produces("application/json")]
    [ApiController]
    public class CageController : FarmBaseController
    {
        private readonly ICageService _cageService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Cage Controller class
        /// </summary>
        public CageController(ICageService cageService, IMapper mapper, IUserService userService) : base(userService)
        {
            _cageService = cageService;
            _mapper = mapper;
        }

        #region Admin     

        [HttpGet("active")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyCollection<CageDetailsDto>>> GetActiveCages([FromQuery] string farmId)
        {
            if (!Guid.TryParse(farmId, out var farmGuid))
            {
                return BadRequest(farmId);
            }
            var cages = await _cageService.GetActiveCages(farmGuid);
            if (cages == null || !cages.Any())
            {
                return NotFound();
            }
            return Ok(_mapper.Map<List<CageDetailsDto>>(cages));
        }

        /// <summary>
        /// Get Cages with paging and sort field
        /// </summary>
        /// <param name="sortField"></param>
        /// <param name="sortDirection"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("paging/{sortField}/{sortDirection}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<IReadOnlyCollection<CageDetailsDto>>> GetCages(string sortField, string sortDirection = "asc", int pageIndex = 0, int pageSize = 10)
        {
            PaginationResponseDto<CageDetailsDto> responseDto = new PaginationResponseDto<CageDetailsDto>();

            var countTotal = await _cageService.CountTotalRecords();
            var cages = await _cageService.GetCages(sortField, sortDirection, pageIndex, pageSize);

            responseDto.Items = _mapper.Map<List<CageDetailsDto>>(cages);
            responseDto.Total = countTotal;

            return Ok(responseDto);
        }

        /// <summary>
        /// Create a new Cage
        /// </summary>
        /// <param name="CageDto"></param>
        /// <returns>A single Cage id when successful</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CreateCageDto), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Guid>> CreateCage([FromBody] CreateCageDto cageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(cageDto);
            }

            var cage = _mapper.Map<Domain.Entities.Cage>(cageDto);
            var id = await _cageService.CreateCage(cage);

            return Ok(id);
        }

        /// <summary>
        /// Update an existing Cage
        /// </summary>
        /// <param name="CageDto"></param>
        /// <returns>The updated Cage when successful</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CreateCageDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CageDetailsDto>> UpdateCage([FromBody] UpdateCageDto cageDto)
        {
            if (cageDto == null)
            {
                return BadRequest(cageDto);
            }

            var cage = _mapper.Map<Domain.Entities.Cage>(cageDto);
            var updated = await _cageService.UpdateCage(cage);

            return Ok(_mapper.Map<CageDetailsDto>(updated));
        }
        #endregion
    }
}
