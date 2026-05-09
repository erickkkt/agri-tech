using AutoMapper;
using Farm.Business.Services.Interfaces;
using Farm.Domain.Entities;
using Farm.Domain.ViewModels.Feed;
using Farm.Domain.ViewModels.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Farm.Api.Controllers
{
    /// <summary>Feed inventory: items, in/out transactions, consumption, summaries.</summary>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/feed")]
    [Produces("application/json")]
    [ApiController]
    public class FeedController : FarmBaseController
    {
        private readonly IFeedService _feedService;
        private readonly IMapper _mapper;

        public FeedController(IFeedService feedService, IMapper mapper, IUserService userService) : base(userService)
        {
            _feedService = feedService;
            _mapper = mapper;
        }

        // ---------- Items ----------

        [HttpGet("items/paging/{pageIndex:int}/{pageSize:int}")]
        public async Task<ActionResult<PaginationResponseDto<FeedItemDto>>> GetItems(int pageIndex, int pageSize)
        {
            var items = await _feedService.GetFeedItems(pageIndex, pageSize);
            var total = await _feedService.CountFeedItems();
            return Ok(new PaginationResponseDto<FeedItemDto>
            {
                Items = _mapper.Map<List<FeedItemDto>>(items),
                Total = total
            });
        }

        [HttpGet("items/{id:guid}")]
        public async Task<ActionResult<FeedItemDto>> GetItem(Guid id)
        {
            var item = await _feedService.GetFeedItem(id);
            if (item == null) return NotFound();
            return Ok(_mapper.Map<FeedItemDto>(item));
        }

        [HttpPost("items")]
        public async Task<ActionResult<Guid>> CreateItem([FromBody] CreateFeedItemDto dto)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var entity = _mapper.Map<FeedItem>(dto);
            return Ok(await _feedService.CreateFeedItem(entity));
        }

        [HttpPut("items")]
        public async Task<ActionResult<FeedItemDto>> UpdateItem([FromBody] UpdateFeedItemDto dto)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var entity = _mapper.Map<FeedItem>(dto);
            var updated = await _feedService.UpdateFeedItem(entity);
            return Ok(_mapper.Map<FeedItemDto>(updated));
        }

        // ---------- Transactions ----------

        [HttpPost("transactions")]
        public async Task<ActionResult<Guid>> CreateTransaction([FromBody] CreateFeedTransactionDto dto)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = _mapper.Map<FeedTransaction>(dto);
            entity.CreatedByUserId = user.Id;
            entity.CreatedByUserName = user.UserName;
            try
            {
                return Ok(await _feedService.CreateTransaction(entity));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("transactions")]
        public async Task<ActionResult<IEnumerable<FeedTransactionDto>>> GetTransactions(
            [FromQuery] Guid? farmId, [FromQuery] Guid? feedItemId,
            [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var list = await _feedService.GetTransactions(farmId, feedItemId, from, to);
            return Ok(_mapper.Map<List<FeedTransactionDto>>(list));
        }

        // ---------- Consumption ----------

        [HttpPost("consumptions")]
        public async Task<ActionResult<Guid>> CreateConsumption([FromBody] CreateFeedConsumptionDto dto)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var entity = _mapper.Map<FeedConsumption>(dto);
            try
            {
                return Ok(await _feedService.CreateConsumption(entity));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("consumptions")]
        public async Task<ActionResult<IEnumerable<FeedConsumptionDto>>> GetConsumptions(
            [FromQuery] Guid? animalId, [FromQuery] Guid? cageId,
            [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var list = await _feedService.GetConsumptions(animalId, cageId, from, to);
            return Ok(_mapper.Map<List<FeedConsumptionDto>>(list));
        }

        // ---------- Summary ----------

        [HttpGet("summary/{farmId:guid}")]
        public async Task<ActionResult<FeedSummaryDto>> Summary(Guid farmId)
            => Ok(await _feedService.GetSummary(farmId));

        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<FeedStockLevelDto>>> LowStock()
            => Ok(await _feedService.GetLowStockItems());
    }
}
