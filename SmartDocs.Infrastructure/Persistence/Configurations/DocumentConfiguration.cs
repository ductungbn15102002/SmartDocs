using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SmartDocs.Infrastructure.Persistence.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Title).IsRequired().HasMaxLength(500);
        builder.Property(d => d.FilePath).IsRequired();
        builder.Property(d => d.FileType).IsRequired().HasMaxLength(10);

        builder.HasOne(d => d.SubmittedBy)
            .WithMany(u => u.Documents)
            .HasForeignKey(d => d.SubmittedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(d => d.ReviewedBy)
            .WithMany()
            .HasForeignKey(d => d.ReviewedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}