using System;
using System.Diagnostics;

namespace _01 {
    public static class Helper {
        public static double multy     = 1;
        public const  long   UNO_MIL_L = 10000;
        public const  long   UNO_SEC_L = UNO_MIL_L / 2;

        public static void SleepMicro(long t) {
            long ticks = DateTime.Now.Ticks;
            while ( ( DateTime.Now.Ticks - ticks ) < t ) ;
        }

        private static int DoNotSellpFor = 0;

        public static void Sleep(long Rlength) {
            if ( DoNotSellpFor > 0 ) {
                DoNotSellpFor--;
                return;
            }

            long nano = (long) ( 1000000000L * multy ) / Rlength;
            //nano /= TimeSpan.TicksPerMillisecond;
            //nano *= 100L;
            //
            //var sw = Stopwatch.StartNew();
            //while ( sw.ElapsedMilliseconds < nano) ;
            try {
                finaliseUpdate( nano );
            } catch {
                DoNotSellpFor = 1000;
            }
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
}