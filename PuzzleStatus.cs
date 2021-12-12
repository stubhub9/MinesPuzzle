using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesPuzzle
{
//TODO:  ? Add user options for GamePaused ?
    //      GamePaused should be triggered by window minimized, lost focus (?user option?)
    public enum PuzzleStatus
    {
        GameDefeat = -1,
        GamePaused,
        GameNew,
        GameOn,
        GameVictory
    }
}
