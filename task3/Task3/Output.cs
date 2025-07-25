using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

        public static void OutputByteArrayWithMessages(string message1, string message2, byte[] array)
        {

            Console.Write(message1);
            OutputByteArray(array);
            Console.WriteLine(message2);

        }

        public static void OutputResult(int num1, int num2)
        {

            Console.WriteLine($"The fair number generation result is {num1} + {num2} = {(num1+num2)%6} (mod 6).");

        }

        public static void OutputComputerWin(int numUser, int numComputer)
        {

            Console.WriteLine($"I win {numComputer} > {numUser}");

        }

        public static void OutputUserWin(int numUser, int numComputer)
        {

            Console.WriteLine($"You win {numUser} > {numComputer}");

        }

        public static void OutputDraw(int numUser, int numComputer)
        {

            Console.WriteLine($"Draw {numComputer} = {numUser}");

        }
    }
}
