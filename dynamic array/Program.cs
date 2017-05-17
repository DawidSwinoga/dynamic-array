using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace dynamic_array
{
    class Program
    {
        static void Main(string[] args)
        {
            DynamicArray dynamicArray = new DynamicArray();
            List<Task> tasks = new List<Task>();

            int threadCount = 10;
            int iterationPerThread = 10;

            for (int i = 0; i < threadCount; ++i)
            {
                tasks.Add(Task.Run(() =>
                        {
                            for (int j = 0; j < iterationPerThread; ++j)
                            {
                                if (!dynamicArray.TryAdd(Thread.CurrentThread.ManagedThreadId))
                                    Console.WriteLine("Cannot add data to array.");
                                var watch = Stopwatch.StartNew();
                                dynamicArray.BlockingAdd(Thread.CurrentThread.ManagedThreadId);
                                watch.Stop();
                                Console.WriteLine("Execution time: " + watch.ElapsedMilliseconds + " miliseconds");
                            }
                        }
                    ))
                    ;
            }

            Task.WaitAll(tasks.ToArray());
            dynamicArray.SaveToFile("test.txt");
            Console.WriteLine("Press any key to finish.");
            Console.ReadKey();
        }
    }
}