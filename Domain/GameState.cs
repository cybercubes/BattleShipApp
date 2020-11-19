using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Domain
{
    public class GameState
        {

            public bool NextMoveByX { get; set; }
            public CellState[][] BoardA { get; set; } = null!;
            public CellState[][] BoardB { get; set; } = null!;
            public GameBoat[] PlayerABoats { get; set; } = null!;
            public GameBoat[] PlayerBBoats { get; set; } = null!;
            
            public JournalEntry[] GameJournal { get; set; } = null!;
            public int Width { get; set; }
            public int Height { get; set; }
            //public GameOption GameOption { get; set; }
        }

}