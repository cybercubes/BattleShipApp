using System;
using System.ComponentModel.Design;
using MenuSystem;
using MenuSystem.Enums;

namespace ica0016_2020f
{
    static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("============> TIC-TAC-TOE KILOSS <=================");

            var menu = new Menu(MenuLevels.Level0);
            var menuA = new Menu(MenuLevels.Level1);
            var menuB = new Menu(MenuLevels.Level2Plus);
            var menuC = new Menu(MenuLevels.Level2Plus);
            
            menu.AddMenuItem(new MenuItem("New game player vs player", "1", DefaultMenuAction));
            menu.AddMenuItem(new MenuItem("New game person vs AI", "2", DefaultMenuAction));
            menu.AddMenuItem(new MenuItem("New game AI vs AI", "3", DefaultMenuAction));
            menu.AddMenuItem(new MenuItem("test submenus", "s", menuA.RunMenu));


            menuA.AddMenuItem(new MenuItem("Sub 2.", "1", menuB.RunMenu));
            
            menuB.AddMenuItem(new MenuItem("sub 3.", "1", menuC.RunMenu));
            
            menuC.AddMenuItem(new MenuItem("end of the line?", "1", DefaultMenuAction));

            menu.RunMenu();
            
        }
        
        public static string DefaultMenuAction()
        {
            Console.WriteLine("Not implemented yet!");
            return "";
        }
    }
}