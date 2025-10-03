using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        // Inicializamos con null! para evitar CS8618 (se asignan en CreateBoardUI).
        private Board _board = null!;
        private Button[,] _buttons = null!;
        private bool _minesPlaced = false;

        // Parámetros del tablero
        private int _rows = 10;
        private int _cols = 10;
        private int _mines = 15;
        private int _cellSize = 28;

        public Form1()
        {
            InitializeComponent();   // <- del Designer
            CreateBoardUI();         // <- creamos los botones y el Board
        }

        private void CreateBoardUI()
        {
            _board = new Board(_rows, _cols, _mines);
            _buttons = new Button[_rows, _cols];

            panelBoard.Controls.Clear();

            for (int r = 0; r < _rows; r++)
            {
                for (int c = 0; c < _cols; c++)
                {
                    var btn = new Button
                    {
                        Size = new Size(_cellSize, _cellSize),
                        Location = new Point(c * _cellSize, r * _cellSize),
                        Tag = new Point(c, r),
                        Margin = Padding.Empty,
                        Padding = Padding.Empty,
                        Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold),
                        BackColor = Color.Gainsboro
                    };

                    btn.MouseUp += Cell_MouseUp;

                    panelBoard.Controls.Add(btn);
                    _buttons[r, c] = btn;
                }
            }
        }

        // Firma con object? para evitar CS8622
        private void Cell_MouseUp(object? sender, MouseEventArgs e)
        {
            // Evita CS8605 (sender puede ser null)
            if (sender is not Button btn) return;

            // Evita CS8605 (Tag podría ser null o de otro tipo)
            if (btn.Tag is not Point p) return;

            if (!_minesPlaced && e.Button == MouseButtons.Left)
            {
                _board.PlaceMinesAvoidingFirstClick(p);
                _minesPlaced = true;
            }

            if (e.Button == MouseButtons.Right)
            {
                ToggleFlag(p);
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                HandleReveal(p);
            }
        }

        private void ToggleFlag(Point p)
        {
            var cell = _board.Grid[p.Y, p.X];
            if (cell.Revealed) return;

            cell.Flagged = !cell.Flagged;
            var btn = _buttons[p.Y, p.X];
            btn.Text = cell.Flagged ? "⚑" : "";
            btn.ForeColor = Color.Firebrick;
        }

        private void HandleReveal(Point p)
        {
            var cell = _board.Grid[p.Y, p.X];
            if (cell.Flagged || cell.Revealed) return;

            if (cell.HasMine)
            {
                RevealAllMines();
                _buttons[p.Y, p.X].BackColor = Color.IndianRed;
                MessageBox.Show("¡BOOM! Fin del juego.");
                return;
            }

            var opened = _board.Reveal(p);
            foreach (var q in opened)
                PaintCell(q);

            if (CheckWin())
                MessageBox.Show("¡Ganaste!");
        }

        private void PaintCell(Point p)
        {
            var cell = _board.Grid[p.Y, p.X];
            var btn = _buttons[p.Y, p.X];

            btn.Enabled = false;
            btn.BackColor = Color.WhiteSmoke;
            btn.Text = cell.AdjacentMines > 0 ? cell.AdjacentMines.ToString() : "";
            btn.ForeColor = NumberColor(cell.AdjacentMines);
        }

        private Color NumberColor(int n) => n switch
        {
            1 => Color.Blue,
            2 => Color.Green,
            3 => Color.Red,
            4 => Color.Navy,
            5 => Color.Maroon,
            6 => Color.Teal,
            7 => Color.Black,
            8 => Color.Gray,
            _ => Color.Black
        };

        private void RevealAllMines()
        {
            for (int r = 0; r < _board.Rows; r++)
            {
                for (int c = 0; c < _board.Cols; c++)
                {
                    if (_board.Grid[r, c].HasMine)
                    {
                        var b = _buttons[r, c];
                        b.Text = "💣";
                        b.BackColor = Color.Salmon;
                    }
                }
            }

            foreach (Control ctl in panelBoard.Controls)
                ctl.Enabled = false;
        }

        private bool CheckWin()
        {
            for (int r = 0; r < _board.Rows; r++)
                for (int c = 0; c < _board.Cols; c++)
                    if (!_board.Grid[r, c].HasMine && !_board.Grid[r, c].Revealed)
                        return false;

            return true;
        }
    }
}
