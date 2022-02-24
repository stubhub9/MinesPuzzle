using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesPuzzle
{
    /// <summary>
    /// Logic for interacting with the PuzzleCells collection.
    /// </summary>
    class PuzzleCells

    {

        #region  Events group
        //Action<List<PuzzleCell>, string, bool, bool> updateGridAction; 

        public event EventHandler<PuzzleCellsEventArgs> UpdatePuzzleGridEvent;


        void OnPuzzleCellsChanged ()
        {
            var e = new PuzzleCellsEventArgs ()
            {
                Cells = _updatedCellsList,
                Mines = _suspectedMinesCount.ToString (),
                AllCellsRevealed = ( _hiddenSafeCellsCount == 0 ),
                HasBoom = _haveBoom,
            };

            UpdatePuzzleGridEvent?.Invoke ( this, e );
            foreach ( var iCell in _updatedCellsList )
            {
                _puzzleCellsList [GetListIndexByRowCol ( iCell.Row, iCell.Col )] = iCell;
            }
            _updatedCellsList = new List<PuzzleCell> ();
        }

        //  End events group.
        #endregion

        #region  Private Fields
        //  *****          Private Fields          *****          *****          *****          *****          *****          *****          Private Fields          *****           *****

        //TODO:  Replace the list with a custom class based on an array of PuzzleCell rows and columns.
        //TODO:  ?? Move array & collections type of methods to the PuzzleCellArray from this PuzzleCells_Logic.
        //  List of all PuzzleCells, ordered by a sequential list of full rows.
        List<PuzzleCell> _puzzleCellsList;
        PuzzleCellArray _puzzleCellsArray;


        //  Game won when zero.
        private int _hiddenSafeCellsCount;

        // Game lost when true.
        private bool _haveBoom;


        //  List of cells, that have been updated since the last OnPuzzleCellsChanged.
        List<PuzzleCell> _updatedCellsList;

        //  Equals number of mines minus number of suspected cells; updates UI.
        private int _suspectedMinesCount;

        //  Set by constructor params; don't need more params.
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
        //  *****          Constructor          *****          *****          *****          *****          *****          *****           *****          Constructor          *****           *****

        public PuzzleCells ( int rows = 10, int mines = 15 )
        {
            Constructor_InitializeVars ( rows, mines );
            Constructor_InitializePuzzleCellsList ();
            Constructor_SetMines ();
        }


        void Constructor_InitializeVars ( int rows, int mines )
        {
            _haveBoom = false;
            _hiddenSafeCellsCount = ( rows * rows ) - mines;
            _suspectedMinesCount = mines;
            _numberOfRows = rows;
            _numberOfMines = mines;
            _random = new Random ();
            _puzzleCellsList = new List<PuzzleCell> ();
            _updatedCellsList = new List<PuzzleCell> ();

        }


        //  Creates the base model of default PuzzleCells; ordered by row and column.
        void Constructor_InitializePuzzleCellsList ()
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


        /// <summary>
        /// Updates the _puzzleCellsList with the mined cells.
        /// </summary>
        void Constructor_SetMines ()
        {
            var newMines = 0;

            Tuple<int, int> point;
            var pointsList = new List<Tuple<int, int>> ();

            do
            {//  Pick a spot.
                var r = _random.Next ( _numberOfRows );
                var c = _random.Next ( _numberOfRows );
                var isDistinct = true;

                foreach ( var iPoint in pointsList )
                {
                    if ( ( r == iPoint.Item1 ) && ( c == iPoint.Item2 ) )
                    {
                        isDistinct = false;
                    }
                }

                if ( isDistinct )
                {
                    newMines++;
                    point = new Tuple<int, int> ( r, c );
                    pointsList.Add ( point );
                    _puzzleCellsList [
                        GetListIndexByRowCol ( point )] =
                        new PuzzleCell ()
                        {
                            Row = r,
                            Col = c,
                            CellValue = CellValue.Mine,
                        };
                }
            } while ( newMines < _numberOfMines );
            Constructor_SetMines_AdjCells ( pointsList );
        }


        //  Called by SetMines; sets the CellValues of the adjacent cells.
        void Constructor_SetMines_AdjCells ( List<Tuple<int, int>> pointsList )
        {
            foreach ( var iPoint in pointsList )
            {
                var cellsList = new List<PuzzleCell> ( FindAdjacentCells_puzzleCellsList ( iPoint.Item1, iPoint.Item2 ) );
                foreach ( var iCell in cellsList )
                {
                    if ( iCell.CellValue != CellValue.Mine )
                    {
                        var cell = iCell;
                        cell.CellValue++;
                        _puzzleCellsList [GetListIndexByRowCol ( cell.Row, cell.Col )] = cell;
                    }
                }
            }
        }

        //  end Constructor method group.
        #endregion

        #region Public Methods
        //  *****       Public Methods        *****          *****          *****          *****          *****       Public Methods        *****          *****          *****  
        // Public methods end with OnPuzzleCellsChanged.


        /// <summary>
        /// Updates UI counters for the new game; after handlers have been registered; .
        /// ?? IReady  ??chained, tracked and boolled??
        /// </summary>
        public void Ready ()
        {
            OnPuzzleCellsChanged ();
        }


        /// <summary>
        /// Toggles suspected/ hidden cellstatus for right clicks.
        /// Fires a delegate for the UI minecount display. 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void ToggleSuspected_RightClicked ( int row, int col )
        {

            PuzzleCell cell = _puzzleCellsList [GetListIndexByRowCol ( row, col )];

            if ( ( cell.CellStatus == CellStatus.Hidden ) || ( cell.CellStatus == CellStatus.Suspected ) )
            {
                switch ( cell.CellStatus )
                {
                    case CellStatus.Hidden:
                        cell.CellStatus = CellStatus.Suspected;
                        _suspectedMinesCount--;
                        break;

                    case CellStatus.Suspected:
                        cell.CellStatus = CellStatus.Hidden;
                        _suspectedMinesCount++;
                        break;
                }

                _updatedCellsList.Add ( cell );
                OnPuzzleCellsChanged ();
            }
        }


        /// <summary>
        /// Updates the cells affected by this selection.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void UpdateSelectedCell_LeftClicked ( int row, int col )
        {
            var indexSelected = GetListIndexByRowCol ( row, col );
            var selectedCell = _puzzleCellsList [indexSelected];
            if ( selectedCell.CellStatus != CellStatus.Hidden )
            { return; }

            if ( selectedCell.CellValue == CellValue.Mine )
            {
                UpdateSelectedCell_Mine ( row, col );
            }

            else
            {
                UpdateSelectedCell_RevealCell ( selectedCell );
            }
            OnPuzzleCellsChanged ();
        }

        void GameWon_UpdateMines ()
        {
            //  Update UI counter.
            _suspectedMinesCount = 0;

            //  Update UI by revealing mines the nice way.
            IEnumerable<PuzzleCell> queryAllMinedCells =
                from cell in _puzzleCellsList
                where ( ( cell.CellValue == CellValue.Mine )
                && ( cell.CellStatus == CellStatus.Hidden ) )
                select cell;

            foreach ( var iCell in queryAllMinedCells )
            {
                var cell = iCell;
                cell.CellStatus = CellStatus.Suspected;
                _updatedCellsList.Add ( cell );
            }
        }


        /// <summary>
        /// UpdatesList contains the Boom cell followed by the other mined cells.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        /// TODO:??  Could replace row, col params with a field??
        void UpdateSelectedCell_Mine ( int row, int col )
        {
            _hiddenSafeCellsCount = -_numberOfMines;
            _haveBoom = true;

            IEnumerable<PuzzleCell> queryAllMinedCells =
                from cell in _puzzleCellsList
                where ( cell.CellValue == CellValue.Mine )
                select cell;

            foreach ( var iCell in queryAllMinedCells )
            {
                var cell = iCell;
                //  Insert the Boom cell @ 0; adding the rest.
                if ( ( iCell.Row == row ) && ( iCell.Col == col ) )
                {
                    cell.CellStatus = CellStatus.Boom;
                    _updatedCellsList.Insert ( 0, cell );
                }

                else
                {
                    //  Credit for suspected mines.
                    if ( iCell.CellStatus == CellStatus.Suspected )
                    {
                        _hiddenSafeCellsCount++;
                    }

                    else
                    {
                        cell.CellStatus = CellStatus.Revealed;
                    }
                    _updatedCellsList.Add ( cell );
                }
            }
        }


        /// <summary>
        /// Changes the state of updated cells; checks if game is won.
        /// </summary>
        /// <param name="cell"></param>
        void UpdateSelectedCell_RevealCell ( PuzzleCell cell )
        {
            var revealedCells = new List<PuzzleCell>
            {
                cell
            };

            if ( cell.CellValue == 0 )
            {
                revealedCells.AddRange ( ZeroCellBonusReveal ( cell.Row, cell.Col ) );
            }


            foreach ( var iCell in revealedCells )
            {
                var cell1 = iCell;
                cell1.CellStatus = CellStatus.Revealed;
                _updatedCellsList.Add ( cell1 );
                _hiddenSafeCellsCount--;

                if ( AllCellsRevealed )
                {
                    GameWon_UpdateMines ();
                }
            }
        }



        #region  UpdateCells Public Method Group


        #endregion

        //  End of Public Methods
        #endregion

        #region  Private Helper Methods
        //  *****       Private   Methods        *****          *****          *****          *****          *****       Private   Methods        *****          *****          *****  

        /// <summary>
        ///  Provides a List of BonusRevealCells
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        List<PuzzleCell> ZeroCellBonusReveal ( int row, int col )
        {
            var zeroCellPoints = new Queue<(int Row, int Col)> ();
            zeroCellPoints.Enqueue ( (row, col) );
            var zeroBonusCells = new List<PuzzleCell> ();
            //var adjacentCells = new List<PuzzleCell> ();
            do
            {
                var adjacentCells = new List<PuzzleCell> ();
                var (Row, Col) = zeroCellPoints.Dequeue ();
                adjacentCells.AddRange ( FindAdjacentCells_puzzleCellsList ( Row, Col ) );
                foreach ( var iCell in adjacentCells )
                {
                    if ( ( !zeroBonusCells.Contains ( iCell ) )
                        && ( iCell.CellStatus != CellStatus.Revealed )
                        && !( ( iCell.Row == row ) && ( iCell.Col == col ) ) )
                    {
                        zeroBonusCells.Add ( iCell );
                        if ( iCell.CellValue == 0 )
                        {
                            zeroCellPoints.Enqueue ( (iCell.Row, iCell.Col) );
                        }
                    }
                }
            } while ( zeroCellPoints.Count > 0 );

            //??  Safer to do this here.
            var returnList = new List<PuzzleCell> ();
            foreach ( var iCell in zeroBonusCells )
            {
                var cell = iCell;
                if ( cell.CellStatus == CellStatus.Suspected )
                {
                    cell.CellStatus = CellStatus.Hidden;
                    _suspectedMinesCount++;
                }
                returnList.Add ( cell );
            }
            return returnList;
        }


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

            adjacentCells.AddRange ( queryAdjacentPuzzleCells );
            return adjacentCells;
        }


        #region GetListIndexByRowCol Method Group

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


        //  Tuple overload
        int GetListIndexByRowCol ( Tuple<int, int> point )
        {
            return GetListIndexByRowCol ( point.Item1, point.Item2 );
        }

        #endregion

        #endregion  //  End Private Helper Methods





















    }
}
