namespace Context
{
    public class CardModelDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public ICollection<BadgeModelDTO> Badges { get; set; }
    }

    public class BadgeModelDTO
    {
        public int Id { get; set; }
        public string Emoji { get; set; }
        public string Label { get; set; }
    }

    public class CarouselModelDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<CarouselImageDTO> Images { get; set; }
    }

    public class CarouselImageDTO
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
    }
}