using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesPuzzle
{
    internal struct PuzzleCell
    {
        public int Row { get; }
        public int Col { get; }
        public CellValue CellValue { get; }
        public CellStatus CellStatus { get; set; }

        public PuzzleCell ( int row, int col, CellValue value, CellStatus status )
        {
            Row = row;
            Col = col;
            CellValue = value;
            CellStatus = status;
        }



    }
}
