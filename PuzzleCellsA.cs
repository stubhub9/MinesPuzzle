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

        public delegate void UpdateHandler ( string updateString, PuzzleCell updatedCell );
        public event UpdateHandler UpdateTilesTillClear;
        //  Update the UI Hidden Tiles Counter with a delegate/ event combo.
        //TODO:?  Could add a List <PuzzleCell> for result returns; Tile returns. 

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
            Constructor_InitializeArray ( rows, mines );
            Constructor_SetMines ( rows, mines );

        }

        private void Constructor_InitializeVars ( int rows, int mines )
        {
            _hiddenCellsCount = ( rows * rows ) - mines;
            _susCellsCount = ( rows * rows );
        }

        private void Constructor_InitializeArray ( int rows, int mines )
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
                    //  Ignored the return; could have assigned a discard?
                    newMines++;
                }
            } while ( newMines < mines );
        }

        //  end Constructor method group.
        #endregion

        //  Updates CellValues next to mines or clears.
        private enum AdjacentCellsTask { ClearCellRevealed, PlacedMine }
        private List<PuzzleCell> Array_AdjacentCells ( int row, int col, AdjacentCellsTask task )
        {
            var returnCellList = new List<PuzzleCell> ();
            var zeroCellList = new List<PuzzleCell> ();
            bool continueDoLoop;
            do
            {
                continueDoLoop = false;
                //  Evaluate valid array addresses.
                var rowMax = ( ( row + 1 ) <= _puzzleCellArray.GetLength ( 0 ) ? ( row + 1 ) : row );
                var colMax = ( ( col + 1 ) <= _puzzleCellArray.GetLength ( 1 ) ? ( col + 1 ) : col );
                var rowMin = ( ( row - 1 ) >= 0 ? ( row - 1 ) : row );
                var colMin = ( ( col - 1 ) >= 0 ? ( col - 1 ) : col );

                #region  r & c loops
                for ( int r = rowMin; r < rowMax; r++ )
                {
                    for ( int c = colMin; c < colMax; c++ )
                    {


                        if ( task == AdjacentCellsTask.PlacedMine )
                        {
                            if ( _puzzleCellArray [r, c].CellValue != CellValue.Mine )
                            {
                                //  Adjacent cells are now adjacent to an additional mine.
                                _puzzleCellArray [r, c].CellValue++;
                            }  //  Returns an unused, empty List <PuzzleCell>.
                        }

                        #region CellValueZero Task
                        else if ( ( task == AdjacentCellsTask.ClearCellRevealed ) && ( ( r != row ) && ( c != col ) ) )
                        { //  Return a list of newly revealed cells, adjacent to but not including the selected cell.

                            if ( _puzzleCellArray [r, c].CellStatus == CellStatus.Suspected )
                            {//  Ignore return, and cell is now hidden.
                                ToggleCellStatusAndSusCellsCount ( r, c );
                            }

                            if ( _puzzleCellArray [r, c].CellStatus == CellStatus.Hidden )
                            { //  Cell is now revealed and add to return list.
                                _puzzleCellArray [r, c].CellStatus = CellStatus.Revealed;
                                returnCellList.Add ( _puzzleCellArray [r, c] );

                                if ( _puzzleCellArray [r, c].CellValue == CellValue.Zero )
                                {//  Add another CellValue.Zero bonus reveal.
                                    zeroCellList.Add ( _puzzleCellArray [r, c] );
                                }
                            }
                        }
                        #endregion//  End ClearCellRevealed
                    }
                }
                #endregion//  End of r,c loops.

                if ( zeroCellList.Count != 0 )
                {
                    row = zeroCellList [0].Row;
                    col = zeroCellList [0].Col;
                    zeroCellList.RemoveAt ( 0 );
                    continueDoLoop = true;
                }
            } while ( continueDoLoop );
            return returnCellList;
        }


        public List<PuzzleCell> UpdateSelectedCell ( int row, int col )
        {
            if ( _puzzleCellArray [row, col].CellStatus == CellStatus.Hidden )
            {

                switch ( _puzzleCellArray [row, col].CellValue )
                {
                    case CellValue.Mine:
                        // Proceed to game lost.

                        break;
                    case CellValue.Zero:
                        //  Proceed to revealed cell with CellValue.Zero task.

                        break;
                    default:
                        //  Reveal this cell.

                        break;
                }

            }

            var updatedCells = new List<PuzzleCell> ();

            return updatedCells;
        }


        public List<PuzzleCell> ToggleCellStatusAndSusCellsCount ( int row, int col )
        {

            //var toggledCell = _puzzleCellArray [row, col];
            //switch ( toggledCell.CellStatus )
            switch ( _puzzleCellArray [row, col].CellStatus )
            {
                //  Only hidden and suspected cells are affected, ignoring all others.
                case CellStatus.Hidden:
                    _puzzleCellArray [row, col].CellStatus = CellStatus.Suspected;
                    _susCellsCount--;
                    break;

                case CellStatus.Suspected:
                    _puzzleCellArray [row, col].CellStatus = CellStatus.Hidden;
                    _susCellsCount++;
                    break;
            }
            //  Update the MainWindow display and the right clicked tile; with an event.
            UpdateTilesTillClear?.Invoke ( _susCellsCount.ToString (), _puzzleCellArray [row, col] );
            //  Returns an updated cell for the button's tag.
            var updatedCells = new List<PuzzleCell> ();
            updatedCells.Add ( _puzzleCellArray [row, col] );
            return updatedCells;
        }




    }
}
