//               Copyright Joël Ganesh 2024.
// Distributed under the Boost Software License, Version 1.0.
//    (See accompanying file LICENSE_1_0.txt or copy at
//          https://www.boost.org/LICENSE_1_0.txt)

namespace IQFit.Logic
{
    // Represents an instance of a (randomized) puzzle together with its solution.
    public class Game
    {
        private readonly Board board;
        private readonly Puzzle puzzle;
        private readonly Random rand = new Random();

        // Generates layout of board and puzzle.
        public Game()
        {
            board = new Board(rand);
            puzzle = new Puzzle(rand, board);
        }

        public Game(int?[,] grid)
        {
            board = new Board(rand, grid);
            puzzle = new Puzzle(rand, board, grid);
        }

        public int?[,] Puzzle()
        {
            return puzzle.grid;
        }

        public void Hint()
        {
            puzzle.Hint();
        }

        public int?[,] Solution()
        {
            return board.grid;
        }
    }
}
