using GameBrain.Enums;

namespace GameBrain
{
    public class GameOptions
    {
        public int GameOptionsId { get; set; }
        public int BoardWidth { get; set; } = 5;

        public int BoardHeight { get; set; } = 5;

        public CanBoatsTouch CanBoatsTouch { get; set; } = CanBoatsTouch.No;

        public MoveOnHit MoveOnHit { get; set; } = MoveOnHit.SamePlayer;
    }
}