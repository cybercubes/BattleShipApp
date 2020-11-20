using System;

namespace Domain
{
    public class GameSaveData
    {
        public int GameSaveDataId { get; set; }
        
        public string TimeStamp { get; set; } = DateTime.Now.ToLongDateString();

        public string SerializedGameData { get; set; } = null!;
        
        //public ICollection<GameOption> GameOptions{ get; set; }
        public GameOption GameOption { get; set; } = null!;

        public string SaveName { get; set; } = null!;


    }
}