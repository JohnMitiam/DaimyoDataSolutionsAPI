using DaimyoDataSolutions.Domain.Entities;
using DaimyoDataSolutions.Domain.Entities.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DaimyoDataSolutions.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;
        public object UserProfiles;

        public ApplicationDbContext
            (DbContextOptions<ApplicationDbContext> options,
            IHttpContextAccessor httpContextAccessor = null)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // DbSetes
        public DbSet<Affiliate> Affiliate { get; set; }
        public DbSet<Products> Product { get; set; }
        public DbSet<ProductCategories> ProductCategories { get; set; }
        public DbSet<Category> Category { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var userId = _httpContextAccessor?.HttpContext?.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";

            var entries = ChangeTracker.Entries<BaseModel>();

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        // Only set CreatedBy if not already set (allows explicit values in tests/services)
                        if (string.IsNullOrWhiteSpace(entry.Entity.CreatedBy))
                        {
                            entry.Entity.CreatedBy = userId;
                        }
                        entry.Entity.DateCreated = DateTime.UtcNow;
                        break;

                    case EntityState.Modified:
                        // Only set UpdatedBy if not already set
                        if (string.IsNullOrWhiteSpace(entry.Entity.UpdatedBy))
                        {
                            entry.Entity.UpdatedBy = userId;
                        }
                        entry.Entity.DateUpdated = DateTime.UtcNow;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Products>()
                .ToTable("Products", (string)null)
                .HasMany(p => p.ProductCategories)
                .WithOne(p => p.Product)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Category>()
                .HasMany(s => s.ProductCategories)
                .WithOne(ps => ps.Category)
                .HasForeignKey(pc => pc.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Affiliate>()
                .HasIndex(aff => aff.Name)
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name);

            modelBuilder.Entity<Products>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Category>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Affiliate>().HasQueryFilter(p => !p.IsDeleted);
        }
    }
}