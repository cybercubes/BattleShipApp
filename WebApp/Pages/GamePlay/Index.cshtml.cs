using System;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Domain;
using Domain.Enums;
using GameBrain;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebApp.Pages.GamePlay
{
    public class Index : PageModel
    {
        
        private readonly ILogger<IndexModel> _logger;
        private readonly DAL.AppDbContext _context;

        public Index(DAL.AppDbContext context,ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        public GameOption GameOption { get; set; } = new GameOption();

        public BattleShip? BattleShip { get; set; } = null;

        public async Task OnGetAsync()
        {
            GameOption = await _context.GameOptions
                .Include(g => g.Boats)
                .Include(g => g.GameSaveData).FirstOrDefaultAsync() ?? new GameOption();

            BattleShip ??= new BattleShip(GameOption);
            
            foreach (var key in Request.Query)
            {
                var boatIndex = int.Parse(key.Key.Split("_").Last()) - 1;
                var boatArray = BattleShip.GetPlaceBoatsByA() ?
                    BattleShip.GetBoatArrays().Item1 : BattleShip.GetBoatArrays().Item2;
                
                
                if (key.Key.Split("_").First() == "p2" && BattleShip.GetPlaceBoatsByA())
                {
                    BattleShip.ChangeWhoPlacesBoats();
                }

                if (key.Key.Contains("_horizontal_"))
                {
                    if (key.Value.Count == 1)
                    {
                        BattleShip.RotateBoat(boatIndex);
                    }
                }

                if (key.Key.Contains("_x_"))
                {
                    boatArray[boatIndex].CoordX = int.Parse(key.Value);
                }
                
                if (key.Key.Contains("_y_"))
                {
                    boatArray[boatIndex].CoordY = int.Parse(key.Value);
                }
            }

            BattleShip.UpdateBoatsOnBoard();
            var boatPlacementViolation = RulesViolated(BattleShip.GetBoatArrays().Item2, BattleShip.GetBoards().Item2);
            if (boatPlacementViolation)
            {
                Console.WriteLine("Rules Violated");
                ModelState.AddModelError("", "Your boat is bad!");
            }

            BattleShip.ChangeWhoPlacesBoats();
            BattleShip.UpdateBoatsOnBoard();
            
            boatPlacementViolation = RulesViolated(BattleShip.GetBoatArrays().Item1, BattleShip.GetBoards().Item1);
            if (boatPlacementViolation)
            {
                Console.WriteLine("Rules Violated");
                ModelState.AddModelError("", "Your boat is bad!");
            }
            
            

        }

        private bool RulesViolated(GameBoat[] boatArray, CellState[,] boardArray)
        {
            return BattleShip!.CheckIfBoatsOverlap(boatArray) ||
                   BattleShip.CheckIfTouchViolated(boatArray, boardArray) ||
                   BattleShip.CheckIfBoatLimitIsViolated(boatArray);
        }
    }
}