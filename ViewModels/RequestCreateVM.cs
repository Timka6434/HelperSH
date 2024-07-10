using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using test_indentity.Models;

namespace test_indentity.ViewModels
{
    public class RequestCreateVM
    {
        [Required]
        [StringLength(300)]
        public string Description { get; set; }

        [Required]
        public string Urgency { get; set; } = "обычная";

        [Required]
        public int RoomId { get; set; } // Используем RoomId для хранения идентификатора выбранного кабинета

        public List<CabinetList> Rooms { get; set; } = new List<CabinetList>();

        [Required]
        public string TypeTechnology { get; set; }
    }
}
