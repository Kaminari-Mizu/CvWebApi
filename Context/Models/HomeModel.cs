using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
///<summary>
///The purpose of this class is basically to set up the tables for your database
///These entities made here (should be called entities, not models) are representations
///of the tables that would be found in the SQL database. Thus this code provides the schema
///that EF Core uses, when applying migrations, to create and manage the tables. Then
///each property of the entities will represent a column in the table
///</summary>
namespace Context
{
    /// <summary>
    /// The HomeModel (HomeEntity) is an abstract class which means that it's
    /// similiar to an interface where the CardModel and CarouselModel will inherit all the properties
    /// of the HomeModel without having to re-initialize them. This means both CardModel
    /// and CarouselModel will have Id and Title properties always.
    /// However, unlike Interfaces, this abstract class will also allow CardModel and CarouselModel to share
    /// its functionality whereas an interface would only define a contract
    /// </summary>

    public abstract class HomeModel
    {
        [Key] //This tells EF Core that the below ID is a primary key and is what will be updated and used to keep track of each entry into a table
        public int Id { get; set; }
        public string Title { get; set; } = null!;
    }

    // Table 1: Card Model (inherits from HomeModel)
    public class CardModel : HomeModel
    {
        public string Image { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }

        //The following line established a One-To-Many relationship between CardModel and BadgeModel
        //This means that each CardModel can have multiple BadgeModels associated with it
        //and subsequently BadgeModel will have a Foreign Key (the CardModel Id) referencing the CardModel it
        //is associated with
        public ICollection<BadgeModel> Badges { get; set; } = new List<BadgeModel>();
    }

    // Table 2: Carousel Model (inherits from HomeModel)
    public class CarouselModel : HomeModel
    {
        //Same as BadgeModel and CardModel
        public ICollection<CarouselImageModel> Images { get; set; } = new List<CarouselImageModel>();
    }

    // Separate Table for Carousel Images
    public class CarouselImageModel
    {
        [Key]
        public int Id { get; set; }
        public List<string> ImageUrl { get; set; } = new List<string>();

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