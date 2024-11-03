//               Copyright Joël Ganesh 2024.
// Distributed under the Boost Software License, Version 1.0.
//    (See accompanying file LICENSE_1_0.txt or copy at
//          https://www.boost.org/LICENSE_1_0.txt)

using System.Collections.Immutable;

namespace IQFit.Logic
{
    // A Piece represents a puzzle piece
    // to be placed on the board
    public class Piece
    {
        public int piece_id;
        public List<int[,]> shapes;

        // Generates all possible configurations of a piece given the specified shapes it has.
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

        // List of the different pieces available.
        public static readonly ImmutableList<Piece> pieces = [
			// Red
			new Piece(0, [
                new int[2, 4] { { 1, 1, 1, 1 }, { 1, 0, 0, 1 } },
                new int[2, 4] { { 1, 1, 1, 1 }, { 0, 0, 0, 1 } }
                ]),
			// Orange
			new Piece(1, [
                new int[2, 4] { { 1, 1, 1, 1 }, { 0, 1, 0, 1 } },
                new int[2, 4] { { 1, 1, 1, 1 }, { 0, 0, 1, 0 } }
                ]),
			// Yellow
			new Piece(2, [
                new int[2, 4] { { 1, 1, 1, 1 }, { 1, 1, 0, 0 } },
                new int[2, 4] { { 1, 1, 1, 1 }, { 1, 0, 0, 0 } }
                ]),
			// Light Green
			new Piece(3, [
                new int[2, 3] { { 1, 1, 1 }, { 1, 0, 1 } },
                new int[2, 3] { { 1, 1, 1 }, { 0, 0, 1 } }
                ]),
			// Dark Green
			new Piece(4, [
                new int[2, 3] { { 1, 1, 1 }, { 0, 1, 1 } },
                new int[2, 3] { { 1, 1, 1 }, { 0, 1, 0 } }
                ]),
			// Light Blue
			new Piece(5, [
                new int[2, 4] { { 1, 1, 1, 1 }, { 0, 1, 1, 0 } },
                new int[2, 4] { { 1, 1, 1, 1 }, { 0, 0, 1, 0 } }
                ]),
			// Blue
			new Piece(6, [
                new int[2, 4] { { 1, 1, 1, 1 }, { 1, 0, 1, 0 } },
                new int[2, 4] { { 1, 1, 1, 1 }, { 0, 0, 0, 1 } }
                ]),
			// Dark Blue
			new Piece(7, [
                new int[2, 3] { { 1, 1, 1 }, { 1, 0, 1 } },
                new int[2, 3] { { 1, 1, 1 }, { 0, 1, 0 } }
                ]),
			// Purple
			new Piece(8, [
                new int[2, 3] { { 1, 1, 1 }, { 1, 1, 0 } },
                new int[2, 3] { { 1, 1, 1 }, { 1, 0, 0 } }]),
			// Pink
			new Piece(9, [
                new int[2, 4] { { 1, 1, 1, 1 }, { 0, 0, 1, 1 } },
                new int[2, 4] { { 1, 1, 1, 1 }, { 0, 1, 0, 0 } }
                ])
            ];
    }
}
