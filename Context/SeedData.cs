using Context;
using Microsoft.EntityFrameworkCore;

public static class SeedData
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        // Seed data for the CardModel (only if it doesn't already exist)
        modelBuilder.Entity<CardModel>().HasData(
            new CardModel
            {
                Id = 1,
                Title = "Experience Summary",
                Image = "src/assets/WorkStock.jpg",
                Country = "South Africa",
                Description = "Graduated CPUT with a Bachelors in Computer Engineering. Part-time sales Assistant at Studio 88 and Outdoor Warehouse. Software Developer Internship at 1Nebula."
            }
        );

        // Seed data for BadgeModel (only if it doesn't already exist)
        modelBuilder.Entity<BadgeModel>().HasData(
            new BadgeModel
            {
                Id = 1,
                Emoji = "🛒",
                Label = "Part-time Sales Assistant: Studio 88",
                CardModelId = 1
            },
            new BadgeModel
            {
                Id = 2,
                Emoji = "🛒",
                Label = "Part-time Sales Assistant: Outdoor Warehouse",
                CardModelId = 1
            },
            new BadgeModel
            {
                Id = 3,
                Emoji = "💻",
                Label = "Software Developer Internship: 1Nebula",
                CardModelId = 1
            },
            new BadgeModel
            {
                Id = 4,
                Emoji = "🎓",
                Label = "CPUT Graduate in Computer Engineering",
                CardModelId = 1
            }
        );

        // Seed data for CarouselModel (only if it doesn't already exist)
        modelBuilder.Entity<CarouselModel>().HasData(
            new CarouselModel
            {
                Id = 2,
                Title = "HomeImages"
            }
        );

        // Seed data for CarouselImageModel (only if it doesn't already exist)
        modelBuilder.Entity<CarouselImageModel>().HasData(
            new CarouselImageModel
            {
                Id = 5,
                ImageUrl = new List<string>
                {
                    "src/assets/profilePic.png",
                    "src/assets/gintoki.png",
                    "src/assets/roxas.jpg",
                    "src/assets/myf.png",
                    "src/assets/ffv.jpg"
                },
                CarouselModelId = 2
            }
        );
    }
}
