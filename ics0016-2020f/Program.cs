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
    static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("============> BattleShip KILOSS <=================");
            
            var gameOptions = new GameOption();

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
                    Console.WriteLine("Invalid input! try again...1");
                    continue;
                }
                var userValue = userInput?.Split(',');
                if (userValue?.Length != 2)
                {
                    Console.WriteLine("Invalid input! try again...2");
                    continue;
                }
                if (!int.TryParse(userValue?[0].Trim(), out x))
                {
                    Console.WriteLine("Invalid input! try again...3");
                    continue;
                }
                if (!int.TryParse(userValue?[1].Trim(), out y))
                {
                    Console.WriteLine("Invalid input! try again...4");
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

        private static void PrintBoatList(GameOption option)
        {
            if (option.Boats == null)
            {
                Console.WriteLine("The boat list is empty");
                return;
            }

            foreach (var boat in option.Boats)
            {
                Console.WriteLine($"* {boat.ToString()}");
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

                if (option.Boats == null)
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

            option.Boats ??= new List<Boat>();
            
            option.Boats.Add(boat);

        }

        private static void RemoveBoat(GameOption option)
        {
            if (option.Boats == null)
            {
                Console.WriteLine("List of boats is empty!");
                return;
            }

            var userInput = "";
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
                option.Boats.Remove(boat);
                return;
            }
            
            Console.WriteLine("Boat was not found...");
        }

        private static void GameOptionSetup(GameOption option)
        {
            SetBoardSize(option);
            
            SetBoatsCanTouch(option);

            SetNextMoveOnHit(option);

        }

        private static void ShipSetup(BattleShip game)
        {
            var boatArray = game.GetBoatArrays().Item1;

            Console.WriteLine("Ships for PlayerA");
            ShipSetupForOneBoard(boatArray, game);
            game.ChangeWhoPlacesBoats();
            Console.WriteLine("Ships for PlayerB");
            boatArray = game.GetBoatArrays().Item2;
            ShipSetupForOneBoard(boatArray, game);

        }

        private static void ShipSetupForOneBoard(GameBoat[] boatArray, BattleShip game)
        {

            do
            {
                var selectAnotherBoat = AskYesNo("Select Boat?");

                if (!selectAnotherBoat)
                {
                    if (game.CheckIfBoatsOverlap(boatArray))
                    {
                        Console.WriteLine("Some boats overlap, please fix it!");
                        continue;
                    }
                    var board = game.GetPlaceBoatsByA() ? game.GetBoards().Item1 : game.GetBoards().Item2;
                    if (game.CheckIfTouchViolated(boatArray, board))
                    {
                        Console.WriteLine("boats' corners are touching, please fix it!");
                        continue;
                    }

                    break;
                }
                
                PrintAvailableBoats(boatArray);
                var shipIndex = SelectShipIndex(boatArray.Length);

                PlaceShip(game, shipIndex);

            } while (true);
        }

        private static void PlaceShip(BattleShip game, int shipIndex)
        {
            var boats = game.GetPlaceBoatsByA() ? game.GetBoatArrays().Item1 : game.GetBoatArrays().Item2;

            if (boats[shipIndex].CoordX < 0)
            {
                boats[shipIndex].CoordX = 0;
                boats[shipIndex].CoordY = 0;
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
                var board = game.GetPlaceBoatsByA() ? game.GetBoards().Item1 : game.GetBoards().Item2;
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
                if (boat == null) continue;
                Console.WriteLine($"{i}) coords: {boat.CoordX}, {boat.CoordY} size: {boat.Size}, horizontal: {boat.Horizontal}");
                i++;
            }
        }

        private static void TempBoatOptionSolution(GameOption option)
        {
            option.Boats = new List<Boat>
            {
                new Boat()
                {
                    Name = "Fubuki", Amount = 2, Size = 2,
                    
                },
                new Boat()
                {
                    Name = "Hamakaze", Amount = 3, Size = 1,
                    
                }
            };
        }

        private static string BattleShip(GameOption gameOptions)
        {
            TempBoatOptionSolution(gameOptions);
            
            if (gameOptions.Boats == null)
            {
                Console.WriteLine("You can't start a game without having a list of ships to choose from, please add ships in the Option menu");
                return "";
            }

            /*var initiateGameOptionSetup = AskYesNo("Do you want to setup rules from scratch? (if no, options saved in 'Setup Game Options' will be used)");

            if (initiateGameOptionSetup)
            {
                GameOptionSetup(gameOptions);
            }*/

            var game = new BattleShip(gameOptions);

            ShipSetup(game);

            BattleShipConsoleUi.DrawBothBoards(game.GetBoards(), game.GetTurn());
            
            var menu = new Menu(MenuLevels.Level1);
            menu.AddMenuItem(new MenuItem(
                $"Make a move",
                userChoice: "p",
                () =>
                {
                    var (x, y) = GetMoveCoordinates(game);
                    game.MakeAMove(x, y);
                    BattleShipConsoleUi.DrawBothBoards(game.GetBoards(), game.GetTurn());
                    return "";
                })
            );
            
            menu.AddMenuItem(new MenuItem(
                $"Save game",
                userChoice: "s",
                () => SaveGameAction(game))
            );


            menu.AddMenuItem(new MenuItem(
                $"Load game",
                userChoice: "l",
                () => LoadGameAction(game))
            );
            
            menu.AddMenuItem(new MenuItem(
                $"Database",
                userChoice: "d",
                () => SaveEntryIntoDb(game))
            );

            var userChoice = menu.RunMenu();


            return userChoice;

        }

        private static (int x, int y) GetMoveCoordinates(BattleShip game)
        {
            int x, y;
            do
            {
                Console.WriteLine("Upper left corner is (1,1)!");
                Console.Write($"Give X (1-{game.GetBoardWidth()}), Y (1-{game.GetBoardHeight()}):");
                var userInput = Console.ReadLine();
                if (userInput == null || userInput.Length < 3)
                {
                    Console.WriteLine("Invalid input! try again...1");
                    continue;
                }
                var userValue = userInput?.Split(',');
                if (userValue?.Length != 2)
                {
                    Console.WriteLine("Invalid input! try again...2");
                    continue;
                }
                if (!int.TryParse(userValue?[0].Trim(), out x))
                {
                    Console.WriteLine("Invalid input! try again...3");
                    continue;
                }
                if (!int.TryParse(userValue?[1].Trim(), out y))
                {
                    Console.WriteLine("Invalid input! try again...4");
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
        
        private static string LoadGameAction(BattleShip game)
        {
            var files = System.IO.Directory.EnumerateFiles(".", "*.json").ToList();
            for (int i = 0; i < files.Count; i++)
            {
                Console.WriteLine($"{i} - {files[i]}");
            }

            var fileNo = Console.ReadLine();
            var fileName = files[int.Parse(fileNo!.Trim())];

            var jsonString = System.IO.File.ReadAllText(fileName);

            game.SetGameStateFromJsonString(jsonString);
            
            BattleShipConsoleUi.DrawBothBoards(game.GetBoards(), game.GetTurn());
            
            return "";
        }
        
        private static string SaveGameAction(BattleShip game)
        {
            // 2020-10-12
            var defaultName = "save_" + DateTime.Now.ToString("yyyy-MM-dd") + ".json";
            Console.Write($"File name ({defaultName}):");
            var fileName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = defaultName;
            }

            var serializedGame = game.GetSerializedGameState();
            
            // Console.WriteLine(serializedGame);
            System.IO.File.WriteAllText(fileName, serializedGame);
            
            return "";
        }
        
        private static string SaveEntryIntoDb(BattleShip game)
        {

            using (var db = new AppDbContext())
            {
                //db.Database.EnsureDeleted();
                //return "";
                
                /*
                var gameSaveData = new GameSaveData()
                {
                    SerializedGameData = game.GetSerializedGameState(),
                };

                var gameOption = game.GetGameOptions();
                gameOption.GameSaveData = gameSaveData;

                db.GameOptions.Add(gameOption);*/

                db.SaveChanges();
                
                Console.WriteLine("From Db");

                foreach (var gameOption in db.GameOptions
                    .Include(g => g.GameSaveData)
                    .OrderByDescending(g => g.GameSaveDataId)
                )
                {
                    Console.WriteLine(gameOption.BoardHeight);
                    Console.WriteLine(gameOption.BoardWidth);
                    Console.WriteLine(gameOption.GameSaveData.TimeStamp);
                    Console.WriteLine(gameOption.CanBoatsTouch);
                    Console.WriteLine(gameOption.MoveOnHit);
                    
                }

            }

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