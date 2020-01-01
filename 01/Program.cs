#region using

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.ES10;
using static _01.Helper;

#endregion

namespace _01 {
    internal static class Program {
        /// <summary>
        ///     Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        private static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );
            new MainClass();
        }
    }

    internal class MainClass : OpenTKFramework.MainClass {
        public const int S = 10000;

        private static int[] arrint = new int[S];

        private CounterArray<int> _array = new CounterArray<int>( ref arrint );

        private long _gets;

        private readonly Random        _r = new Random();
        private          long          _sets;
        private          string        _sortere = "null";
        public           List<ISorter> Sorts    = new List<ISorter>();

        public MainClass() {
            this._array.Get += GetCounter;
            this._array.Set += SetCounter;

            //this.Sorts.Add( new Tree() );
            //this.Sorts.Add( new Counting() );
            //this.Sorts.Add( new Pigeonhole() );
            this.Sorts.Add( new Comb() );
            this.Sorts.Add( new Cocktail() );
            this.Sorts.Add( new Bubble() );
            this.Sorts.Add( new Selection() );
            this.Sorts.Add( new Heap() );
            this.Sorts.Add( new OddEven() );
            this.Sorts.Add( new Heap() );
            this.Sorts.Add( new Cycle() );
            this.Sorts.Add( new My() );
            this.Sorts.Add( new Quick() );
            this.Sorts.Add( new Quick3() );
            this.Sorts.Add( new Merge() );
            this.Sorts.Add( new Introsort() );
            this.Sorts.Add( new Bitonic() );
            this.Sorts.Add( new Stooge() );
            this.Sorts.Add( new Binary() );
            this.Sorts.Add( new Time() );
            this.Sorts.Add( new Shell() );
            this.Sorts.Add( new Radix() );
            this.Sorts.Add( new Insertion() );

            foreach ( var sort in this.Sorts ) sort.MArray = this._array;

            for ( var i = 0; i < this._array.Length; i++ ) this._array[i] = i;

            Create( new Size( 2000, 800 ) );

            //this.Window.WindowState = WindowState.Fullscreen;
            //this.Window.WindowBorder = WindowBorder.Hidden;
            //this.Window.Bounds = Screen.PrimaryScreen.Bounds;

            this.Window.VSync = VSyncMode.Off;

            this.Window.KeyPress += (s, e) => this.Window.Close();

            this.Window.Location = new Point( 300, 100 );

            this.Window.Closing += delegate { Environment.Exit( 0 ); };

            if ( true ) {
                new Thread( () => Sorter( false ) ).Start();
                multy = 2;
            }
            else {
                new Thread( () => TestOneSort( new Heap(), .02, 100 ) ).Start();
            }

            h = this.Window.ClientSize.Height;
            w = this.Window.ClientSize.Width;
            s = w / (float) this._array.Length;
            m = h / (float) w;

            Run();
        }

        private void Flip(CounterArray<int> arr, int p1, int p2) {
            var t = arr[p1];
            arr[p1] = arr[p2];
            arr[p2] = t;
        }

        private void Shuffle(CounterArray<int> arr) {
            this._sortere = nameof(Shuffle);
            if ( this._r.NextDouble() > .5 ) {
                for ( var i = 0; i < arr.Length; i++ ) arr[i] = i;

                for ( var i = 0; i < arr.Length; i++ ) {
                    Flip( arr, i, this._r.Next( arr.Length ) );
                    Sleep( arr.Length * 2 );
                }
            }
            else {
                for ( var i = 0; i < arr.Length; i++ ) {
                    arr[i] = this._r.Next( arr.Length );
                    Sleep( arr.Length * 2 );
                }
            }
        }

        private void Reverse(CounterArray<int> arr) {
            this._sortere = nameof(Reverse);
            for ( var i = 0; i < arr.Length / 2; i++ ) {
                Flip( arr, i, arr.Length - i - 1 );
                Sleep( arr.Length );
            }
        }

        private void Sorter(bool testSorter = false) {
            for ( var i = 0;; i++ ) {
                var _sorter = this.Sorts[i % this.Sorts.Count];

                if ( testSorter ) {
                    TestOneSort( _sorter, .03 );
                    continue;
                }

                this._gets = 0;
                this._sets = 0;
                Shuffle( this._array );

                Thread.Sleep( 1000 );

                this._gets = 0;
                this._sets = 0;

                this._sortere = _sorter.GetType().Name + " sort";
                _sorter.Sort();

                Thread.Sleep( 1000 );

                this._gets = 0;
                this._sets = 0;

                Reverse( this._array );

                Thread.Sleep( 1000 );

                this._gets = 0;
                this._sets = 0;

                this._sortere = _sorter.GetType().Name + " sort";
                _sorter.Sort();

                Thread.Sleep( 2000 );
            }
        }

        private void TestOneSort(ISorter s, double setSpeed = .2, int end = 20) {
            var speed = Helper.multy;
            Helper.multy = setSpeed;
            for ( var f = 10; f < end; f++ ) {
                var ax = new int[(int) Math.Pow( f, 4 )];
                var ar = new CounterArray<int>( ref ax );
                ar.Get += GetCounter;
                ar.Set += SetCounter;

                //s.MArray = ar;

                s.MArray    = ar;
                this._array = ar;

                for ( var i = 0; i < ar.Length; i++ ) ar[i] = i;

                this._gets = 0;
                this._sets = 0;

                //Reverse( ar );
                Shuffle( ar );

                Sleep( 1 );

                this._gets = 0;
                this._sets = 0;

                this._sortere = s.GetType().Name + " sort";
                s.Sort();

                Sleep( 1 );

                this._gets = 0;
                this._sets = 0;

                Sleep( 1 );
                Sleep( 1 );
            }

            Helper.multy = speed;
        }

        private void SetCounter(int index, int value) { this._sets++; }

        private void GetCounter(int index) { this._gets++; }

        #region Overrides of MainClass

        /// <inheritdoc />
        public override void Initialize(object sender, EventArgs e) { }

        /// <inheritdoc />
        public override void Shutdown(object sender, EventArgs e) { }

        public override void Render(object sender, FrameEventArgs e) {
            this.I.ClearScreen( Color.Black );
            //h = (int) ( h * multi );
            //this.I.DrawString( "Algorithms: " + this._sortere, new PointF( 10, 20 ), Color.BlueViolet );
            //this.I.DrawString( "Gats: "       + this._gets,    new PointF( 10, 40 ), Color.BlueViolet );
            //this.I.DrawString( "Sets: "       + this._sets,    new PointF( 10, 55 ), Color.BlueViolet );

            this.Window.Title = "FPS: " + base.frameRate + ":        " + this._sortere + "             arrayLength:" + IntText( this._array.Length ) + "     Gats: " + IntText( this._gets ) + "     Sets: " + IntText( this._sets );

            Render( 0, this._array.Length );
        }

        private static string IntText(long score) {
            const int l = 2;

            var s = score.ToString();
            if ( s.Length < l + l ) return s;

            var e = s.Length - l;

            return s.Substring( 0, l ) + "," + s.Substring( l, l ) + "E+" + e.ToString( "D3" ); // + "  # " + score;

            //if ( score > 10000000 ) return ( score / 1000000F ).ToString( "F" ) + "E";
            //if ( score > 1000000 ) return ( score / 100000F ).ToString( "F" ) + "T";
            //if ( score > 100000 ) return ( score / 10000F ).ToString( "F" )   + "G";
            //if ( score > 10000 ) return ( score / 10000F ).ToString( "F" )    + "M";
            //if ( score > 1000 ) return ( score / 1000F ).ToString( "F" )      + "K";
        }

        public static int   h;
        public static int   w;
        public static float s;
        public static float m;

        private void Render(int start, int amount) {
            var p = this._array.Length <= w ? 1 : this._array.Length / w / 1;

            for ( var i = start; i < start + amount; i += p ) {
                var value = this._array.GetNoEvent( i );
                DrawRectImp( i, value );
            }
        }

        public double colorOffset;

        public DrawMode drawMode = DrawMode.Stairs;

        private void DrawRectImp(int px, int height) {
            var i     = px;
            var value = height;

            var p = this._array.Length <= w ? 1 : this._array.Length / w / 1;

            float x  = 0;
            float y  = 0;
            float a  = 0;
            float b  = 0;
            float nx = 0;
            float ny = 0;

            var c = Fromvaule( i, value, 1F / this._array.Length * Math.PI * 2 );

            a = s < 1 ? 1 : s;

            switch (this.drawMode) {
                case DrawMode.Triangle:
                    x = i * s;
                    y = h / 2f + value / 2f * m * s;
                    b = -value * m * s;
                    this.I.DrawRect( new RectangleF( x, y, a, b ), c );
                    break;
                case DrawMode.Stairs:
                    x = i * s;
                    y = h;
                    b = -value * m * s;
                    this.I.DrawRect( new RectangleF( x, y, a, b ), c );
                    break;
                case DrawMode.Snake:
                    x = (float) ( w * ( (float) value / _array.Length ) / 2F * Math.Cos( ( (float) ( 2.0f * Math.PI * ( (float) i / this._array.Length ) ) ) ) ) + w / 2F; //calculate the x component
                    y = (float) ( h * ( (float) value / _array.Length ) / 2F * Math.Sin( ( (float) ( 2.0f * Math.PI * ( (float) i / this._array.Length ) ) ) ) ) + h / 2F; //calculate the y component

                    nx = (float) ( w * ( (float) value / _array.Length ) / 2F * Math.Cos( ( (float) ( 2.0f * Math.PI * ( (float) ( i + p ) / this._array.Length ) ) ) ) ) + w / 2F; //calculate the x component next
                    ny = (float) ( h * ( (float) value / _array.Length ) / 2F * Math.Sin( ( (float) ( 2.0f * Math.PI * ( (float) ( i + p ) / this._array.Length ) ) ) ) ) + h / 2F; //calculate the y component next

                    this.I.Draw( new List<Vector2> { new Vector2( x, y ), new Vector2( nx, ny ), new Vector2( ( w / 2F ), ( h / 2F ) ) }, c );
                    break;
                case DrawMode.FlipStairs:
                    b =  -value * m * s;
                    a /= 2;
                    x =  i * s / 2;
                    y =  1F    * h;
                    this.I.DrawRect( new RectangleF( w / 2F - x, y, a, b ), c );
                    this.I.DrawRect( new RectangleF( w / 2F + x, y, a, b ), c );
                    break;
                case DrawMode.FlipTriangles:
                    b =  -value * m * s;
                    x =  i * s      / 2;
                    y =  h / 2f + value / 2f * m * s;
                    a /= 2;
                    this.I.DrawRect( new RectangleF( w / 2F - x, y, a, b ), c );
                    this.I.DrawRect( new RectangleF( w / 2F + x, y, a, b ), c );
                    break;
                case DrawMode.Cycle:
                    x = (float) ( w / 2F * Math.Cos( (float) ( 2.0f * Math.PI * ( (float) i / this._array.Length ) ) ) ) + w / 2F; //calculate the x component
                    y = (float) ( h / 2F * Math.Sin( (float) ( 2.0f * Math.PI * ( (float) i / this._array.Length ) ) ) ) + h / 2F; //calculate the y component

                    nx = (float) ( w / 2F * Math.Cos( (float) ( 2.0f * Math.PI * ( (float) ( i + p ) / this._array.Length ) ) ) ) + w / 2F; //calculate the x component next
                    ny = (float) ( h / 2F * Math.Sin( (float) ( 2.0f * Math.PI * ( (float) ( i + p ) / this._array.Length ) ) ) ) + h / 2F; //calculate the y component next

                    this.I.Draw( new List<Vector2> { new Vector2( x, y ), new Vector2( nx, ny ), new Vector2( ( w / 2F ), ( h / 2F ) ) }, c );
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public enum DrawMode {
            Triangle,
            Stairs,
            Cycle,
            Snake,
            FlipTriangles,
            FlipStairs
        }

        private Color Fromvaule(int i, double h, double v) {
            var a = v;
            h += this.colorOffset;
            return Color.FromArgb( NSin( h * a, Math.Sin ), NSin( h * a, Math.Cos ), NSin( h * a, d => -Math.Sin( d ) ) );
        }

        private int NSin(double i, Func<double, double> mathFunc) => (int) ( ( mathFunc( i ) + 1D ) / 2D * 255D );

        public override void Update(object sender, FrameEventArgs e) {
            h = this.Window.ClientSize.Height;
            w = this.Window.ClientSize.Width;
            s = w / (float) this._array.Length;
            m = h / (float) w;

            //this.colorOffset -= 5;
        }

        #endregion
    }
}