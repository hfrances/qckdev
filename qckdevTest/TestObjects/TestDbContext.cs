using Microsoft.EntityFrameworkCore;

namespace qckdevTest.TestObjects
{
    class TestDbContext : DbContext
    {

        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        { }

        public DbSet<Entities.Parent> Parents { get; set; }
        public DbSet<Entities.Child> Childs { get; set; }


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
