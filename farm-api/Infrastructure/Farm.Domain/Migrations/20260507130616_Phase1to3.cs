using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Farm.Domain.Migrations
{
    /// <inheritdoc />
    public partial class Phase1to3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnimalUpdate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorUserName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    UpdateType = table.Column<int>(type: "int", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    MediaUrls = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalUpdate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnimalUpdate_Animal_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CameraFeed",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FarmId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    StreamUrl = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AccessRoles = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CameraFeed", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CameraFeed_Cage_CageId",
                        column: x => x.CageId,
                        principalTable: "Cage",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CameraFeed_Farm_FarmId",
                        column: x => x.FarmId,
                        principalTable: "Farm",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DiseaseRecord",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiseaseName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    DiagnosedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DiagnosedBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    RecoveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChangedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiseaseRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiseaseRecord_Animal_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FarmVerification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FarmId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DocumentUrls = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewerNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FarmVerification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FarmVerification_Farm_FarmId",
                        column: x => x.FarmId,
                        principalTable: "Farm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeedItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Manufacturer = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    NutritionInfo = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    LowStockThreshold = table.Column<float>(type: "real", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ForumThread",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    AuthorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorUserName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastReplyAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    PostCount = table.Column<int>(type: "int", nullable: false),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false),
                    IsPinned = table.Column<bool>(type: "bit", nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChangedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumThread", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GrowthLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Weight = table.Column<float>(type: "real", nullable: false),
                    Height = table.Column<float>(type: "real", nullable: true),
                    BodyConditionScore = table.Column<int>(type: "int", nullable: true),
                    PhotoUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChangedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrowthLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GrowthLog_Animal_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvestmentOffer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FarmId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    TotalShares = table.Column<int>(type: "int", nullable: false),
                    AvailableShares = table.Column<int>(type: "int", nullable: false),
                    PricePerShare = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProfitRatio = table.Column<decimal>(type: "decimal(5,4)", nullable: false),
                    ExpectedHarvestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChangedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentOffer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvestmentOffer_Animal_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvestmentOffer_Farm_FarmId",
                        column: x => x.FarmId,
                        principalTable: "Farm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "Listing",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FarmId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SellerUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Species = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Province = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChangedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Listing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Listing_Farm_FarmId",
                        column: x => x.FarmId,
                        principalTable: "Farm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetFarmId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReviewerUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChangedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Review_Farm_TargetFarmId",
                        column: x => x.TargetFarmId,
                        principalTable: "Farm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingPartner",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ServiceArea = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    AnimalTypesSupported = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ContactInfo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingPartner", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vaccine",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Manufacturer = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    RecommendedSpecies = table.Column<int>(type: "int", nullable: true),
                    IntervalDays = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vaccine", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Treatment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiseaseRecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Medication = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Dosage = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AdministeredBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Outcome = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChangedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Treatment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Treatment_Animal_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Treatment_DiseaseRecord_DiseaseRecordId",
                        column: x => x.DiseaseRecordId,
                        principalTable: "DiseaseRecord",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "FeedConsumption",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeedItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ConsumedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Quantity = table.Column<float>(type: "real", nullable: false),
                    GrowthStage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedConsumption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeedConsumption_Animal_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animal",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FeedConsumption_Cage_CageId",
                        column: x => x.CageId,
                        principalTable: "Cage",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FeedConsumption_FeedItem_FeedItemId",
                        column: x => x.FeedItemId,
                        principalTable: "FeedItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeedTransaction",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FeedItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FarmId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<float>(type: "real", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeedTransaction_Farm_FarmId",
                        column: x => x.FarmId,
                        principalTable: "Farm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeedTransaction_FeedItem_FeedItemId",
                        column: x => x.FeedItemId,
                        principalTable: "FeedItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ForumPost",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ThreadId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorUserName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ParentPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Body = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChangedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChangedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumPost", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForumPost_ForumThread_ThreadId",
                        column: x => x.ThreadId,
                        principalTable: "ForumThread",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HarvestEvent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OfferId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HarvestType = table.Column<int>(type: "int", nullable: false),
                    HarvestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GrossRevenue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HarvestEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HarvestEvent_Animal_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HarvestEvent_InvestmentOffer_OfferId",
                        column: x => x.OfferId,
                        principalTable: "InvestmentOffer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InvestmentOrder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OfferId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvestorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvestorUserName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ShareQty = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConfirmedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChangedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChangedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvestmentOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvestmentOrder_InvestmentOffer_OfferId",
                        column: x => x.OfferId,
                        principalTable: "InvestmentOffer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ListingPhoto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListingPhoto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListingPhoto_Listing_ListingId",
                        column: x => x.ListingId,
                        principalTable: "Listing",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VaccineSchedule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VaccineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdministeredDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AdministeredBy = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ReminderSent = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ChangedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaccineSchedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VaccineSchedule_Animal_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VaccineSchedule_Vaccine_VaccineId",
                        column: x => x.VaccineId,
                        principalTable: "Vaccine",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfitDistribution",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HarvestEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvestorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CalculatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfitDistribution", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfitDistribution_HarvestEvent_HarvestEventId",
                        column: x => x.HarvestEventId,
                        principalTable: "HarvestEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShareCertificate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CertificateNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ContractDocumentUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShareCertificate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShareCertificate_InvestmentOrder_OrderId",
                        column: x => x.OrderId,
                        principalTable: "InvestmentOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Alert",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    FarmId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AnimalId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FeedItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VaccineScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Payload = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReadByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alert", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alert_Animal_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animal",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Alert_Farm_FarmId",
                        column: x => x.FarmId,
                        principalTable: "Farm",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Alert_FeedItem_FeedItemId",
                        column: x => x.FeedItemId,
                        principalTable: "FeedItem",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Alert_VaccineSchedule_VaccineScheduleId",
                        column: x => x.VaccineScheduleId,
                        principalTable: "VaccineSchedule",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { new Guid("f5c9a1e8-4c89-4e3b-bb8c-7d9a1a2b3c40"), "Farm Owner", true, "FARM.OWNER" },
                    { new Guid("f5c9a1e8-4c89-4e3b-bb8c-7d9a1a2b3c41"), "Farm Staff", true, "FARM.STAFF" },
                    { new Guid("f5c9a1e8-4c89-4e3b-bb8c-7d9a1a2b3c42"), "Veterinarian", true, "VET" },
                    { new Guid("f5c9a1e8-4c89-4e3b-bb8c-7d9a1a2b3c43"), "Marketplace Buyer", true, "BUYER" },
                    { new Guid("f5c9a1e8-4c89-4e3b-bb8c-7d9a1a2b3c44"), "Animal Investor", true, "INVESTOR" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alert_AnimalId",
                table: "Alert",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_Alert_FarmId_CreatedAt",
                table: "Alert",
                columns: new[] { "FarmId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Alert_FeedItemId",
                table: "Alert",
                column: "FeedItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Alert_IsRead_CreatedAt",
                table: "Alert",
                columns: new[] { "IsRead", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Alert_VaccineScheduleId",
                table: "Alert",
                column: "VaccineScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalUpdate_AnimalId_RecordedAt",
                table: "AnimalUpdate",
                columns: new[] { "AnimalId", "RecordedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CameraFeed_CageId",
                table: "CameraFeed",
                column: "CageId");

            migrationBuilder.CreateIndex(
                name: "IX_CameraFeed_FarmId",
                table: "CameraFeed",
                column: "FarmId");

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseRecord_AnimalId_DiagnosedAt",
                table: "DiseaseRecord",
                columns: new[] { "AnimalId", "DiagnosedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_FarmVerification_FarmId",
                table: "FarmVerification",
                column: "FarmId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeedConsumption_AnimalId",
                table: "FeedConsumption",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedConsumption_CageId",
                table: "FeedConsumption",
                column: "CageId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedConsumption_FeedItemId_ConsumedAt",
                table: "FeedConsumption",
                columns: new[] { "FeedItemId", "ConsumedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_FeedItem_Code",
                table: "FeedItem",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeedTransaction_FarmId_FeedItemId_TransactionDate",
                table: "FeedTransaction",
                columns: new[] { "FarmId", "FeedItemId", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_FeedTransaction_FeedItemId",
                table: "FeedTransaction",
                column: "FeedItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ForumPost_ThreadId_CreatedAt",
                table: "ForumPost",
                columns: new[] { "ThreadId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ForumThread_Category_LastReplyAt",
                table: "ForumThread",
                columns: new[] { "Category", "LastReplyAt" });

            migrationBuilder.CreateIndex(
                name: "IX_GrowthLog_AnimalId_RecordedAt",
                table: "GrowthLog",
                columns: new[] { "AnimalId", "RecordedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_HarvestEvent_AnimalId_HarvestDate",
                table: "HarvestEvent",
                columns: new[] { "AnimalId", "HarvestDate" });

            migrationBuilder.CreateIndex(
                name: "IX_HarvestEvent_OfferId",
                table: "HarvestEvent",
                column: "OfferId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentOffer_AnimalId",
                table: "InvestmentOffer",
                column: "AnimalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentOffer_FarmId",
                table: "InvestmentOffer",
                column: "FarmId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentOffer_Status_CreatedAt",
                table: "InvestmentOffer",
                columns: new[] { "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentOrder_InvestorUserId",
                table: "InvestmentOrder",
                column: "InvestorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_InvestmentOrder_OfferId_InvestorUserId",
                table: "InvestmentOrder",
                columns: new[] { "OfferId", "InvestorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Listing_FarmId",
                table: "Listing",
                column: "FarmId");

            migrationBuilder.CreateIndex(
                name: "IX_Listing_Province_Species_Status",
                table: "Listing",
                columns: new[] { "Province", "Species", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ListingPhoto_ListingId",
                table: "ListingPhoto",
                column: "ListingId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfitDistribution_HarvestEventId_InvestorUserId",
                table: "ProfitDistribution",
                columns: new[] { "HarvestEventId", "InvestorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Review_TargetFarmId_CreatedAt",
                table: "Review",
                columns: new[] { "TargetFarmId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ShareCertificate_OrderId",
                table: "ShareCertificate",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Treatment_AnimalId_StartDate",
                table: "Treatment",
                columns: new[] { "AnimalId", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Treatment_DiseaseRecordId",
                table: "Treatment",
                column: "DiseaseRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_Vaccine_Name",
                table: "Vaccine",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VaccineSchedule_AnimalId_ScheduledDate",
                table: "VaccineSchedule",
                columns: new[] { "AnimalId", "ScheduledDate" });

            migrationBuilder.CreateIndex(
                name: "IX_VaccineSchedule_Status_ScheduledDate",
                table: "VaccineSchedule",
                columns: new[] { "Status", "ScheduledDate" });

            migrationBuilder.CreateIndex(
                name: "IX_VaccineSchedule_VaccineId",
                table: "VaccineSchedule",
                column: "VaccineId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alert");

            migrationBuilder.DropTable(
                name: "AnimalUpdate");

            migrationBuilder.DropTable(
                name: "CameraFeed");

            migrationBuilder.DropTable(
                name: "FarmVerification");

            migrationBuilder.DropTable(
                name: "FeedConsumption");

            migrationBuilder.DropTable(
                name: "FeedTransaction");

            migrationBuilder.DropTable(
                name: "ForumPost");

            migrationBuilder.DropTable(
                name: "GrowthLog");

            migrationBuilder.DropTable(
                name: "ListingPhoto");

            migrationBuilder.DropTable(
                name: "ProfitDistribution");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropTable(
                name: "ShareCertificate");

            migrationBuilder.DropTable(
                name: "ShippingPartner");

            migrationBuilder.DropTable(
                name: "Treatment");

            migrationBuilder.DropTable(
                name: "VaccineSchedule");

            migrationBuilder.DropTable(
                name: "FeedItem");

            migrationBuilder.DropTable(
                name: "ForumThread");

            migrationBuilder.DropTable(
                name: "Listing");

            migrationBuilder.DropTable(
                name: "HarvestEvent");

            migrationBuilder.DropTable(
                name: "InvestmentOrder");

            migrationBuilder.DropTable(
                name: "DiseaseRecord");

            migrationBuilder.DropTable(
                name: "Vaccine");

            migrationBuilder.DropTable(
                name: "InvestmentOffer");

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("f5c9a1e8-4c89-4e3b-bb8c-7d9a1a2b3c40"));

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("f5c9a1e8-4c89-4e3b-bb8c-7d9a1a2b3c41"));

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("f5c9a1e8-4c89-4e3b-bb8c-7d9a1a2b3c42"));

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("f5c9a1e8-4c89-4e3b-bb8c-7d9a1a2b3c43"));

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("f5c9a1e8-4c89-4e3b-bb8c-7d9a1a2b3c44"));
        }
    }
}
