using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesPuzzle
{
    /// <summary>
    ///  Provides data for PuzzleGrid button tags and the PuzzleLogic matrix.
    ///  ___int Row, Col,
    ///  ___enum CellValue, CellStatus
    /// </summary>
    public struct PuzzleCell
    {
        public int Row { get; set; }
        public int Col { get; set; }
        internal CellValue CellValue { get; set; }
        public CellStatus CellStatus { get; set; }

        //  Constructors  ******************************************************************************

        //  Used for mine placement.
        internal PuzzleCell ( int row, int col, CellValue value, CellStatus status )
        {
            Row = row;
            Col = col;
            CellValue = value;
            CellStatus = status;
        }

        //// Used for updating the CellValue for the number of mines adjacent.
        //public PuzzleCell ( int row, int col, int cellValue, CellStatus status )
        //{
        //    Row = row;
        //    Col = col;
        //    CellValue = (CellValue) cellValue;
        //    CellStatus = status;
        //}

    }
}
