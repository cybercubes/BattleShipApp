using System.Collections.Generic;
using Domain.Enums;

namespace Domain
{
    public class GameOption
    {
        public int GameOptionId { get; set; }
        public int BoardWidth { get; set; } = 5;

        public int BoardHeight { get; set; } = 5;

        public int BoatLimit { get; set; } = 2;

        public CanBoatsTouch CanBoatsTouch { get; set; } = CanBoatsTouch.No;

        public MoveOnHit MoveOnHit { get; set; } = MoveOnHit.SamePlayer;

        public ICollection<Boat> Boats { get; set; } = null!;

        public int GameSaveDataId { get; set; }
        public GameSaveData GameSaveData { get; set; } = null!;
    }
}