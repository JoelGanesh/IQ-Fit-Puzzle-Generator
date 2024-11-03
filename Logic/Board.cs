//               Copyright Joël Ganesh 2024.
// Distributed under the Boost Software License, Version 1.0.
//    (See accompanying file LICENSE_1_0.txt or copy at
//          https://www.boost.org/LICENSE_1_0.txt)

namespace IQFit.Logic
{
    using static Utility;

    public class Board : Grid
    {
        private readonly Random rand;
        private readonly int index;

        public Board(Random rand, int?[,]? grid = null) : base()
        {
            this.rand = rand;
            index = 0;
            List<Piece> pieces = Piece.pieces.ToList();

            // The field parameter can be used to fill a board,
            // for instance to solve a puzzle.
            HashSet<int> set = new HashSet<int>();
            if (grid != null)
            {
                this.grid = (int?[,])grid.Clone();
                for (int i = 0; i < 50; i++)
                {
                    int? piece_id = grid[i % 10, i / 10];
                    if (piece_id != null)
                    {
                        set.Add(piece_id.Value);
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
            UpdateIndex(ref index);
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

                    bool filled = Fill(index, piece, shape_id);
                    if (filled)
                    {
                        UpdateIndex(ref index);

                        bool success = FillBoard(index, pieces);
                        if (success)
                        {
                            return true;
                        }
                        index = Unfill(piece.piece_id);
                    }
                }
                pieces.Insert(piece_id, piece);
            }
            return false;
        }
    }
}
