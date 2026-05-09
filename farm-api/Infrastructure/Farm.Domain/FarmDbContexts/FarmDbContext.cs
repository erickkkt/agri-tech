using Farm.Domain.Attibutes;
using Farm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;

namespace Farm.Domain.FarmDbContexts
{
    public class FarmDbContext : DbContext
    {
        public FarmDbContext(DbContextOptions<FarmDbContext> options) : base(options)
        {
        }

        public DbSet<Animal> Animals { get; set; }
        public DbSet<Cage> Cages { get; set; }
        public DbSet<Entities.Farm> Farms { get; set; }
        public DbSet<Audit> Audits { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        // Phase 1 - Farm Management extension
        public DbSet<Vaccine> Vaccines { get; set; }
        public DbSet<VaccineSchedule> VaccineSchedules { get; set; }
        public DbSet<DiseaseRecord> DiseaseRecords { get; set; }
        public DbSet<Treatment> Treatments { get; set; }
        public DbSet<FeedItem> FeedItems { get; set; }
        public DbSet<FeedTransaction> FeedTransactions { get; set; }
        public DbSet<FeedConsumption> FeedConsumptions { get; set; }
        public DbSet<GrowthLog> GrowthLogs { get; set; }
        public DbSet<Alert> Alerts { get; set; }

        // Phase 2 - Marketplace
        public DbSet<Listing> Listings { get; set; }
        public DbSet<ListingPhoto> ListingPhotos { get; set; }
        public DbSet<ForumThread> ForumThreads { get; set; }
        public DbSet<ForumPost> ForumPosts { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<FarmVerification> FarmVerifications { get; set; }
        public DbSet<ShippingPartner> ShippingPartners { get; set; }

        // Phase 3 - Investment
        public DbSet<InvestmentOffer> InvestmentOffers { get; set; }
        public DbSet<InvestmentOrder> InvestmentOrders { get; set; }
        public DbSet<ShareCertificate> ShareCertificates { get; set; }
        public DbSet<AnimalUpdate> AnimalUpdates { get; set; }
        public DbSet<CameraFeed> CameraFeeds { get; set; }
        public DbSet<HarvestEvent> HarvestEvents { get; set; }
        public DbSet<ProfitDistribution> ProfitDistributions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Animal>().ToTable("Animal");
            modelBuilder.Entity<Cage>().ToTable("Cage");
            modelBuilder.Entity<Entities.Farm>().ToTable("Farm");
            modelBuilder.Entity<Audit>().ToTable("Audit");
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Role>().ToTable("Role");

            // Phase 1 entities
            modelBuilder.Entity<Vaccine>().ToTable("Vaccine");
            modelBuilder.Entity<VaccineSchedule>().ToTable("VaccineSchedule");
            modelBuilder.Entity<DiseaseRecord>().ToTable("DiseaseRecord");
            modelBuilder.Entity<Treatment>().ToTable("Treatment");
            modelBuilder.Entity<FeedItem>().ToTable("FeedItem");
            modelBuilder.Entity<FeedTransaction>().ToTable("FeedTransaction");
            modelBuilder.Entity<FeedConsumption>().ToTable("FeedConsumption");
            modelBuilder.Entity<GrowthLog>().ToTable("GrowthLog");
            modelBuilder.Entity<Alert>().ToTable("Alert");

            // Phase 2 entities
            modelBuilder.Entity<Listing>().ToTable("Listing");
            modelBuilder.Entity<ListingPhoto>().ToTable("ListingPhoto");
            modelBuilder.Entity<ForumThread>().ToTable("ForumThread");
            modelBuilder.Entity<ForumPost>().ToTable("ForumPost");
            modelBuilder.Entity<Review>().ToTable("Review");
            modelBuilder.Entity<FarmVerification>().ToTable("FarmVerification");
            modelBuilder.Entity<ShippingPartner>().ToTable("ShippingPartner");

            modelBuilder.Entity<Listing>()
                .HasIndex(l => new { l.Province, l.Species, l.Status });
            modelBuilder.Entity<Listing>()
                .HasIndex(l => l.FarmId);
            modelBuilder.Entity<ForumThread>()
                .HasIndex(t => new { t.Category, t.LastReplyAt });
            modelBuilder.Entity<ForumPost>()
                .HasIndex(p => new { p.ThreadId, p.CreatedAt });
            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.TargetFarmId, r.CreatedAt });
            modelBuilder.Entity<FarmVerification>()
                .HasIndex(v => v.FarmId).IsUnique();

