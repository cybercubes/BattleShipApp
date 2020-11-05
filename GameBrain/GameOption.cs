using System.Collections.Generic;
using GameBrain.Enums;

namespace GameBrain
{
    public class GameOption
    {
        public int GameOptionId { get; set; }
        public int BoardWidth { get; set; } = 5;

        public int BoardHeight { get; set; } = 5;

        public CanBoatsTouch CanBoatsTouch { get; set; } = CanBoatsTouch.No;

        public MoveOnHit MoveOnHit { get; set; } = MoveOnHit.SamePlayer;

        public ICollection<GameSaveData> GameSaveDatas { get; set; }
    }
}