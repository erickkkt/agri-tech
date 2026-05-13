using Farm.Business.Services.Interfaces;
using Farm.Domain.Entities;
using Farm.Domain.FarmDbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        private readonly FarmDbContext _db;

        public InvestmentController(IInvestmentService service, IUserService userService, FarmDbContext db) : base(userService)
        {
            _service = service;
            _db = db;
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

        // ---------- Commitment / bank transfer ----------

        /// <summary>Single order detail (owner only).</summary>
        [HttpGet("orders/{id:guid}")]
        public async Task<ActionResult<InvestmentOrder>> GetOrder(Guid id)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();

            var order = await _db.InvestmentOrders.AsNoTracking()
                .Include(o => o.Offer).ThenInclude(o => o.Animal)
                .Include(o => o.Offer).ThenInclude(o => o.Farm)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();
            if (order.InvestorUserId != user.Id) return Forbid();
            return Ok(order);
        }

        /// <summary>
        /// Returns the investment commitment "contract" — bank transfer instructions
        /// for the investor + a denormalised view of all parties (farm owner, investor,
        /// animal, amount, profit ratio, expected harvest). Frontend renders this as
        /// a printable cam-kết document.
        /// </summary>
        [HttpGet("orders/{id:guid}/commitment")]
        public async Task<ActionResult<CommitmentDto>> GetCommitment(Guid id)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();

            var order = await _db.InvestmentOrders.AsNoTracking()
                .Include(o => o.Offer).ThenInclude(o => o.Animal)
                .Include(o => o.Offer).ThenInclude(o => o.Farm)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();
            if (order.InvestorUserId != user.Id) return Forbid();

            var farm = order.Offer?.Farm;
            var animal = order.Offer?.Animal;

            return Ok(new CommitmentDto
            {
                OrderId = order.Id,
                CreatedAt = order.CreatedAt,
                Status = order.Status.ToString(),
                TransferReference = order.TransferReference,
                BankTransferConfirmedAt = order.BankTransferConfirmedAt,

                TotalAmount = order.TotalAmount,
                Currency = "VND",
                ProfitRatio = order.Offer?.ProfitRatio ?? 0,
                ExpectedHarvestDate = order.Offer?.ExpectedHarvestDate,
                OfferTitle = order.Offer?.Title,
                OfferDescription = order.Offer?.Description,

                Investor = new PartyDto
                {
                    UserId = order.InvestorUserId,
                    Name = order.InvestorUserName ?? user.UserName,
                },
                Farm = new FarmPartyDto
                {
                    Id = farm?.Id ?? Guid.Empty,
                    Name = farm?.Name,
                    OwnerName = farm?.OwnerName,
                    Location = farm?.Location,
                    BankName = farm?.BankName,
                    BankAccountNumber = farm?.BankAccountNumber,
                    BankAccountHolder = farm?.BankAccountHolder,
                    BankBranch = farm?.BankBranch,
                },
                Animal = new AnimalRefDto
                {
                    Id = animal?.Id ?? Guid.Empty,
                    Code = animal?.Code,
                    Name = animal?.Name,
                    Species = animal?.Species.ToString(),
                    Weight = animal?.Weight ?? 0,
                    DateOfBirth = animal?.DateOfBirth,
                }
            });
        }

        /// <summary>
        /// Admin endpoint — mark an order's bank transfer as confirmed. Triggers
        /// `ConfirmOrder` which moves the order from Pending → Confirmed and
        /// issues the share certificate.
        /// </summary>
        [HttpPost("orders/{id:guid}/confirm-bank-transfer")]
        public async Task<ActionResult> ConfirmBankTransfer(Guid id)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();

            var order = await _db.InvestmentOrders.FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();

            order.BankTransferConfirmedAt = DateTime.UtcNow;
            order.ChangedByUserId = user.Id;
            order.ChangedByUserName = user.UserName;
            order.ChangedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            // Auto-confirm so investor gets the certificate
            await _service.ConfirmOrder(id);
            return Ok();
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

    // ---------- Commitment DTOs ----------

    public class CommitmentDto
    {
        public Guid OrderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public string TransferReference { get; set; }
        public DateTime? BankTransferConfirmedAt { get; set; }

        public decimal TotalAmount { get; set; }
        public string Currency { get; set; }
        public decimal ProfitRatio { get; set; }
        public DateTime? ExpectedHarvestDate { get; set; }
        public string OfferTitle { get; set; }
        public string OfferDescription { get; set; }

        public PartyDto Investor { get; set; }
        public FarmPartyDto Farm { get; set; }
        public AnimalRefDto Animal { get; set; }
    }

    public class PartyDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
    }

    public class FarmPartyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string OwnerName { get; set; }
        public string Location { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankAccountHolder { get; set; }
        public string BankBranch { get; set; }
    }

    public class AnimalRefDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Species { get; set; }
        public float Weight { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
