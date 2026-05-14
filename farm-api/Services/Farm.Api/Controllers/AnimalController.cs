using AutoMapper;
using Farm.Business.Services.Interfaces;
using Farm.Domain.Entities;
using Farm.Domain.Enum;
using Farm.Domain.FarmDbContexts;
using Farm.Domain.ViewModels.Animal;
using Farm.Domain.ViewModels.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Farm.Api.Controllers
{
    /// <summary>
    /// Animal Controller
    /// </summary>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/animals")]
    [Produces("application/json")]
    [ApiController]
    public class AnimalController : FarmBaseController
    {
        private readonly IAnimalService _animalService;
        private readonly IMapper _mapper;
        private readonly FarmDbContext _db;

        /// <summary>
        /// Animal Controller class
        /// </summary>
        public AnimalController(IAnimalService animalService, IMapper mapper, IUserService userService, FarmDbContext db) : base(userService)
        {
            _animalService = animalService;
            _mapper = mapper;
            _db = db;
        }

        /// <summary>
        /// Suggested VND price per kg by species. Used to auto-fill the Sell dialog.
        /// Admin can override; this is a starting estimate.
        /// </summary>
        private static readonly IReadOnlyDictionary<Species, decimal> PricePerKgBySpecies = new Dictionary<Species, decimal>
        {
            [Species.Deer]  = 350_000m,
            [Species.Sheep] = 200_000m,
            [Species.Cow]   = 180_000m
        };
        private const decimal DefaultPricePerKg = 200_000m;

        #region Admin     

        /// <summary>
        /// Get Animals with paging and sort field
        /// </summary>
        /// <param name="sortField"></param>
        /// <param name="sortDirection"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("paging/{sortField}/{sortDirection}/{pageNumber}/{pageSize}")]
        public async Task<ActionResult<IReadOnlyCollection<AnimalDetailsDto>>> GetAnimals(string sortField, string sortDirection = "asc", int pageIndex = 0, int pageSize = 10)
        {
            PaginationResponseDto<AnimalDetailsDto> responseDto = new PaginationResponseDto<AnimalDetailsDto>();

            var countTotal = await _animalService.CountTotalRecords();
            var animals = await _animalService.GetAnimals(sortField, sortDirection, pageIndex, pageSize);

            responseDto.Items = _mapper.Map<List<AnimalDetailsDto>>(animals);
            responseDto.Total = countTotal;

            return Ok(responseDto);
        }

        /// <summary>
        /// Create a new Animal
        /// </summary>
        /// <param name="animalDto"></param>
        /// <returns>A single Animal id when successful</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CreateAnimalDto), StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<Guid>> CreateAnimal([FromBody] CreateAnimalDto animalDto)
        {
            var user = await AuthorizedUser;
            
            if (user == null)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(animalDto);
            }

            var animal = _mapper.Map<Domain.Entities.Animal>(animalDto);
            
            animal.ChangedByUserId = user.Id;
            animal.ChangedByUserName = user.UserName;
            
            var id = await _animalService.CreateAnimal(animal);

            return Ok(id);
        }

        /// <summary>
        /// Update an existing Animal
        /// </summary>
        /// <param name="animalDto"></param>
        /// <returns>The updated Animal when successful</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CreateAnimalDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AnimalDetailsDto>> UpdateAnimal([FromBody] UpdateAnimalDto animalDto)
        {
            var user = await AuthorizedUser;

            if (user == null)
            {
                return Unauthorized();
            }

            if (animalDto == null)
            {
                return BadRequest(animalDto);
            }

            var animal = _mapper.Map<Domain.Entities.Animal>(animalDto);

            animal.ChangedByUserId = user.Id;
            animal.ChangedByUserName = user.UserName;

            var updated = await _animalService.UpdateAnimal(animal);

            return Ok(_mapper.Map<AnimalDetailsDto>(updated));
        }

        /// <summary>
        /// Get a single animal by id (full detail incl. cage/farm).
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AnimalDetailsDto>> GetAnimalById(Guid id)
        {
            var animal = await _animalService.GetAnimal(id);
            if (animal == null) return NotFound();
            return Ok(_mapper.Map<AnimalDetailsDto>(animal));
        }

        /// <summary>
        /// Returns a suggested marketplace price for the animal based on weight × species coefficient.
        /// Admin can override in the Sell dialog.
        /// </summary>
        [HttpGet("{id:guid}/price-suggestion")]
        public async Task<ActionResult<PriceSuggestionDto>> GetPriceSuggestion(Guid id)
        {
            var animal = await _animalService.GetAnimal(id);
            if (animal == null) return NotFound();
            var rate = PricePerKgBySpecies.TryGetValue(animal.Species, out var r) ? r : DefaultPricePerKg;
            var suggested = (decimal)animal.Weight * rate;
            return Ok(new PriceSuggestionDto
            {
                Weight = animal.Weight,
                Species = animal.Species,
                PricePerKg = rate,
                SuggestedPrice = Math.Round(suggested, 0),
                Currency = "VND"
            });
        }

        /// <summary>
        /// Create a marketplace Listing for this animal. The listing immediately becomes
        /// visible to end-users on farm-user/marketplace.
        /// </summary>
        [HttpPost("{id:guid}/list-for-sale")]
        public async Task<ActionResult<Guid>> ListForSale(Guid id, [FromBody] ListAnimalForSaleDto dto)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            if (dto == null) return BadRequest();

            var animal = await _animalService.GetAnimal(id);
            if (animal == null) return NotFound();

            var listing = new Listing
            {
                AnimalId = animal.Id,
                FarmId = animal.Cage?.FarmId ?? dto.FarmId,
                SellerUserId = user.Id,
                Title = string.IsNullOrWhiteSpace(dto.Title)
                    ? $"{animal.Name} ({animal.Code})"
                    : dto.Title,
                Description = dto.Description ?? animal.Description,
                Category = dto.Category,
                Species = animal.Species,
                Status = ListingStatus.Active,
                Price = dto.Price,
                Currency = string.IsNullOrWhiteSpace(dto.Currency) ? "VND" : dto.Currency,
                Quantity = dto.Quantity <= 0 ? 1 : dto.Quantity,
                Unit = string.IsNullOrWhiteSpace(dto.Unit) ? "con" : dto.Unit,
                Province = dto.Province,
                CreatedAt = DateTime.UtcNow,
                ChangedAt = DateTime.UtcNow,
                ChangedByUserId = user.Id,
                ChangedByUserName = user.UserName
            };

            _db.Listings.Add(listing);
            await _db.SaveChangesAsync();

            if (dto.PhotoUrls != null)
            {
                int order = 0;
                foreach (var url in dto.PhotoUrls.Where(u => !string.IsNullOrWhiteSpace(u)))
                {
                    _db.ListingPhotos.Add(new ListingPhoto { ListingId = listing.Id, Url = url, Order = order++ });
                }
                await _db.SaveChangesAsync();
            }

            return Ok(listing.Id);
        }

        /// <summary>
        /// Create an InvestmentOffer for this animal. The offer becomes visible on
        /// farm-user/investments so end-users can buy shares.
        /// </summary>
        [HttpPost("{id:guid}/open-investment")]
        public async Task<ActionResult<Guid>> OpenInvestment(Guid id, [FromBody] OpenInvestmentDto dto)
        {
            var user = await AuthorizedUser;
            if (user == null) return Unauthorized();
            if (dto == null) return BadRequest();
            if (dto.TotalShares <= 0) return BadRequest(new { error = "TotalShares must be > 0" });
            if (dto.PricePerShare <= 0) return BadRequest(new { error = "PricePerShare must be > 0" });
            if (dto.ProfitRatio < 0 || dto.ProfitRatio > 1) return BadRequest(new { error = "ProfitRatio must be in [0, 1]" });

            var animal = await _animalService.GetAnimal(id);
            if (animal == null) return NotFound();

            // Resolve FarmId from the animal (preferred) or fall back to dto.
            // Without this, a missing FarmId would surface as a confusing FK violation at SaveChanges.
            var farmId = animal.Cage?.FarmId ?? dto.FarmId;
            if (farmId == Guid.Empty)
                return BadRequest(new { error = "Không xác định được trang trại của vật nuôi này." });

            // Block duplicate offers on the same animal (DB also has a partial unique index on Status=Open)
            var existingOpen = await _db.InvestmentOffers
                .AnyAsync(o => o.AnimalId == id && o.Status == InvestmentOfferStatus.Open);
            if (existingOpen)
                return BadRequest(new { error = "Vật nuôi này đã có offer đang mở. Đóng offer cũ trước." });

            var offer = new InvestmentOffer
            {
                AnimalId = animal.Id,
                FarmId = farmId,
                Title = string.IsNullOrWhiteSpace(dto.Title) ? $"Đầu tư vào {animal.Name} ({animal.Code})" : dto.Title,
                Description = dto.Description ?? animal.Description,
                TotalShares = dto.TotalShares,
                AvailableShares = dto.TotalShares,
                PricePerShare = dto.PricePerShare,
                ProfitRatio = dto.ProfitRatio,
                ExpectedHarvestDate = dto.ExpectedHarvestDate,
                Status = InvestmentOfferStatus.Open,
                CreatedAt = DateTime.UtcNow,
                ChangedAt = DateTime.UtcNow,
                ChangedByUserId = user.Id,
                ChangedByUserName = user.UserName
            };

            _db.InvestmentOffers.Add(offer);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                // Race condition or stale data slipped past the AnyAsync check.
                return BadRequest(new { error = "Vật nuôi này đã có offer đang mở. Đóng offer cũ trước." });
            }
            return Ok(offer.Id);
        }

        /// <summary>True if the EF update exception was a Postgres unique-constraint violation (SQLSTATE 23505).</summary>
        private static bool IsUniqueViolation(DbUpdateException ex)
            => ex.InnerException is Npgsql.PostgresException pg && pg.SqlState == "23505";

        /// <summary>History of marketplace listings created for this animal.</summary>
        [HttpGet("{id:guid}/sales-history")]
        public async Task<ActionResult<IEnumerable<Listing>>> GetSalesHistory(Guid id)
        {
            return Ok(await _db.Listings.AsNoTracking()
                .Where(l => l.AnimalId == id)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync());
        }

        /// <summary>History of investment offers created for this animal.</summary>
        [HttpGet("{id:guid}/investment-history")]
        public async Task<ActionResult<IEnumerable<InvestmentOffer>>> GetInvestmentHistory(Guid id)
        {
            return Ok(await _db.InvestmentOffers.AsNoTracking()
                .Where(o => o.AnimalId == id)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync());
        }

        #endregion
    }

    // ---------- DTOs ----------

    public class PriceSuggestionDto
    {
        public float Weight { get; set; }
        public Species Species { get; set; }
        public decimal PricePerKg { get; set; }
        public decimal SuggestedPrice { get; set; }
        public string Currency { get; set; } = "VND";
    }

    public class ListAnimalForSaleDto
    {
        // All reference-type fields are nullable on purpose: the project enables
        // <Nullable>enable</Nullable>, which combined with [ApiController] makes
        // non-nullable reference types REQUIRED in [FromBody] binding. The frontend
        // omits empty strings (province, description, …) and that would otherwise
        // trip automatic model-state validation → 400 Bad Request before the
        // controller body runs.
        public Guid FarmId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public ListingCategory Category { get; set; } = ListingCategory.Breeding;
        public decimal Price { get; set; }
        public string? Currency { get; set; } = "VND";
        public int Quantity { get; set; } = 1;
        public string? Unit { get; set; } = "con";
        public string? Province { get; set; }
        public List<string>? PhotoUrls { get; set; }
    }

    public class OpenInvestmentDto
    {
        // See note on ListAnimalForSaleDto — keep reference-type fields nullable
        // so the request is not rejected by [ApiController] validation when the
        // frontend omits optional fields.
        public Guid FarmId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int TotalShares { get; set; }
        public decimal PricePerShare { get; set; }
        /// <summary>0..1, e.g. 0.7 = investors get 70% of harvest revenue.</summary>
        public decimal ProfitRatio { get; set; }
        public DateTime? ExpectedHarvestDate { get; set; }
    }
}
