using Farm.Business.Services.Interfaces;
using Farm.Domain.Entities;
using Farm.Domain.Enum;
using Farm.Domain.FarmDbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Farm.Api.Controllers
{
    /// <summary>
    /// Marketplace miscellaneous endpoints: reviews, farm verification, shipping partners.
    /// </summary>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/marketplace")]
    [Produces("application/json")]
    [ApiController]
    public class MarketplaceController : FarmBaseController
    {
        private readonly FarmDbContext _db;

        public MarketplaceController(FarmDbContext db, IUserService userService) : base(userService) { _db = db; }

        // ---------- Reviews ----------

        [AllowAnonymous]
        [HttpGet("reviews/{farmId:guid}")]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviews(Guid farmId)
        {
            return Ok(await _db.Reviews.AsNoTracking()
                .Where(r => r.TargetFarmId == farmId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync());
        }

        [HttpPost("reviews")]
        public async Task<ActionResult<Guid>> CreateReview([FromBody] Review review)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            review.Id = Guid.Empty;
            review.ReviewerUserId = user.Id;
            review.CreatedAt = DateTime.UtcNow;
            review.ChangedByUserId = user.Id;
            review.ChangedByUserName = user.UserName;
            _db.Reviews.Add(review);
            await _db.SaveChangesAsync();
            return Ok(review.Id);
        }

        // ---------- Farm Verification ----------

        [HttpGet("verifications/{farmId:guid}")]
        public async Task<ActionResult<FarmVerification>> GetVerification(Guid farmId)
        {
            var v = await _db.FarmVerifications.AsNoTracking().FirstOrDefaultAsync(x => x.FarmId == farmId);
            return v == null ? NotFound() : Ok(v);
        }

        [HttpPost("verifications/submit")]
        public async Task<ActionResult<Guid>> Submit([FromBody] FarmVerification payload)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            payload.Id = Guid.Empty;
            payload.Status = FarmVerificationStatus.Pending;
            payload.SubmittedAt = DateTime.UtcNow;
            _db.FarmVerifications.Add(payload);
            await _db.SaveChangesAsync();
            return Ok(payload.Id);
        }

        [HttpPost("verifications/{id:guid}/decide")]
        public async Task<ActionResult> Decide(Guid id, [FromQuery] bool approve, [FromQuery] string notes = null)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            var v = await _db.FarmVerifications.FirstOrDefaultAsync(x => x.Id == id);
            if (v == null) return NotFound();
            v.Status = approve ? FarmVerificationStatus.Verified : FarmVerificationStatus.Rejected;
            v.VerifiedAt = DateTime.UtcNow;
            v.VerifiedByUserId = user.Id;
            v.ReviewerNotes = notes;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // ---------- Shipping Partners ----------

        [AllowAnonymous]
        [HttpGet("shipping-partners")]
        public async Task<ActionResult<IEnumerable<ShippingPartner>>> ShippingPartners(
            [FromQuery] string province = null, [FromQuery] string species = null)
        {
            var qry = _db.ShippingPartners.AsNoTracking().Where(s => s.IsActive).AsQueryable();
            if (!string.IsNullOrWhiteSpace(province))
                qry = qry.Where(s => s.ServiceArea.Contains(province));
            if (!string.IsNullOrWhiteSpace(species))
                qry = qry.Where(s => s.AnimalTypesSupported.Contains(species));
            return Ok(await qry.ToListAsync());
        }
    }
}
