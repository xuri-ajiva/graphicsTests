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
        public const int           S       = 1000;
        private      string        sortere = "null";
        public       List<ISorter> _sorts  = new List<ISorter>();

        int[] array = new int[S];


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

                var s = this.r.Next( this._sorts.Count );
                this.sortere = _sorts[s].ToString();
                this._sorts[s].sort();

                //new bubble() { array = this.array }.sort();

                //new merge() { array = this.array }.sort();

                //new heap() { array = this.array }.sort();

                Thread.Sleep( 1000 );
            }
        }


        public MainClass() {
            //this._sorts.Add( new bubble() );
            this._sorts.Add( new heap() );
            //this._sorts.Add( new quick() );

            foreach ( var sort in this._sorts ) {
                sort.array = this.array;
            }

            for ( int i = 0; i < this.array.Length; i++ ) {
                this.array[i] = i;
            }

            Create( new Size( 2000, 200 ) );

            this.Window.VSync = VSyncMode.Off;

            this.Window.TargetRenderFrequency = 100;
            this.Window.TargetUpdateFrequency = 100;

            this.Window.Closing += delegate(object sender, CancelEventArgs args) { Environment.Exit( 0 ); };

            this.Window.Location = new Point( 300, 100 );

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
            var s = (float) w / (float) this.array.Length;

            var multi = (float) h / (float) w;

            //h = (int) ( h * multi );

            for ( int i = 0; i < this.array.Length; i++ ) {
                this.I.DrawRect( new RectangleF( (float) i * s, (float) h, (float) s, (float) -this.array[i] * multi * s ), fromvaule( i, this.array[i], ( 1 / (float) this.array.Length ) * 5 ) );
            }

            this.I.DrawString( sortere, new PointF( 10, this.Window.ClientSize.Height - 20 ), Color.BlueViolet );
        }

        Color fromvaule(int i, int h, double v) {
            var a = v;
            return Color.FromArgb( NSin( h * a, Math.Sin ), NSin( h * a, Math.Cos ), NSin( h * a, d => -Math.Sin( d ) ) );
        }

        int NSin(double i, Func<double, double> mathFunc) { return (int) ( ( mathFunc( i ) + 1D ) / 2D * 255D ); }

        public override void Update(object sender, FrameEventArgs e) { }

        #endregion
    }

    class heap : ISorter {
        private const int S = MainClass.S;

        #region Implementation of ISorter

        /// <inheritdoc />
        public int[] array { get; set; }

        /// <inheritdoc />
        public void sort() { heapSort( array, array.Length ); }

        static void heapSort(int[] arr, int n) {
            for ( int i = n / 2 - 1; i >= 0; i-- ) {
                Thread.Sleep( (int) ( 1000F / S ) );
                heapify( arr, n, i );
            }

            for ( int i = n - 1; i >= 0; i-- ) {
                int temp = arr[0];
                arr[0] = arr[i];
                arr[i] = temp;

                Thread.Sleep( (int) ( 1000F / S ) );
                heapify( arr, i, 0 );
            }
        }

        static void heapify(int[] arr, int n, int i) {
            int largest                                           = i;
            int left                                              = 2 * i + 1;
            int right                                             = 2 * i + 2;
            if ( left  < n && arr[left]  > arr[largest] ) largest = left;
            if ( right < n && arr[right] > arr[largest] ) largest = right;
            if ( largest != i ) {
                int swap = arr[i];
                arr[i]       = arr[largest];
                arr[largest] = swap;
                heapify( arr, n, largest );
            }
        }

        #endregion
    }

    class merge : ISorter {
        #region Implementation of ISorter

        /// <inheritdoc />
        public int[] array { get; set; }

        /// <inheritdoc />
        public void sort() { array = MergeSort( array.ToList() ).ToArray(); }


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

        #endregion
    }

    class bubble : ISorter {
        private const int S = MainClass.S;
        
        #region Implementation of ISorter

        /// <inheritdoc />
        public int[] array { get; set; }

        /// <inheritdoc />
        public void sort() {
            for ( int write = 0; write < this.array.Length; write++ ) {
                Thread.Sleep( (int) ( 1000F / S ) );
                for ( int sort = 0; sort < this.array.Length - 1; sort++ ) {
                    if ( array[sort] > array[sort + 1] ) {
                        var temp = array[sort + 1];
                        array[sort + 1] = array[sort];
                        array[sort]     = temp;
                    }
                }
            }
        }

        #endregion
    }

    class quick : ISorter {
        
        private const int S = MainClass.S;

        private int[] _array;

        private static void quicksort(int links, int rechts, ref int[] daten) {
            if ( links < rechts ) {
                int teiler = teile( links, rechts, ref daten );
                quicksort( links, teiler - 1, ref daten );
                quicksort( teiler        + 1, rechts, ref daten );
            }
        }

        private static int teile(int links, int rechts, ref int[] daten) {
            int i = links;
            int j     = rechts - 1;
            int pivot = daten[rechts];

            do {
                while ( daten[i] <= pivot && i < rechts ) i += 1;
                while ( daten[j] >= pivot && j > links ) j -= 1;

                
                Thread.Sleep( (int) ( 1000F / S ) );

                if ( i < j ) {
                    int z = daten[i];
                    daten[i] = daten[j];
                    daten[j] = z;
                }
            } while ( i < j );

            if ( daten[i] > pivot ) {
                int z = daten[i];
                daten[i] = daten[rechts];
                daten[rechts] = z;
            }

            return i;
        }

        #region Implementation of ISorter

        /// <inheritdoc />
        public int[] array { get => this._array; set => this._array = value; }

        /// <inheritdoc />
        public void sort() { quicksort( 0, _array.Length - 1, ref this._array ); }

        #endregion
    }

    interface ISorter {
        int[] array { get; set; }
        void  sort();
    }
}