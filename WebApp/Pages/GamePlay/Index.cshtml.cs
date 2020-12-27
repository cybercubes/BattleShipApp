using System;
using System.Globalization;
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

        public async Task OnGetAsync(int? xCoord, int? yCoord)
        {
            GameOption = await _context.GameOptions
                .Include(g => g.Boats)
                .Include(g => g.GameSaveData)
                .FirstOrDefaultAsync() ?? new GameOption();
            

            BattleShip ??= new BattleShip(GameOption);
            var initGame = false;



            if (Request.Query["PlaceNormal"].ToString() == "Place Ships")
            {

                for (var i = 0; i < BattleShip.GetBoatArrays().Item1.Count(); i++)
                {
                    if (Request.Query.Count == 0) break;

                    var boatIndex = i + 1;
                    var x = int.Parse(Request.Query["p1_x_bIndex_" + boatIndex]);
                    var y = int.Parse(Request.Query["p1_y_bIndex_" + boatIndex]);
                    var horizontal = Request.Query["p1_horizontal_bIndex_" + boatIndex];

                    if (horizontal.Count == 1)
                    {
                        BattleShip.RotateBoat(i);
                    }

                    BattleShip.PlaceBoat(i, x, y);

                }

                BattleShip.ChangeWhoPlacesBoats();

                for (var i = 0; i < BattleShip.GetBoatArrays().Item2.Count(); i++)
                {
                    if (Request.Query.Count == 0) break;

                    var boatIndex = i + 1;
                    var x = int.Parse(Request.Query["p2_x_bIndex_" + boatIndex]);
                    var y = int.Parse(Request.Query["p2_y_bIndex_" + boatIndex]);
                    var horizontal = Request.Query["p2_horizontal_bIndex_" + boatIndex];

                    if (horizontal.Count == 1)
                    {
                        BattleShip.RotateBoat(i);
                    }

                    BattleShip.PlaceBoat(i, x, y);

                }

                BattleShip.UpdateBoatsOnBoard();
                var boatPlacementViolation =
                    RulesViolated(BattleShip.GetBoatArrays().Item2, BattleShip.GetBoards().Item2);
                if (boatPlacementViolation)
                {
                    Console.WriteLine("Rules Violated");
                    ModelState.AddModelError("", "Your boat is placement bad! for player B");
                }

                BattleShip.ChangeWhoPlacesBoats();
                BattleShip.UpdateBoatsOnBoard();

                boatPlacementViolation = RulesViolated(BattleShip.GetBoatArrays().Item1, BattleShip.GetBoards().Item1);
                if (boatPlacementViolation)
                {
                    Console.WriteLine("Rules Violated");
                    ModelState.AddModelError("", "Your boat placement is bad!  player A");
                }

                initGame = true;
            }

            if (Request.Query["PlaceAuto"].ToString() == "Place Automatically")
            {
                BattleShip.AutoShipSetupForOneBoard(BattleShip.GetBoatArrays().Item1);
                BattleShip.ChangeWhoPlacesBoats();
                BattleShip.AutoShipSetupForOneBoard(BattleShip.GetBoatArrays().Item2);
                
                initGame = true;
            }

            if (Request.Query["SaveGame"].ToString() == "Save This Game")
            {
                var serializedGame = BattleShip.GetSerializedGameState();
                
                var gameSaveData = new GameSaveData()
                {
                    SerializedGameData = serializedGame,
                    SaveName = Request.Query["saveName"].ToString()
                };

                GameOption.GameSaveData.Add(gameSaveData);
                _context.GameOptions.Update(GameOption);
                await _context.SaveChangesAsync();
            }
            //CheckBoards();

            if (initGame)
            {
                while (GameOption.GameSaveData.Any(x => x.SaveName.Contains("Dt_use")))
                {
                    GameOption.GameSaveData.Remove(GameOption.GameSaveData.Last(x => x.SaveName.Contains("Dt_use")));
                }

                var serializedGame = BattleShip.GetSerializedGameState();
                
                var gameSaveData = new GameSaveData()
                {
                    SerializedGameData = serializedGame,
                    SaveName = "Dt_use" + DateTime.Now.ToString(CultureInfo.InvariantCulture),
                };

                GameOption.GameSaveData.Add(gameSaveData);
                _context.GameOptions.Update(GameOption);
                await _context.SaveChangesAsync();
                
                //TODO: delete DT_USE containing savenames
            }

            if (GameOption.GameSaveData.Any(x => x.SaveName.Contains("Dt_use")))
            {
                var saveData = GameOption.GameSaveData.Last(x => x.SaveName.Contains("Dt_use"));
                
                BattleShip.SetGameStateFromJsonString(saveData.SerializedGameData, GameOption);
                BattleShip.SetWinnerString(saveData.WinnerString);
                CheckBoards();
            }
            
            if (xCoord != null && yCoord != null)
            {
                BattleShip.MakeAMove(xCoord.Value, yCoord.Value);
                Console.WriteLine(BattleShip.GetWinnerString());

                if (BattleShip.GetWinnerString() != "")
                {
                    ModelState.AddModelError("Winner", BattleShip.GetWinnerString());
                }

                var gameSaveData = new GameSaveData()
                {
                    SerializedGameData = BattleShip.GetSerializedGameState(),
                    SaveName = "Dt_use" + DateTime.Now.ToString(CultureInfo.InvariantCulture),
                    WinnerString = BattleShip.GetWinnerString()
                };

                GameOption.GameSaveData.Add(gameSaveData);
                _context.GameOptions.Update(GameOption);
                await _context.SaveChangesAsync();
            }

        }

        private bool RulesViolated(GameBoat[] boatArray, CellState[,] boardArray)
        {
            return BattleShip!.CheckIfBoatsOverlap(boatArray) ||
                   BattleShip.CheckIfTouchViolated(boatArray, boardArray) ||
                   BattleShip.CheckIfBoatLimitIsViolated(boatArray);
        }

        private void CheckBoards()
        {
            for (var rowIndex = 0; rowIndex < BattleShip!.GetBoardHeight(); rowIndex++)
            {
                for (var colIndex = 0; colIndex < BattleShip.GetBoardWidth(); colIndex++)
                {
                    Console.Write($"| {BattleShip.GetBoards().Item1[rowIndex, colIndex]} |");
                }
                Console.WriteLine();
                for (var colIndex = 0; colIndex < BattleShip.GetBoardWidth(); colIndex++)
                {
                    Console.Write($"+---+");
                }
                Console.WriteLine();
                
            }

            Console.WriteLine("======================================================");
            
            for (var rowIndex = 0; rowIndex < BattleShip!.GetBoardHeight(); rowIndex++)
            {
                for (var colIndex = 0; colIndex < BattleShip.GetBoardWidth(); colIndex++)
                {
                    Console.Write($"| {BattleShip.GetBoards().Item2[rowIndex, colIndex]} |");
                }
                Console.WriteLine();
                for (var colIndex = 0; colIndex < BattleShip.GetBoardWidth(); colIndex++)
                {
                    Console.Write($"+---+");
                }
                Console.WriteLine();
                
            }

        }
    }
}