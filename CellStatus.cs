using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesPuzzle
{
    /// <summary>
    /// Provides an enum value for a PuzzleCell struct property.
    /// Values:  Boom = -2,  Revealed,  Hidden, Suspected.
    /// </summary>
    public enum CellStatus
    {
        /*  Suspected can only change to hidden; 
        *  Clicking on a tile will reveal one or more tiles or maybe a boom from a mine.
        */
        Boom = -2,
        Revealed,
        Hidden,
        Suspected
    }
}
