using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesPuzzle
{
    internal enum CellValue
    {
        //  Number of mines adjacent to cell;
        //  or if mine, or boom.
        //Boom = -2,
        Mine = -1,
        Zero,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight
    }
}
