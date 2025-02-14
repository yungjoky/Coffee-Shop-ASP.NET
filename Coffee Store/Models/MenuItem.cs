using System.ComponentModel.DataAnnotations.Schema;

namespace Coffee_Store.Models
{
    public class MenuItem
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string Category { get; set; }
        public string? Image { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
