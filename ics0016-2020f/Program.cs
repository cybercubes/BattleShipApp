using System;
using MenuSystem;

namespace ica0016_2020f
{
    static class Program
    {
        private static void DefaultMenuAction()
        {
            Console.WriteLine("Not implemented yet!");
        }
        static void Main(string[] args)
        {
            Console.WriteLine("============> TIC-TAC-TOE KILOSS <=================");

            var menuA = new Menu(MenuLevel.Level1);
            menuA.MenuItems.Add(new MenuItem("Sub 2.", "1", menuA.RunMenu));
            
            var menuB = new Menu(MenuLevel.Level2Plus);
            menuB.MenuItems.Add(new MenuItem("go to submenu 2 .", "1", DefaultMenuAction));
            
            var menu = new Menu(MenuLevel.Level0);
            menu.MenuItems.Add(new MenuItem("got to submenu", "s", menuA.RunMenu));
            menu.MenuItems.Add(new MenuItem("New game player vs player", "1", DefaultMenuAction));
            menu.MenuItems.Add(new MenuItem("New game person vs AI", "2", DefaultMenuAction));
            menu.MenuItems.Add(new MenuItem("New game AI vs AI", "3", DefaultMenuAction));
            menu.RunMenu();
            
        }
        
        
    }
}