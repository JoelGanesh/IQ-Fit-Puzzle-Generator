namespace IQFit.Logic
{
    using static Utility;

    public class Board : Grid
    {
        private readonly Random rand;
        private readonly int index;

        public Board(Random rand, string field = "") : base()
        {
            this.rand = rand;
            index = 0;
            List<Piece> pieces = Piece.pieces.ToList();

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
