using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Task3
{
    internal class GameController
    {

        private Dice[] dices;

        private Menu menu = new Menu();

        private InputManager inputManager = new InputManager();

        private string[]? args;

        private int userIndex = -1;
        private int computerIndex = -1;
        public GameController(string[] args)
        {

            this.args = args;

        }

        public void Start()
        {

            if (args.Length == 0)
            {
                menu.menuNullArgs();
                return;
            }

            dices = new Dice[args.Length];

            try
            {
                for (int i = 0; i < args.Length; i++)
                {

                    dices[i] = new Dice(inputManager.convertInput(args[i]));

                }

            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);

                menu.menuIncorrectArgs();
                return;

            }

            FirstMove();        

        }


        private void FirstMove()
        {

            Console.WriteLine("Let's determine who makes the first move.");
            Console.WriteLine("I selected a random value in the range 0..1 ");

            int randomValue = Random.RandomValueInt32(1);
            byte[] key = Random.RandomKey256();
            byte[] hmac = Random.HmacSHA256(randomValue, key);

            Console.Write($"(HMAC=");
            Output.OutputByteArray(hmac);
            Console.WriteLine(")");

            string input = menu.menuFirstMove();

            if (input == "X")
            {
                return;
            }

            if (input == "?")
            {
            }
            
            Console.WriteLine($"My selection: {randomValue}");

            Console.Write($"(KEY=");
            Output.OutputByteArray(key);
            Console.WriteLine(")");

            int userValue = inputManager.stringToInt(input);

            if (userValue == randomValue)
            {

                Console.WriteLine($"You make the first move and your choose ...");

                while (true)
                {
                    input = menu.menuChoiseDice(args, computerIndex);

                    if (input == "X")
                    {
                        return;
                    }

                    if (input == "?")
                    {
                    }
                    else
                    {
                        userIndex = inputManager.stringToInt(input);
                        break;
                    }

                }

                Console.WriteLine($"Your choose {args[userIndex]}");

                computerIndex = Random.RandomValueInt32(args.Length - 1);

                if (computerIndex >= userIndex)
                {
                    computerIndex++;
                }

                Console.WriteLine($"I make the first move and choose the [{args[computerIndex]}] dice.");

            }
            else
            {
                computerIndex = Random.RandomValueInt32(args.Length - 1);

                Console.WriteLine($"I make the first move and choose the [{args[computerIndex]}] dice.");

                while(true)
                {
                    input = menu.menuChoiseDice(args, computerIndex);

                    if (input == "X")
                    {
                        return;
                    }

                    if (input == "?")
                    {
                    }
                    else
                    {
                        userIndex = inputManager.stringToInt(input);
                        break;
                    }
                    
                }

                Console.WriteLine($"Your choose {args[userIndex]}");
            }

            SecondMove();
        }

        
        private void SecondMove()
        {

        }
    }
}
