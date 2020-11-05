using GameBrain;
using Microsoft.EntityFrameworkCore;
using GameOption = GameBrain.GameOption;

namespace DAL
{
    public class AppDbContext : DbContext
    {

        public DbSet<GameSaveData> GameSaveDatas { get; set; }
        public DbSet<GameOption> GameOptions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=BattleShips;Trusted_Connection=True;");
        }
    }
}