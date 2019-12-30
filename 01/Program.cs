using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using OpenTK.Graphics.OpenGL;
using static _01.Helper;

namespace _01 {
    internal static class Program {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        private static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );
            new MainClass();
        }
    }

    public static class Helper {
        public const long UNO_MIL_L = 10000;
        public const long UNO_SEC_L = UNO_MIL_L / 2;

        public static void SleepMicro(long t) {
            long ticks = DateTime.Now.Ticks;
            while ( ( DateTime.Now.Ticks - ticks ) < t ) ;
        }

        public static void Sleep(long length) {
            long nano = 2000000000L / length;
            //nano /= TimeSpan.TicksPerMillisecond;
            //nano *= 100L;
            //
            //var sw = Stopwatch.StartNew();
            //while ( sw.ElapsedMilliseconds < nano) ;
            finaliseUpdate( nano );
        }

        static long nanoTime() {
            long nano = 10000L * Stopwatch.GetTimestamp();
            nano /= TimeSpan.TicksPerMillisecond;
            nano *= 100L;
            return nano;
        }

        private static void finaliseUpdate(long nanoSeconds) {
            long timeEspand;
            long startTime = nanoTime();
            do {
                timeEspand = nanoTime() - startTime;
            } while ( timeEspand < nanoSeconds );
        }

        public static void SleepMil(long mu) { SleepMicro( mu * UNO_MIL_L ); }
        public static void SleepSec(long mu) { SleepMicro( mu * UNO_MIL_L ); }
    }

    internal class MainClass : OpenTKFramework.MainClass {
        public const int           S       = 1000;
        private      string        _sortere = "null";
        public       List<ISorter> Sorts  = new List<ISorter>();

        private CounterArray<int> _array = new CounterArray<int>( new int[S] );

        private void Flip(int p1, int p2) {
            int t = this._array[p1];
            this._array[p1] = this._array[p2];
            this._array[p2] = t;
        }

        private Random _r = new Random();

        private void Shuffle() {
            this._sortere = nameof(Shuffle);
            if ( this._r.NextDouble() > .5 ) {
                for ( int i = 0; i < this._array.Length; i++ ) {
                    this._array[i] = i;
                }

                for ( int i = 0; i < this._array.Length; i++ ) {
                    Flip( i, this._r.Next( this._array.Length ) );
                    Sleep( this._array.Length*2 );
                }
            }
            else {
                for ( int i = 0; i < this._array.Length; i++ ) {
                    this._array[i] = this._r.Next( this._array.Length );
                    Sleep( this._array.Length*2 );
                }
            }
        }

        private void Sorter() {
            for ( int i = 0;; i++ ) {
                this._gets = 0;
                this._sets = 0;
                Shuffle();

                Thread.Sleep( 1000 );

                this._gets = 0;
                this._sets = 0;

                this._sortere = this.Sorts[i % this.Sorts.Count].GetType().Name;
                this.Sorts[i % this.Sorts.Count].Sort();

                Thread.Sleep( 2000 );
            }
        }


        public MainClass() {
            this._array.Get += GetCounter;
            this._array.Set += SetCounter;
            
            //this.Sorts.Add( new counting() );
            this.Sorts.Add( new Bitonic() );
            //this.Sorts.Add( new Tree() );
            this.Sorts.Add( new Quick3() );
            this.Sorts.Add( new OddEven() );
            this.Sorts.Add( new Stooge() );
            this.Sorts.Add( new Binary() );
            this.Sorts.Add( new Time() );
            this.Sorts.Add( new Shell() );
            this.Sorts.Add( new Cocktail() );
            this.Sorts.Add( new Radix() );
            this.Sorts.Add( new Insertion() );
            this.Sorts.Add( new My() );
            this.Sorts.Add( new Selection() );
            this.Sorts.Add( new Heap() );
            //this._sorts.Add( new phole() );
            this.Sorts.Add( new Merge() );
            this.Sorts.Add( new Comb() );
            this.Sorts.Add( new Quick() );
            this.Sorts.Add( new Cycle() );
            this.Sorts.Add( new Bubble() );

            foreach ( var sort in this.Sorts ) {
                sort.MArray = this._array;
            }

            for ( int i = 0; i < this._array.Length; i++ ) {
                this._array[i] = i;
            }

            Create( new Size( 2000, 800 ) );

            this.Window.VSync = VSyncMode.Off;

            this.Window.TargetRenderFrequency = 100;
            this.Window.TargetUpdateFrequency = 100;

            this.Window.Closing += delegate(object sender, CancelEventArgs args) { Environment.Exit( 0 ); };

            this.Window.Location = new Point( 300, 100 );

            new Thread( Sorter ).Start();

            Run();
        }

        private void SetCounter(int index, int value) { this._sets++; }

        private void GetCounter(int index) { this._gets++; }

        private long _gets = 0L;
        private long _sets = 0L;

        #region Overrides of MainClass

        /// <inheritdoc />
        public override void Initialize(object sender, EventArgs e) { }

        /// <inheritdoc />
        public override void Shutdown(object sender, EventArgs e) { }

        public override void Render(object sender, FrameEventArgs e) {
            this.I.ClearScreen( Color.Black );

            var h = this.Window.ClientSize.Height;
            var w = this.Window.ClientSize.Width;
            var s = (float) w / (float) this._array.Length;

            //s = s < 1 ? 1 : s;

            var multi = (float) h / (float) w;

            //h = (int) ( h * multi );

            for ( int i = 0; i < this._array.Length; i++ ) {
                var value = this._array.GetNoEvent( i );

                //this.I.DrawRect( new RectangleF( (float) i * s, (float) h, (float) s, (float) -value * multi * s ), Fromvaule( i, value, ( 1 / (float) this._array.Length ) * Math.PI * 2 ) );
                this.I.DrawRect( new RectangleF( (float) i * s, (float) h/2 + value * multi * s /2F, (float)s < 1 ? 1 : s, (float) -value * multi * s ), Fromvaule( i, value, ( 1 / (float) this._array.Length ) * Math.PI * 2 ) );
            }

            this.I.DrawString( "Algorithms: "+this._sortere, new PointF( 10,  20 ), Color.BlueViolet );
            this.I.DrawString( "Gats: "+this._gets, new PointF( 10,  40 ), Color.BlueViolet );
            this.I.DrawString( "Sets: "+this._sets, new PointF( 10,  55 ), Color.BlueViolet );
        }

        private Color Fromvaule(int i, int h, double v) {
            var a = v;
            return Color.FromArgb( NSin( h * a, Math.Sin ), NSin( h * a, Math.Cos ), NSin( h * a, d => -Math.Sin( d ) ) );
        }

        private int NSin(double i, Func<double, double> mathFunc) { return (int) ( ( mathFunc( i ) + 1D ) / 2D * 255D ); }

        public override void Update(object sender, FrameEventArgs e) { }

        #endregion
    }
}