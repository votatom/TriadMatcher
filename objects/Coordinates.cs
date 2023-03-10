using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triad_Matcher
{
    public class Coordinates
    {
        public int row;
        public int col;

        public bool Compare(Coordinates secondCoord)
        {
            int smallerRow;
            int smallerCol;
            int biggerRow;
            int biggerCol;
            if (secondCoord.row <= this.row)
            {
                smallerRow = secondCoord.row;
                biggerRow = this.row;
            }
            else
            {
                smallerRow = this.row;
                biggerRow = secondCoord.row;
            }
            if(secondCoord.col <= this.col)
            {
                smallerCol = secondCoord.col;
                biggerCol = this.col;
            }
            else
            {
                smallerCol = this.col;
                biggerCol = secondCoord.col;
            }
            if(biggerRow - smallerRow == 1 && biggerCol - smallerCol == 0 || biggerRow - smallerRow == 0 && biggerCol - smallerCol == 1)
            {
                return true;
            }
            return false;
        }

        public bool IsSame(Coordinates second)
        {
            if (this.col == second.col && this.row == second.row)
            {
                return true;
            }
            return false;
    }
    }

    
}
