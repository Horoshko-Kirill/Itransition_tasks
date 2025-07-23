using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Task3
{
    internal class Menu
    {

        public void menuIncorrectArgs()
        {

            while (true)
            {

                Console.WriteLine("X - exit");
                Console.Write("Your selection: ");

                string? input = Console.ReadLine();

                if (input != "X")
                {
                    Console.WriteLine("Incorrect input. Please try again");
                }
                else
                {
                    break;
                }

            }
        }


        public void menuNullArgs()
        {

            while (true)
            {

                Console.WriteLine("No arguments. Please restart game with arguments.");
                Console.WriteLine("X - exit");
                Console.Write("Your selection: ");

                string? input = Console.ReadLine();

                if (input != "X")
                {
                    Console.WriteLine("Incorrect input. Please try again");
                }
                else
                {
                    break;
                }

            }
        }

        public string menuFirstMove()
        {
            string? input;

            while (true)
            {

                Console.WriteLine("Try to guess my selection");
                Console.WriteLine("0 - 0");
                Console.WriteLine("1 - 1");
                Console.WriteLine("X - exit");
                Console.WriteLine("? - help");

                Console.Write("Your selection: ");

                input = Console.ReadLine();

                if ( input != "X" && input != "0" && input != "1" && input != "?")
                {
                    Console.WriteLine("Incorrect input. Please try again");
                }
                else
                {
                    break;
                }

            }

            return input;
        }


        public string menuChoiseDice(string[] args, int computeIndex)
        {

            string? input;

            InputManager inputManager = new InputManager();

            while (true)
            {


                if (computeIndex < 0)
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        Console.WriteLine($"{i} - {args[i]}");
                    }
                }
                else
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        if (i != computeIndex)
                        {

                            if (i < computeIndex)
                            {
                                Console.WriteLine($"{i} - {args[i]}");
                            }
                            else
                            {
                                Console.WriteLine($"{i - 1} - {args[i]}");
                            }
                        }

                    }
                }

                Console.WriteLine("X - exit");
                Console.WriteLine("? - help");

                Console.Write("Your selection: ");

                input = Console.ReadLine();

                if (input != "X" && input != "?")
                {

                    int num = inputManager.stringToInt(input);

                    if (num < 0 && (computeIndex == -1 && num > args.Length - 1 || computeIndex != -1 && num > args.Length - 2))
                    {
                        Console.WriteLine("Incorrect input. Please try again");
                    }
                    else
                    {
                        break;
                    }
                    
                }
                else
                {
                    break;
                }

            }

            return input;
        }
    }
}
