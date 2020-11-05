using System;

namespace GameBrain
{
    public class GameSaveData
    {
        public int GameSaveDataId { get; set; }
        
        public string TimeStamp { get; set; } = DateTime.Now.ToLongDateString();
        
        public string SerializedGameData { get; set; }
        
        public int GameOptionsId { get; set; }
        public GameOption GameOption { get; set; }
        
        
    }
}