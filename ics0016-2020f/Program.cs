using System;
using System.Linq;
using GameBrain;
using GameConsoleUi;
using MenuSystem;
using MenuSystem.Enums;

namespace ica0016_2020f
{
    static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("============> BattleShip KILOSS <=================");

            var menu = new Menu(MenuLevels.Level0);
            menu.AddMenuItem(new MenuItem("New PvP game", "1", BattleShip));

            menu.RunMenu();

        }

        private static string BattleShip()
        {
            // initiate a function that will define board dimensions
            
            // intiate a function that will define ship amounts
            
            // initiate some function to fill out 2 boards with ships
            
            
            var game = new BattleShip(5);
            
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
                () => { return SaveGameAction(game); })
            );


            menu.AddMenuItem(new MenuItem(
                $"Load game",
                userChoice: "l",
                () => { return LoadGameAction(game); })
            );

            var userChoice = menu.RunMenu();


            return userChoice;

        }
        
        static (int x, int y) GetMoveCoordinates(BattleShip game)
        {
            int x, y;
            do
            {
                Console.WriteLine("Upper left corner is (1,1)!");
                Console.Write($"Give X (1-{game.GetBoardSize()}), Y (1-{game.GetBoardSize()}):");
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
                if (x > game.GetBoardSize() || y > game.GetBoardSize() || x < 0 || y < 0)
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
        
        static string LoadGameAction(BattleShip game)
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
        
        static string SaveGameAction(BattleShip game)
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

        private static string DefaultMenuAction()
        {
            Console.WriteLine("Not implemented yet!");
            return "";
        }
    }
}