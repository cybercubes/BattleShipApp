using Domain;
using Microsoft.EntityFrameworkCore;
using GameOption = Domain.GameOption;

namespace DAL
{
    public class AppDbContext : DbContext
    {

        public DbSet<GameSaveData> GameSaveDatas { get; set; } = null!;
        public DbSet<GameOption> GameOptions { get; set; } = null!;

        public DbSet<Boat> Boats { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder
                .UseSqlServer(
                    @"
                        Server=barrel.itcollege.ee,1533;
                        User Id=student;
                        Password=Student.Bad.password.0;
                        Database=kiloss_battleship;
                        MultipleActiveResultSets=true
                        "
                );

            //.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=MyDatabase;Trusted_Connection=True;");
        }
        
        /*protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Boat>()
                .HasIndex(u => u.Name)
                .IsUnique();

            builder.Entity<Boat>()
                .HasIndex(u => u.Size)
                .IsUnique();


        }*/
    }
}