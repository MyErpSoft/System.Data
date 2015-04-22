using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Data.DataEntities.Test {
    public struct Size {
        public int Width;
        public int Height;
    }

    public class MyForm {
        public string Name { get; set; }
        public int Width { get; set; }
    }

    public class MyForm2 : MyForm {
        public Size Size { get; set; }
    }

    public static class PerformanceTest {
        public static void Do(int debugCount,int releaseCount, TimeSpan time, Action<int> action) {
            System.Diagnostics.Stopwatch watch = new Diagnostics.Stopwatch();
            watch.Start();
#if DEBUG
            var count = debugCount;
#else
            var count = releaseCount;
#endif
            for (int i = 0; i < count; i++) {
                action(i);
            }
            watch.Stop();
            Console.WriteLine(string.Format("expected:{0}，actual:{1}。", time, watch.Elapsed));
            Assert.IsTrue((watch.Elapsed.Ticks - time.Ticks) < (time.Ticks * 0.1));
        }
    }
}
