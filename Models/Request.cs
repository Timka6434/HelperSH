using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace test_indentity.Models
{
    public class Request
    {
        public int Id { get; set; }

        [Required]
        [StringLength(300)]
        public string Description { get; set; }

        [Required]
        public string Urgency { get; set; } // "обычная" или "срочная"

        [Required]
        public string Room { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? UserId { get; set; } // Поле может быть null

        public AppUser? User { get; set; } // Связь может быть null

        public string Status { get; set; } // "активна", "отменена", "выполнена"

        public string TypeTechnology {  get; set; } // тут будет выбор техники назначенной в кабинет
    }
}
