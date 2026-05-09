using AutoMapper;
using Farm.Business.Services.Interfaces;
using Farm.Domain.Entities;
using Farm.Domain.ViewModels.Disease;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Farm.Api.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/diseases")]
    [Produces("application/json")]
    [ApiController]
    public class DiseaseController : FarmBaseController
    {
        private readonly IDiseaseService _service;
        private readonly IMapper _mapper;

        public DiseaseController(IDiseaseService service, IMapper mapper, IUserService userService) : base(userService)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet("animal/{animalId:guid}")]
        public async Task<ActionResult<IEnumerable<DiseaseRecordDto>>> GetByAnimal(Guid animalId)
        {
            var list = await _service.GetByAnimal(animalId);
            return Ok(_mapper.Map<List<DiseaseRecordDto>>(list));
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateDiseaseRecordDto dto)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = _mapper.Map<DiseaseRecord>(dto);
            entity.ChangedByUserId = user.Id;
            entity.ChangedByUserName = user.UserName;
            return Ok(await _service.Create(entity));
        }

        [HttpPut]
        public async Task<ActionResult<DiseaseRecordDto>> Update([FromBody] UpdateDiseaseRecordDto dto)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = _mapper.Map<DiseaseRecord>(dto);
            entity.ChangedByUserId = user.Id;
            entity.ChangedByUserName = user.UserName;
            var updated = await _service.Update(entity);
            return Ok(_mapper.Map<DiseaseRecordDto>(updated));
        }

        [HttpPost("treatments")]
        public async Task<ActionResult<Guid>> CreateTreatment([FromBody] CreateTreatmentDto dto)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = _mapper.Map<Treatment>(dto);
            entity.ChangedByUserId = user.Id;
            entity.ChangedByUserName = user.UserName;
            return Ok(await _service.CreateTreatment(entity));
        }

        [HttpGet("treatments/animal/{animalId:guid}")]
        public async Task<ActionResult<IEnumerable<TreatmentDto>>> TreatmentsByAnimal(Guid animalId)
        {
            var list = await _service.GetTreatmentsByAnimal(animalId);
            return Ok(_mapper.Map<List<TreatmentDto>>(list));
        }
    }
}
