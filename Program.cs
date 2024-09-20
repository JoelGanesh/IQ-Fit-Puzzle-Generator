//               Copyright Joël Ganesh 2024.
// Distributed under the Boost Software License, Version 1.0.
//    (See accompanying file LICENSE_1_0.txt or copy at
//          https://www.boost.org/LICENSE_1_0.txt)

namespace IQFit
{
	using System;
	using System.Collections.Generic;
	using System.Collections.Immutable;
	using static Utility;

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

	public class Board
	{
		public readonly int[,] grid;
		private readonly Random rand;
		private readonly int index;

		public Board(Random rand, string field = "")
		{
			this.rand = rand;
			grid = new int[10, 5];
			index = 0;
			List<Piece> pieces = Utility.pieces.ToList();

			// The field parameter can be used to fill a board,
			// for instance to solve a puzzle.
			HashSet<int> set = new HashSet<int>();
			if (field != "")
			{
				string[] rows = field.Split('\n');
				for (int j = 0; j < 5; j++)
				{
					string row = rows[j];
					for (int i = 0; i < Math.Min(10, row.Length); i++)
					{
						char c = row[i];
						if (c >= '0' && c <= '9')
						{
							grid[i, j] = c - '0' + 1;
							set.Add(c - '0');
						}
					}
				}
			}
			for (int i = 9; i >= 0; i--)
			{
				if (set.Contains(i))
				{
					pieces.RemoveAt(i);
				}
			}
			UpdateIndex(grid, ref index);
			FillBoard(index, pieces.ToList());
		}

		// Attempts to fill the board, starting at a certain index,
		// taking a random sequence of pieces from the list.
		private bool FillBoard(int index, List<Piece> pieces)
		{
			if (index >= 50 && pieces.Count == 0)
			{
				return true;
			}
			if (index >= 50 || pieces.Count == 0)
			{
				return false;
			}

			int[] piece_ordering = GenerateOrdering(rand, pieces.Count);
			int[] shape_ordering = GenerateOrdering(rand, 8);
			for (int i = 0; i < pieces.Count; i++)
			{
				int piece_id = piece_ordering[i];
				Piece piece = pieces[piece_id];

				pieces.RemoveAt(piece_id);
				for (int j = 0; j < 8; j++)
				{
					int shape_id = shape_ordering[j];

					bool filled = Fill(grid, index, piece, shape_id);
					if (filled)
					{
						UpdateIndex(grid, ref index);

						bool success = FillBoard(index, pieces);
						if (success)
						{
							return true;
						}
						index = Unfill(grid, piece.piece_id);
					}
				}
				pieces.Insert(piece_id, piece);
			}
			return false;
		}

		public void Display()
		{
			Utility.Display(grid);
		}
	}

	public class Puzzle
	{
		private readonly int[,] puzzle;
		private readonly int[,] board;
		private readonly Random rand;

		public Puzzle(Random rand, Board board)
		{
			this.rand = rand;
			this.board = board.grid;

			puzzle = GeneratePuzzle();
		}

		// Generates a puzzle which has unique solution (being the board),
		// such that it gives away as little pieces as necessary.
		// Returns the desired puzzle.
		private int[,] GeneratePuzzle()
		{
			int[,] puzzle = new int[10, 5];
			List<Piece> pieces = Utility.pieces.ToList();
			(puzzle, _) = GeneratePuzzle(puzzle, pieces);

			return puzzle;
		}

		// Helper method of the generic GeneratePuzzle method.
		// It returns a puzzle with a unique solution paired with the number of pieces used.
		private (int[,], int) GeneratePuzzle(int[,] puzzle, List<Piece> pieces)
		{
			int[] piece_ordering = GenerateOrdering(rand, pieces.Count);
			int pieces_count_min = int.MaxValue;
			int[,] puzzle_min = new int[10, 5];

			for (int i = 0; i < pieces.Count; i++)
			{
				int piece_id = piece_ordering[i];
				Piece piece = pieces[piece_id];
				pieces.RemoveAt(piece_id);

				Copy(puzzle, piece.piece_id);

				bool is_solvable = Solvable(puzzle, 0, pieces);
				if (!is_solvable)
				{
					int[,] puzzle_copy;
					int pieces_count_copy;
					(puzzle_copy, pieces_count_copy) = GeneratePuzzle(puzzle, pieces);
					if (++pieces_count_copy < pieces_count_min)
					{
						puzzle_min = (int[,])puzzle_copy.Clone();
						pieces_count_min = pieces_count_copy;
					}
				}

				int[,] puzzle_clone = (int[,])puzzle.Clone();

				Unfill(puzzle, piece.piece_id);
				pieces.Insert(piece_id, piece);

				if (is_solvable)
				{
					return (puzzle_clone, 1);
				}
				if (pieces_count_min <= 1)
				{
					return (puzzle_min, pieces_count_min);
				}
			}

			return (puzzle_min, pieces_count_min);
		}

