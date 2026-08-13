using MarketPrice.Data.Configurations;
using MarketPrice.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MarketPrice.Data.Configurations;

public class VerificationOtpConfiguration
    : IEntityTypeConfiguration<VerificationOtp>
{
    public void Configure(
        EntityTypeBuilder<VerificationOtp> builder)
    {
        builder.ToTable("VerificationOtps");

        builder.HasKey(x => x.VerificationOtpId);

        builder.Property(x => x.CodeHash)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.Destination)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.VerificationMethod)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.DateCreated)
            .IsRequired();

        builder.Property(x => x.DateExpires)
            .IsRequired();

        builder.Property(x => x.Attempts)
            .IsRequired();

        builder.Property(x => x.MaxAttempts)
            .IsRequired();

        builder.Property(x => x.IsUsed)
            .IsRequired();

        builder.Property(x => x.IsInvalidated)
            .IsRequired();

        builder.HasIndex(x => x.VerificationId);
    }
}