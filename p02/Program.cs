using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameFramework;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace p02
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            new MainClass();
        }
    }

    internal class MainClass : OpenTKFramework.MainClass
    {
        private readonly Color bg = Color.FromArgb(1, 82, 82, 82);

        private const int GRID_SIZE_I       = 200;
        private const int GRID_SIZE_I_SQUAR = GRID_SIZE_I * GRID_SIZE_I;

        public Cell[,] Grid = new Cell[GRID_SIZE_I, GRID_SIZE_I];

        public MainClass()
        {
            //this.Grid[GRID_SIZE_I / 2, GRID_SIZE_I / 2] = new Cell { Position = new Point(100, 100), Color = Color.Blue };

            Create(new Size(1000, 1000));
            this.Window.Location = new Point(300, 100);

            this.Window.VSync = VSyncMode.Off;

            this.Window.Resize += (sender, args) => {
                SizeI                  = (int) (1f      * this.Window.ClientSize.Width / GRID_SIZE_I);
                this.Window.ClientSize = new Size(SizeI * GRID_SIZE_I, SizeI * GRID_SIZE_I);
            };

            Color ne   = RngColor();
            bool  move = false;
            this.Window.MouseDown += (sender, args) => {
                ne   = RngColor();
                move = true;
            };

            this.Window.MouseUp += (object sender, MouseButtonEventArgs args) => {
                move = false;
            };

            this.Window.MouseMove += delegate(object sender, MouseMoveEventArgs args) {
                if (!move) return;

                var pos = MkIndex(args.Position);

                MoveCell(new Cell { Color = ne }, pos.X, pos.Y);
            };

            Run();
        }

        #region Overrides of MainClass

        /// <inheritdoc />
        public override void Initialize(object sender, EventArgs e) { }

        /// <inheritdoc />
        public override void Shutdown(object sender, EventArgs e) { }

        public override void Render(object sender, FrameEventArgs e)
        {
            this.I.ClearScreen(this.bg);

            for (int i = 0; i < GRID_SIZE_I; i++)
            {
                this.I.DrawLine(i    * SizeI, 0, i        * SizeI, GRID_SIZE_I * SizeI, Color.Black);
                this.I.DrawLine(0, i * SizeI, GRID_SIZE_I * SizeI, i           * SizeI, Color.Black);
            }

            var ca = 0;
            for (var i0 = 0; i0 < this.Grid.GetLength(0); i0++)
            {
                for (var i1 = 0; i1 < this.Grid.GetLength(1); i1++)
                {
                    var drawable = this.Grid[i0, i1];
                    if (drawable == null)
                        continue;

                    if (drawable.Dieing == 0)
                        ca++;

                    drawable.Draw(this.I, i0 * SizeI, i1 * SizeI);
                }
            }

            this.CellsAlive = ca;

            this.I.DrawString("FPS: " + this.frameRate, PointF.Empty, Color.Beige);
        }

        public const  float MAX_DIEING = 5;
        public        int   CellsAlive = 0;
        public static int   SizeI      = 20;
        private const float SPEED      = .9f;

        public override void Update(object sender, FrameEventArgs e)
        {
            for (int i = 0; i < GRID_SIZE_I; i++)
            {
                for (int j = 0; j < GRID_SIZE_I; j++)
                {
                    if (!(this.Grid[i, j] is Cell cell))
                    {
                        continue;
                    }

                    if (cell.Dieing > 0)
                    {
                        if (this.CellsAlive > GRID_SIZE_I_SQUAR / 10)
                        {
                            if (R.NextDouble() > 0.84 * SPEED)
                            {
                                cell.Dieing += 1;
                            }
                        }

                        else if (R.NextDouble() > 0.92 * SPEED)
                        {
                            cell.Dieing += 1;
                        }

                        if (cell.Dieing > MAX_DIEING)
                            this.Grid[i, j] = null;
                    }
                    else
                    {
                        if (R.NextDouble() > 0.5 * SPEED) //change color//move and mutate
                        {
                            cell.Color = GenVar(cell.Color);
                        }
                        else if (R.NextDouble() > 0.999 * SPEED) //die
                        {
                            cell.Dieing = 1;
                        }

                        if (R.NextDouble() > 0.98 * SPEED) //reproduce
                        {
                            MoveCell(new Cell { Color = cell.Color }, i, j);
                        }
                        if (R.NextDouble() > 0.8 * SPEED)  //move and mutate
                        {
                            if (R.NextDouble() > 0.8 * SPEED) 
                                this.Grid[i, j] = new Cell { Color =  GenVar(cell.Color), Dieing = 1 };
                            else
                                this.Grid[i, j] = null;

                            MoveCell(cell, i, j);
                        }

                        if (this.CellsAlive > GRID_SIZE_I_SQUAR / 10)
                        {
                            if (R.NextDouble() > 0.97 * SPEED)
                            {
                                cell.Dieing = 1;
                            }
                        }

                        else if (this.CellsAlive > GRID_SIZE_I_SQUAR / 20)
                        {
                            if (R.NextDouble() > 0.995 * SPEED)
                            {
                                cell.Dieing = 1;
                            }
                        }

                        else if (this.CellsAlive < GRID_SIZE_I_SQUAR / 40)
                        {
                            if (R.NextDouble() > 0.99 * SPEED)
                            {
                                MoveCell(new Cell { Color = GenVar(cell.Color) }, i, j);
                            }
                        }
                    }
                }
            }
        }

        private Color GenVar(Color cellColor)
        {
            var cmin = 100;

            if (R.NextDouble() < .95) return cellColor;

            int   min = -10;
            int   max = 20;
            float r   = cellColor.R;
            float g   = cellColor.G;
            float b   = cellColor.B;
            if (R.NextDouble() > .95)
            {
                min  =  -10;
                max  =  50;
                cmin =  150;
                g    /= 3f;
                r    /= 3f;
                b    /= 3f;
            }
            else
            {
                min  = -1;
                max  = 10;
                cmin = 40;
            }

            var j = R.NextDouble();

            if (j < .333)
            {
                r = (byte) Math.Max(cmin, Math.Min(cellColor.R + R.Next(min, max), 255));
            }
            else if (j < .666)
            {
                g = (byte) Math.Max(cmin, Math.Min(cellColor.G + R.Next(min, max), 255));
            }
            else
            {
                b = (byte) Math.Max(cmin, Math.Min(cellColor.B + R.Next(min, max), 255));
            }

            return Color.FromArgb(255, (int) r, (int) g, (int) b);
        }

        public void MoveCell(Cell c, int i, int j)
        {
            while (true)
            {
                var dir = R.NextDouble();

                if (dir < .25)
                {
                    i += 1;
                }
                else if (dir < .5)
                {
                    i -= 1;
                }
                else if (dir < .75)
                {
                    j += 1;
                }
                else
                {
                    j -= 1;
                }

                Mkij(ref i);
                Mkij(ref j);

                var nm = this.Grid[i, j];
                this.Grid[i, j] = c;

                if (nm != null)
                {
                    if (c.Dieing == 0 && nm.Dieing == 0)
                        MixColor(c, nm);

                    c = nm;
                    continue;
                }

                break;
            }
        }

        /// <summary>Blends the specified colors together.</summary>
        /// <param name="color">Color to blend onto the background color.</param>
        /// <param name="backColor">Color to blend the other color onto.</param>
        /// <param name="amount">How much of <paramref name="color"/> to keep,
        /// “on top of” <paramref name="backColor"/>.</param>
        /// <returns>The blended colors.</returns>
        public static Color Blend(Color color, Color backColor, double amount)
        {
            byte r = (byte) ((color.R * amount) + backColor.R * (1 - amount));
            byte g = (byte) ((color.G * amount) + backColor.G * (1 - amount));
            byte b = (byte) ((color.B * amount) + backColor.B * (1 - amount));
            return Color.FromArgb(r, g, b);
        }

        private static void MixColor(Cell cell1, Cell cell2)
        {
            cell1.Color = Blend(cell1.Color, cell2.Color, .2);
            cell2.Color = Blend(cell2.Color, cell1.Color, .2);
        }

        void Mkij(ref int i)
        {
            while (i < 0)
            {
                i += GRID_SIZE_I;
            }

            while (i >= GRID_SIZE_I)
            {
                i -= GRID_SIZE_I;
            }
        }

        private Color RngColor()
        {
            return Color.FromArgb(R.Next(0, int.MaxValue));
        }

        public static readonly Random R = new Random();

        public Point MkPos(PointF pos)
        {
            return new Point(((int) (pos.X / SizeI) * SizeI), ((int) (pos.Y / SizeI) * SizeI));
        }

        public Point MkIndex(PointF pos)
        {
            return new Point(((int) (pos.X / SizeI)), ((int) (pos.Y / SizeI)));
        }

        #endregion

        internal class Cell
        {
            #region Implementation of IDrawable

            public void Draw(GraphicsManager g, int x, int y)
            {
                g.DrawRect(x, y, x + SizeI, y + SizeI, this.Color);
            }

            public Color Color { get; set; }

            private int dieing = 0;

            public int Dieing {
                get => this.dieing;
                set {
                    this.dieing = value;
                    if (value > 1)
                        this.Color = Blend(this.Color, Color.Black, 1.0 * this.dieing / MAX_DIEING);
                }
            }

            #endregion
        }

        internal class Cycle : IDrawable
        {
            public Cycle(float radius)
            {
                this.Radius = radius;
            }

            #region Implementation of IDrawable

            /// <inheritdoc />
            public void Draw(GraphicsManager g)
            {
                g.DrawCycle(this.Position, this.Radius, this.Color, (int) this.Radius);
            }

            /// <inheritdoc />
            public Point Position { get; set; }

            /// <inheritdoc />
            public Color Color { get; set; }

            public float Radius { get; set; } = 20;

            /// <inheritdoc />
            public bool Inside(Point pos)
            {
                return IsInside(this.Position.X, this.Position.Y, this.Radius, pos.X, pos.Y);
            }

            private static bool IsInside(float circleX, float circleY,
                                         float rad,     float x, float y)
            {
                return (x - circleX) * (x - circleX) +
                       (y - circleY) * (y - circleY) <=
                       rad * rad;
            }

            #endregion
        }
    }

    interface IDrawable
    {
        void Draw(GraphicsManager g);

        Point Position { get; set; }
        Color Color    { get; set; }

        bool Inside(Point pos);
    }
}
