﻿using System;

namespace Domain
{
    public class Game
    {
        public int GameId { get; set; }

        public int GameOptionId { get; set; }
        public GameOption GameOption { get; set; }

        public string Description { get; set; } = DateTime.Now.ToLongDateString();

        public int PlayerAId { get; set; }
        public Player PlayerA { get; set; }
        
        public int PlayerBId { get; set; }
        public Player PlayerB { get; set; }
    }
}