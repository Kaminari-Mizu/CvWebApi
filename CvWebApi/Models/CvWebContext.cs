using Microsoft.EntityFrameworkCore;

namespace CvWebApi.Models
{
    public class CvWebContext : DbContext
    {
        public CvWebContext(DbContextOptions<CvWebContext> options)
            : base(options)
        { 
        }

        public DbSet<HomeModel> Home { get; set; } = null!;
        public DbSet<CardModel> Cards { get; set; } = null!;    // Table for Cards
        public DbSet<BadgeModel> Badges { get; set; } = null!;   // Table for Badges
        public DbSet<CarouselModel> Carousels { get; set; } = null!; // Table for Carousels
        public DbSet<CarouselImageModel> CarouselImages { get; set; } // Table for Images
        //public DbSet<HobbiesModel> Hobbies { get; set; } = null!;
        //public DbSet<DetailsModel> Details { get; set; } = null!;
        //public DbSet<ContactModel> Contacts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HomeModel>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<CardModel>("Card")
                .HasValue<CarouselModel>("Carousel");

            modelBuilder.Entity<BadgeModel>()
                .HasOne(b => b.Card)
                .WithMany(c => c.Badges)
                .HasForeignKey(b => b.CardModelId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CarouselImageModel>()
                .HasOne(i => i.Carousel)
                .WithMany(c => c.Images)
                .HasForeignKey(i => i.CarouselModelId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