            // Phase 3 entities
            modelBuilder.Entity<InvestmentOffer>().ToTable("InvestmentOffer");
            modelBuilder.Entity<InvestmentOrder>().ToTable("InvestmentOrder");
            modelBuilder.Entity<ShareCertificate>().ToTable("ShareCertificate");
            modelBuilder.Entity<AnimalUpdate>().ToTable("AnimalUpdate");
            modelBuilder.Entity<CameraFeed>().ToTable("CameraFeed");
            modelBuilder.Entity<HarvestEvent>().ToTable("HarvestEvent");
            modelBuilder.Entity<ProfitDistribution>().ToTable("ProfitDistribution");

            modelBuilder.Entity<InvestmentOffer>()
                .HasIndex(o => new { o.Status, o.CreatedAt });
            modelBuilder.Entity<InvestmentOffer>()
                .HasIndex(o => o.AnimalId).IsUnique();
            modelBuilder.Entity<InvestmentOrder>()
                .HasIndex(o => new { o.OfferId, o.InvestorUserId });
            modelBuilder.Entity<InvestmentOrder>()
                .HasIndex(o => o.InvestorUserId);
            modelBuilder.Entity<AnimalUpdate>()
                .HasIndex(u => new { u.AnimalId, u.RecordedAt });
            modelBuilder.Entity<HarvestEvent>()
                .HasIndex(h => new { h.AnimalId, h.HarvestDate });
            modelBuilder.Entity<ProfitDistribution>()
                .HasIndex(p => new { p.HarvestEventId, p.InvestorUserId });


            // create indexes for tables
            modelBuilder.Entity<Animal>()
                .HasIndex(p => new { p.Name, p.Code })
                .IsUnique(true);
            modelBuilder.Entity<Cage>()
                .HasIndex(a => new { a.Name })
                .IsUnique(true);
            modelBuilder.Entity<Audit>()
                .HasIndex(a => new { a.EntityName, a.PrimaryKeyValue });

            // Phase 1 indexes
            modelBuilder.Entity<Vaccine>()
                .HasIndex(v => v.Name).IsUnique();
            modelBuilder.Entity<VaccineSchedule>()
                .HasIndex(v => new { v.AnimalId, v.ScheduledDate });
            modelBuilder.Entity<VaccineSchedule>()
                .HasIndex(v => new { v.Status, v.ScheduledDate });
            modelBuilder.Entity<FeedItem>()
                .HasIndex(f => f.Code).IsUnique();
            modelBuilder.Entity<FeedTransaction>()
                .HasIndex(f => new { f.FarmId, f.FeedItemId, f.TransactionDate });
            modelBuilder.Entity<FeedConsumption>()
                .HasIndex(f => new { f.FeedItemId, f.ConsumedAt });
            modelBuilder.Entity<GrowthLog>()
                .HasIndex(g => new { g.AnimalId, g.RecordedAt });
            modelBuilder.Entity<DiseaseRecord>()
                .HasIndex(d => new { d.AnimalId, d.DiagnosedAt });
            modelBuilder.Entity<Treatment>()
                .HasIndex(t => new { t.AnimalId, t.StartDate });
            modelBuilder.Entity<Alert>()
                .HasIndex(a => new { a.IsRead, a.CreatedAt });
            modelBuilder.Entity<Alert>()
                .HasIndex(a => new { a.FarmId, a.CreatedAt });

