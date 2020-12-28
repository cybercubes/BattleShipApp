using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
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

        public bool BoatPlaceError = false;

        public async Task OnGetAsync(int? xCoord, int? yCoord)
        {
            GameOption = await _context.GameOptions
                .Include(g => g.Boats)
                .Include(g => g.GameSaveData)
                .FirstOrDefaultAsync();
            

            BattleShip ??= new BattleShip(GameOption);
            var initGame = false;

            if (!GameOption.Boats.Any())
            {
                ModelState.AddModelError("", "The Boat pool is empty, add some boats!");
                return;
            }

            if (Request.Query.ContainsKey("PlaceNormal"))
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
                    ModelState.AddModelError("", "Your boat is placement bad! for player B");
                    BoatPlaceError = true;
                }

                BattleShip.ChangeWhoPlacesBoats();
                BattleShip.UpdateBoatsOnBoard();
                boatPlacementViolation = 
                    RulesViolated(BattleShip.GetBoatArrays().Item1, BattleShip.GetBoards().Item1);
                if (boatPlacementViolation)
                {
                    ModelState.AddModelError("", "Your boat placement is bad!  player A");
                    BoatPlaceError = true;
                }

                initGame = true;
            }

            if (Request.Query.ContainsKey("PlaceAuto"))
            {
                BattleShip.AutoShipSetupForOneBoard(BattleShip.GetBoatArrays().Item1);
                BattleShip.ChangeWhoPlacesBoats();
                BattleShip.AutoShipSetupForOneBoard(BattleShip.GetBoatArrays().Item2);
                
                initGame = true;
            }

            if (Request.Query.ContainsKey("SaveGame"))
            {
                var serializedGame = Request.Query["boardState"].ToString();
                
                var gameSaveData = new GameSaveData()
                {
                    SerializedGameData = serializedGame,
                    SaveName = Request.Query["saveName"].ToString()
                };

                GameOption.GameSaveData.Add(gameSaveData);
                _context.GameOptions.Update(GameOption);
                await _context.SaveChangesAsync();
            }

            if (Request.Query.ContainsKey("loadId"))
            {
                var saveId = int.Parse(Request.Query["loadId"].ToString());
                
                var saveData = GameOption.GameSaveData.First(x => x.GameSaveDataId == saveId);
                
                
                BattleShip.SetGameStateFromJsonString(saveData.SerializedGameData, GameOption);
                BattleShip.SetWinnerString(saveData.WinnerString);

                initGame = true;
                CheckBoards();
            }
            
            if (Request.Query.ContainsKey("ContinueGame"))
            {
                try {
                    var saveData = GameOption.GameSaveData.Last(x => x.SaveName.Contains("Dt_use"));
                    BattleShip.SetGameStateFromJsonString(saveData.SerializedGameData, GameOption);
                    BattleShip.SetWinnerString(saveData.WinnerString);
                }
                catch
                {
                    ModelState.AddModelError("", "Can't Continue any games right now! Start a new one or load!");
                }
            }

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
            }
            
            if (GameOption.GameSaveData.Any(x=> x.SaveName.Contains("Dt_use")) && !Request.Query.ContainsKey("PlaceBoats"))
            {
                var saveData = GameOption.GameSaveData.Last(x => x.SaveName.Contains("Dt_use"));
                BattleShip.SetGameStateFromJsonString(saveData.SerializedGameData, GameOption);
                BattleShip.SetWinnerString(saveData.WinnerString);
                
            }

            if (xCoord != null && yCoord != null)
            {
                if (BattleShip.GetWinnerString() != "")
                {
                    ModelState.AddModelError("Winner", BattleShip.GetWinnerString());
                }
                
                BattleShip.MakeAMove(xCoord.Value, yCoord.Value);

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