using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task3
{
    internal static class Output
    {

        public static void OutputByteArray(byte[] array)
        {
            foreach (byte b in array)
            {
                Console.Write($"{b:X2}");
            }
        }
    }
}