		// Copies entries with a certain value from the board.
		private void Copy(int[,] puzzle, int value)
		{
			for (int i = 0; i < 50; i++)
			{
				if (board[i / 5, i % 5] == value)
				{
					puzzle[i / 5, i % 5] = value;
				}
			}
		}

		// Determines if a puzzle has a unique solution or not.
		private static bool Solvable(int[,] puzzle, int index, List<Piece> pieces)
		{
			int solution_count = 0;
			Solve(puzzle, index, pieces, ref solution_count);
			return solution_count == 1;
		}

		// Computes solutions to a puzzle by brute force.
		// It returns early if it finds out a solution is not unique.
		private static void Solve(int[,] puzzle, int index, List<Piece> pieces, ref int solution_count)
		{
			if (solution_count > 1)
			{
				return;
			}

			UpdateIndex(puzzle, ref index);
			if (index >= 50 && pieces.Count == 0)
			{
				solution_count++;
				return;
			}
			if (index >= 50 || pieces.Count == 0)
			{
				return;
			}

			for (int i = 0; i < pieces.Count; i++)
			{
				Piece piece = pieces[i];
				pieces.RemoveAt(i);

				bool early_break = false;
				for (int j = 0; j < 8; j++)
				{
					bool filled = Fill(puzzle, index, piece, j);
					if (filled)
					{
						UpdateIndex(puzzle, ref index);

						Solve(puzzle, index, pieces.ToList(), ref solution_count);

						index = Unfill(puzzle, piece.piece_id);
						if (solution_count > 1)
						{
							early_break = true;
							break;
						}
					}
				}

				pieces.Insert(i, piece);
				if (early_break)
				{
					return;
				}
			}
		}

		public void Display()
		{
			Utility.Display(puzzle);
		}
	}

	public struct Piece
	{
		public int piece_id;
		public List<int[,]> shapes;

		public Piece(int id, List<int[,]> shapes)
		{
			piece_id = id;
			this.shapes = new List<int[,]>(8);
			foreach (int[,] shape in shapes)
			{
				this.shapes.AddRange(Rotations(shape));
			}
		}

		// Adds the rotations of a certain shape.
		private List<int[,]> Rotations(int[,] shape)
		{
			List<int[,]> shapes = [shape];

			for (int i = 0; i < 3; ++i)
			{
				shape = Rotate(shape);
				shapes.Add(shape);
			}

			return shapes;
		}

		// Computes a 90 degree rotation of a shape.
		private int[,] Rotate(int[,] shape)
		{
			int n = shape.GetLength(0);
			int m = shape.GetLength(1);
			int[,] rotation = new int[m, n];

			for (int i = 0; i < m; i++)
			{
				for (int j = 0; j < n; j++)
				{
					rotation[i, j] = shape[n - 1 - j, i];
				}
			}

			return rotation;
		}
	}

	public static class Utility
	{
		// Generate a randomised sequence of the integers 0, 1, ..., n - 1,
		// using the Fisher-Yates Shuffling Algorithm.
		public static int[] GenerateOrdering(Random rand, int n)
		{
			int[] ordering = new int[n];
			for (int i = 0; i < n; i++)
			{
				ordering[i] = i;
			}
			for (int i = 0; i < n - 1; i++)
			{
				int j = rand.Next(i, n);
				(ordering[i], ordering[j]) = (ordering[j], ordering[i]);
			}
			return ordering;
		}

