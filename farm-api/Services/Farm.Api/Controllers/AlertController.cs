using AutoMapper;
using Farm.Business.Services.Interfaces;
using Farm.Domain.ViewModels.Alert;
using Farm.Domain.ViewModels.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Farm.Api.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/alerts")]
    [Produces("application/json")]
    [ApiController]
    public class AlertController : FarmBaseController
    {
        private readonly IAlertService _service;
        private readonly INotificationPublisher _publisher;
        private readonly IMapper _mapper;

        public AlertController(IAlertService service, INotificationPublisher publisher, IMapper mapper, IUserService userService) : base(userService)
        {
            _service = service;
            _publisher = publisher;
            _mapper = mapper;
        }

        [HttpGet("paging/{pageIndex:int}/{pageSize:int}")]
        public async Task<ActionResult<PaginationResponseDto<AlertDto>>> GetAlerts(
            int pageIndex, int pageSize,
            [FromQuery] bool? unread, [FromQuery] Guid? farmId)
        {
            var total = await _service.CountAlerts(unread, farmId);
            var list = await _service.GetAlerts(unread, farmId, pageIndex, pageSize);
            return Ok(new PaginationResponseDto<AlertDto>
            {
                Items = _mapper.Map<List<AlertDto>>(list),
                Total = total
            });
        }

        [HttpGet("summary")]
        public async Task<ActionResult<AlertSummaryDto>> Summary([FromQuery] Guid? farmId)
            => Ok(await _service.GetSummary(farmId));

        [HttpPost("{id:guid}/read")]
        public async Task<ActionResult<AlertDto>> MarkRead(Guid id)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            var alert = await _service.MarkRead(id, user.Id);
            if (alert == null) return NotFound();
            return Ok(_mapper.Map<AlertDto>(alert));
        }

        [HttpPost("read-all")]
        public async Task<ActionResult<int>> MarkAllRead([FromQuery] Guid? farmId)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            return Ok(await _service.MarkAllRead(farmId, user.Id));
        }
    }
}
