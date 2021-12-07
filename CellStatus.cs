using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesPuzzle
{
    /// <summary>
    /// Provides an enum value for a PuzzleCell struct property.
    /// Values from -2 to 1.
    /// </summary>
    public enum CellStatus
    {
        /*  Boom would be revealed, suspected has to be hidden; 
        *  clicking on a tile will reveal one or more tiles and maybe a boom from a mine.
        */
        Boom = -2,
        Revealed,
        Hidden,
        Suspected
    }
}
