using AutoMapper;
using Farm.Business.Services.Interfaces;
using Farm.Domain.Entities;
using Farm.Domain.ViewModels.GrowthLog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Farm.Api.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/growth-logs")]
    [Produces("application/json")]
    [ApiController]
    public class GrowthLogController : FarmBaseController
    {
        private readonly IGrowthLogService _service;
        private readonly IMapper _mapper;

        public GrowthLogController(IGrowthLogService service, IMapper mapper, IUserService userService) : base(userService)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet("animal/{animalId:guid}")]
        public async Task<ActionResult<IEnumerable<GrowthLogDto>>> GetByAnimal(Guid animalId, [FromQuery] int take = 100)
        {
            var list = await _service.GetByAnimal(animalId, take);
            return Ok(_mapper.Map<List<GrowthLogDto>>(list));
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateGrowthLogDto dto)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = _mapper.Map<GrowthLog>(dto);
            entity.ChangedByUserId = user.Id;
            entity.ChangedByUserName = user.UserName;
            return Ok(await _service.Create(entity));
        }

        [HttpPut]
        public async Task<ActionResult<GrowthLogDto>> Update([FromBody] UpdateGrowthLogDto dto)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = _mapper.Map<GrowthLog>(dto);
            entity.ChangedByUserId = user.Id;
            entity.ChangedByUserName = user.UserName;
            var updated = await _service.Update(entity);
            return Ok(_mapper.Map<GrowthLogDto>(updated));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            var ok = await _service.Delete(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
