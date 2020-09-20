using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace qckdevTest.TestObjects
{
    class TestDbContext : DbContext
    {

        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        { }


        public DbSet<Entities.Parent> Parents { get; set; }
        public DbSet<Entities.Child> Childs { get; set; }
        public DbSet<Entities.Orphan> Orphans { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Entities.Parent>(builder =>
            {
                builder.HasKey(x => x.Id);
                builder.HasIndex(x => x.Number).IsUnique();
                builder.Property(x => x.Name).IsRequired();
                builder
                    .HasMany(x => x.Childs)
                    .WithOne(x => x.Parent)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Entities.Child>(builder =>
            {
                builder.HasKey(x => new { x.Id, x.Line });
                builder.Property(x => x.Description).IsRequired();
                builder
                    .HasOne(x => x.Parent)
                    .WithMany(x => x.Childs);
            });

            modelBuilder.Entity<Entities.Orphan>(builder =>
            {
                builder.HasKey(x => new { x.Id, x.Line });
            });
        }

        public static TestDbContext CreateInstance()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TestDbContext>();

            optionsBuilder.UseInMemoryDatabase("TestDb");
            return new TestDbContext(optionsBuilder.Options);
        }

    }
}
