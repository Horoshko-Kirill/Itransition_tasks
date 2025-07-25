using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleTables;

namespace Task3
{
    internal static class TableProbability
    {

        public static void PrintTableProbability(double[,] proability, string[] args)
        {


            List<string> headers = new List<string> { "user" };
            headers.AddRange(args);

            ConsoleTable table = new ConsoleTable(headers.ToArray());

            for (int i = 0; i < proability.GetLength(0); i++)
            {
                List<object> row = new List<object> { args[i] };

                for (int j = 0;  j < proability.GetLength(1); j++)
                {
                    row.Add(proability[i, j]);
                }

                table.AddRow(row.ToArray());
            }


            table.Write();
        }

    }
}
