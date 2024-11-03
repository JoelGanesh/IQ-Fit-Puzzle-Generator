//               Copyright Joël Ganesh 2024.
// Distributed under the Boost Software License, Version 1.0.
//    (See accompanying file LICENSE_1_0.txt or copy at
//          https://www.boost.org/LICENSE_1_0.txt)

namespace IQFit.Logic
{
    // Represents the layout of a 10x5 grid, which is the layout used for the game.
    public class Grid()
    {
        public int?[,] grid = new int?[10, 5];

        // Updates the index to the first unfilled index of the board.
        public void UpdateIndex(ref int index)
        {
            while (index < 50 && grid[index / 5, index % 5] != null)
            {
                index++;
            }
        }

        // Attempts to fill the board with a certain piece, so that the uppermost left part of the piece is on index.
        // Returns a boolean according to whether the action was performed succesfully or not.
        public bool Fill(int index, Piece piece, int shape_id)
        {
            int[,] shape = piece.shapes[shape_id];
            int index0 = index / 5, index1 = index % 5;
            for (int j = 0; j < shape.GetLength(1); j++)
            {
                if (shape[0, j] != 0)
                {
                    break;
                }
                index1--;
            }

            // Early return false if the shape would exceed the board limitations.
            if (index0 + shape.GetLength(0) > 10 || index1 < 0 || index1 + shape.GetLength(1) > 5)
            {
                return false;
            }

            // Check if it is possible to fill the board with the shape at the desired positions.
            for (int i = 0; i < shape.GetLength(0); i++)
            {
                for (int j = 0; j < shape.GetLength(1); j++)
                {
                    if (shape[i, j] != 0 && grid[index0 + i, index1 + j] != null)
                    {
                        return false;
                    }
                }
            }

            // Now that we have established that it is possible to place the
            // piece on the board, we will actually update the board.
            for (int i = 0; i < shape.GetLength(0); i++)
            {
                for (int j = 0; j < shape.GetLength(1); j++)
                {
                    if (shape[i, j] != 0)
                    {
                        grid[index0 + i, index1 + j] = piece.piece_id;
                    }
                }
            }

            return true;
        }

        // Removes all occurrences of a value in the grid and returns the minimal index updated.
        public int Unfill(int value)
        {
            int? min_index = null;
            for (int i = 49; i >= 0; i--)
            {
                if (grid[i / 5, i % 5] == value)
                {
                    grid[i / 5, i % 5] = null;
                    min_index = i;
                }
            }
            if (min_index == null)
            {
                return -1;
            }
            return min_index.Value;
        }


        // Displays the grid in the console.
        public void Display()
        {
            string t = "";
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                t += '-';
            }
            Console.WriteLine($"+{t}+");
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                string s = "";
                for (int i = 0; i < grid.GetLength(0); i++)
                {
                    s += grid[i, j] == null ? " " : grid[i, j];
                }
                Console.WriteLine($"|{s}|");
            }
            Console.WriteLine($"+{t}+");
            Console.WriteLine();
        }
    }
}
