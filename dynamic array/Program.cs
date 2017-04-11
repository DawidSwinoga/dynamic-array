using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace dynamic_array
{
    class Program
    {
        static void Main(string[] args)
        {
            DynamicArray dynamicArray = new DynamicArray();
            dynamicArray.ItemAdded += (addedItem, currentSize) => Console.WriteLine("Added: " + addedItem + "\t current array size: " + currentSize);
            dynamicArray.ArrayResized += size => Console.WriteLine("Current array size: " + size);
            for (int i = 0; i < 39; i++)
            {
                dynamicArray.Add(5);
            }

            try
            {
                int a = dynamicArray[200];
            }
            catch (IndexOutOfRangeException e)
            {
                Console.WriteLine(e.Message);
            }
            dynamicArray[100] = 10;
            Console.WriteLine(dynamicArray[99]);

            Console.ReadKey();
        }
    }
}
