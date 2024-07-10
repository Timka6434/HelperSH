using System.ComponentModel.DataAnnotations;

namespace test_indentity.Models
{
    public class Consumable
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
