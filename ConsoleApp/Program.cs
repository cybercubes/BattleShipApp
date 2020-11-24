using System;
using System.Collections.Generic;
using System.Linq;
using DAL;
using Domain;
using Domain.Enums;
using GameBrain;
using GameConsoleUi;
using MenuSystem;
using MenuSystem.Enums;
using Microsoft.EntityFrameworkCore;

namespace ica0016_2020f
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("============> BattleShip KILOSS <=================");
            
            var gameOptions = new GameOption();
            
            using var db = new AppDbContext();

            if (!db.GameOptions.Any())
            {
                MakeNewEntry();
            }

            foreach (var gameOption in db.GameOptions
                .Include(g => g.Boats)
                .Include(g => g.GameSaveData)
            )
            {
                gameOptions = gameOption;
            }


            var menu = new Menu(MenuLevels.Level0);
            var optionMenu = new Menu(MenuLevels.Level1);
            
            menu.AddMenuItem(new MenuItem(
                "New game",
                "1",
                () =>
                {
                    BattleShip(gameOptions);

                    return "";
                })
            );
            
            menu.AddMenuItem(new MenuItem(
                "Options",
                "2",
                optionMenu.RunMenu)
            );

            optionMenu.AddMenuItem(new MenuItem(
                "Setup Game Options",
                "1",
                () =>
                {
                    GameOptionSetup(gameOptions);
                    
                    return "";
                })
            );
            
            optionMenu.AddMenuItem(new MenuItem(
                "Add new Boat",
                "2",
                () =>
                {
                    SetNewBoat(gameOptions);
                    
                    return "";
                })
            );
            
            optionMenu.AddMenuItem(new MenuItem(
                "Remove Boat",
                "3",
                () =>
                {
                    RemoveBoat(gameOptions);
                    
                    return "";
                })
            );
            
            optionMenu.AddMenuItem(new MenuItem(
                "Show Boats",
                "5",
                () =>
                {
                    PrintBoatList(gameOptions);
                    
                    return "";
                })
            );

            menu.RunMenu();

        }

        private static void MakeNewEntry()
        {
            using var db = new AppDbContext();
            db.GameOptions.Add(new GameOption());
            db.SaveChanges();
        }

        private static void UpdateOptionsInDb(GameOption option)
        {
            using var db = new AppDbContext();
            db.GameOptions.Update(option);
            db.SaveChanges();
        }

        private static void SetBoardSize(GameOption option)
        {
            int x, y;
            do
            {
                
                Console.WriteLine("Enter board Size (X,Y): ");
                Console.Write(">");
                var userInput = Console.ReadLine();
                if (userInput == null || userInput.Length < 3)
                {
                    Console.WriteLine("Invalid input! try again...");
                    continue;
                }
                var userValue = userInput.Split(',');
                if (userValue.Length != 2)
                {
                    Console.WriteLine("Invalid input! try again...");
                    continue;
                }
                if (!int.TryParse(userValue[0].Trim(), out x))
                {
                    Console.WriteLine("Invalid input! try again...");
                    continue;
                }
                if (!int.TryParse(userValue[1].Trim(), out y))
                {
                    Console.WriteLine("Invalid input! try again...");
                    continue;
                }

                break;
            } while (true);

            option.BoardHeight = y;
            option.BoardWidth = x;
        }

        private static void SetBoatsCanTouch(GameOption option)
        {
            int userChoice;

            var boatsTouches = GetEnumList<CanBoatsTouch>();
            do
            {
                Console.WriteLine("Can boats touch?: ");
                PrintEnum(boatsTouches);
                Console.Write(">");
                var userInput = Console.ReadLine();
                if (userInput == null)
                {
                    Console.WriteLine("Invalid input! try again...");
                    continue;
                }
                
                if (!int.TryParse(userInput.Trim(), out userChoice))
                {
                    Console.WriteLine("Invalid input! try again...");
                    continue;
                }

                if (userChoice > boatsTouches.Count + 1 || userChoice < 1)
                {
                    Console.WriteLine("Invalid input! try again...");
                    continue;
                }

                break;
            } while (true);

            option.CanBoatsTouch = boatsTouches[userChoice - 1];
        }

        private static void SetNextMoveOnHit(GameOption option)
        {
            int userChoice;
            
            var moveOnHit = GetEnumList<MoveOnHit>();
            do
            {
                Console.WriteLine("What are Move on Hit rules?: ");
                PrintEnum(moveOnHit);
                Console.Write(">");
                var userInput = Console.ReadLine();
                if (userInput == null)
                {
                    Console.WriteLine("Invalid input! try again...");
                    continue;
                }
                
                if (!int.TryParse(userInput.Trim(), out userChoice))
                {
                    Console.WriteLine("Invalid input! try again...");
                    continue;
                }

                if (userChoice > moveOnHit.Count + 1 || userChoice < 1)
                {
                    Console.WriteLine("Invalid input! try again...");
                    continue;
                }

                break;
            } while (true);

            option.MoveOnHit = moveOnHit[userChoice - 1];
        }

        private static void SetBoatLimit(GameOption option)
        {
            if (!AskYesNo("Should boat Limit be enforced?"))
            {
                option.BoatLimit = -1;
                return;
            }

            int userChoice;
            
            do
            {
                Console.WriteLine("Enter Boat Limit: ");
                Console.Write(">");
                var userInput = Console.ReadLine();
                if (userInput == null)
                {
                    Console.WriteLine("Invalid input! try again...");
                    continue;
                }
                
                if (!int.TryParse(userInput.Trim(), out userChoice))
                {
                    Console.WriteLine("Invalid input! try again...");
                    continue;
                }

                if (userChoice <= 0)
                {
                    Console.WriteLine("Limit can be equal to or less than zero");
                    continue;
                }

                break;
            } while (true);
            
            option.BoatLimit = userChoice;
        }

        private static void PrintBoatList(GameOption option)
        {
            if (option.Boats.Count == 0)
            {
                Console.WriteLine("The boat list is empty");
                return;
            }

            foreach (var boat in option.Boats)
            {
                Console.WriteLine($"* {boat}");
            }
        }

        private static void SetNewBoat(GameOption option)
        {
            var boat = new Boat();

            do
            {
                Console.WriteLine("Enter boat Name: ");
                Console.Write(">");
                var userInput = Console.ReadLine();
                if (userInput == null)
                {
                    Console.WriteLine("Invalid input! try again...");
                    continue;
                }

                if (option.Boats.Count == 0)
                {
                    boat.Name = userInput;
                    break;
                }

                if (option.Boats.Any(x => x.Name == userInput))
                {
                    Console.WriteLine("There is already a boat with this name! try again...");
                    continue;
                }

                boat.Name = userInput;
                break;
            } while (true);
            
            do
            {
                Console.WriteLine("Enter boat Size: ");
                Console.Write(">");
                var userInput = Console.ReadLine();
                if (userInput == null)
                {
                    Console.WriteLine("Invalid input! try again...");
                    continue;
                }

                if (!int.TryParse(userInput.Trim(), out var x))
                {
                    Console.WriteLine("Invalid input! try again...");
                    continue;
                }

                if (x < 1)
                {
                    Console.WriteLine("Size value must be higher than 1! try again...");
                    continue;
                }

                boat.Size = x;
                break;
            } while (true);

            do
            {
                Console.WriteLine("Enter boat Amount: ");
                Console.Write(">");
                var userInput = Console.ReadLine();
                if (userInput == null)
                {
                    Console.WriteLine("Invalid input! try again...");
                    continue;
                }

                if (!int.TryParse(userInput.Trim(), out var x))
                {
                    Console.WriteLine("Invalid input! try again...");
                    continue;
                }

                if (x < 0)
                {
                    Console.WriteLine("Size value must be higher than 0! try again...");
                    continue;
                }

                boat.Amount = x;
                break;
            } while (true);

            Console.WriteLine(boat.ToString());
            var saveChanges = AskYesNo("Is this boat OK?");

            if (!saveChanges)
            {
                return;
            }

            option.Boats.Add(boat);

            UpdateOptionsInDb(option);

        }

        private static void RemoveBoat(GameOption option)
        {
            if (option.Boats.Count == 0)
            {
                Console.WriteLine("List of boats is empty!");
                return;
            }

            string? userInput;
            do
            {
                Console.WriteLine("Enter the Name of the boat you want to delete: ");
                Console.Write(">");
                userInput = Console.ReadLine();
                if (userInput == null)
                {
                    Console.WriteLine("Please don't submit an empty name! try again...");
                    continue;
                }

                break;
            } while (true);

            foreach (var boat in option.Boats)
            {
                if (boat.Name != userInput) continue;

                var confirm = AskYesNo($"are you sure you want to delete '{boat.Name}'?");
                if (!confirm) return;

                option.Boats.Remove(boat);

                var db = new AppDbContext();
                db.Boats.Remove(boat);
                db.SaveChanges();

                return;
            }
            
            Console.WriteLine("Boat was not found...");
        }

        private static void GameOptionSetup(GameOption option)
        {
            SetBoardSize(option);

            SetBoatLimit(option);
            
            SetBoatsCanTouch(option);

            SetNextMoveOnHit(option);

            UpdateOptionsInDb(option);

        }

        private static void ShipSetup(BattleShip game)
        {
            var boatArray = game.GetBoatArrays().Item1;

            Console.WriteLine("Ships for PlayerB");
            ShipSetupForOneBoard(boatArray, game);
            game.ChangeWhoPlacesBoats();
            Console.WriteLine("Ships for PlayerA");
            boatArray = game.GetBoatArrays().Item2;
            ShipSetupForOneBoard(boatArray, game);

        }

        private static void ShipSetupForOneBoard(GameBoat[] boatArray, BattleShip game)
        {
            var skip = AskYesNo("Do you want to place boats randomly?");

            if (skip)
            {
                game.AutoShipSetupForOneBoard(boatArray);
                return;
            }

            do
            {
                var selectAnotherBoat = AskYesNo("Select Boat?");

                if (!selectAnotherBoat)
                {
                    if (game.CheckIfBoatLimitIsViolated(boatArray))
                    {
                        Console.WriteLine("Boats don't match the boat limit rule, please fix it!");
                        continue;
                    }

                    if (game.CheckIfBoatsOverlap(boatArray))
                    {
                        Console.WriteLine("Some boats overlap, please fix it!");
                        continue;
                    }
                    var board = game.GetPlaceBoatsByA() ? game.GetBoards().Item1 : game.GetBoards().Item2;
                    if (game.CheckIfTouchViolated(boatArray, board))
                    {
                        Console.WriteLine("Boats' corners are touching, please fix it!");
                        continue;
                    }

                    break;
                }
                
                PrintAvailableBoats(boatArray);
                var shipIndex = SelectShipIndex(boatArray.Length);

                if ((boatArray[shipIndex].Horizontal && boatArray[shipIndex].Size > game.GetBoardWidth())
                    || (!boatArray[shipIndex].Horizontal && boatArray[shipIndex].Size > game.GetBoardHeight())
                )
                {
                    Console.WriteLine("This boat is too big for this board, choose another boat...");
                    continue;
                }
                
                PlaceShip(game, shipIndex);

            } while (true);
        }

        private static void PlaceShip(BattleShip game, int shipIndex)
        {
            var boats = game.GetPlaceBoatsByA() ? game.GetBoatArrays().Item1 : game.GetBoatArrays().Item2;
            CellState[,] board;

            if (boats[shipIndex].CoordX < 0)
            {
                boats[shipIndex].CoordX = 0;
                boats[shipIndex].CoordY = 0;
                game.UpdateBoatsOnBoard();
                board = game.GetPlaceBoatsByA() ? game.GetBoards().Item1 : game.GetBoards().Item2;
                BattleShipConsoleUi.DrawBoard(board, false);
            }
            
            do
            {
                Console.WriteLine("Select action: 'x' - stop, '< > \\/ /\\' - move boat, 'SpaceBar' - rotate, 'r' - remove boat");
                var input = Console.ReadKey();
                Console.WriteLine("");

                if (input.Key == ConsoleKey.RightArrow)
                {
                    game.PlaceBoat(shipIndex, boats[shipIndex].CoordX + 1, boats[shipIndex].CoordY);
                }
                
                if (input.Key == ConsoleKey.LeftArrow)
                {
                    game.PlaceBoat(shipIndex, boats[shipIndex].CoordX - 1, boats[shipIndex].CoordY);
                }
                
                if (input.Key == ConsoleKey.UpArrow)
                {
                    game.PlaceBoat(shipIndex, boats[shipIndex].CoordX, boats[shipIndex].CoordY - 1);
                }
                
                if (input.Key == ConsoleKey.DownArrow)
                {
                    game.PlaceBoat(shipIndex, boats[shipIndex].CoordX, boats[shipIndex].CoordY + 1);
                    
                }

                if (input.Key == ConsoleKey.Spacebar)
                {
                    game.RotateBoat(shipIndex);
                    
                }
                
                if (input.KeyChar == 'r')
                {
                    game.PlaceBoat(shipIndex, -1, -1);
                    break;
                }
                
                game.UpdateBoatsOnBoard();
                board = game.GetPlaceBoatsByA() ? game.GetBoards().Item1 : game.GetBoards().Item2;
                BattleShipConsoleUi.DrawBoard(board, false);

                if (input.KeyChar == 'x')
                {
                    break;
                }
            } while (true);
        }

        private static int SelectShipIndex(int boatArrayLength)
        {
            do
            {
                Console.WriteLine("Please select a ship from the list");
                Console.Write(">");
                var userInput = Console.ReadLine();

                if (!int.TryParse(userInput, out var x))
                {
                    Console.WriteLine("Value is not numeric! try again...");
                    continue;
                }

                if (x - 1 >= 0 && x - 1 <= boatArrayLength - 1) return x - 1;
                Console.WriteLine("Value is out of bounds of the boat array! try again...");
            } while (true);
        }

        private static void PrintAvailableBoats(GameBoat[] boats)
        {
            var i = 1;
            foreach (var boat in boats)
            {
                Console.WriteLine($"{i}) coords: {boat.CoordX}, {boat.CoordY} size: {boat.Size}, horizontal: {boat.Horizontal}");
                i++;
            }
        }

        private static string BattleShip(GameOption gameOptions)
        {
            if (gameOptions.Boats.Count == 0)
            {
                Console.WriteLine("You can't start a game without having a list of ships to choose from, please add ships in the Options menu");
                return "";
            }

            var game = new BattleShip(gameOptions);
            
            if (game.CountBoatsFromOptions() < gameOptions.BoatLimit)
            {
                Console.WriteLine("Not enough boats to meet the boat limit rule, please add more boats or change the boat limit");
                return "";
            }

            ShipSetup(game);

            BattleShipConsoleUi.DrawBothBoards(game.GetBoards(), game.GetTurn());
            
            var menu = new Menu(MenuLevels.Level1);
            menu.AddMenuItem(new MenuItem(
                "Make a move",
                userChoice: "p",
                () =>
                {
                    if (game.GetWinnerString() != "")
                    {
                        Console.WriteLine(game.GetWinnerString());
                        return "";
                    }

                    var (x, y) = GetMoveCoordinates(game);
                    game.RecordMove(x, y);
                    game.MakeAMove(x, y);
                    
                    //can add a fancy BattleShipConsoleUi method that displays the winner string in a cool ascii art
                    BattleShipConsoleUi.DrawBothBoards(game.GetBoards(), game.GetTurn());
                    Console.WriteLine(game.GetWinnerString());
                    return "";
                })
            );
            
            menu.AddMenuItem(new MenuItem(
                "Replay from Journal",
                userChoice: "j",
                () => ReplayFromJournal(game))
            );
            
            menu.AddMenuItem(new MenuItem(
                "Save game",
                userChoice: "s",
                () => SaveGameAction(game, gameOptions))
            );


            menu.AddMenuItem(new MenuItem(
                "Load game",
                userChoice: "l",
                () => LoadGameAction(game, gameOptions))
            );

            var userChoice = menu.RunMenu();


            return userChoice;

        }

        private static string ReplayFromJournal(BattleShip game)
        {
            int choice;

            do
            {
                var entryCounter = 0;
                Console.WriteLine($"{entryCounter}) THE VERY BEGINNING.");
                
                
                foreach (var entry in game.GetJournal())
                {
                    Console.WriteLine($"{entryCounter + 1}) Move: {entry.X}, {entry.Y}");
                    entryCounter++;
                }
                
                Console.WriteLine("Please enter the move number from which you want to continue... (x to cancel)");
                Console.Write(">");
                var userChoice = Console.ReadLine();
                if (userChoice == null)
                {
                    Console.WriteLine("Invalid input! try again... or x to cancel!");
                    continue;
                }

                if (userChoice.ToLower() == "x") return "";

                if (!int.TryParse(userChoice, out choice))
                {
                    Console.WriteLine("Invalid input! try again... or press x to cancel!");
                    continue;
                }

                if (choice > game.GetJournalCount() || choice < 0) 
                {
                    Console.WriteLine("Chosen value is outside of journal's bounds... try again... or press x to cancel!");
                    continue;
                }

                break;
            } while (true);


            game.ReplayJournalEntry(choice - 1);
            BattleShipConsoleUi.DrawBothBoards(game.GetBoards(), game.GetTurn());
            

            return "";
        }

        private static (int x, int y) GetMoveCoordinates(BattleShip game)
        {
            int x, y;
            do
            {
                Console.WriteLine("Upper left corner is (1,1)!");
                Console.WriteLine($"Give X (1-{game.GetBoardWidth()}), Y (1-{game.GetBoardHeight()}):");
                Console.Write(">");
                var userInput = Console.ReadLine();
                if (userInput == null || userInput.Length < 3)
                {
                    Console.WriteLine("Invalid input! try again...");
                    continue;
                }
                var userValue = userInput.Split(',');
                if (userValue.Length != 2)
                {
                    Console.WriteLine("Invalid input! try again...");
                    continue;
                }
                if (!int.TryParse(userValue[0].Trim(), out x))
                {
                    Console.WriteLine("Invalid input! try again...");
                    continue;
                }
                if (!int.TryParse(userValue[1].Trim(), out y))
                {
                    Console.WriteLine("Invalid input! try again...");
                    continue;
                }
                if (x > game.GetBoardWidth() || y > game.GetBoardHeight() || x < 0 || y < 0)
                {
                    Console.WriteLine("Chosen tile is out of bounds! try again...");
                    continue;
                }

                break;
            } while (true);

            x -= 1;
            y -= 1;

            return (x, y);
        }
        
        private static string LoadGameAction(BattleShip game, GameOption gameOption)
        {
            /*var files = System.IO.Directory.EnumerateFiles(".", "*.json").ToList();
            for (int i = 0; i < files.Count; i++)
            {
                Console.WriteLine($"{i} - {files[i]}");
            }

            var fileNo = Console.ReadLine();
            var fileName = files[int.Parse(fileNo!.Trim())];

            var jsonString = System.IO.File.ReadAllText(fileName);

            game.SetGameStateFromJsonString(jsonString);
            */
            
            using var db = new AppDbContext();

            var saveGames = db.GameSaveDatas.ToList();

            if (saveGames.Count == 0)
            {
                Console.WriteLine("There are no saves available");
                return "";
            }

            for (int i = 0; i < saveGames.Count; i++)
            {
                Console.WriteLine($"{i} - {saveGames[i].TimeStamp} = {saveGames[i].SaveName}");
            }
            
            Console.WriteLine("Please Select a save to load.. (x to cancel)");
            Console.Write(">");
            var saveNo = Console.ReadLine();

            if (saveNo == null)
            {
                Console.WriteLine("Invalid input! try again...");
                return "";
            }

            if (saveNo.ToLower() == "x")
            {
                BattleShipConsoleUi.DrawBothBoards(game.GetBoards(), game.GetTurn());
                
                return "";
            }

            if (!int.TryParse(saveNo, out var userChoice))
            {
                Console.WriteLine("Invalid input! try again...");
                return "";
            }

            if (userChoice >= saveGames.Count)
            {
                Console.WriteLine("Invalid input! try again...");
                return "";
            }

            var saveName = saveGames[userChoice];

            var jsonString = saveName.SerializedGameData;
            game.SetGameStateFromJsonString(jsonString, gameOption);
            
            BattleShipConsoleUi.DrawBothBoards(game.GetBoards(), game.GetTurn());
            
            return "";
        }
        
        private static string SaveGameAction(BattleShip game, GameOption gameOption)
        {
            // 2020-10-12
            /*var defaultName = "save_" + DateTime.Now.ToString("yyyy-MM-dd") + ".json";
            Console.Write($"File name ({defaultName}):");
            var fileName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = defaultName;
            }*/

            var serializedGame = game.GetSerializedGameState();
            
            using var db = new AppDbContext();

            var saveName = "";

            do
            {
                Console.WriteLine("Please Name Your Save... (x to cancel)");
                Console.Write(">");
                saveName = Console.ReadLine();

                if (saveName != null)
                {
                    break;
                }

                Console.WriteLine("Please enter a valid name!");

            } while (true);

            if (saveName.ToLower() == "x")
            {
                BattleShipConsoleUi.DrawBothBoards(game.GetBoards(), game.GetTurn());
                
                return "";
            }
            
            var gameSaveData = new GameSaveData()
            {
                SerializedGameData = serializedGame,
                SaveName = saveName,
            };
            
            gameOption.GameSaveData.Add(gameSaveData);
            db.GameOptions.Update(gameOption);
            db.SaveChanges();


            //Console.WriteLine(serializedGame);
            //System.IO.File.WriteAllText(fileName, serializedGame);
            
            return "";
        }

        private static void PrintEnum<T>(List<T> enumList)
        {
            for (int i = 0; i < enumList.Count; i++)
            {
                Console.WriteLine($"{i + 1}) {enumList[i]}");
            }
        }

        private static List<T> GetEnumList<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }
        
        private static bool AskYesNo(string question)
        {
            do
            {
                Console.WriteLine(question);
                Console.Write(">");
                var answer = Console.ReadLine();

                if (string.IsNullOrEmpty(answer))
                {
                    Console.WriteLine("Please answer with 'y,n' or 'yes,no'!");
                    continue;
                }

                answer = answer.Trim().ToLower();
                switch (answer)
                {
                    case "y":
                    case "yes":
                        return true;
                    case "n":
                    case "no":
                        return false;
                    default:
                        Console.WriteLine("Please answer with 'y,n' or 'yes,no'!");
                        break;
                }
            } while (true);
        }
    }
}