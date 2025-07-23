using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BussinessObjects.Models
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) { }

        public LibraryDbContext() { }
        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; }
        public DbSet<BorrowDetail> BorrowDetails { get; set; }
        public DbSet<Fine> Fines { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json")
             .Build().GetConnectionString("MyCnn");
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User - BorrowRecord (1-n)
            modelBuilder.Entity<BorrowRecord>()
                .HasOne(br => br.User)
                .WithMany(u => u.BorrowRecords)
                .HasForeignKey(br => br.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Book - Author (n-1)
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Book - Category (n-1)
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Books)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Book - Publisher (n-1, optional)
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Publisher)
                .WithMany(p => p.Books)
                .HasForeignKey(b => b.PublisherId)
                .OnDelete(DeleteBehavior.SetNull);

            // BorrowRecord - BorrowDetail (1-n)
            modelBuilder.Entity<BorrowDetail>()
                .HasOne(bd => bd.BorrowRecord)
                .WithMany(br => br.BorrowDetails)
                .HasForeignKey(bd => bd.BorrowId)
                .OnDelete(DeleteBehavior.Cascade);

            // Book - BorrowDetail (1-n)
            modelBuilder.Entity<BorrowDetail>()
                .HasOne(bd => bd.Book)
                .WithMany(b => b.BorrowDetails)
                .HasForeignKey(bd => bd.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            // BorrowRecord - Fine (1-1, optional)
            modelBuilder.Entity<Fine>()
                .HasOne(f => f.BorrowRecord)
                .WithOne(br => br.Fine)
                .HasForeignKey<Fine>(f => f.BorrowId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
} 