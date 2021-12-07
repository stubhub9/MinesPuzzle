using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesPuzzle
{
    class PuzzleCellsA
    {
        #region  Events group
        //  Delegate for PuzzleCells 

        public delegate void UpdateHandler ( string updateString );
        public event UpdateHandler UpdateTilesTillClear; 

        #endregion


        #region  Private Fields
        //  *****          Private Fields          *****          *****          *****          *****          *****          *****          *****          *****          *****          *****
        private int _susCellsCount;
        private int _hiddenCellsCount;

        private List<PuzzleCell> _mineCellsList;

        private PuzzleCell [,] _puzzleCellArray;
        #endregion

        //#region  Properties
        ////  *****          Properties          *****          *****          *****          *****          *****          *****          *****          *****          *****          *****
        //public List<PuzzleCell> MineList 
        //{ get => _mineCellsList; }
        //#endregion

        #region  Constructor Method Group
        //  *****          Constructor          *****          *****          *****          *****          *****          *****          *****          *****          *****          *****
        public PuzzleCellsA ( int rows = 10, int mines = 15 )
        {
            Constructor_InitializeVars ( rows, mines );
            Constructor_BuildArray ( rows, mines );
            Constructor_SetMines ( rows, mines );

        }

        private void Constructor_InitializeVars ( int rows, int mines )
        {
            _hiddenCellsCount = ( rows * rows ) - mines;
            _susCellsCount = 0;
        }

        private void Constructor_BuildArray ( int rows, int mines )
        {
            _puzzleCellArray = new PuzzleCell [rows, rows];
            for ( int r = 0; r < rows; r++ )
            {
                for ( int c = 0; c < rows; c++ )
                {
                    //  Set the row and column values for each cell in the new array.
                    _puzzleCellArray [r, c].Col = c;
                    _puzzleCellArray [r, c].Row = r;
                }
            }
        }


        private void Constructor_SetMines ( int rows, int mines )
        {
            var random = new Random ();
            var newMines = 0;
            do
            {
                var randomRow = random.Next ( rows );
                var randomCol = random.Next ( rows );

                //  Randomly pick the new mine locations.
                if ( _puzzleCellArray [randomRow, randomCol].CellValue != CellValue.Mine )
                {
                    _puzzleCellArray [randomRow, randomCol].CellValue = CellValue.Mine;
                    //  Update the CellValue of adjacent cells.
                    Array_AdjacentCells ( randomRow, randomCol, AdjacentCellsTask.PlacedMine );

                    newMines++;
                }
            } while ( newMines < mines );
        }

        //  end Constructor method group.
        #endregion

        //  Updates CellValues next to mines or clears.
        private enum AdjacentCellsTask { RevealedClearCell, PlacedMine }
        private void Array_AdjacentCells ( int row, int col, AdjacentCellsTask task )
        {
            //  Evaluate valid array addresses.
            var rowMax = ( ( row + 1 ) <= _puzzleCellArray.GetLength ( 0 ) ? ( row + 1 ) : row );
            var colMax = ( ( col + 1 ) <= _puzzleCellArray.GetLength ( 1 ) ? ( col + 1 ) : col );
            var rowMin = ( ( row - 1 ) >= 0 ? ( row - 1 ) : row );
            var colMin = ( ( col - 1 ) >= 0 ? ( col - 1 ) : col );

            if ( task == AdjacentCellsTask.PlacedMine )
            {
                //_puzzleCellArray [row, col].CellValue = CellValue.Mine;  //  redundant?
                for ( int r = rowMin; r < rowMax; r++ )
                {
                    for ( int c = colMin; c < colMax; c++ )
                    {
                        if ( _puzzleCellArray [r, c].CellValue != CellValue.Mine )
                        {
                            //  Adjacent cells are now adjacent to an additional mine.
                            _puzzleCellArray [r, c].CellValue++;
                        }
                    }
                }
            }

            else if ( task == AdjacentCellsTask.RevealedClearCell )
            {
                //  Update adjacent cells of a CellValue.ZeroRevealed event to CellStatus. Revealed
                //  Supply adjacent cells of a CellValue.ZeroRevealed event to the UpdateList.
            }

        }


        public List<PuzzleCell> UpdateSelectedCell ( int row, int col )
        {
            var updatedCells = new List<PuzzleCell> ();



            return updatedCells;
        }


        public List<PuzzleCell> ToggleCellStatusAndSusCellsCount ( int row, int col )
        {
            var updatedCells = new List<PuzzleCell> ();

            var toggledCell = _puzzleCellArray [row, col];
            switch ( toggledCell.CellStatus )
            {
                //  Only hidden and suspected cells are affected, ignoring all others.
                case CellStatus.Hidden:
                    toggledCell.CellStatus = CellStatus.Suspected;
                    //_suspectedCellsCountdown--;
                    break;

                case CellStatus.Suspected:
                    toggledCell.CellStatus = CellStatus.Hidden;
                    //_suspectedCellsCountdown++;
                    break;
            }
            //  Returns an updated cell for the button's tag.
            //return toggledCell;


            return updatedCells;
        }




    }
}
