using System;
using System.Collections.Generic;
using System.Linq;
using MenuSystem.Enums;

namespace MenuSystem
{

    public class Menu
    {
        private Dictionary<string, MenuItem> MenuItems { get; set; } = new Dictionary<string, MenuItem>();
        
        private readonly MenuLevels _menuLevel;

        private readonly string[] _reservedActions = {"x", "m", "r"};
        
        public Menu(MenuLevels level)
        {
            _menuLevel = level;
        }

        public void AddMenuItem(MenuItem item)
        {
            if (item.UserChoice == "")
            {
                throw new ArgumentException("UserChoice is an empty string");
            }

            if (_reservedActions.Contains(item.UserChoice.ToLower()))
            {
                throw new ArgumentException("UserChoice is reserved");
            }
            
            MenuItems.Add(item.UserChoice.ToLower(), item);
        }
        public string RunMenu()
        {
            if (MenuItems.Count == 0)
            {
                Console.WriteLine("that menu has no options available");
                return "";
            }
            
            var userChoice = "";
            do
            {
                Console.Write("");
                
                foreach (var menuItem in MenuItems)
                {
                    Console.WriteLine(menuItem.Value);
                }

                switch (_menuLevel)
                {
                    case MenuLevels.Level0:
                        Console.WriteLine("X) exit");
                        break;
                    case MenuLevels.Level1:
                        Console.WriteLine("M) return to Main");
                        Console.WriteLine("X) exit");
                        break;
                    case MenuLevels.Level2Plus:
                        Console.WriteLine("R) return to previous");
                        Console.WriteLine("M) return to Main");
                        Console.WriteLine("X) exit");
                        break;
                    default:
                        throw new Exception("unknown menu depth");
                }

                Console.Write(">");

                userChoice = Console.ReadLine()?.ToLower().Trim() ?? "";
                Console.WriteLine("Your Choice was:" + userChoice);

                //is it a reserved keyword
                if (!_reservedActions.Contains(userChoice))
                {
                    //no it wasn't, try to find a keyword in MenuItems
                    if (MenuItems.TryGetValue(userChoice, out var userMenuItem))
                    {
                        userChoice = userMenuItem.MethodToExecute();
                    }
                    else
                    {
                        Console.WriteLine("No such option!!");
                    }
                }
                
                if (userChoice == "x")
                {
                    if (_menuLevel == MenuLevels.Level0)
                        Console.WriteLine("closing down.........");
                    break;
                }
                
                if (userChoice == "m" && _menuLevel != MenuLevels.Level0)
                {
                    break;
                }
                
                if (userChoice == "r" && _menuLevel == MenuLevels.Level2Plus)
                {
                    userChoice = null;
                    break;
                }

            } while (true);

            return userChoice!;
        }
    }
}