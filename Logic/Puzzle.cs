using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQFit.Logic
{
    public class Puzzle : Grid
    {
        private readonly int[,] solution;
        private readonly Random rand;

        public Puzzle(Random rand, Board board) : base()
        {
            this.rand = rand;
            solution = board.grid;

            GeneratePuzzle();
        }

        // Generates a puzzle which has unique solution (being the board),
        // such that it gives away as little pieces as necessary.
        // Returns the desired puzzle.
        private void GeneratePuzzle()
        {
            List<Piece> pieces = Piece.pieces.ToList();
            (grid, _) = GeneratePuzzle(pieces);
        }

        // Helper method of the generic GeneratePuzzle method.
        // It returns a puzzle with a unique solution paired with the number of pieces used.
        private (int[,], int) GeneratePuzzle(List<Piece> pieces)
        {
            int[] piece_ordering = Utility.GenerateOrdering(rand, pieces.Count);
            int pieces_count_min = int.MaxValue;
            int[,] puzzle_min = new int[10, 5];

            for (int i = 0; i < pieces.Count; i++)
            {
                int piece_id = piece_ordering[i];
                Piece piece = pieces[piece_id];
                pieces.RemoveAt(piece_id);

                Copy(piece.piece_id);

                bool is_solvable = Solvable(0, pieces);
                if (!is_solvable)
                {
                    int[,] puzzle_copy;
                    int pieces_count_copy;
                    (puzzle_copy, pieces_count_copy) = GeneratePuzzle(pieces);
                    if (++pieces_count_copy < pieces_count_min)
                    {
                        puzzle_min = (int[,])puzzle_copy.Clone();
                        pieces_count_min = pieces_count_copy;
                    }
                }

                int[,] puzzle_clone = (int[,])grid.Clone();

                Unfill(piece.piece_id);
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

        // Copies entries with a certain value from the solution.
        private void Copy(int value)
        {
            for (int i = 0; i < 50; i++)
            {
                if (solution[i / 5, i % 5] == value)
                {
                    grid[i / 5, i % 5] = value;
                }
            }
        }

        // Determines if a puzzle has a unique solution or not.
        private bool Solvable(int index, List<Piece> pieces)
        {
            int solution_count = 0;
            Solve(index, pieces, ref solution_count);
            return solution_count == 1;
        }

        // Computes solutions to a puzzle by brute force.
        // It returns early if it finds out a solution is not unique.
        private void Solve(int index, List<Piece> pieces, ref int solution_count)
        {
            if (solution_count > 1)
            {
                return;
            }

            UpdateIndex(ref index);
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
                    bool filled = Fill(index, piece, j);
                    if (filled)
                    {
                        UpdateIndex(ref index);

                        Solve(index, pieces.ToList(), ref solution_count);

                        index = Unfill(piece.piece_id);
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
    }
}
