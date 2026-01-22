using MarketPrice.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace MarketPrice.Data
{
    public class MarketPriceDbContext : DbContext
    {
        public DbSet<Commodity> Commodities { get; set; }
        public DbSet<DeliveryDetail> DeliveryDetails { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<LookupDataType> LookupDataTypes { get; set; }
        public DbSet<LookupData> LookupData { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<UnitOfMeasure> UnitOfMeasures { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Verification> Verifications { get; set; }
        public DbSet<CommodityType> CommodityTypes { get; set; }
        public DbSet<UserSecurityDetail> UserSecurityDetails { get; set; }


        public MarketPriceDbContext(DbContextOptions<MarketPriceDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // # Position — use expression overloads so EF binds nav props to the intended FKs
            modelBuilder.Entity<Position>(entity =>
            {
                entity.HasKey(p => p.PositionId);

                entity.HasOne(p => p.User)            // ties to Position.User navigation
                      .WithMany()                     // or .WithMany(u => u.Positions) if you add collection
                      .HasForeignKey(p => p.UserId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.CurrentStatus)   // ties to Position.CurrentStatus navigation
                      .WithMany()
                      .HasForeignKey(p => p.CurrentStatusId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(p => p.Commodity)      // ties to Position.Commodity navigation
                      .WithMany()
                      .HasForeignKey(p => p.CommodityId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.PositionType)   // ties to Position.PositionType navigation
                      .WithMany()
                      .HasForeignKey(p => p.PositionTypeId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.NoAction);

                // one-to-one: DeliveryDetail.Position navigation not present in domain model,
                // so use WithOne() without lambda to avoid referencing a missing nav property.
                entity.HasOne<DeliveryDetail>()
                      .WithOne()
                      .HasForeignKey<DeliveryDetail>(dd => dd.PositionId)
                      .IsRequired();

                entity.Property(p => p.Quantity).HasPrecision(18, 4);
                entity.Property(p => p.UnitPrice).HasPrecision(18, 2);
                entity.Property(p => p.PositionId).HasDefaultValueSql("NEWID()");
            });


            // # User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.IdCardNumber)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne<LookupData>()
                .WithMany()
                .HasForeignKey(u => u.AccountTypeId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.UserId)
                .HasDefaultValueSql("NEWID()");

            // # UserSecurityDetail
            modelBuilder.Entity<UserSecurityDetail>()
                .HasKey(us => us.SecurityId); // Define primary key.

            modelBuilder.Entity<UserSecurityDetail>()
                .HasIndex(u => u.UserId)
                .IsUnique();

            modelBuilder.Entity<UserSecurityDetail>()
                .Property(us => us.SecurityId)
                .HasDefaultValueSql("NEWID()");

            // # Rating
            modelBuilder.Entity<Rating>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(r => r.RatedUserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<Rating>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(r => r.RaterUserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            modelBuilder.Entity<Rating>()
                .HasCheckConstraint("CHK_Ratings_Score", "[Score] BETWEEN 1 AND 5");

            modelBuilder.Entity<Rating>()
                .HasIndex(r => new { r.RatedUserId, r.RaterUserId })
                .IsUnique();

            modelBuilder.Entity<Rating>()
                .Property(r => r.RatingId)
                .HasDefaultValueSql("NEWID()");

            // # Verification
            modelBuilder.Entity<Verification>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .IsRequired();

            modelBuilder.Entity<Verification>()
                .HasOne<LookupData>()
                .WithMany()
                .HasForeignKey(v => v.VerificationTypeId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            modelBuilder.Entity<Verification>()
                .HasOne<LookupData>()
                .WithMany()
                .HasForeignKey(v => v.CurrentVerificationStatusId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            modelBuilder.Entity<Verification>()
                .Property(v => v.VerificationId)
                .HasDefaultValueSql("NEWID()");

            // # Commodity — use expression overloads so UnitOfMeasure nav binds to UnitOfMeasureId
            modelBuilder.Entity<Commodity>(entity =>
            {
                entity.HasKey(c => c.CommodityId);

                entity.HasOne<CommodityType>()
                      .WithMany()
                      .HasForeignKey(c => c.CommodityTypeId)
                      .IsRequired();

                entity.HasOne(c => c.UnitOfMeasure)
                      .WithMany()
                      .HasForeignKey(c => c.UnitOfMeasureId)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(c => c.CommodityId).HasDefaultValueSql("NEWID()");
            });

            // # CommodityType
            modelBuilder.Entity<CommodityType>()
                .HasOne<LookupData>()
                .WithMany()
                .HasForeignKey(ct => ct.CommodityGroupId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            modelBuilder.Entity<CommodityType>()
                .HasOne(ct => ct.Name) // ties the name navigation property
                .WithMany()
                .HasForeignKey(ct => ct.NameId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            modelBuilder.Entity<CommodityType>()
                .HasOne<UnitOfMeasure>()
                .WithMany()
                .HasForeignKey(ct => ct.DefaultUnitOfMeasureId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<CommodityType>()
                .Property(ct => ct.CommodityTypeId)
                .HasDefaultValueSql("NEWID()");

            // # Location
            modelBuilder.Entity<Location>()
                .HasOne<LookupData>()
                .WithMany()
                .HasForeignKey(l => l.LocationTypeId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            modelBuilder.Entity<Location>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<Location>()
                .HasOne<LookupData>()
                .WithMany()
                .HasForeignKey(l => l.RegionId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            modelBuilder.Entity<Location>()
                .Property(l => l.Latitude)
                .HasPrecision(18, 10); // High precision for coordinates

            modelBuilder.Entity<Location>()
                .Property(l => l.Longitude)
                .HasPrecision(18, 10); // High precision for coordinates

            modelBuilder.Entity<Location>()
                .Property(l => l.LocationId)
                .HasDefaultValueSql("NEWID()");

            // # LookupData
            modelBuilder.Entity<LookupData>()
                .HasOne<LookupDataType>()
                .WithMany()
                .HasForeignKey(ld => ld.LookupDataTypeId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            modelBuilder.Entity<LookupData>()
                .Property(ld => ld.LookupDataId)
                .ValueGeneratedNever();

            // # LookupDataType
            modelBuilder.Entity<LookupDataType>()
                .Property(lt => lt.LookupDataTypeId)
                .ValueGeneratedNever();

            modelBuilder.Entity<DeliveryDetail>()
                .Property(dd => dd.Fee)
                .HasPrecision(18, 2); // Standard currency precision

            modelBuilder.Entity<DeliveryDetail>()
                .Property(dd => dd.MaxDistance)
                .HasPrecision(10, 2);

            modelBuilder.Entity<DeliveryDetail>()
                .Property(dd => dd.DeliveryDetailId)
                .HasDefaultValueSql("NEWID()");

            // # UnitOfMeasure
            modelBuilder.Entity<UnitOfMeasure>()
                .Property(u => u.UnitOfMeasureId)
                .HasDefaultValueSql("NEWID()");
        }

    }
}