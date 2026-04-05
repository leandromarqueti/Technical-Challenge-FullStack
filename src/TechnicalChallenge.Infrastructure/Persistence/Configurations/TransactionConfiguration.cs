using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TechnicalChallenge.Domain.Entities;

namespace TechnicalChallenge.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("Transactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Description)
            .IsRequired()
            .HasMaxLength(400);

        builder.Property(t => t.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(t => t.Date)
            .IsRequired();

        builder.Property(t => t.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt);

        builder.Property(t => t.UserId)
            .IsRequired();

        //Relacionamento com Category
        builder.HasOne(t => t.Category)
            .WithMany(c => c.Transactions)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        //Relacionamento com Person
        builder.HasOne(t => t.Person)
            .WithMany(p => p.Transactions)
            .HasForeignKey(t => t.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        //Índices para otimizar consultas de filtro
        builder.HasIndex(t => t.Date);
        builder.HasIndex(t => t.Type);
        builder.HasIndex(t => t.CategoryId);
        builder.HasIndex(t => t.PersonId);
        builder.HasIndex(t => t.UserId);
    }
}
