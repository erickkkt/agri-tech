using Farm.Business.Services.Interfaces;
using Farm.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Farm.Api.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/investment")]
    [Produces("application/json")]
    [ApiController]
    public class InvestmentController : FarmBaseController
    {
        private readonly IInvestmentService _service;

        public InvestmentController(IInvestmentService service, IUserService userService) : base(userService)
        {
            _service = service;
        }

        // ---------- Offers ----------

        [AllowAnonymous]
        [HttpGet("offers")]
        public async Task<ActionResult<IEnumerable<InvestmentOffer>>> GetOpenOffers([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 20)
        {
            var list = await _service.GetOpenOffers(pageIndex, pageSize);
            return Ok(list);
        }

        [AllowAnonymous]
        [HttpGet("offers/{id:guid}")]
        public async Task<ActionResult<InvestmentOffer>> GetOffer(Guid id)
        {
            var o = await _service.GetOfferById(id);
            return o == null ? NotFound() : Ok(o);
        }

        [HttpPost("offers")]
        public async Task<ActionResult<Guid>> CreateOffer([FromBody] InvestmentOffer offer)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            offer.Id = Guid.Empty;
            offer.ChangedByUserId = user.Id;
            offer.ChangedByUserName = user.UserName;
            return Ok(await _service.CreateOffer(offer));
        }

        // ---------- Orders ----------

        [HttpPost("orders")]
        public async Task<ActionResult<Guid>> PlaceOrder([FromBody] InvestmentOrder order)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            order.InvestorUserId = user.Id;
            order.InvestorUserName = user.UserName;
            order.Id = Guid.Empty;
            try
            {
                return Ok(await _service.PlaceOrder(order));
            }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
            catch (KeyNotFoundException) { return NotFound(); }
        }

        [HttpGet("orders/me")]
        public async Task<ActionResult<IEnumerable<InvestmentOrder>>> MyOrders()
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            return Ok(await _service.GetOrdersByInvestor(user.Id));
        }

        [HttpPost("orders/{id:guid}/confirm")]
        public async Task<ActionResult<InvestmentOrder>> Confirm(Guid id)
        {
            try { return Ok(await _service.ConfirmOrder(id)); }
            catch (KeyNotFoundException) { return NotFound(); }
        }

        // ---------- Harvest & Profit ----------

        [HttpPost("harvest-events")]
        public async Task<ActionResult<Guid>> CreateHarvest([FromBody] HarvestEvent evt)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            return Ok(await _service.RecordHarvest(evt));
        }

        [HttpPost("profit-distributions/run/{harvestEventId:guid}")]
        public async Task<ActionResult<int>> Distribute(Guid harvestEventId)
        {
            try { return Ok(await _service.DistributeProfit(harvestEventId)); }
            catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
            catch (KeyNotFoundException) { return NotFound(); }
        }

        // ---------- Animal updates ----------

        [HttpPost("animal-updates")]
        public async Task<ActionResult<Guid>> RecordUpdate([FromBody] AnimalUpdate update)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            update.AuthorUserId = user.Id;
            update.AuthorUserName = user.UserName;
            return Ok(await _service.RecordAnimalUpdate(update));
        }

        [HttpGet("animal-updates/{animalId:guid}")]
        public async Task<ActionResult<IEnumerable<AnimalUpdate>>> GetUpdates(Guid animalId, [FromQuery] int take = 50)
        {
            return Ok(await _service.GetAnimalUpdates(animalId, take));
        }
    }
}
