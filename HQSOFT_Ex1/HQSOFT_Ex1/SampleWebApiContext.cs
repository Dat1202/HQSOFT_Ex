using HQSOFT_Ex1.Entities;
using Microsoft.EntityFrameworkCore;

namespace HQSOFT_Ex1
{
    public class SampleWebApiContext: DbContext
    {
        public SampleWebApiContext(DbContextOptions<SampleWebApiContext> options)
            : base(options)
        {

        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId);
        }
    }
}
