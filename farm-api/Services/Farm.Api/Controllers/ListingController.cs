using AutoMapper;
using Farm.Business.Services.Interfaces;
using Farm.Domain.Entities;
using Farm.Domain.ViewModels.Listing;
using Farm.Domain.ViewModels.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Farm.Api.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/listings")]
    [Produces("application/json")]
    [ApiController]
    public class ListingController : FarmBaseController
    {
        private readonly IListingService _service;
        private readonly IMapper _mapper;

        public ListingController(IListingService service, IMapper mapper, IUserService userService) : base(userService)
        {
            _service = service;
            _mapper = mapper;
        }

        /// <summary>Public-style search across active listings.</summary>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<PaginationResponseDto<ListingDto>>> Search([FromQuery] ListingSearchDto query)
        {
            var (items, total) = await _service.Search(query ?? new ListingSearchDto());
            return Ok(new PaginationResponseDto<ListingDto>
            {
                Items = items.Select(l => new ListingDto
                {
                    Id = l.Id, FarmId = l.FarmId, FarmName = l.Farm?.Name,
                    SellerUserId = l.SellerUserId, Title = l.Title, Description = l.Description,
                    Category = l.Category, Species = l.Species, Status = l.Status,
                    Price = l.Price, Currency = l.Currency, Quantity = l.Quantity, Unit = l.Unit,
                    Province = l.Province, CreatedAt = l.CreatedAt,
                    PhotoUrls = l.Photos?.OrderBy(p => p.Order).Select(p => p.Url).ToList() ?? new()
                }).ToList(),
                Total = total
            });
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ListingDto>> Get(Guid id)
        {
            var l = await _service.GetById(id);
            if (l == null) return NotFound();
            return Ok(new ListingDto
            {
                Id = l.Id, FarmId = l.FarmId, FarmName = l.Farm?.Name, Title = l.Title,
                Description = l.Description, Category = l.Category, Species = l.Species, Status = l.Status,
                Price = l.Price, Currency = l.Currency, Quantity = l.Quantity, Unit = l.Unit,
                Province = l.Province, CreatedAt = l.CreatedAt,
                PhotoUrls = l.Photos?.OrderBy(p => p.Order).Select(p => p.Url).ToList() ?? new()
            });
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreateListingDto dto)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = new Listing
            {
                FarmId = dto.FarmId, SellerUserId = user.Id, Title = dto.Title, Description = dto.Description,
                Category = dto.Category, Species = dto.Species, Price = dto.Price, Currency = dto.Currency,
                Quantity = dto.Quantity, Unit = dto.Unit, Province = dto.Province,
                ChangedByUserId = user.Id, ChangedByUserName = user.UserName
            };
            return Ok(await _service.Create(entity, dto.PhotoUrls));
        }

        [HttpPut]
        public async Task<ActionResult<ListingDto>> Update([FromBody] UpdateListingDto dto)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _service.GetById(dto.Id);
            if (existing == null) return NotFound();
            existing.Title = dto.Title; existing.Description = dto.Description;
            existing.Category = dto.Category; existing.Species = dto.Species;
            existing.Price = dto.Price; existing.Currency = dto.Currency;
            existing.Quantity = dto.Quantity; existing.Unit = dto.Unit;
            existing.Province = dto.Province; existing.Status = dto.Status;
            existing.ChangedByUserId = user.Id; existing.ChangedByUserName = user.UserName;
            existing.ChangedAt = DateTime.UtcNow;

            await _service.Update(existing, dto.PhotoUrls);
            return await Get(existing.Id);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            return (await _service.Delete(id)) ? NoContent() : NotFound();
        }
    }
}
