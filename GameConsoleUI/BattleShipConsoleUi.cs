using System;
using Domain.Enums;

namespace GameConsoleUi
{
    public static class BattleShipConsoleUi
    {

        public static void DrawBothBoards((CellState[,], CellState[,]) boards, bool isAsTurn)
        {
            DrawBoard(isAsTurn ? boards.Item1 : boards.Item2, true);
            Console.WriteLine("YOU\\/=====================/\\THEM");
            DrawBoard(isAsTurn ? boards.Item2 : boards.Item1, false);
            Console.WriteLine($"Player {(isAsTurn ? "A" : "B")}'s Turn");
        }
        
        public static void DrawBoard(CellState[,] board, bool hideShips)
        {
            // add plus 1, since this is 0 based. length 0 is returned as -1;
            var width = board.GetUpperBound(1) + 1; // x
            var height = board.GetUpperBound(0) + 1; // y

            for (int colIndex = 0; colIndex < width; colIndex++)
            {
                Console.Write($"+---+");
            }
            Console.WriteLine();

            for (var rowIndex = 0; rowIndex < height; rowIndex++)
            {
                for (var colIndex = 0; colIndex < width; colIndex++)
                {
                    Console.Write($"| {CellString(board[rowIndex, colIndex], hideShips)} |");
                }
                Console.WriteLine();
                for (var colIndex = 0; colIndex < width; colIndex++)
                {
                    Console.Write($"+---+");
                }
                Console.WriteLine();
                
            }
        }

        private static string CellString(CellState cellState, bool hideShips)
        {
            switch (cellState)
            {
                case CellState.Empty:
                    return " ";
                case CellState.Ship:
                    return hideShips ? " " : "8";
                case CellState.Miss:
                    return "X";
                case CellState.HitShip:
                    return "H";
            }

            return "-";
        }

    }
}