using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task3
{
    internal class Probability
    {

        public double[,] probability;


        public Probability(int countDices)
        {
            probability = new double[countDices, countDices];
        }


        public void probavilityCalculate(Dice[] dices)
        {
            for (int i = 0; i < dices.Length; i++)
            {

                for (int j = 0; j < dices.Length; j++)
                {

                    double countMore = 0;

                    for (int k = 0; k < 6; k++)
                    {


                        for (int l = 0; l < 6; l++)
                        {

                            if (dices[i].getFace(k) > dices[j].getFace(l))
                            {
                                countMore++;
                            }

                        }

                    }

                    probability[i,j] = countMore/36;

                }

            }
        }
    }
}
