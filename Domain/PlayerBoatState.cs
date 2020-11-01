using System;

namespace Domain
{
    public class PlayerBoatState
    {
        public int PlayerBoatStateId { get; set; }

        public int PlayerId { get; set; }
        public Player Player { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        
        // serialized to json
        public string BoardState { get; set; }
    }
}