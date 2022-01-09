using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesPuzzle
{/// <summary>
/// TODO:  Is this faster than LINQ?
/// TODO:  Rename file or delete?
/// </summary>
    class PuzzleCellsAlpha
    {
        #region  Events group
        public event EventHandler<PuzzleCellsEventArgs> UpdateGrid;

         void OnPuzzleCellsChanged ( int mines, List<PuzzleCell> cells )
        {
            var e = new PuzzleCellsEventArgs ()
            {
                Cells = cells,
                Mines = mines.ToString (),
                AllCellsRevealed = LastCellRevealed,
                HasBoom = MineWasRevealed,
            };
            //  object  sender, EventArgs  e
            UpdateGrid?.Invoke ( this, e );
        }


        //  End events group.
        #endregion


        #region  Private Fields
        //  *****          Private Fields          *****          *****          *****          *****          *****          *****          Private Fields          *****           *****
        private int _hiddenSafeCellsCount;
        private bool _haveBoom;
        private int _minesToClear;

        private List<PuzzleCell> _mineCellsList;

        private PuzzleCell [,] _puzzleCellArray;
        #endregion

        #region Properties
        //  *****          Properties          *****          *****          *****          *****          *****          Properties          *****          *****          *****          *****
        public bool LastCellRevealed
        {
            get
            {
                return ( _hiddenSafeCellsCount == 0 );
            }
        }


        public bool MineWasRevealed
        { get => _haveBoom; }


        public PuzzleCell [,] PuzzleCellArray
        { get => _puzzleCellArray; }
        #endregion


        #region  Constructor Method Group
        //  *****          Constructor          *****          *****          *****          *****          *****          *****          *****          *****          *****          *****
        public PuzzleCellsAlpha ( int rows = 10, int mines = 15 )
        {
            Constructor_InitializeVars ( rows, mines );
            Constructor_InitializeArray ( rows );
            Constructor_SetMines ( rows, mines );
        }

        private void Constructor_InitializeVars ( int rows, int mines )
        {
            _haveBoom = false;
            _hiddenSafeCellsCount = ( rows * rows ) - mines;
            _minesToClear = mines;

            // Just to suppress an Error List  info Message.
            _mineCellsList = new List<PuzzleCell> ();
        }

        private void Constructor_InitializeArray ( int rows )
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

                    _mineCellsList.Add ( _puzzleCellArray [randomRow, randomCol] );
                    newMines++;
                }
            } while ( newMines < mines );
        }

        //  end Constructor method group.
        #endregion  


        #region Public Methods
        //  *****       Public Methods        *****          *****          *****          *****          *****       Public Methods        *****          *****          *****  


        /// <summary>
        /// Updates UI counters for the new game; after handlers have been registered; .
        /// </summary>
        public void Ready ()
        {
            var _ = new List<PuzzleCell> ();
            OnPuzzleCellsChanged ( _minesToClear, _ );
        }



        #region  UpdateCells Public Method Group
        public void UpdateSelectedCell ( int row, int col )
        {
            var selectedCell = _puzzleCellArray [row, col];

            //  Only hidden cells are affected by this method.
            if ( selectedCell.CellStatus == CellStatus.Hidden )
            {
                var updatedCells = new List<PuzzleCell> ();

                switch ( selectedCell.CellValue )
                {

                    case CellValue.Mine:
                        UpdateSelectedCell_Mined ( selectedCell );
                        _haveBoom = true;
                        selectedCell.CellStatus = CellStatus.Boom;
                        updatedCells.Add ( selectedCell );
                        updatedCells.AddRange ( _mineCellsList );
                        break;


                    default:
                        _hiddenSafeCellsCount--;
                        if ( LastCellRevealed )
                        { _minesToClear = 0;   }

                        selectedCell.CellStatus = CellStatus.Revealed;
                        _puzzleCellArray [row, col] = selectedCell;
                        updatedCells.Add ( selectedCell );

                        if ( selectedCell.CellValue == 0 )
                        { updatedCells.AddRange ( Array_AdjacentCells ( row, col, AdjacentCellsTask.ZeroCellRevealed ) ); }

                        break;
                }

                OnPuzzleCellsChanged ( _minesToClear, updatedCells );
            }
        }


        private void UpdateSelectedCell_Mined ( PuzzleCell selectedCell )
        {
            var iRemove = 0;
            var m = 0;

            for ( var i = 0; i < _mineCellsList.Count; i++ )
            {
                var mineCell = _mineCellsList [i];
                //  Identify the selected cell from list of mined cells.
                if ( ( selectedCell.Col == _mineCellsList [i].Col ) && ( selectedCell.Row == _mineCellsList [i].Row ) )
                { iRemove = i; }

                //  Take note of mines that were correctly indentified.
                if ( _puzzleCellArray [mineCell.Row, mineCell.Col].CellStatus == CellStatus.Suspected )
                { m++; }

                else
                {
                    mineCell.CellStatus = CellStatus.Revealed;
                    _mineCellsList [i] = mineCell;
                }
                //TODO:  Currenty don't need to update the array; maybe with Dep Prop.
                //_puzzleCellArray [mineCell.Row, mineCell.Col] = mineCell;
            }

            _minesToClear = m - _mineCellsList.Count ();
            /*  Update the display with a negative number for mines unidentified; 
             *  and revealing the chosen cell as a mine. */
            _mineCellsList.RemoveAt ( iRemove );
        }

        #endregion


        public void ToggleCellStatusAndSusCellsCount ( int row, int col )
        {
            if ( ( _puzzleCellArray [row, col].CellStatus == CellStatus.Hidden ) || ( _puzzleCellArray [row, col].CellStatus == CellStatus.Suspected ) )
            {
                switch ( _puzzleCellArray [row, col].CellStatus )
                {
                    //  Only hidden and suspected cells are affected, ignoring all others.
                    case CellStatus.Hidden:
                        _puzzleCellArray [row, col].CellStatus = CellStatus.Suspected;
                        _minesToClear--;
                        break;

                    case CellStatus.Suspected:
                        _puzzleCellArray [row, col].CellStatus = CellStatus.Hidden;
                        _minesToClear++;
                        break;
                }

                var selectedCell = _puzzleCellArray [row, col];

                var updatedCells = new List<PuzzleCell> { selectedCell };
                OnPuzzleCellsChanged ( _minesToClear, updatedCells );
            }
        }
        //  End of Public Methods
        #endregion 



        #region  Private Helper Methods
        /// <summary>
        /// TODO:  REDO:  using IEnumerables, LINQ    **************                             *************************                           ********************                        ***********************                        **********************
        /// </summary>
        //  Updates CellValues next to mines or clears.
        enum AdjacentCellsTask { ZeroCellRevealed, PlacedMine }
        List<PuzzleCell> Array_AdjacentCells ( int row, int col, AdjacentCellsTask task )
        {
            var returnCellList = new List<PuzzleCell> ();
            var zeroCellList = new List<PuzzleCell> ();
            bool continueDoLoop;
            do
            {
                continueDoLoop = false;
                //  Evaluate valid array addresses.
                var rowMax = ( ( ( row + 2 ) <= _puzzleCellArray.GetLength ( 0 ) ) ? ( row + 2 ) : row + 1 );
                var colMax = ( ( col + 2 ) <= _puzzleCellArray.GetLength ( 1 ) ? ( col + 2 ) : col + 1 );
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
                        else if ( ( task == AdjacentCellsTask.ZeroCellRevealed )
                            && !( ( r == row ) && ( c == col ) ) )
                        { //  Return a list of newly revealed cells, adjacent to but not including the selected cell.

                            if ( _puzzleCellArray [r, c].CellStatus == CellStatus.Suspected )
                            {//  Ignore return, and cell is now hidden.
                                ToggleCellStatusAndSusCellsCount ( r, c );
                            }

                            if ( _puzzleCellArray [r, c].CellStatus == CellStatus.Hidden )
                            { //  Cell is now revealed and add to return list.
                                _puzzleCellArray [r, c].CellStatus = CellStatus.Revealed;
                                returnCellList.Add ( _puzzleCellArray [r, c] );
                                _hiddenSafeCellsCount--;

                                if ( _puzzleCellArray [r, c].CellValue == CellValue.Zero )
                                {//  Add another CellValue.Zero bonus reveal.
                                    zeroCellList.Add ( _puzzleCellArray [r, c] );
                                }
                            }
                        }
                        #endregion//  End ZeroCellRevealed
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
        //  End of Array_AdjacentCells.
        #endregion  //  End Private Helper Methods



    }
}
