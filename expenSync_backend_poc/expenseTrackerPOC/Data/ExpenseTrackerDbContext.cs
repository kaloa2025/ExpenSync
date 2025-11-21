using expenseTrackerPOC.Models;
using Microsoft.EntityFrameworkCore;

namespace expenseTrackerPOC.Data
{
    public class ExpenseTrackerDbContext : DbContext
    {
        public ExpenseTrackerDbContext(DbContextOptions<ExpenseTrackerDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ExpenseType> ExpenseTypes { get; set; }
        public DbSet<CategoryIcon> CategoryIcons { get; set; }
        public DbSet<ModeOfPayment> ModeOfPayments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships
            modelBuilder.Entity<Category>()
               .HasOne(c => c.User)
               .WithMany(u => u.Categories)
               .HasForeignKey(c => c.UserId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Category>()
                .HasIndex(c => new { c.UserId, c.CategoryName })
                .IsUnique();

            modelBuilder.Entity<RefreshToken>()
               .HasOne(rt => rt.User)
               .WithMany(u => u.RefreshTokens)
               .HasForeignKey(rt => rt.UserId);

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.Token)
                .IsUnique();

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.UserId);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.ExpenseType)
                .WithMany(e => e.Transactions)
                .HasForeignKey(t => t.ExpenseTypeId);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.ModeOfPayment)
                .WithMany(e => e.Transactions)
                .HasForeignKey(t => t.ModeOfPaymentId);

            modelBuilder.Entity<Category>()
                .HasOne(c => c.Icon)
                .WithMany(i => i.Categories)
                .HasForeignKey(c => c.IconId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
