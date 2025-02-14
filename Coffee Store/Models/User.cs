using System;
using Coffee_Store.Models;

namespace Coffee_Store.Models
{
    public class User
    {   
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Order> Orders { get; set; }


    }
}
