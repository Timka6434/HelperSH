using System.ComponentModel.DataAnnotations;

namespace test_indentity.Models
{
    public class CabinetList
    {
        [Key]
        public int CabinetId { get; set; }
        [Required]
        public string CabinetName { get; set; }
        [Required]
        public string CabinetNumber {  get; set; }

        public string? FullCabinetName { get; set; }

        public ICollection<Equipment> Equipments { get; set; }
    }
}
