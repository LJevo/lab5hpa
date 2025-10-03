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
        private int _cellSize = 50;


        private Panel _headerPanel = null!;
        private Label _lblMines = null!;
        private Label _lblTime = null!;
        private Label _lblScore = null!;
        private Button _btnReset = null!;
        private System.Windows.Forms.Timer _timer = null!;
        private int _elapsedSeconds = 0;
        private int _flagsPlaced = 0;
        private int _score = 0; // celdas seguras reveladas
        public Form1()
        {
            InitializeComponent();
            BuildHeaderUI();                 // crea HUD y _timer
            NewGame(_rows, _cols, _mines, _cellSize); // crea tablero + resetea HUD
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        private void NewGame(int rows, int cols, int mines, int cellSize = 28)
        {
            if (rows <= 0 || cols <= 0) throw new ArgumentException("Dimensiones inválidas.");
            if (mines < 0 || mines >= rows * cols) throw new ArgumentException("Cantidad de minas inválida.");

            _rows = rows; _cols = cols; _mines = mines; _cellSize = cellSize;

            _timer?.Stop();                // <-- por si se llama antes de BuildHeaderUI
            _elapsedSeconds = 0;
            _flagsPlaced = 0;
            _score = 0;
            _minesPlaced = false;

            CreateBoardUI();
            UpdateHud();
        }


        private void BuildHeaderUI()
        {
            _headerPanel = new Panel
            {
                Location = new Point(12, 12),
                Size = new Size(600, 44),
                BackColor = Color.FromArgb(235, 235, 235)
            };

            _lblMines = new Label
            {
                AutoSize = true,
                Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold),
                Location = new Point(8, 12),
                Text = "Minas: 0"
            };

            _btnReset = new Button
            {
                Size = new Size(40, 28),
                Location = new Point(140, 8),
                Text = "",                
                FlatStyle = FlatStyle.Flat
            };
            _btnReset.FlatAppearance.BorderSize = 0;

            _btnReset.BackgroundImage = Properties.Resources.reset1;
            _btnReset.BackgroundImageLayout = ImageLayout.Zoom;

            _btnReset.Click += (s, e) => NewGame(_rows, _cols, _mines, _cellSize);

            _lblScore = new Label
            {
                AutoSize = true,
                Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold),
                Location = new Point(200, 12),
                Text = "Score: 0"
            };

            _lblTime = new Label
            {
                AutoSize = true,
                Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold),
                Location = new Point(360, 12),
                Text = "Tiempo: 00:00"
            };

            _headerPanel.Controls.Add(_lblMines);
            _headerPanel.Controls.Add(_btnReset);
            _headerPanel.Controls.Add(_lblScore);
            _headerPanel.Controls.Add(_lblTime);
            this.Controls.Add(_headerPanel);

            // Mueve el tablero debajo del header
            panelBoard.Top = _headerPanel.Bottom + 8;

            // Timer del reloj
            _timer = new System.Windows.Forms.Timer { Interval = 1000 };
            _timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _elapsedSeconds++;
            UpdateHud();
        }

        private void UpdateHud()
        {
            int remaining = Math.Max(0, _mines - _flagsPlaced);
            _lblMines.Text = $"Minas: {remaining}";
            var ts = TimeSpan.FromSeconds(_elapsedSeconds);
            _lblTime.Text = $"Tiempo: {ts.Minutes:00}:{ts.Seconds:00}";
            _lblScore.Text = $"Score: {_score}";
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
                        FlatStyle = FlatStyle.Flat,   // evita bordes 3D
                        BackgroundImage = Properties.Resources.tile2,
                        UseVisualStyleBackColor = false,// tu imagen de fondo
                        BackgroundImageLayout = ImageLayout.Stretch, // ajusta al tamaño
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
                _timer.Start();               // <-- ¡arranca el reloj aquí!
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

            if (cell.Flagged)
            {
                btn.Text = "";
                btn.Image = Properties.Resources.flag;
                btn.ImageAlign = ContentAlignment.MiddleCenter;
                btn.BackgroundImageLayout = ImageLayout.Stretch;
                _flagsPlaced++;                 // <-- cuenta banderas
            }
            else
            {
                btn.Image = null;
                btn.Text = "";
                _flagsPlaced = Math.Max(0, _flagsPlaced - 1); // <-- evita negativos
            }

            UpdateHud();                        // <-- refresca “Minas:”
        }

        private void HandleReveal(Point p)
        {
            var cell = _board.Grid[p.Y, p.X];
            if (cell.Flagged || cell.Revealed) return;

            if (cell.HasMine)
            {
                _timer.Stop();                           // <-- para reloj al perder
                RevealAllMines();
                _buttons[p.Y, p.X].BackColor = Color.IndianRed;
                MessageBox.Show("¡BOOM! Fin del juego.");
                return;
            }

            var opened = _board.Reveal(p);
            foreach (var q in opened)
                PaintCell(q);

            _score += opened.Count;                      // <-- suma puntaje
            UpdateHud();                                 // <-- refresca HUD

            if (CheckWin())
            {
                _timer.Stop();                           // <-- para reloj al ganar
                MessageBox.Show("¡Ganaste!");
            }
        }


        private void PaintCell(Point p)
        {
            var cell = _board.Grid[p.Y, p.X];
            var btn = _buttons[p.Y, p.X];

            btn.Enabled = false;

            if (cell.AdjacentMines == 0)
            {
                // Celda vacía (flood fill)
                btn.BackgroundImage = Properties.Resources.blank_space2;
                btn.BackgroundImageLayout = ImageLayout.Stretch;
                btn.Text = "";
            }
            else
            {
                // Seleccionamos la imagen en base al número
                switch (cell.AdjacentMines)
                {
                    case 1:
                        btn.BackgroundImage = Properties.Resources.num1;
                        break;
                    case 2:
                        btn.BackgroundImage = Properties.Resources.num2;
                        break;
                    case 3:
                        btn.BackgroundImage = Properties.Resources.num3;
                        break;
                    case 4:
                        btn.BackgroundImage = Properties.Resources.num4;
                        break;
                    case 5:
                        btn.BackgroundImage = Properties.Resources.num5;
                        break;
                    case 6:
                        btn.BackgroundImage = Properties.Resources.num6;
                        break;
                    case 7:
                        btn.BackgroundImage = Properties.Resources.num7;
                        break;
                    case 8:
                        btn.BackgroundImage = Properties.Resources.num8;
                        break;
                }

                btn.BackgroundImageLayout = ImageLayout.Stretch;
                btn.Text = "";
            }
        }

        private void RevealAllMines()
        {
            for (int r = 0; r < _board.Rows; r++)
            {
                for (int c = 0; c < _board.Cols; c++)
                {
                    if (_board.Grid[r, c].HasMine)
                    {
                        var b = _buttons[r, c];
                        b.BackgroundImage = Properties.Resources.mine3;
                        b.BackgroundImageLayout = ImageLayout.Stretch;
                        b.Text = "";
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

        private void panelBoard_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
