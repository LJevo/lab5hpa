using System;
using System.Collections.Generic;
using System.Drawing;

namespace WinFormsApp1
{
    public class Cell
    {
        public bool HasMine { get; set; }
        public bool Revealed { get; set; }
        public bool Flagged { get; set; }
        public int AdjacentMines { get; set; } = 0;
    }

    public class Board
    {
        public int Rows { get; }
        public int Cols { get; }
        public int MineCount { get; }

        public Cell[,] Grid { get; }
        private readonly Random _rng = new Random();

        public Board(int rows, int cols, int mineCount)
        {
            if (rows <= 0 || cols <= 0) throw new ArgumentException("Dimensiones inválidas.");
            if (mineCount < 0 || mineCount >= rows * cols) throw new ArgumentException("Cantidad de minas inválida.");

            Rows = rows;
            Cols = cols;
            MineCount = mineCount;

            Grid = new Cell[Rows, Cols];
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    Grid[r, c] = new Cell();
        }

        /// Coloca las minas aleatoriamente evitando la celda del primer clic.
        /// Debe llamarse justo después del primer clic del jugador.
      
        public void PlaceMinesAvoidingFirstClick(Point firstClick)
        {
            // 1) Genera todas las posiciones excepto la del primer clic
            var positions = new List<Point>(Rows * Cols);
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    if (!(r == firstClick.Y && c == firstClick.X))
                        positions.Add(new Point(c, r));

            // 2) Baraja con Fisher–Yates
            for (int i = positions.Count - 1; i > 0; i--)
            {
                int j = _rng.Next(i + 1);
                (positions[i], positions[j]) = (positions[j], positions[i]);
            }

            // 3) Marca minas en las primeras MineCount posiciones
            for (int i = 0; i < MineCount && i < positions.Count; i++)
            {
                var p = positions[i];
                Grid[p.Y, p.X].HasMine = true;
            }

            // 4) Calcula números adyacentes
            ComputeAdjacentNumbers();
        }

        /// <summary>
        /// Calcula el número de minas alrededor de cada celda.
        
        private void ComputeAdjacentNumbers()
        {
            int[] dr = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] dc = { -1, 0, 1, -1, 1, -1, 0, 1 };

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Grid[r, c].HasMine)
                    {
                        Grid[r, c].AdjacentMines = -1; // marca especial
                        continue;
                    }

                    int count = 0;
                    for (int k = 0; k < 8; k++)
                    {
                        int nr = r + dr[k];
                        int nc = c + dc[k];
                        if (nr >= 0 && nr < Rows && nc >= 0 && nc < Cols && Grid[nr, nc].HasMine)
                            count++;
                    }
                    Grid[r, c].AdjacentMines = count;
                }
            }
        }

        
        /// Revela una celda. Si es 0, hace BFS para abrir regiones vacías.
        /// Devuelve la lista de posiciones que quedaron abiertas en esta acción.
        /// </summary>
        public List<Point> Reveal(Point p)
        {
            var opened = new List<Point>();
            if (!InBounds(p)) return opened;

            var start = Grid[p.Y, p.X];
            if (start.Revealed || start.Flagged) return opened;

            var q = new Queue<Point>();
            q.Enqueue(p);

            while (q.Count > 0)
            {
                var cur = q.Dequeue();
                if (!InBounds(cur)) continue;

                var cell = Grid[cur.Y, cur.X];
                if (cell.Revealed || cell.Flagged) continue;

                cell.Revealed = true;
                opened.Add(cur);

                // Si no es mina y su número es 0, expandimos a vecinos
                if (!cell.HasMine && cell.AdjacentMines == 0)
                {
                    foreach (var nb in Neighbors(cur))
                    {
                        if (InBounds(nb))
                        {
                            var ncell = Grid[nb.Y, nb.X];
                            if (!ncell.Revealed && !ncell.HasMine)
                                q.Enqueue(nb);
                        }
                    }
                }
            }

            return opened;
        }

        public bool InBounds(Point p) => p.Y >= 0 && p.Y < Rows && p.X >= 0 && p.X < Cols;

        private IEnumerable<Point> Neighbors(Point p)
        {
            for (int r = p.Y - 1; r <= p.Y + 1; r++)
                for (int c = p.X - 1; c <= p.X + 1; c++)
                    if (!(r == p.Y && c == p.X))
                        yield return new Point(c, r);
        }
    }
}
