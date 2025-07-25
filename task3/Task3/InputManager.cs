using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Task3
{
    internal class InputManager
    {

        public int stringToInt(string value)
        {

            try
            {
                return Convert.ToInt32(value);
            }
            catch(FormatException ex)
            {
                return -1;
            }

        }

        public int[] convertInput(string input)
        {

            int[] result = new int[6];

            int index = 0;

            string buffer = "";

            for (int i = 0; i < input.Length; i++)
            {

                if (index > 5)
                {
                    throw new Exception("Invalid arguments. Please restart game with correct arguments.");
                }

                if (input[i] != ',' && i != input.Length-1)
                {
                    buffer += input[i];
                }
                else
                {
                    if (i == input.Length-1)
                    {
                        buffer += input[i];
                    }

                    result[index] = stringToInt(buffer);

                    if (result[index] == -1)
                    {

                        throw new Exception("Invalid arguments. Please restart game with correct arguments.");

                    }

                    index++;
                    buffer = "";

                }
            }

            if (index < 5)
            {
                throw new Exception("Invalid arguments. Please restart game with correct arguments.");
            }

            return result;

        }



    }
}
