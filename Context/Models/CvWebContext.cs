using Microsoft.EntityFrameworkCore;
///<summary>
///This class sets up the context EF Core requires to actually construct and interact with the database using the 
///preset entity configurations from the DbContext class.
///This class is the bridge between the database and the application, which allows EF Core to track and manage entities.
///</summary>

/// <summary>
/// Represents the database context for EF Core.
/// This class is responsible for interacting with the database using entity configurations
/// defined in the DbContext class. It serves as a bridge between the database and the application,
/// allowing EF Core to track and manage entities.
/// </summary>
namespace Domain
{
    public class CvWebContext : DbContext
    {
        /// <summary>
        /// The below line creates a new instance of CvWebContext using the provided options.
        /// These options define the database connection and configuration settings.
        /// </summary>
        /// <param name="options">Database context configuration options.</param>
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


        /// <summary>
        /// Configures the entity relationships and model mappings using Fluent API
        /// which is a means of configuring entity relationships, constraints, and database mappings
        /// with EF Core through method chaining instead of data annotations.
        /// </summary>
        /// <param name="modelBuilder">The ModelBuilder instance used to configure entity mappings.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            /// <summary>
            /// Configures table inheritance for HomeModel using a discriminator column.
            /// The discriminator column provides EF Core a way to distinguish between CardModel and CarouselModel
            /// when querying or saving data.
            /// </summary>
            modelBuilder.Entity<HomeModel>()
                .HasDiscriminator<string>("Discriminator")
                .HasValue<CardModel>("Card")
                .HasValue<CarouselModel>("Carousel");

            modelBuilder.Entity<HomeModel>()
            .Property(h => h.Id)
            .ValueGeneratedOnAdd();  // <-- Explicitly tell EF to auto-generate ID


            /// <summary>
            /// Configures a one-to-many relationship between CardModel and BadgeModel.
            /// Each CardModel can have multiple BadgeModel entries.
            /// Deleting a CardModel will cascade delete its related BadgeModel entries.
            /// </summary>
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

            /// <summary>
            /// Calls a separate class to seed initial data into the database.
            /// </summary>
            SeedData.Seed(modelBuilder);
        }


    }
}