            // Avoid multiple cascade paths on Alert (multiple optional FKs to same root tables)
            modelBuilder.Entity<Alert>()
                .HasOne(a => a.Farm).WithMany()
                .HasForeignKey(a => a.FarmId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Alert>()
                .HasOne(a => a.Animal).WithMany()
                .HasForeignKey(a => a.AnimalId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Alert>()
                .HasOne(a => a.FeedItem).WithMany()
                .HasForeignKey(a => a.FeedItemId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Alert>()
                .HasOne(a => a.VaccineSchedule).WithMany()
                .HasForeignKey(a => a.VaccineScheduleId)
                .OnDelete(DeleteBehavior.NoAction);

            // Treatment optionally tied to a DiseaseRecord
            modelBuilder.Entity<Treatment>()
                .HasOne(t => t.DiseaseRecord)
                .WithMany(d => d.Treatments)
                .HasForeignKey(t => t.DiseaseRecordId)
                .OnDelete(DeleteBehavior.SetNull);

            // FeedConsumption can reference Animal OR Cage (both optional)
            modelBuilder.Entity<FeedConsumption>()
                .HasOne(f => f.Animal).WithMany()
                .HasForeignKey(f => f.AnimalId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<FeedConsumption>()
                .HasOne(f => f.Cage).WithMany()
                .HasForeignKey(f => f.CageId)
                .OnDelete(DeleteBehavior.NoAction);

            // default role data
            modelBuilder.Entity<Role>()
                .HasData(
                    new Role()
                    {
                        Id = new Guid("68DFFDA0-B650-44AC-A599-6CDBDFD641E4"),
                        Name = "HO.SYSADMIN",
                        Description = "System Administrator",
                        IsActive = true
                    },
                    new Role()
                    {
                        Id = new Guid("F5C9A1E8-4C89-4E3B-BB8C-7D9A1A2B3C40"),
                        Name = "FARM.OWNER",
                        Description = "Farm Owner",
                        IsActive = true
                    },
                    new Role()
                    {
                        Id = new Guid("F5C9A1E8-4C89-4E3B-BB8C-7D9A1A2B3C41"),
                        Name = "FARM.STAFF",
                        Description = "Farm Staff",
                        IsActive = true
                    },
                    new Role()
                    {
                        Id = new Guid("F5C9A1E8-4C89-4E3B-BB8C-7D9A1A2B3C42"),
                        Name = "VET",
                        Description = "Veterinarian",
                        IsActive = true
                    },
                    new Role()
                    {
                        Id = new Guid("F5C9A1E8-4C89-4E3B-BB8C-7D9A1A2B3C43"),
                        Name = "BUYER",
                        Description = "Marketplace Buyer",
                        IsActive = true
                    },
                    new Role()
                    {
                        Id = new Guid("F5C9A1E8-4C89-4E3B-BB8C-7D9A1A2B3C44"),
                        Name = "INVESTOR",
                        Description = "Animal Investor",
                        IsActive = true
                    });
        }


        public override int SaveChanges()
        {
            KeepTrackChanges();

            return base.SaveChanges();
        }

        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            KeepTrackChanges();

            return await base.SaveChangesAsync(cancellationToken);
        }

        private void KeepTrackChanges()
        {
            ChangeTracker.DetectChanges();
            var modifiedEntityEntries = ChangeTracker.Entries().Where(p => p.State == EntityState.Modified).ToList();
            var time = DateTime.UtcNow;

            foreach (var modifiedEntityEntry in modifiedEntityEntries)
            {
                var modifiedEntityEntryType = modifiedEntityEntry.Entity.GetType();

                var trackEntityChangesAttribute = modifiedEntityEntryType
                                                .GetCustomAttributes(false)
                                                .OfType<TrackAuditAttribute>()
                                                .SingleOrDefault();

                if (trackEntityChangesAttribute == null)
                {
                    continue;
                }

                var changedByUserId = modifiedEntityEntry.CurrentValues["ChangedByUserId"]?.ToString();
                if (!Guid.TryParse(changedByUserId, out var changedByUserIdAsGuid))
                {
                    throw new Exception("ChangedByUserId is not a valid GUID");
                }

                var changedByUserName = modifiedEntityEntry.CurrentValues["ChangedByUserName"]?.ToString() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(changedByUserName))
                {
                    throw new Exception("ChangedByUserName must be supplied");
                }

                //var tableName = modifiedEntityEntry.Metadata.Relational().TableName;
                var tableName = modifiedEntityEntry.Metadata.GetDefaultTableName();

                object primaryKey = null;

                modifiedEntityEntry.OriginalValues.SetValues(modifiedEntityEntry.GetDatabaseValues());

                foreach (var property in modifiedEntityEntry.Properties)
                {
                    var propertyEntityEntryType = modifiedEntityEntryType.GetProperty(property.Metadata.Name);

                    var dontTrackPropertyChangeAttribute = propertyEntityEntryType
                        ?.GetCustomAttributes(false)
                        .OfType<NoTrackAuditAttribute>()
                        .SingleOrDefault();

                    if (dontTrackPropertyChangeAttribute != null)
                    {
                        continue;
                    }

                    if (property.Metadata.IsPrimaryKey())
                    {
                        primaryKey = property.CurrentValue;
                        continue;
                    }

                    if (property.IsModified)
                    {
                        string originalValue;
                        string currentValue;
                        var propertyType = property.Metadata.PropertyInfo?.PropertyType;

                        if (propertyType == typeof(DateTime) ||
                            propertyType == typeof(DateTime?))
                        {
                            originalValue = property.OriginalValue?.ToString();
                            if (!string.IsNullOrWhiteSpace(originalValue))
                            {
                                if (DateTime.TryParseExact(
                                                            originalValue,
                                                            "MM/dd/yyyy HH:mm:ss",
                                                            CultureInfo.InvariantCulture,
                                                            DateTimeStyles.None,
                                                            out var intermediateDate))
                                {
                                    originalValue = intermediateDate.ToString("yyyy-MM-dd HH:mm:ss");
                                }
                            }

                            currentValue = property.CurrentValue?.ToString();
                            if (!string.IsNullOrWhiteSpace(currentValue))
                            {
                                if (DateTime.TryParseExact(
                                                            currentValue,
                                                            "MM/dd/yyyy HH:mm:ss",
                                                            CultureInfo.InvariantCulture,
                                                            DateTimeStyles.None,
                                                            out var intermediateDate))
                                {
                                    currentValue = intermediateDate.ToString("yyyy-MM-dd HH:mm:ss");
                                }
                            }
                        }
                        else
                        {
                            originalValue = property.OriginalValue?.ToString();
                            currentValue = property.CurrentValue?.ToString();
                        }

                        var decimalField = false;
                        var decimalsDiffer = false;
                        if (propertyType == typeof(decimal) ||
                            propertyType == typeof(decimal?))
                        {
                            decimalField = true;

                            if (property.CurrentValue != null &&
                                property.OriginalValue != null &&
                                decimal.Compare(decimal.Parse(originalValue), decimal.Parse(currentValue)) != 0)
                            {
                                decimalsDiffer = true;
                            }
                        }

                        if (decimalField && decimalsDiffer || !decimalField && originalValue != currentValue)
                        {
                            var audit = new Audit
                            {
                                EntityName = tableName,
                                PrimaryKeyValue = new Guid(primaryKey?.ToString()),
                                PropertyName = property.Metadata.Name,
                                OriginalValue = originalValue,
                                CurrentValue = currentValue,
                                ChangedByUserId = changedByUserIdAsGuid,
                                ChangedByUserName = changedByUserName,
                                ChangedDateUtc = time
                            };

                            Audits.Add(audit);
                        }
                    }
                }
            }
        }
    }
}
