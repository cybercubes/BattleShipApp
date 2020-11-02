using GameBrain.Enums;

namespace GameBrain
{
    using System;

    namespace GameBrain
    {
        public class GameState
        {
            public int GameStateId { get; set; }
            public bool NextMoveByX { get; set; }
            public CellState[][] BoardA { get; set; } = null!;
            public CellState[][] BoardB { get; set; } = null!;
            public int Width { get; set; }
            public int Height { get; set; }

            public string TimeStamp { get; set; } = DateTime.Now.ToLongDateString();
            
            
            
            public int GameOptionsId { get; set; }
            public GameOptions GameOptions { get; set; }
        }
    }

}