using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using OpenTK.Graphics.OpenGL;
using static _01.Helper;
using KeyPressEventArgs = OpenTK.KeyPressEventArgs;

namespace _01 {
    internal static class Program {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        private static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );
            new MainClass( );
        }
    }

    internal class MainClass : OpenTKFramework.MainClass {
        public const int           S        = 5000;
        private      string        _sortere = "null";
        public       List<ISorter> Sorts    = new List<ISorter>();

        static int[] arrint = new int[S];

        private CounterArray<int> _array = new CounterArray<int>( ref arrint );

        private void Flip(CounterArray<int> arr, int p1, int p2) {
            int t = arr[p1];
            arr[p1] = arr[p2];
            arr[p2] = t;
        }

        private Random _r = new Random();

        private void Shuffle(CounterArray<int> arr) {
            this._sortere = nameof(Shuffle);
            if ( this._r.NextDouble() > .5 ) {
                for ( int i = 0; i < arr.Length; i++ ) {
                    arr[i] = i;
                }

                for ( int i = 0; i < arr.Length; i++ ) {
                    Flip( arr, i, this._r.Next( arr.Length ) );
                    Sleep( arr.Length * 2 );
                }
            }
            else {
                for ( int i = 0; i < arr.Length; i++ ) {
                    arr[i] = this._r.Next( arr.Length );
                    Sleep( arr.Length * 2 );
                }
            }
        }

        void Reverse(CounterArray<int> arr) {
            this._sortere = nameof(Reverse);
            for ( int i = 0; i < arr.Length / 2; i++ ) {
                Flip( arr, i, arr.Length - i - 1 );
                Sleep( arr.Length );
            }
        }

        private void Sorter() {
            for ( int i = 0;; i++ ) {
                var _sorter = this.Sorts[i % this.Sorts.Count];

                this._gets = 0;
                this._sets = 0;
                Shuffle( this._array );

                Thread.Sleep( 1000 );

                this._gets = 0;
                this._sets = 0;

                this._sortere = _sorter.GetType().Name;
                _sorter.Sort();

                Thread.Sleep( 1000 );

                this._gets = 0;
                this._sets = 0;

                Reverse( this._array );

                Thread.Sleep( 1000 );

                this._gets = 0;
                this._sets = 0;

                this._sortere = _sorter.GetType().Name;
                _sorter.Sort();

                Thread.Sleep( 2000 );
            }
        }

        void TestOneSort() {
            for ( int f = 1; f < 100; f++ ) {
                var ax = new int[(int) Math.Pow( f, f )];
                var ar = new CounterArray<int>( ref ax );
                ar.Get += GetCounter;
                ar.Set += SetCounter;

                ISorter s = new Heap() { MArray = ar };
                //s.MArray = ar;

                this._array = ar;

                for ( int i = 0; i < ar.Length; i++ ) {
                    ar[i] = i;
                }

                this._gets = 0;
                this._sets = 0;
                Shuffle( ar );

                Sleep( 1 );

                this._gets = 0;
                this._sets = 0;

                this._sortere = s.GetType().Name;
                s.Sort();

                Sleep( 1 );

                this._gets = 0;
                this._sets = 0;

                Reverse( ar );

                Sleep( 1 );

                this._gets = 0;
                this._sets = 0;

                this._sortere = s.GetType().Name;
                s.Sort();

                Sleep( 1 );
                Sleep( 1 );
            }
        }

        public MainClass() {
            this._array.Get += GetCounter;
            this._array.Set += SetCounter;

            this.Sorts.Add( new OddEven() );
            this.Sorts.Add( new Heap() );
            this.Sorts.Add( new Merge() );
            //this.Sorts.Add( new counting() );
            this.Sorts.Add( new Introsort() );
            this.Sorts.Add( new Bitonic() );
            //this.Sorts.Add( new Tree() );
            this.Sorts.Add( new Quick3() );
            this.Sorts.Add( new Stooge() );
            this.Sorts.Add( new Binary() );
            this.Sorts.Add( new Time() );
            this.Sorts.Add( new Shell() );
            this.Sorts.Add( new Cocktail() );
            this.Sorts.Add( new Radix() );
            this.Sorts.Add( new Insertion() );
            this.Sorts.Add( new My() );
            this.Sorts.Add( new Selection() );
            //this._sorts.Add( new phole() );
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

            //this.Window.WindowState = WindowState.Fullscreen;
            //this.Window.WindowBorder = WindowBorder.Hidden;
            //this.Window.Bounds = Screen.PrimaryScreen.Bounds;

            this.Window.KeyPress += (s, e) => this.Window.Close();

            this.Window.TargetRenderFrequency = 100;
            this.Window.TargetUpdateFrequency = 100;

            this.Window.Location = new Point( 300, 100 );

            this.Window.Closing += delegate(object sender, CancelEventArgs args) { Environment.Exit( 0 ); };

           new Thread( Sorter ).Start();multy = 2;
            //new Thread( TestOneSort ).Start(); Helper.multy = .5;

            h = this.Window.ClientSize.Height;
            w = this.Window.ClientSize.Width;
            s = (float) w / (float) this._array.Length;
            m = (float) h / (float) w;
            
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
            //h = (int) ( h * multi );

            Render( 0,this._array.Length );


            this.I.DrawString( "Algorithms: " + this._sortere, new PointF( 10, 20 ), Color.BlueViolet );
            this.I.DrawString( "Gats: "       + this._gets,    new PointF( 10, 40 ), Color.BlueViolet );
            this.I.DrawString( "Sets: "       + this._sets,    new PointF( 10, 55 ), Color.BlueViolet );
        }
        
        public static int   h;
        public static int   w;
        public static float s;
        public static float m;

        void Render(int start, int amount) {
            var p = (this._array.Length / w)/2;

            for ( int i = start; i < start + amount; i+= p ) {
                var value = this._array.GetNoEvent( i );
                DrawRectImp( i, value );
            }
        }

        void DrawRectImp(int px, int height) {
            var i     = px;
            var value = height;

            //this.I.DrawRect( new RectangleF( (float) i * s, (float) h, (float) s, (float) -value * m * s ), Fromvaule( i, value, ( 1 / (float) this._array.Length ) * Math.PI * 2 ) );

            this.I.DrawRect( new RectangleF( (float) i * s, (float) h / 2 + value * m * s / 2F, (float) s < 1 ? 1 : s, (float) -value * m * s ), Fromvaule( i, value, ( 1 / (float) this._array.Length ) * Math.PI * 2 ) );
        }

        private Color Fromvaule(int i, int h, double v) {
            var a = v;
            return Color.FromArgb( NSin( h * a, Math.Sin ), NSin( h * a, Math.Cos ), NSin( h * a, d => -Math.Sin( d ) ) );
        }

        private int NSin(double i, Func<double, double> mathFunc) { return (int) ( ( mathFunc( i ) + 1D ) / 2D * 255D ); }

        public override void Update(object sender, FrameEventArgs e) {
            h = this.Window.ClientSize.Height;
            w = this.Window.ClientSize.Width;
            s = (float) w / (float) this._array.Length;
            m = (float) h / (float) w;
        }

        #endregion
    }
}