//               Copyright Joël Ganesh 2024.
// Distributed under the Boost Software License, Version 1.0.
//    (See accompanying file LICENSE_1_0.txt or copy at
//          https://www.boost.org/LICENSE_1_0.txt)

using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Win32;

using IQFit.Logic;

namespace IQFit
{
    public partial class MainWindow : Window
    {
        private Canvas[,] canvas = new Canvas[10, 5];
        private Game? game;
        private int?[,] colorGrid = new int?[10, 5];

        private double width = 500;
        private double height = 250;

        public MainWindow()
        {
            game = null;
            InitializeComponent();
            SetupGrid();
        }

        private void UpdateOnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            width = e.NewSize.Width;
            height = e.NewSize.Height;

            SetupGrid();
        }

        private void SetupGrid()
        {
            grid.Children.Clear();

            Dictionary<Point, List<Point>> connections = Connections();
            for (int j = 0; j < 5; j++)
            {
                for (int i = 0; i < 10; i++)
                {
                    Canvas c = new Canvas()
                    {
                        //Background = Brushes.WhiteSmoke,
                        Width = width / 10,
                        Height = height / 5,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    foreach (Point p in connections[new Point(i, j)])
                    {
                        double x = p.X - i;
                        double y = p.Y - j;

                        double x_center = c.Width / 2;
                        double y_center = c.Height / 2;

                        Line l_outer = new Line()
                        {
                            X1 = x_center,
                            X2 = x_center * (1 + x),
                            Y1 = y_center,
                            Y2 = y_center * (1 + y),
                            Stroke = Brushes.Black,
                            StrokeThickness = 3 + Math.Min(c.Width, c.Height) / 3,
                        };
                        c.Children.Add(l_outer);

                        Line l_inner = new Line()
                        {
                            X1 = x_center,
                            X2 = x_center * (1 + x),
                            Y1 = y_center,
                            Y2 = y_center * (1 + y),
                            Stroke = ColorIDToBrush(colorGrid[i, j]),
                            StrokeThickness = Math.Min(c.Width, c.Height) / 3
                        };
                        c.Children.Add(l_inner);
                    }

                    Ellipse e = new Ellipse()
                    {
                        Width = width / 12,
                        Height = height / 6,
                        Stroke = Brushes.Black,
                        Fill = ColorIDToBrush(colorGrid[i, j]),
                        StrokeThickness = 2,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    Canvas.SetLeft(e, width / 120);
                    Canvas.SetTop(e, height / 60);
                    c.Children.Add(e);

                    canvas[i, j] = c;
                    grid.Children.Add(c);
                }
            }
        }

        private Dictionary<Point, List<Point>> Connections()
        {
            Dictionary<Point, List<Point>> connections = [];
            for (int j = 0; j < 5; j++)
            {
                for (int i = 0; i < 10; i++)
                {
                    List<Point> neighbors = Neighbors(i, j);
                    Point p = new Point(i, j);
                    connections.Add(p, []);
                    foreach (Point q in neighbors)
                    {
                        connections[p].Add(q);
                    }
                }
            }
            return connections;
        }

        // Checks if the neighbors of p are symmetric
        private bool Symmetric(Point p, List<Point> neighbors)
        {
            foreach (Point q in neighbors)
            {
                Point r = new Point(p.X - (q.X - p.X), p.Y - (q.Y - p.Y));
                if (!neighbors.Contains(r))
                {
                    return false;
                }
            }
            return true;
        }

        // Returns the list of neighboring indices of (i, j) of the same color.
        private List<Point> Neighbors(int i, int j)
        {
            List<Point> neighbors = [];
            if (colorGrid[i, j] != null)
            {
                if (i < 9 && colorGrid[i, j] == colorGrid[i + 1, j])
                {
                    neighbors.Add(new Point(i + 1, j));
                }
                if (i > 0 && colorGrid[i, j] == colorGrid[i - 1, j])
                {
                    neighbors.Add(new Point(i - 1, j));
                }
                if (j < 4 && colorGrid[i, j] == colorGrid[i, j + 1])
                {
                    neighbors.Add(new Point(i, j + 1));
                }
                if (j > 0 && colorGrid[i, j] == colorGrid[i, j - 1])
                {
                    neighbors.Add(new Point(i, j - 1));
                }
            }
            return neighbors;
        }

        private Brush ColorIDToBrush(int? color)
        {
            return color switch
            {
                0 => Brushes.Crimson,
                1 => Brushes.DarkOrange,
                2 => Brushes.Gold,
                3 => Brushes.GreenYellow,
                4 => Brushes.Green,
                5 => Brushes.Cyan,
                6 => Brushes.DodgerBlue,
                7 => Brushes.Blue,
                8 => Brushes.DarkViolet,
                9 => Brushes.HotPink,
                _ => Brushes.Gainsboro,
            };
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            game = new Game();
            colorGrid = game.Puzzle();
            SetupGrid();
        }

        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            if (game == null)
            {
                return;
            }

            colorGrid = game.Puzzle();
            SetupGrid();
        }

        private void Hint_Click(object sender, RoutedEventArgs e)
        {
            if (game == null)
            {
                return;
            }

            game.Hint();
            SetupGrid();
        }

        private void Solution_Click(object sender, RoutedEventArgs e)
        {
            if (game == null)
            {
                return;
            }
            colorGrid = game.Solution();
            SetupGrid();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            colorGrid = new int?[10, 5];
            OpenFileDialog openFileDialog = new OpenFileDialog();
            {
                openFileDialog.Filter = "txt files (*.txt)|*.txt";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == true)
                {
                    Stream fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        string grid = reader.ReadToEnd();
                        for (int i = 0; i < Math.Min(grid.Length, 50); i++)
                        {
                            char c = grid[i];
                            if (c >= '0' && c <= '9')
                            {
                                colorGrid[i % 10, i / 10] = c - '0';
                            }
                        }
                    }
                }
            }
            game = new Game(colorGrid);
            colorGrid = game.Puzzle();
            SetupGrid();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string contents = "";
            for (int i = 0; i < 50; i++)
            {
                int? color = colorGrid[i % 10, i / 10];
                contents += color == null ? " " : color.Value;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            {
                saveFileDialog.Filter = "txt files (*.txt)|*.txt";
                saveFileDialog.RestoreDirectory = true;

                if (saveFileDialog.ShowDialog() == true)
                {
                    Stream fileStream = saveFileDialog.OpenFile();

                    using (StreamWriter writer = new StreamWriter(fileStream))
                    {
                        writer.Write(contents);
                    }
                }
            }
        }
    }
}