namespace Domain
{
    public class JournalEntry
    {
        public int X { get; set; }
        public int Y { get; set; }

        public JournalEntry()
        {
            
        }
        
        public JournalEntry(int x, int y)
        {
            X = x;
            Y = y;
        }

    }
}