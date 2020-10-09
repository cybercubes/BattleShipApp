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
            menu.AddMenuItem(new MenuItem("New game human vs human. Pointless.", "1", BattleShip));

            menu.RunMenu();

        }

        private static string BattleShip()
        {
            var game = new BattleShip();
            
            BattleShipConsoleUi.DrawBoard(game.GetBoard());
            
            var menu = new Menu(MenuLevels.Level0);
            menu.AddMenuItem(new MenuItem(
                $"Player {(game.NextMoveByX ? "X" : "O")} make a move",
                userChoice: "p",
                () =>
                {
                    var (x, y) = GetMoveCordinates(game);
                    game.MakeAMove(x, y);
                    BattleShipConsoleUi.DrawBoard(game.GetBoard());
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
            menu.AddMenuItem(new MenuItem(
                $"Exit game",
                userChoice: "e",
                DefaultMenuAction)
            );

            var userChoice = menu.RunMenu();


            return userChoice;

        }
        
        static (int x, int y) GetMoveCordinates(BattleShip game)
        {
            Console.WriteLine("Upper left corner is (1,1)!");
            Console.Write("Give X (1-3), Y (1-3):");
            var userValue = Console.ReadLine()?.Split(',');

            var x = int.Parse(userValue[0].Trim()) - 1;
            var y = int.Parse(userValue[1].Trim()) - 1;

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
            
            BattleShipConsoleUi.DrawBoard(game.GetBoard());
            
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