
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

///<summary>
///The DTO (Data Transfer Object) is used to control data transferred between the server and client.
///It provides a way to ensure that on the client-side, only the necessary data is transferred
///and sensitive data (such as passwords) are not exposed. 
///
/// Furthermore, in the case of having a One-to-Many relationship
///setup between entities, it can prevent a cyclical referrence from occuring by avoiding direct navigation property serialization. 
///
///This is why the DTOs are used to map the entities to the client-side and vice versa. This mapping can be achieved
///with an AutoMapper which is setup in a different folder in the project.
/// </summary>
namespace Context
{
    public class CardModelDTO
    {
       // public int Id { get; set; }
       
        public string? Title { get; set; }
        
        public string? Image { get; set; }
        
        public string? Country { get; set; }
        
        public string? Description { get; set; }
       
        public ICollection<BadgeModelDTO>? Badges { get; set; }
    }

    public class BadgeModelDTO
    {
       public int Id { get; set; }
        
        public string? Emoji { get; set; }
        
        public string? Label { get; set; }
    }

    public class CarouselModelDTO
    {
        public int Id { get; set; }
        
        public string? Title { get; set; }
        
        public ICollection<CarouselImageDTO>? Images { get; set; }
    }

    public class CarouselImageDTO
    {
        public int Id { get; set; }
        
        public List<string>? ImageUrl { get; set; }
    }
}