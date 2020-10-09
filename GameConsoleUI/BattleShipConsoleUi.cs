using System;
using GameBrain;

namespace GameConsoleUi
{
    public static class BattleShipConsoleUi
    {
        public static void DrawBoard(CellState[,] board)
        {
            // add plus 1, since this is 0 based. length 0 is returned as -1;
            var width = board.GetUpperBound(0) + 1; // x
            var height = board.GetUpperBound(1) + 1; // y

            for (int colIndex = 0; colIndex < width; colIndex++)
            {
                Console.Write($"+---+");
            }
            Console.WriteLine();

            for (int rowIndex = 0; rowIndex < height; rowIndex++)
            {
                for (int colIndex = 0; colIndex < width; colIndex++)
                {
                    Console.Write($"| {CellString(board[colIndex, rowIndex])} |");
                }
                Console.WriteLine();
                for (int colIndex = 0; colIndex < width; colIndex++)
                {
                    Console.Write($"+---+");
                }
                Console.WriteLine();
                
            }
        }

        public static string CellString(CellState cellState)
        {
            switch (cellState)
            {
                case CellState.Empty: return " ";
                case CellState.O: return "O";
                case CellState.X: return "X";
            }

            return "-";
        }

    }
}