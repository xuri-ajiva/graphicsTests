using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Input;

namespace _02 {
    static class Program {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );
            new MainClass();
        }
    }

    class MainClass : OpenTKFramework.MainClass {
        private const int COLORS_I     = 200;
        private const int COLOR_STEP_I = 240 / COLORS_I;
        private const int COLOR_HALF   = 255 / 2;

        private       float scale   = 1;
        private const int   maxSize = 3000;
        byte[,]             areal   = new byte[maxSize, maxSize];
        private int         w, h, w2, h2, ox, oy;


        #region Overrides of MainClass

        /// <inheritdoc />
        public override void Initialize(object sender, EventArgs e) { }

        /// <inheritdoc />
        public override void Shutdown(object sender, EventArgs e) { }

        /// <inheritdoc />
        public override void Render(object sender, FrameEventArgs e) {
            this.I.ClearScreen( Color.FromArgb( 1, 82, 82, 82 ) );
            SizeF s = new SizeF( this.scale, this.scale );
            this.Window.Title = "FPS: " + this.frameRate;

            for ( float i = 0; i < maxSize; i += 2 / this.scale ) {
                for ( float j = 0; j < maxSize; j += 2 / this.scale ) {
                    var x = i + this.ox;
                    var y = j + this.oy;

                    if ( this.scale * x < 0  || this.scale * y < 0 ) continue;
                    if ( this.scale * x >= w || this.scale * y >= h ) continue;

                    if ( x < 0        || y < 0 ) continue;
                    if ( x >= maxSize || y >= maxSize ) continue;

                    var v = this.areal[(int) i, (int) j];

                    if ( v == 0 ) continue;

                    var p = new PointF( this.scale * x, this.scale * y );

                    //if ( Math.Abs( ( (int) p.X - p.X ) ) < this.scale/2 || Math.Abs( ( (int) p.Y - p.Y ) ) < this.scale /2)
                    this.I.DrawPoint( p, colorFromValue( v ) );
                    //this.I.DrawRect( new RectangleF( p, new SizeF(1,1) ), colorFromValue( v ) );
                }
            }
        }


        private Color colorFromValue(byte b) {
            return Color.FromArgb( (int) ( ( Math.Sin( b * COLOR_STEP_I ) + 1 ) * COLOR_HALF ), (int) ( ( Math.Cos( b * COLOR_STEP_I ) + 1 ) * COLOR_HALF ), (int) ( ( -Math.Sin( b * COLOR_STEP_I ) + 1 ) * COLOR_HALF ) );

            //if ( b == 0 ) return Color.Black;
            //if ( b == 1 ) return Color.Red;
            //if ( b == 2 ) return Color.Aqua;
            //if ( b == 3 ) return Color.Coral;
            //if ( b == 4 ) return Color.DarkOrchid;
            //if ( b == 5 ) return Color.DeepPink;
            //if ( b == 6 ) return Color.GreenYellow;
            //if ( b == 7 ) return Color.Green;

            return Color.Blue;
        }

        PointF Map(float x, float y, float s, byte[,] a, int m, int h) {
            var p = new PointF();
            x *= this.scale;
            y *= this.scale;

            p.X = h + x;
            p.Y = h + y;

            return p;
        }

        /// <inheritdoc />
        public override void Update(object sender, FrameEventArgs e) {
            this.w  = this.Window.ClientSize.Width;
            this.h  = this.Window.ClientSize.Height;
            this.h2 = this.Window.ClientSize.Height / 2;
            this.w2 = this.Window.ClientSize.Width  / 2;
        }

        #endregion


        public MainClass() {
            for ( int i = 0; i < maxSize; i++ ) {
                for ( int j = 0; j < maxSize; j++ ) {
                    this.areal[i, j] = 0;
                }
            }

            Create( new Size( 800, 500 ) );

            //this.Window.WindowState = WindowState.Fullscreen;
            //this.Window.WindowBorder = WindowBorder.Hidden;
            //this.Window.Bounds = Screen.PrimaryScreen.Bounds;

            this.Window.VSync = VSyncMode.Off;

            this.Window.KeyPress += (s, e) => this.Window.Close();

            this.Window.MouseWheel += delegate(object s, MouseWheelEventArgs a) {
                this.scale += a.DeltaPrecise * .1F;
                if ( this.scale <= 0 ) this.scale = 0.05F;
                Console.WriteLine( a.Delta + " | " + this.scale );
            };
            bool down = false;
            this.Window.MouseDown += (sender, args) => down = true;
            this.Window.MouseUp   += (sender, args) => down = false;

            this.Window.MouseMove += delegate(object sender, MouseMoveEventArgs args) {
                if ( !down ) return;

                this.ox += args.XDelta;
                this.oy += args.YDelta;
            };

            this.Window.Location = new Point( 300, 100 );

            this.Window.Closing += delegate { Environment.Exit( 0 ); };
            this.w              =  this.Window.ClientSize.Width;
            this.h              =  this.Window.ClientSize.Height;
            this.h2             =  this.Window.ClientSize.Height / 2;
            this.w2             =  this.Window.ClientSize.Width  / 2;

            this.mv.AddRange( Enumerable.Repeat( false, COLORS_I ) );

            Random r = new Random();

            for ( int i = 0; i < COLORS_I / 2; i++ ) {
                var v = r.NextDouble() > .5;
                Thread.Sleep( 1 );
                this.mv[r.Next( 0, COLORS_I )] = v;
                Thread.Sleep( 1 );
            }

            for ( int i = 0; i < 10; i++ ) {
                this._creeps.Add( new Creep( Direction.n, new Point( ( maxSize / 2 ) + (int) ( i * r.NextDouble() * 100 ), ( maxSize / 2 ) + (int) ( i * r.NextDouble() * 100 ) ) ) );
            }

            new Thread( Work ).Start();

            Run();
        }

        List<Creep> _creeps = new List<Creep>( 10 );


        private void Work() {
            while ( true ) {
                foreach ( var c in this._creeps ) {
                    if ( c.pos.X < 0 ) c.pos.X        = 1;
                    if ( c.pos.Y < 0 ) c.pos.Y        = 1;
                    if ( c.pos.Y >= maxSize ) c.pos.Y = maxSize - 2;
                    if ( c.pos.X >= maxSize ) c.pos.X = maxSize - 2;
                    WorkDirection( this.areal[c.pos.X, c.pos.Y], c );
                }
            }
        }

        private List<bool> mv = new List<bool>();

        void WorkDirection(byte value, Creep c) {
            bool icrese = true;

            bool dx = this.mv[value];

            if ( value == COLORS_I - 1 ) {
                dx                           = true;
                icrese                       = false;
                this.areal[c.pos.X, c.pos.Y] = 0;
            }

            if ( icrese )
                this.areal[c.pos.X, c.pos.Y]++;

            switch (c.direction) {
                case Direction.n:
                    if ( dx ) {
                        c.pos.X++;
                        c.direction = Direction.o;
                    }
                    else {
                        c.pos.X--;
                        c.direction = Direction.w;
                    }

                    break;

                case Direction.s:
                    if ( !dx ) {
                        c.pos.X++;
                        c.direction = Direction.o;
                    }
                    else {
                        c.pos.X--;
                        c.direction = Direction.w;
                    }

                    break;

                case Direction.o:
                    if ( dx ) {
                        c.pos.Y++;
                        c.direction = Direction.s;
                    }
                    else {
                        c.pos.Y--;
                        c.direction = Direction.n;
                    }

                    break;

                case Direction.w:
                    if ( !dx ) {
                        c.pos.Y++;
                        c.direction = Direction.s;
                    }
                    else {
                        c.pos.Y--;
                        c.direction = Direction.n;
                    }

                    break;
            }

            //Console.WriteLine( "arnt: " + this.arnt );
        }

        enum Direction {
            n, s, o, w
        }

        class Creep {
            public Point     pos;
            public Direction direction;

            /// <inheritdoc />
            public Creep(Direction direction, Point pos) {
                this.direction = direction;
                this.pos       = pos;
            }
        }
    }
}
