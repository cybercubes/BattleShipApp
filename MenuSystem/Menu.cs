using System;
using System.Collections.Generic;

namespace MenuSystem
{
    public class Menu
    {
        public List<MenuItem> MenuItems { get; set; }

        public Menu()
        {
            MenuItems = new List<MenuItem>();
            MenuItems.Add(new MenuItem("new game player vs player", "1"));
            MenuItems.Add(new MenuItem("New game person vs AI", "2"));
            MenuItems.Add(new MenuItem("new game AI vs AI", "3"));
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
                
                Console.WriteLine("L) Load game");
                Console.WriteLine("X) exit");
                Console.Write(">");
                
                userChoice = Console.ReadLine()?.ToLower().Trim() ?? "";

                Console.WriteLine("Your Choice was:" + userChoice);
                switch (userChoice)
                {
                    case "1":
                        Console.WriteLine("not implemented yet");
                        break;
                
                    case "2":
                        Console.WriteLine("not implemented yet");
                        break;
                
                    case "3":
                        Console.WriteLine("not implemented yet");
                        break;
                
                    case "l":
                        Console.WriteLine("not implemented yet");
                        break;
                    case "x":
                        Console.WriteLine("closing down");
                        break;
                    default:
                        Console.WriteLine("Invalid input, no option matches input");
                        break;
                }    
            } while (userChoice != "x");
        
        }
    }
}