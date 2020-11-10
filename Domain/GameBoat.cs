namespace Domain
{
    public class GameBoat
    {
        public int GameBoatId { get; set; }

        public int CoordX { get; set; } = -1;

        public int CoordY { get; set; } = -1;

        public bool Horizontal { get; set; } = true;

        public int Size { get; set; }
    }
}