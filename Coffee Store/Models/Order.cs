using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coffee_Store.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int MenuItemId { get; set; }  // This will link to the specific MenuItem

        [Required]
        public int Quantity { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public Orderstatus status { get; set; }

        // Navigation properties
        public User User { get; set; }
        public MenuItem MenuItem { get; set; }  // This will give you access to the item name and other details
    }
}