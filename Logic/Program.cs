//               Copyright Joël Ganesh 2024.
// Distributed under the Boost Software License, Version 1.0.
//    (See accompanying file LICENSE_1_0.txt or copy at
//          https://www.boost.org/LICENSE_1_0.txt)

namespace IQFit.Logic
{
    using System;

    internal class Program
    {
        // Repeatedly computes puzzles and solutions.
        // The solution is only shown when a key is pressed.
        // If the Escape key is pressed, the application closes after showing the solution.
        static void Main()
        {
            ConsoleKey c;
            do
            {
                Game game = new Game();
                game.DisplayPuzzle();

                c = Console.ReadKey(true).Key;
                game.DisplaySolution();
            }
            while (c != ConsoleKey.Escape);
            Console.ReadKey();
        }
    }

    // Represents an instance of a (randomized) puzzle together with its solution.
    public class Game
    {
        private readonly Board board;
        private readonly Puzzle puzzle;
        private readonly Random rand;

        // Generates layout of board and puzzle.
        public Game()
        {
            rand = new Random();
            board = new Board(rand, "");
            puzzle = new Puzzle(rand, board);
        }

        public void DisplayPuzzle()
        {
            puzzle.Display();
        }

        public void DisplaySolution()
        {
            board.Display();
        }
    }
}