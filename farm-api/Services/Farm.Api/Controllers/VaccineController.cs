using AutoMapper;
using Farm.Business.Services.Interfaces;
using Farm.Domain.Entities;
using Farm.Domain.ViewModels.Paging;
using Farm.Domain.ViewModels.Vaccine;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Farm.Api.Controllers
{
    /// <summary>Manages vaccines and per-animal vaccine schedules.</summary>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/vaccines")]
    [Produces("application/json")]
    [ApiController]
    public class VaccineController : FarmBaseController
    {
        private readonly IVaccineService _vaccineService;
        private readonly IAlertService _alertService;
        private readonly INotificationPublisher _publisher;
        private readonly IMapper _mapper;

        public VaccineController(IVaccineService vaccineService,
                                 IAlertService alertService,
                                 INotificationPublisher publisher,
                                 IMapper mapper,
                                 IUserService userService) : base(userService)
        {
            _vaccineService = vaccineService;
            _alertService = alertService;
            _publisher = publisher;
            _mapper = mapper;
        }

        // ---------- Vaccine catalog ----------

        [HttpGet("paging/{sortField}/{sortDirection}/{pageIndex}/{pageSize}")]
        public async Task<ActionResult<PaginationResponseDto<VaccineDetailsDto>>> GetVaccines(
            string sortField, string sortDirection = "asc", int pageIndex = 0, int pageSize = 10)
        {
            var total = await _vaccineService.CountVaccines();
            var list = await _vaccineService.GetVaccines(sortField, sortDirection, pageIndex, pageSize);
            return Ok(new PaginationResponseDto<VaccineDetailsDto>
            {
                Items = _mapper.Map<List<VaccineDetailsDto>>(list),
                Total = total
            });
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<VaccineDetailsDto>> GetVaccine(Guid id)
        {
            var v = await _vaccineService.GetVaccine(id);
            if (v == null) return NotFound();
            return Ok(_mapper.Map<VaccineDetailsDto>(v));
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateVaccine([FromBody] CreateVaccineDto dto)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = _mapper.Map<Vaccine>(dto);
            return Ok(await _vaccineService.CreateVaccine(entity));
        }

        [HttpPut]
        public async Task<ActionResult<VaccineDetailsDto>> UpdateVaccine([FromBody] UpdateVaccineDto dto)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = _mapper.Map<Vaccine>(dto);
            var updated = await _vaccineService.UpdateVaccine(entity);
            return Ok(_mapper.Map<VaccineDetailsDto>(updated));
        }

        // ---------- Schedules ----------

        [HttpGet("schedules/animal/{animalId:guid}")]
        public async Task<ActionResult<IEnumerable<VaccineScheduleDto>>> GetSchedules(Guid animalId)
        {
            var list = await _vaccineService.GetSchedulesByAnimal(animalId);
            return Ok(_mapper.Map<List<VaccineScheduleDto>>(list));
        }

        [HttpGet("schedules/upcoming/{daysAhead:int}")]
        public async Task<ActionResult<IEnumerable<VaccineScheduleDto>>> GetUpcoming(int daysAhead = 7)
        {
            var list = await _vaccineService.GetUpcomingSchedules(daysAhead);
            return Ok(_mapper.Map<List<VaccineScheduleDto>>(list));
        }

        [HttpPost("schedules")]
        public async Task<ActionResult<Guid>> CreateSchedule([FromBody] CreateVaccineScheduleDto dto)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = _mapper.Map<VaccineSchedule>(dto);
            entity.ChangedByUserId = user.Id;
            entity.ChangedByUserName = user.UserName;
            try
            {
                return Ok(await _vaccineService.CreateSchedule(entity));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("schedules/administer")]
        public async Task<ActionResult<VaccineScheduleDto>> Administer([FromBody] AdministerVaccineDto dto)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var s = await _vaccineService.AdministerSchedule(dto.ScheduleId, dto.AdministeredDate, dto.AdministeredBy, dto.Notes);
                return Ok(_mapper.Map<VaccineScheduleDto>(s));
            }
            catch (KeyNotFoundException) { return NotFound(); }
        }
    }
}
