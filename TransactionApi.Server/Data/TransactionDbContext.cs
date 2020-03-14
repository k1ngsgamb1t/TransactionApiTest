using Microsoft.EntityFrameworkCore;
using TransactionApi.Server.Data.Entities;

namespace TransactionApi.Server.Data
{
    public class TransactionDbContext : DbContext
    {
        public virtual DbSet<Transaction> Transaction { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId);
                entity.Property(e => e.TransactionId).HasMaxLength(50).ValueGeneratedNever();
                entity.Property(e => e.Currency).HasMaxLength(3);
            });
        }
    }
}