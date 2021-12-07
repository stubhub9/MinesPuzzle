using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesPuzzle
{
    public class PuzzleCells
    {

        #region  Private Fields
        //  *****          Private Fields          *****          *****          *****                    *****          *****          *****          *****          *****          *****          *****

        private bool _hasBoom;

        private int _hiddenSafeCells;
        private int _numberOfMines;
        private int _numberOfRows;

        private int _suspectedCellsCountdown;
        private List<PuzzleCell> _mineCellsList;
        private PuzzleCell [,] _puzzleCellArray;


        //REDACT??  An enum for case/ switch methods; just to be fancy>>>>>>>>>>>>>
        private enum VariousTasksEnum
        {
            CellsAddRowAndCol,
        }
        #endregion

        #region  Properties

        //  *****          Properties          *****          *****          *****          *****          *****          *****          *****          *****          *****          *****          *****

        /// <summary>
        /// Selected cell was mined.
        /// Consumed by PuzzleLogic.
        /// </summary>
        public bool HasBoom
        { get => _hasBoom; }

        //TODO:  ?Should PuzzleLogic consume this bool property OR int HiddenSafeCells?????????????????????????????????????????????????????
        //public bool AllCellsCleared 
        //{
        //    get
        //    {
        //        if ( _hiddenSafeCells == 0 )
        //        {
        //            return true;
        //        }
        //        else
        //            return false;
        //    }

        /// <summary>
        /// Supplies PuzzleLogic.PuzzleStatus logic chain.  When this value equals zero; the game is won; unless game was already lost.
        /// </summary>
        public int HiddenSafeCells
        { get => _hiddenSafeCells; }

        //  Used for the UI  mine counter.  Move to Logic   >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        public int SuspectedMinesCountdown
        { get => _suspectedCellsCountdown; }

        /// <summary>
        ///REDACT?  Provides data for button.Tags???????????????????????????????????????????????????????????????????????????????????????????????????????????
        /// </summary>
        public PuzzleCell [,] PuzzleCellArray
        {
            get => _puzzleCellArray;
        }
        #endregion

        #region  Constructor
        //  *****          Constructor          *****          *****          *****                    *****          *****          *****          *****          *****          *****          *****
        public PuzzleCells ( int numberOfRows, int numberOfMines )
        {
            //  flag for client
            _hasBoom = false;
            //  flag for client, count of safe tiles, yet to be revealed
            _hiddenSafeCells = ( numberOfRows * numberOfRows ) - numberOfMines;
            //  ??Move to logic--  Count of unrevealed mines at this point  ????
            _suspectedCellsCountdown = numberOfMines;

            //  ??  should use this strictly as a construction param  and delete the field
            _numberOfMines = numberOfMines;
            //  ??this could be replaced be a ?Count? ??
            _numberOfRows = numberOfRows;
            BuildArray ();
        }
        #endregion


        //  *****          Public Methods/ Method Groups          *****          *****          *****                    *****          *****          *****          *****          *****          *****

        public List<PuzzleCell> UpdateSelectedCell ( int row, int col )
        {
            var updatedTiles = new List<PuzzleCell> ();
            var cell = _puzzleCellArray [row, col];

            //  Only hidden cells will be processed.
            if ( cell.CellStatus == CellStatus.Hidden )
            {
                switch ( cell.CellValue )
                {
                    case CellValue.Mine:
                        //      GameLost condition      *************         GameLost condition      *************   
                        updatedTiles = UpdateSelectedCell_Boom ( cell );
                        break;

                    default:
                        updatedTiles = UpdateSelectedCell_RevealHiddenCells ( cell );

                        if ( _hiddenSafeCells < 1 )
                        {
                            //      GameWon condition           ************        GameWon condition                **************
                            updatedTiles.AddRange ( UpdateSelectedCell_RevealMines () );
                        }
                        break;
                }

            }
            return updatedTiles;

        }

        /// <summary>
        /// Right clicks on a tile should toggle the suspected or hidden, cell status
        /// and affect the number of suspected mines.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public PuzzleCell ToggleCellStatusAndSusCellsCount ( int row, int col )
        {
            var toggledCell = _puzzleCellArray [row, col];
            switch ( toggledCell.CellStatus )
            {
                //  Only hidden and suspected cells are affected, ignoring all others.
                case CellStatus.Hidden:
                    toggledCell.CellStatus = CellStatus.Suspected;
                    _suspectedCellsCountdown--;
                    break;

                case CellStatus.Suspected:
                    toggledCell.CellStatus = CellStatus.Hidden;
                    _suspectedCellsCountdown++;
                    break;
            }
            //  Returns an updated cell for the button's tag.
            return toggledCell;
        }


        //  Private methods      *******************************************************************************

        /// <summary>
        /// Returns a List of adjacent tiles; 
        /// for tiles adjacent to selected or clear tiles.
        /// </summary>
        /// <param name="centerCellRow"></param>
        /// <param name="centerCellColumn"></param>
        /// <returns></returns>
        private List<PuzzleCell> AdjacentCells ( int centerCellRow, int centerCellColumn )
        {
            var adjacentCells = new List<PuzzleCell> ();
            var row = centerCellRow;
            var col = centerCellColumn;
            //var array = cellArray;
            var rowMax = _puzzleCellArray.GetLength ( 0 );
            var colMax = _puzzleCellArray.GetLength ( 1 );

            for ( var r = row - 1; r <= ( row + 1 ); r++ )
            {
                //  Is this a valid row?
                if ( ( r >= 0 ) && ( r < rowMax ) )
                {
                    for ( var c = col - 1; c <= ( col + 1 ); c++ )
                    {
                        //  Is this a valid column, and also not the cell being evaluated?
                        if ( ( c >= 0 ) && ( c < colMax ) &&
                            ( ( c != col ) && ( r != row ) ) )
                        {
                            adjacentCells.Add ( _puzzleCellArray [r, c] );

                            //Hard Test
                            if ( adjacentCells.Count > 8 )
                                throw new ArgumentException ();
                        }
                    }
                }
            }  // End of "adjacent cells" row/ col loop.
            return adjacentCells;
        }


        private void BuildArray ()
        {
            _puzzleCellArray = new PuzzleCell [_numberOfRows, _numberOfRows];
            BuildArray_Iterator ( VariousTasksEnum.CellsAddRowAndCol );
            BuildArray_AddMines ();
            //ArrayInit ();
        }


        private void BuildArray_Iterator ( VariousTasksEnum JobType )
        {
            var rows = _puzzleCellArray.GetLength ( 0 );
            var cols = _puzzleCellArray.GetLength ( 1 );
            for ( int r = 0; r < rows; r++ )
            {
                for ( int c = 0; c < cols; c++ )
                {
                    if ( JobType == VariousTasksEnum.CellsAddRowAndCol )
                    {
                        _puzzleCellArray [r, c].Col = c;
                        _puzzleCellArray [r, c].Row = r;
                    }


                }
            }
        }



        private void BuildArray_AddMines ()
        {
            var random = new Random ();
            var mines = 0;
            _mineCellsList = new List<PuzzleCell> ();
            //  Initialize all mine cells.
            do
            {
                var row = random.Next ( _numberOfRows );
                var col = random.Next ( _numberOfRows );

                if ( _puzzleCellArray [row, col].CellValue != CellValue.Mine )
                {
                    var puzzleCell = new PuzzleCell ( row, col, CellValue.Mine, CellStatus.Hidden );
                    _puzzleCellArray [row, col] = puzzleCell;
                    _mineCellsList.Add ( puzzleCell );
                    mines++;
                    BuildArray_UpdateCellsAdjacentToMine ( puzzleCell );
                }

            } while ( mines < _numberOfMines );
        }

        //  So this is wrong >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
        private void BuildArray_UpdateCellsAdjacentToMine ( PuzzleCell minedCell )
        {
            //  Given a list of cells [max 8] that are adjacent to the current mine; 
            //  increment the current value of cells that aren't mined.
            var cellList = AdjacentCells ( minedCell.Row, minedCell.Col );
            for ( int i = 0; i < cellList.Count; i++ )
            {
                var cell = cellList [i];
                if ( cell.CellValue != CellValue.Mine )
                    cell.CellValue++;
                cellList [i] = cell;
            }
        }


        private List<PuzzleCell> UpdateSelectedCell_Boom ( PuzzleCell chosenCell )
        {
            var cellList = new List<PuzzleCell> ();
            _hasBoom = true;
            foreach ( var item in _mineCellsList )
            {
                var itemCell = item;
                itemCell.CellStatus = CellStatus.Revealed;
                ////TODO:  Change this evaluation; to a simple Assingment (thisCell = Boom)
                //                if ( ( itemCell.Row == chosenCell.Row ) && ( itemCell.Col == chosenCell.Col ) )
                //                {
                //                    itemCell.CellStatus = CellStatus.Boom;
                //                }
                cellList.Add ( itemCell );
            }
            var i = ( chosenCell.Row * chosenCell.Col ) + chosenCell.Col;
            var boomCell = cellList [i];
            boomCell.CellStatus = CellStatus.Boom;
            cellList [i] = boomCell;

            //Error Check  >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
            if ( ( boomCell.Col != chosenCell.Col ) || ( boomCell.Row != chosenCell.Row ) )
            {
                throw new ArgumentException ();
            }

            return cellList;
        }


        private List<PuzzleCell> UpdateSelectedCell_RevealHiddenCells ( PuzzleCell cell )
        {
            var cellList = new List<PuzzleCell> ();
            var updatedTiles = new List<PuzzleCell> ();

            cellList.Add ( cell );
            while ( cellList.Count != 0 )
            {
                var cellItem = cellList [0];
                cellList.Remove ( cellItem );

                if ( cellItem.CellStatus != CellStatus.Revealed )
                {
                    cellItem.CellStatus = CellStatus.Revealed;
                    _hiddenSafeCells--;

                    //  Used to check for bonus zero-chained reveal.
                    if ( cellItem.CellValue == 0 )
                    {
                        cellList.AddRange ( AdjacentCells ( cellItem.Row, cellItem.Col ) );
                    }

                    updatedTiles.Add ( cellItem );
                }
            }
            return updatedTiles;
        }

        //WHY?  
        //  Think Rename to GameWon_RevealMinesOnly plus the selected cell are all that are revealed on win.
        //  Unchosen empty tiles remain hidden on purpose.
        private List<PuzzleCell> UpdateSelectedCell_RevealMines ()
        {
            //TODO      ForEach this.
            var cellList = new List<PuzzleCell> ();
            for ( int i = 0; i < _mineCellsList.Count; i++ )
            {
                var cell = _mineCellsList [i];
                cell.CellStatus = CellStatus.Revealed;
                cellList.Add ( cell );
            }
            return cellList;
        }






    }
}
