using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CvWebApi.Models
{
    // Base class for shared properties
    public abstract class HomeModel
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
    }

    // Table 1: Card Model (inherits from HomeModel)
    public class CardModel : HomeModel
    {
        public string Image { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }

        // One-to-Many Relationship: A card can have multiple badges
        public ICollection<BadgeModel> Badges { get; set; } = new List<BadgeModel>();
    }

    // Table 2: Carousel Model (inherits from HomeModel)
    public class CarouselModel : HomeModel
    {
        public ICollection<CarouselImageModel> Images { get; set; } = new List<CarouselImageModel>();
    }

    // Separate Table for Carousel Images
    public class CarouselImageModel
    {
        [Key]
        public int Id { get; set; }
        public string ImageUrl { get; set; }

        // Foreign key to CarouselModel
        public int CarouselModelId { get; set; }
        public CarouselModel? Carousel { get; set; }
    }

    // Table 3: Badge Model (Related to CardModel)
    public class BadgeModel
    {
        [Key]
        public int Id { get; set; }
        public string Emoji { get; set; }
        public string Label { get; set; }

        // Foreign Key to CardModel
        public int CardModelId { get; set; }
        public CardModel? Card { get; set; }
    }
}
