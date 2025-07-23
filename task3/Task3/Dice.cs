using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task3
{
    internal class Dice
    {

        private int[] faces;

        public Dice(int[] faces) {
        
            this.faces = faces;

        }


        public int getFace(int index)
        {
            return faces[index];
        }

    }
}
