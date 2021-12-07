using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesPuzzle
{
    /// <summary>
    ///  Provides an enum value for a PuzzleCell stuct property.
    ///  Values from -1 (is a mine) to 8 (number of mines adjacent).
    /// </summary>
    public enum CellValue
    {
        //  Number of mines adjacent to cell;
        //  or if mine.
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
