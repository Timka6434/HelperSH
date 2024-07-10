using System.ComponentModel.DataAnnotations;

namespace test_indentity.Models
{
    public class Equipment
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string InventoryNumber { get; set; }

        [Required]
        public string Location { get; set; }

        public string? Description { get; set; }

        public int CabinetId { get; set; }

        public CabinetList CabinetList { get; set; }

    }
}
