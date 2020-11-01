using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Boat
    {
        public int BoatId { get; set; }
        
        [Range(1, int.MaxValue)]
        public int BoatSize { get; set; }

        [MaxLength(128)]
        public int Name { get; set; }
        
        public ICollection<GameOptionBoat> GameOptionBoats { get; set; }
    }
}