		// Updates the index to the first unfilled index of the board.
		public static void UpdateIndex(int[,] board, ref int index)
		{
			while (index < 50 && board[index / 5, index % 5] != 0)
			{
				index++;
			}
		}

		// Attempts to fill the board with a certain piece, so that the uppermost left part of the piece is on index.
		// Returns a boolean according to whether the action was performed succesfully or not.
		public static bool Fill(int[,] board, int index, Piece piece, int shape_id)
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
					if (shape[i, j] != 0 && board[index0 + i, index1 + j] != 0)
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
						board[index0 + i, index1 + j] = piece.piece_id;
					}
				}
			}

			return true;
		}

		// Removes all occurrences of a value in an array and returns the minimal index updated.
		public static int Unfill(int[,] arr, int value)
		{
			int? min_index = null;
			for (int i = 49; i >= 0; i--)
			{
				if (arr[i / 5, i % 5] == value)
				{
					arr[i / 5, i % 5] = 0;
					min_index = i;
				}
			}
			if (min_index == null)
			{
				return -1;
			}
			return min_index.Value;
		}

		// Displays a 2D array in the console.
		public static void Display(int[,] arr)
		{
			string t = "";
			for (int i = 0; i < arr.GetLength(0); i++)
			{
				t += '-';
			}
			Console.WriteLine($"+{t}+");
			for (int j = 0; j < arr.GetLength(1); j++)
			{
				string s = "";
				for (int i = 0; i < arr.GetLength(0); i++)
				{
					s += arr[i, j] == 0 ? " " : arr[i, j] - 1;
				}
				Console.WriteLine($"|{s}|");
			}
			Console.WriteLine($"+{t}+");
			Console.WriteLine();
		}

		// List of the different pieces available.
		public static ImmutableList<Piece> pieces = [
			// Red
			new Piece(1, [
				new int[2, 4] { { 1, 1, 1, 1 }, { 1, 0, 0, 1 } },
				new int[2, 4] { { 1, 1, 1, 1 }, { 0, 0, 0, 1 } }
				]),
			// Orange
			new Piece(2, [
				new int[2, 4] { { 1, 1, 1, 1 }, { 0, 1, 0, 1 } },
				new int[2, 4] { { 1, 1, 1, 1 }, { 0, 0, 1, 0 } }
				]),
			// Yellow
			new Piece(3, [
				new int[2, 4] { { 1, 1, 1, 1 }, { 1, 1, 0, 0 } },
				new int[2, 4] { { 1, 1, 1, 1 }, { 1, 0, 0, 0 } }
				]),
			// Light Green
			new Piece(4, [
				new int[2, 3] { { 1, 1, 1 }, { 1, 0, 1 } },
				new int[2, 3] { { 1, 1, 1 }, { 0, 0, 1 } }
				]),
			// Dark Green
			new Piece(5, [
				new int[2, 3] { { 1, 1, 1 }, { 0, 1, 1 } },
				new int[2, 3] { { 1, 1, 1 }, { 0, 1, 0 } }
				]),
			// Light Blue
			new Piece(6, [
				new int[2, 4] { { 1, 1, 1, 1 }, { 0, 1, 1, 0 } },
				new int[2, 4] { { 1, 1, 1, 1 }, { 0, 0, 1, 0 } }
				]),
			// Blue
			new Piece(7, [
				new int[2, 4] { { 1, 1, 1, 1 }, { 1, 0, 1, 0 } },
				new int[2, 4] { { 1, 1, 1, 1 }, { 0, 0, 0, 1 } }
				]),
			// Dark Blue
			new Piece(8, [
				new int[2, 3] { { 1, 1, 1 }, { 1, 0, 1 } },
				new int[2, 3] { { 1, 1, 1 }, { 0, 1, 0 } }
				]),
			// Purple
			new Piece(9, [
				new int[2, 3] { { 1, 1, 1 }, { 1, 1, 0 } },
				new int[2, 3] { { 1, 1, 1 }, { 1, 0, 0 } }]),
			// Pink
			new Piece(10, [
				new int[2, 4] { { 1, 1, 1, 1 }, { 0, 0, 1, 1 } },
				new int[2, 4] { { 1, 1, 1, 1 }, { 0, 1, 0, 0 } }
				])
			];
	}
}