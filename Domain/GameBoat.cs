using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class GameBoat
    {
        public int GameBoatId { get; set; }

        [Range(1, int.MaxValue)]
        public int BoatSize { get; set; }

        [MaxLength(128)]
        public int Name { get; set; }

        public bool IsSunken { get; set; }

        public int PlayerId { get; set; }
        public Player Player { get; set; }
    }
}