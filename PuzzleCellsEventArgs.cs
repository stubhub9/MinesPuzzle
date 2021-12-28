using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesPuzzle
{
    class PuzzleCellsEventArgs
    {
        public List<PuzzleCell> Cells { get; set; }
        public string Mines { get; set; }
        public bool HasBoom { get; set; }
        public bool AllCellsRevealed { get; set; }
    }
}
