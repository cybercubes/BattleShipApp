using System;
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

            menu.MenuItems.Add(new MenuItem("got to submenu", "s", menuA.RunMenu));
            menu.MenuItems.Add(new MenuItem("New game player vs player", "1", Menu.DefaultMenuAction));
            menu.MenuItems.Add(new MenuItem("New game person vs AI", "2", Menu.DefaultMenuAction));
            menu.MenuItems.Add(new MenuItem("New game AI vs AI", "3", Menu.DefaultMenuAction));
            
            menuA.MenuItems.Add(new MenuItem("Sub 2.", "1", menuB.RunMenu));
            menuB.MenuItems.Add(new MenuItem("go to submenu 2 .", "1", Menu.DefaultMenuAction));

            menu.RunMenu();
            
        }
        
        
    }
}