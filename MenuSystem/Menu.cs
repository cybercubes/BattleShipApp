using System;
using System.Collections.Generic;
using System.Linq;

namespace MenuSystem
{
    public enum MenuLevel
    {
        Level0,
        Level1,
        Level2Plus
    }
    
    public class Menu
    {
        public List<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        private readonly MenuLevel _menuLevel;
        
        public Menu(MenuLevel level)
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
                    case MenuLevel.Level0:
                        Console.WriteLine("X) exit");
                        break;
                    case MenuLevel.Level1:
                        Console.WriteLine("M) return to Main");
                        Console.WriteLine("X) exit");
                        break;
                    case MenuLevel.Level2Plus:
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
    }
}