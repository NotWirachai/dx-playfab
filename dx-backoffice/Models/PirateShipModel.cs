using System.ComponentModel.DataAnnotations;

namespace dx_backoffice.Models
{
    public class PirateShipModel
    {
        [Key]
        public int Id { get; set; }
        public int Level { get; set; }
        public int TotalPower { get; set; }
        public string BuildingUnlocked { get; set; }
        public string ConstructionTime { get; set; }
        public int UseGemToFinish { get; set; }
    }
}
