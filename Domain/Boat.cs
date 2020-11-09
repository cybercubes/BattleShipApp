﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Boat
    {
        public int BoatId { get; set; }

        [MaxLength(32)]
        public string Name { get; set; }

        [Range(1, int.MaxValue)]
        public int Size { get; set; }

        [Range(1, int.MaxValue)]
        public int Amount { get; set; }
        
        public int GameOptionId { get; set; }
        public GameOption GameOption { get; set; } = null!;

    }
}