using System;
using System.Collections.Generic;
using System.Linq;
using MenuSystem.Enums;

namespace MenuSystem
{

    public class Menu
    {
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        private readonly MenuLevels _menuLevel;
        
        public Menu(MenuLevels level)
        {
            _menuLevel = level;
        }

        public void RunMenu()
        {
            var userChoice = "";
            do
            {
                Console.Write("");
                foreach (var menuItem in MenuItems)
                {
                    Console.WriteLine(menuItem);
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
                if (userChoice == "x")
                {
                    Console.WriteLine("closing down.........");
                    break;  
                }

                var userMenuItem = MenuItems.FirstOrDefault(t => t.UserChoice == userChoice);
                if (userMenuItem != null)
                {
                    userMenuItem.MethodToExecute();
                }
                else
                {
                    Console.WriteLine("No such option!!");    
                }
                
            } while (userChoice != "x");
        }
        public static void DefaultMenuAction()
        {
            Console.WriteLine("Not implemented yet!");
        }
    }
}