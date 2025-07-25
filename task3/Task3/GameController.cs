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

        private Probability probabilityManager;

        private int userIndexDice = -1;
        private int computerIndexDice = -1;
        private int userIndexFace = -1;
        private int computerIndexFace = -1;
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
            probabilityManager = new Probability(args.Length);

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

            probabilityManager.probavilityCalculate(dices);

            RootMove();        

        }


        private void RootMove()
        {

            Console.WriteLine("Let's determine who makes the first move.");
            Console.WriteLine("I selected a random value in the range 0..1 ");

            int randomValue = Random.RandomValueInt32(2);
            byte[] key = Random.RandomKey256();
            byte[] hmac = Random.HmacSHA256(randomValue, key);

            Output.OutputByteArrayWithMessages("(HMAC=", ")", hmac);

            string input = "";

            while (true)
            {

                input = menu.menuFirstMove();

                if (input == "X")
                {
                    return;
                }

                if (input == "?")
                {
                    TableProbability.PrintTableProbability(probabilityManager.probability, args);
                }
                else
                {
                    break;
                }

            }
            
            Console.WriteLine($"My selection: {randomValue}");

            Output.OutputByteArrayWithMessages("(KEY=", ")", key);

            int userValue = inputManager.stringToInt(input);

            if (userValue == randomValue)
            {

                Console.WriteLine($"You make the first move and your choose ...");

                while (true)
                {
                    input = menu.menuChoiseDice(args, computerIndexDice);

                    if (input == "X")
                    {
                        System.Environment.Exit(2);
                    }

                    if (input == "?")
                    {
                        TableProbability.PrintTableProbability(probabilityManager.probability, args);
                    }
                    else
                    {
                        userIndexDice = inputManager.stringToInt(input);
                        break;
                    }

                }

                Console.WriteLine($"Your choose [{args[userIndexDice]}]");

                computerIndexDice = Random.RandomValueInt32(args.Length-1);

                if (computerIndexDice >= userIndexDice)
                {
                    computerIndexDice++;
                }

                Console.WriteLine($"I make the first move and choose the [{args[computerIndexDice]}] dice.");

                ComputerChoiceDice();
                UserChoiceDice();
            }
            else
            {
                computerIndexDice = Random.RandomValueInt32(args.Length);

                Console.WriteLine($"I make the first move and choose the [{args[computerIndexDice]}] dice.");

                while(true)
                {
                    input = menu.menuChoiseDice(args, computerIndexDice);

                    if (input == "X")
                    {
                        System.Environment.Exit(2);
                    }

                    if (input == "?")
                    {
                        TableProbability.PrintTableProbability(probabilityManager.probability, args);
                    }
                    else
                    {
                        userIndexDice = inputManager.stringToInt(input);
                        break;
                    }
                    
                }

                if (userIndexDice >= computerIndexDice)
                {
                    userIndexDice++;
                }

                Console.WriteLine($"Your choose [{args[userIndexDice]}]");

                ComputerChoiceDice();
                UserChoiceDice();
            }

            LastMove();
        }


        private void SecondMove(out int numComputer, out int numUser)
        {
            numComputer = 0;
            numUser = 0;

            Console.WriteLine("I selected a random value in the range 0..5");

            int randomValue = Random.RandomValueInt32(6);
            byte[] key = Random.RandomKey256();
            byte[] hmac = Random.HmacSHA256(randomValue, key);

            Output.OutputByteArrayWithMessages("(HMAC=", ")", hmac);

            Console.WriteLine("And your number modulo 6.");

            string input = "";

            while (true)
            {

                input = menu.menuSecondMove();

                if (input == "X")
                {
                    System.Environment.Exit(2);
                }

                if (input == "?")
                {
                    TableProbability.PrintTableProbability(probabilityManager.probability, args);
                }
                else
                {
                    break;
                }

            }

            Console.WriteLine($"My selection: {randomValue}");

            Output.OutputByteArrayWithMessages("(KEY=", ")", key);

            numComputer = randomValue;
            numUser = inputManager.stringToInt(input);
        }
        
        private void ComputerChoiceDice()
        {

            Console.WriteLine("It's time for my roll.");

            int numComputer = 0;
            int numUser = 0;

            SecondMove(out numUser, out numComputer);            

            Output.OutputResult(numComputer, numUser);

            computerIndexFace = (numUser + numComputer)%6;

            Console.WriteLine($"My roll result is {dices[computerIndexDice].getFace(computerIndexFace)}");
        }


        private void UserChoiceDice()
        {

            Console.WriteLine("It's time for your roll.");

            int numComputer = 0;
            int numUser = 0;

            SecondMove(out numUser, out numComputer);

            Output.OutputResult(numComputer, numUser);

            userIndexFace = (numUser + numComputer) % 6;

            Console.WriteLine($"Your roll result is {dices[userIndexDice].getFace(userIndexFace)}");
        }

        private void LastMove()
        {

            int numUser =  dices[userIndexDice].getFace(userIndexFace);
            int numComputer = dices[computerIndexDice].getFace(computerIndexFace); 


            if (numUser == numComputer)
            {
                Output.OutputDraw(numUser, numComputer);
            }
            else
            {
                if (numComputer > numUser)
                {
                    Output.OutputComputerWin(numUser, numComputer);
                }
                else
                {
                    Output.OutputUserWin(numUser, numComputer);
                }
            }


            Console.WriteLine("Do you want to repeat?");

            string input = menu.menuRepeat();

            if (input == "X")
            {
                System.Environment.Exit(2);
            }

            RootMove();
        }
    }
}
