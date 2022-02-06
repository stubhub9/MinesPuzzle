using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesPuzzle
{
    /// <summary>
    /// IQuerable 1D List of sequential full rows.
    /// </summary>
    class PuzzleCellsA
    //class PuzzleCellsA : IEnumerable<PuzzleCell>
    {

        #region  Events group
        //Action<List<PuzzleCell>, string, bool, bool> updateGridAction; 


        public event EventHandler<PuzzleCellsEventArgs> UpdatePuzzleGridEvent;

        void OnPuzzleCellsChanged ( int mines, List<PuzzleCell> cells )
        {
            var e = new PuzzleCellsEventArgs ()
            {
                Cells = cells,
                Mines = mines.ToString (),
                AllCellsRevealed = ( _hiddenSafeCellsCount == 0 ),
                HasBoom = _haveBoom,
            };
            //  object  sender, EventArgs  e
            UpdatePuzzleGridEvent?.Invoke ( this, e );
        }

        //  End events group.
        #endregion


        #region  Private Fields
        //  *****          Private Fields          *****          *****          *****          *****          *****          *****          Private Fields          *****           *****
        //  Property fields

        //  Game won when zero.
        private int _hiddenSafeCellsCount;

        // Game lost when true.
        private bool _haveBoom;

        //  List of all PuzzleCells, ordered by a sequential list of full rows.
        List<PuzzleCell> _puzzleCellsList;

        //  Equals number of mines minus number of suspected cells; updates UI.
        private int _suspectedMinesCount;

        //  Determined by constructor params; don't need more params.
        int _numberOfRows;
        int _numberOfMines;

        //  Only used for mine placement, but want to restrict new instantances.
        Random _random;


        #endregion

        #region Properties
        //  *****          Properties          *****          *****          *****          *****          *****          Properties          *****          *****          *****          *****

        public bool AllCellsRevealed
        {
            get
            {
                return ( _hiddenSafeCellsCount == 0 );
            }
        }


        #endregion


        #region  Constructor Method Group
        //  *****          Constructor          *****          *****          *****          *****          *****          *****          *****          *****          *****          *****
        public PuzzleCellsA ( int rows = 10, int mines = 15 )
        {
            Constructor_InitializeVars ( rows, mines );
            Constructor_InitializeList ();
            Constructor_SetMines_List ();
        }

        private void Constructor_InitializeVars ( int rows, int mines )
        {
            _haveBoom = false;
            _hiddenSafeCellsCount = ( rows * rows ) - mines;
            _suspectedMinesCount = mines;
            _numberOfRows = rows;
            _numberOfMines = mines;
            _random = new Random ();
        }


        //TODO: InitList; add async??
        private void Constructor_InitializeList ()
        {
            _puzzleCellsList.Clear ();
            for ( int row = 0; row < _numberOfRows; row++ )
            {
                for ( int col = 0; col < _numberOfRows; col++ )
                {
                    //  Set the row and column values for each cell in the new list.
                    var newCell = new PuzzleCell ()
                    {
                        Col = col,
                        Row = row,
                        /* Default CellStatus and Value*/
                    };

                    _puzzleCellsList.Add ( newCell );
                }
            }
        }


        void Constructor_SetMines_List ()
        {
            var newMines = 0;
            do
            {
                //  Add a mine to _puzzleCellsList; increment adjacent values.
                var cell = Constructor_SetMines_SetMine ();
                //var list = FindAdjacentCells_puzzleCellsList ( cell.Row, cell.Col );
                var list = FindAdjacentCells_puzzleCellsList ( cell.Row, cell.Col );
                foreach ( var item in list )
                {
                    cell = item;
                    if ( cell.CellValue != CellValue.Mine )
                    { cell.CellValue++; }
                    _puzzleCellsList [GetListIndexByRowCol ( cell.Row, cell.Col )] = cell;
                }

                newMines++;
            } while ( newMines < _numberOfMines );
        }

        /// <summary>
        /// Updates the PuzzleCells List with the returned mined cell.
        /// </summary>
        /// <returns></returns>
        PuzzleCell Constructor_SetMines_SetMine ()
        {
            PuzzleCell cell;
            var taskIsNotComplete = true;
            do
            {
                var row = _random.Next ( _numberOfRows );
                var col = _random.Next ( _numberOfRows );
                var cellIndex = GetListIndexByRowCol ( row, col );
                cell = _puzzleCellsList [cellIndex];

                if ( cell.CellValue != CellValue.Mine )
                {
                    cell.CellValue = CellValue.Mine;
                    _puzzleCellsList [cellIndex] = cell;
                    taskIsNotComplete = false;
                }
            } while ( taskIsNotComplete );
            return cell;
        }



        //  end Constructor method group.
        #endregion


        //   Use as an indexer/ enumerator for the _puzzleCellsList.
        int GetListIndexByRowCol ( int row, int col )
        {
            int indexRC = ( row * _numberOfRows ) + col;
            //TEST  ASSERT:  The cell at index must equal row and column.
            if ( _puzzleCellsList [indexRC].Col != col )
            {
                throw new NotImplementedException ( "Bad Math :(" );
            }
            return indexRC;
        }


        #region Public Methods
        //  *****       Public Methods        *****          *****          *****          *****          *****       Public Methods        *****          *****          *****  

        /// <summary>
        /// Updates UI counters for the new game; after handlers have been registered; .
        /// ?? IReady  ??chained, tracked and boolled??
        /// </summary>
        public void Ready ()
        {
            var _1 = new List<PuzzleCell> ();
            OnPuzzleCellsChanged ( _suspectedMinesCount, _1 );
        }


        /// <summary>
        ///TODO:  RENAME
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void UpdateSelectedCell ( int row, int col )
        {
            var indexSelected = GetListIndexByRowCol ( row, col );
            var selectedCell = _puzzleCellsList [indexSelected];

            //  Provides updates to the UI.
            var updatedCells = new List<PuzzleCell> ();

            if ( selectedCell.CellStatus != CellStatus.Hidden )
            { return; }

            if ( selectedCell.CellValue == CellValue.Mine )
            { UpdateSelectedCell_Mine ( row, col ); }

            else
            {
                updatedCells.Add (
                    UpdateSelectedCell_RevealCell ( selectedCell ) );

                if ( selectedCell.CellValue == 0 )
                { updatedCells.AddRange ( UpdateSelectedCell_ZeroCell ( row, col ) ); }

                if ( AllCellsRevealed )
                { _suspectedMinesCount = 0; }
            }
        }

        List<PuzzleCell> UpdateSelectedCell_ZeroCell ( int row, int col )
        {
            var revealedCells = new List<PuzzleCell> ();
            var zeroCellPoints = new Queue<(int Row, int Col)> ();
            zeroCellPoints.Enqueue ( (row, col) );
            var keepChecking = false;
            do
            {
                var zeroCell = zeroCellPoints.Dequeue ();
                var r = zeroCell.Row;
                var c = zeroCell.Col;

                IEnumerable<PuzzleCell> queryAdjacentPuzzleCells =
                    from cell in _puzzleCellsList
                    where ( ( cell.CellStatus >= CellStatus.Hidden )
                    && ( cell.Row >= r - 1 ) && ( cell.Row <= r + 1 ) )
                    && ( ( cell.Col >= c - 1 ) && ( cell.Col <= c + 1 ) )
                    && !( ( cell.Col == c ) && ( cell.Row == r ) )
                    select cell;

                foreach ( PuzzleCell cellQ in queryAdjacentPuzzleCells )
                {
                    if ( cellQ.CellStatus == CellStatus.Suspected )
                    { CellStatus_ToggleSuspected ( cellQ.Row, cellQ.Col ); }

                    if ( cellQ.CellStatus == CellStatus.Hidden )
                    {
                        revealedCells.Add (
                            UpdateSelectedCell_RevealCell ( cellQ ) );

                        if ( cellQ.CellValue == 0 )
                        {
                            zeroCellPoints.Enqueue ( (cellQ.Row, cellQ.Col) );
                            keepChecking = true;
                        }
                    }
                }

            } while ( keepChecking );
            return revealedCells;
        }


        /// <summary>
        /// Returns a list of the Boom cell followed by the other mined cells.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        List<PuzzleCell> UpdateSelectedCell_Mine ( int row, int col )
        {
            var updateList = new List<PuzzleCell> ();
            IEnumerable<PuzzleCell> queryAllMinedCells =
                from cell in _puzzleCellsList
                where ( cell.CellValue == CellValue.Mine )
                select cell;

            _hiddenSafeCellsCount = -_numberOfMines;
            foreach ( var item in queryAllMinedCells )
            {
                var cell = item;
                if ( ( item.Row == row ) && ( item.Col == col ) )
                {
                    cell.CellStatus = CellStatus.Boom;
                    updateList.Insert ( 0, cell );
                }

                else
                {
                    if ( item.CellStatus == CellStatus.Suspected )
                    {
                        _hiddenSafeCellsCount++;
                    }

                    else
                    {
                        cell.CellStatus = CellStatus.Revealed;
                    }
                    updateList.Add ( cell );
                }
            }
            return updateList;
        }


        PuzzleCell UpdateSelectedCell_RevealCell ( PuzzleCell cell )
        {
            cell.CellStatus = CellStatus.Revealed;
            _hiddenSafeCellsCount--;
            _puzzleCellsList [
                GetListIndexByRowCol ( cell.Row, cell.Col )] = cell;
            return cell;
        }

        /// <summary>
        /// Toggles suspected/ hidden cellstatus for right clicks and zero cell reveals.
        /// Fires a delegate for the UI minecount display. 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void CellStatus_ToggleSuspected ( int row, int col )
        {

            PuzzleCell cell = _puzzleCellsList [GetListIndexByRowCol ( row, col )];

            if ( ( cell.CellStatus != CellStatus.Hidden ) || ( cell.CellStatus != CellStatus.Suspected ) )
            {
                return;
            }

            switch ( cell.CellStatus )
            {
                //  Only hidden and suspected cells are affected, ignoring all others.
                case CellStatus.Hidden:
                    cell.CellStatus = CellStatus.Suspected;
                    _suspectedMinesCount--;
                    break;

                case CellStatus.Suspected:
                    cell.CellStatus = CellStatus.Hidden;
                    _suspectedMinesCount++;
                    break;
            }
            var updatedCells = new List<PuzzleCell> { cell };
            //  Fires an update for this cell
            OnPuzzleCellsChanged ( _suspectedMinesCount, updatedCells );
        }



        #region  UpdateCells Public Method Group


        #endregion

        //  End of Public Methods
        #endregion


        /// <summary>
        /// Returns all cells adjacent to the row and column of the center square.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        List<PuzzleCell> FindAdjacentCells_puzzleCellsList ( int row, int col )
        {
            var adjacentCells = new List<PuzzleCell> ();

            IEnumerable<PuzzleCell> queryAdjacentPuzzleCells =
                from cell in _puzzleCellsList
                where ( ( cell.Row >= row - 1 ) && ( cell.Row <= row + 1 ) )
                && ( ( cell.Col >= col - 1 ) && ( cell.Col <= col + 1 ) )
                && !( ( cell.Col == col ) && ( cell.Row == row ) )
                select cell;

            foreach ( var cell in queryAdjacentPuzzleCells )
            {
                adjacentCells.Add ( cell );
            }

            return adjacentCells;
        }




        #region  Private Helper Methods

        #endregion  //  End Private Helper Methods





















    }
}
