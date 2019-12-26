using OpenTK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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


    internal class MainClass : OpenTKFramework.MainClass {
        int[] array = new int[100];


        void flip(int p1, int p2) {
            int t = this.array[p1];
            this.array[p1] = this.array[p2];
            this.array[p2] = t;
        }

        Random r = new Random();

        void shuffle() {
            for ( int i = 0; i < this.array.Length; i++ ) {
                flip( i, this.r.Next( this.array.Length ) );
            }
        }

        void sorter() {
            while ( true ) {
                shuffle();

                Thread.Sleep( 1000 );

                for ( int write = 0; write < this.array.Length; write++ ) {
                    for ( int sort = 0; sort < this.array.Length - 1; sort++ ) {
                        Thread.Sleep( 1 );
                        if ( array[sort] > array[sort + 1] ) {
                            var temp = array[sort + 1];
                            array[sort + 1] = array[sort];
                            array[sort]     = temp;
                        }
                    }
                }

                Thread.Sleep( 1000 );
            }
        }

        private static List<int> MergeSort(List<int> unsorted) {
            if ( unsorted.Count <= 1 ) return unsorted;

            List<int> left  = new List<int>();
            List<int> right = new List<int>();

            int middle = unsorted.Count / 2;
            for ( int i = 0; i < middle; i++ ) //Dividing the unsorted list
            {
                left.Add( unsorted[i] );
            }

            for ( int i = middle; i < unsorted.Count; i++ ) {
                right.Add( unsorted[i] );
            }

            left  = MergeSort( left );
            right = MergeSort( right );
            return Merge( left, right );
        }

        private static List<int> Merge(List<int> left, List<int> right) {
            List<int> result = new List<int>();

            while ( left.Count > 0 || right.Count > 0 ) {
                if ( left.Count > 0 && right.Count > 0 ) {
                    if ( left.First() <= right.First() ) //Comparing First two elements to see which is smaller
                    {
                        result.Add( left.First() );
                        left.Remove( left.First() ); //Rest of the list minus the first element
                    }
                    else {
                        result.Add( right.First() );
                        right.Remove( right.First() );
                    }
                }
                else if ( left.Count > 0 ) {
                    result.Add( left.First() );
                    left.Remove( left.First() );
                }
                else if ( right.Count > 0 ) {
                    result.Add( right.First() );

                    right.Remove( right.First() );
                }
            }

            return result;
        }

        public MainClass() {
            for ( int i = 0; i < this.array.Length; i++ ) {
                this.array[i] = i;
            }

            Create(new Size(1700,1000));

            this.Window.VSync = VSyncMode.Off;

            this.Window.TargetRenderFrequency = 100;
            this.Window.TargetUpdateFrequency = 100;

            this.Window.Closing += delegate(object sender, CancelEventArgs args) { Environment.Exit( 0 ); };
            
            this.Window.Location = new Point(300,100);

            new Thread( sorter ).Start();
            Run();
        }

        #region Overrides of MainClass

        /// <inheritdoc />
        public override void Initialize(object sender, EventArgs e) { }

        /// <inheritdoc />
        public override void Shutdown(object sender, EventArgs e) { }

        public override void Render(object sender, FrameEventArgs e) {
            this.I.ClearScreen( Color.Black );

            var h = this.Window.ClientSize.Height;
            var w = this.Window.ClientSize.Width;
            var s = w / this.array.Length;

            var multi = (float) h / w;

            //h = (int) ( h * multi );

            for ( int i = 0; i < this.array.Length; i++ ) {
                this.I.DrawRect( new RectangleF( (float) i * s, (float) h, (float) s, (float) -this.array[i] * multi * s ), fromvaule( i, this.array[i] ,w) );
            }

            //this.I.DrawString( base.totalFramesUpdated.ToString(), new PointF( 10, this.Window.ClientSize.Height - 20 ), Color.BlueViolet );
        }

        Color fromvaule(int i, int h, double v) {
            var a = this.array.Length/v; return Color.FromArgb( NSin( h*a, Math.Sin ), NSin( h*a, Math.Cos ), NSin( h*a, d => -Math.Sin( d ) ) ); }

        int NSin(double i, Func<double, double> mathFunc) { return (int) ( ( mathFunc( i ) + 1D ) / 2D * 255D ); }

        public override void Update(object sender, FrameEventArgs e) { }

        #endregion
    }
}