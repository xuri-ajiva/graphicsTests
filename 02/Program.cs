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
using OpenTKFramework.Framework;

namespace _02 {
    internal static class Program {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        private static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );

            while ( true ) {
                new MainClass();
            }
        }
    }

    internal class MainClass : OpenTKFramework.MainClass {
        private struct Paket {
            public Point pos;
            public byte  value;

            public Paket(Point pos, byte value) {
                this.pos   = pos;
                this.value = value;
            }
        }

        private Queue<Paket> _update = new Queue<Paket>();

        private const int COLORS_I     = 40;
        private const int COLOR_STEP_I = 240 / COLORS_I;
        private const int COLOR_HALF   = 255 / 2;

        private       float   scale   = .05F;
        private       float   PS      = 20;
        private const int     maxSize = 20000;
        private       byte[,] areal   = new byte[maxSize, maxSize];
        private       int     w, h, w2, h2, ox, oy;


        #region Overrides of MainClass

        /// <inheritdoc />
        public override void Initialize(object sender, EventArgs e) { }

        /// <inheritdoc />
        public override void Shutdown(object sender, EventArgs e) { }

        /// <inheritdoc />
        public override void Render(object sender, FrameEventArgs e) {
            this.I.ClearScreen( Color.FromArgb( 1, 82, 82, 82 ) );
            this.Window.Title = "     ArraySize: " + maxSize + "     Position: {x: " + ox + ",y: " + oy + "}     FPS: " + this.frameRate + "     Scale: " + this.scale + "     PixelSize: " + this.PS;

            //while ( this._update.Count > 0 ) {
            //    var p = this._update.Dequeue();
            //    if ( p.value == 0 ) continue;
            //
            //    var x = p.pos.X + this.ox;
            //    var y = p.pos.Y + this.oy;
            //    
            //    if ( this.scale * x < 0  || this.scale * y < 0 ) continue;
            //    if ( this.scale * x >= w || this.scale * y >= h ) continue;
            //    
            //    if ( x < 0        || y < 0 ) continue;
            //    if ( x >= maxSize || y >= maxSize ) continue;
            //    
            //    
            //    var po = new PointF( this.scale * x, this.scale * y );
            //    
            //    //if ( Math.Abs( ( (int) p.X - p.X ) ) < this.scale/2 || Math.Abs( ( (int) p.Y - p.Y ) ) < this.scale /2)
            //    this.I.DrawPoint( po, colorFromValue( p.value ) );
            //    //this.I.DrawRect( new RectangleF( p, new SizeF(1,1) ), colorFromValue( v ) );
            //}

            //
            //for ( int i = 0; i < this.w; i++ ) {
            //    for ( int j = 0; j < this.h; j++ ) {
            //        var x = i + this.ox;
            //        var y = j + this.oy;
            //        if ( this.scale * x < 0  || this.scale * y < 0 ) continue;
            //        if ( this.scale * x >= maxSize || this.scale * y >= maxSize ) continue;
            //
            //        var v = this.areal[(int) x, (int) y];
            //
            //        if ( v == 0 ) continue;
            //
            //        var p = new PointF( this.scale * x, this.scale * y );
            //        
            //        this.I.DrawPoint( p, colorFromValue( v ) );
            //    }
            //}

            //SizeF s = new SizeF( PS, PS);
            for ( float i = 0; i < maxSize; i += PS ) {
                for ( float j = 0; j < maxSize; j += PS ) {
                    var x = i + this.ox;
                    var y = j + this.oy;

                    if ( x < 0        || y < 0 ) continue;
                    if ( x >= maxSize || y >= maxSize ) continue;

                    var v = this.areal[(int) i, (int) j];

                    if ( v == 0 ) continue;

                    if ( this.scale * x < 0  || this.scale * y < 0 ) continue;
                    if ( this.scale * x >= w || this.scale * y >= h ) continue;

                    var p = new PointF( this.scale * x, this.scale * y );
                    var s = new SizeF( this.scale * ( x + this.PS ) - this.scale * x, this.scale * ( y + PS ) - this.scale * y );

                    //if ( Math.Abs( ( (int) p.X - p.X ) ) < .1 || Math.Abs( ( (int) p.Y - p.Y ) ) < .1 )
                    this.I.DrawRect( new RectangleF( p, s ), colorFromValue( v ) );
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

            // return Color.Blue;
        }

        /// <inheritdoc />
        public override void Update(object sender, FrameEventArgs e) {
            this.w  = this.Window.ClientSize.Width;
            this.h  = this.Window.ClientSize.Height;
            this.h2 = this.Window.ClientSize.Height / 2;
            this.w2 = this.Window.ClientSize.Width  / 2;
        }

        #endregion

        private bool exit = true;

        private bool fe = true;

        List<Thread> _ts = new List<Thread>();

        public MainClass() {
            for ( int i = 0; i < maxSize; i++ ) {
                for ( int j = 0; j < maxSize; j++ ) {
                    this.areal[i, j] = 0;
                }
            }

            Create( new Size( 1000, 1000 ) );
            this.Window.Location = new Point( 300, 100 );

            //this.Window.WindowState = WindowState.Fullscreen;
            //this.Window.WindowBorder = WindowBorder.Hidden;
            //this.Window.Bounds = Screen.PrimaryScreen.Bounds;

            this.Window.VSync = VSyncMode.Off;

            this.Window.KeyDown += delegate(object s, KeyboardKeyEventArgs e) {
                if ( e.Key == Key.Escape ) {
                    exit = false;
                    this.Window.Close();
                }

                if ( e.Key == Key.ControlLeft ) {
                    this.fe = !this.fe;
                    Console.WriteLine( this.fe );
                }
            };

            this.Window.MouseWheel += delegate(object s, MouseWheelEventArgs a) {
                if ( fe ) {
                    this.PS += a.DeltaPrecise * .1F;
                    Console.WriteLine( a.Delta + " | " + this.PS );
                }
                else {
                    this.scale += a.DeltaPrecise * .1F;
                    Console.WriteLine( a.Delta + " | " + this.scale );
                }

                if ( this.scale <= 0 ) this.scale = 0.05F;
                if ( this.PS    <= .01 ) this.PS  = .01F;
            };
            bool down = false;
            this.Window.MouseDown += (sender, args) => down = true;
            this.Window.MouseUp   += (sender, args) => down = false;

            this.Window.MouseMove += delegate(object sender, MouseMoveEventArgs args) {
                if ( !down ) return;

                this.ox += (int) ( args.XDelta * this.PS );
                this.oy += (int) ( args.YDelta * this.PS );
            };

            this.Window.Closing += delegate {
                if ( this.exit ) Environment.Exit( 0 );
                else
                { foreach ( var t in this._ts )
                        t.Abort();
                    this.areal = null;
                    GC.Collect();
                }
            };
            this.w  = this.Window.ClientSize.Width;
            this.h  = this.Window.ClientSize.Height;
            this.h2 = this.Window.ClientSize.Height / 2;
            this.w2 = this.Window.ClientSize.Width  / 2;

            this.mv.AddRange( Enumerable.Repeat( false, COLORS_I+10 ) );

            Random r = new Random();

            for ( int i = 0; i < COLORS_I / 4; i++ ) {
                var v = r.NextDouble() > .5;
                Thread.Sleep( 1 );
                this.mv[r.Next( 0, COLORS_I )] = v;
                Thread.Sleep( 1 );
            }
            //this.mv[2] = true;
            //this.mv[8] = true;

            //this.mv[13] = true;
            //this.mv[21] = true;
            //this.mv[39] = true;
            //this.mv[40] = true;
            //this.mv[41] = true;
            //this.mv[75] = true;
            //this.mv[77] = true;
            //this.mv[78] = true;
            //this.mv[89] = true;

            //this.mv[4] = true;
            //this.mv[9] = true;
            //this.mv[10] = true;
            //this.mv[24] = true;
            //this.mv[30] = true;
            //this.mv[41] = true;
            //this.mv[52] = true;
            //this.mv[61] = true;
            //this.mv[64] = true;
            //this.mv[65] = true;
            //this.mv[70] = true;
            //this.mv[74] = true;
            //this.mv[78] = true;
            //this.mv[81] = true;

            //for ( int i = 0; i < 1; i++ ) {
            //    this._creeps.Add( new Creep( Direction.n, new Point( ( maxSize / 2 ) + (int) ( i * r.NextDouble() * 100 ), ( maxSize / 2 ) + (int) ( i * r.NextDouble() * 100 ) ) ) );
            //}

            this._creeps.Add( new Creep( Direction.n, new Point( maxSize     / 4, maxSize     / 4 ) ) );
            this._creeps.Add( new Creep( Direction.n, new Point( maxSize / 4 * 2, maxSize / 4 * 2 ) ) );
            this._creeps.Add( new Creep( Direction.n, new Point( maxSize / 4 * 3, maxSize / 4 * 3 ) ) );

            Console.WriteLine( "setupDirections" );

            for ( var i = 0; i < COLORS_I; i++ ) {
                Console.WriteLine( "mv[" + i + "] = "+this.mv[i]+";           /// # " + ( this.mv[i] ? "R" : "L" ) );
            }

            foreach ( var c in this._creeps ) {
                var t = new Thread( () => Work( c ) );
                t.Start();
                this._ts.Add( t );
            }

            Run();
        }

        private List<Creep> _creeps = new List<Creep>();


        private void Work(Creep c) {
            while ( true ) {
                if ( c.pos.X < 0 ) c.pos.X        = 1;
                if ( c.pos.Y < 0 ) c.pos.Y        = 1;
                if ( c.pos.Y >= maxSize ) c.pos.Y = maxSize - 2;
                if ( c.pos.X >= maxSize ) c.pos.X = maxSize - 2;
                WorkDirection( this.areal[c.pos.X, c.pos.Y], c );

                //Thread.Sleep( 1 );
            }
        }

        private List<bool> mv = new List<bool>();

        private void WorkDirection(byte value, Creep c) {
            bool icrese = true;

            bool dx = this.mv[value];

            if ( value >= COLORS_I - 1 ) {
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

            //this._update.Enqueue( new paket(c.pos, this.areal[c.pos.X, c.pos.Y]) );
            //Console.WriteLine( "arnt: " + this.arnt );
        }

        private enum Direction {
            n, s, o, w
        }

        private class Creep {
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
