using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MinesPuzzle
{

    public class PuzzleLogic //: INotifyPropertyChanged
    {
        #region Events

        public delegate void PuzzleStatusHandler ( PuzzleStatus gameStatus );
        public event PuzzleStatusHandler UpdateGameStyle;


        public delegate void TimerDisplayHandler ( string time );
        public event TimerDisplayHandler UpdateTimeDisplay;
        #endregion

        #region  Private Fields
        //  *****          Private Fields          *****          *****          *****          *****          *****        Private Fields          *****          *****          *****          *****
        private PuzzleCells _puzzleCells;

        private DispatcherTimer _dispatcherTimer;
        private int _elapsedTime;
        private PuzzleStatus _puzzleStatus;

        #endregion


        #region  Properties
        //  *****          Properties          *****          *****          *****          *****          *****          Properties          *****          *****          *****          *****

        public int ElapsedTime
        { get => _elapsedTime; }

        public PuzzleStatus PuzzleStatus
        { get => _puzzleStatus; }

        //  Provides the tags for the PuzzleGrid buttons.
        public PuzzleCell [,] PuzzleCellArray
        { get => _puzzleCells.PuzzleCellArray; }

        public PuzzleCells ThePuzzleCells
        { get => _puzzleCells; }

        #endregion

        #region  Constructor Method Group
        //  *****          Constructor          *****          *****          *****          *****          *****          Constructor          *****          *****          *****
        public PuzzleLogic ( int numberOfRows, int numberOfMines )
        {
            _puzzleCells = new PuzzleCells ( numberOfRows, numberOfMines );
            InitializeTimer ();
            InitializeVars ();

        }

        private void InitializeVars ()
        {
            _puzzleStatus = PuzzleStatus.GameNew;
            _dispatcherTimer.Start ();
            _elapsedTime = 0;

            //_isGameLost = false;
            //_isGameOver = false;
            //_isGamePaused = true;
            //_isGameWon = false;
        }


        private void InitializeTimer ()
        {
            _dispatcherTimer = new DispatcherTimer ();
            _dispatcherTimer.Tick += /*new*/ DispatcherTimer_Tick;
            _dispatcherTimer.Interval = new TimeSpan ( 0, 0, 1 );
        }
        //  End Constructor Method Group
        #endregion 




        #region  Public Methods
        //  *****       Public Methods        *****          *****          *****          *****          *****       Public Methods        *****          *****          *****  

        public void Ready ()
        {
            _puzzleCells.Ready ();
            UpdateGameStyle?.Invoke ( _puzzleStatus );
            UpdateTimeDisplay?.Invoke ( _elapsedTime.ToString () );
        }




        
        public List<PuzzleCell> TileWasSelected ( int row, int col )
        {

            var updatedTiles = new List<PuzzleCell> ();


            if ( PuzzleStatusCheck () )
            {
                updatedTiles = _puzzleCells.UpdateSelectedCell ( row, col );

                //  Checking for state changes.
                if ( _puzzleCells.AllCellsRevealed )
                {
                    _puzzleStatus = PuzzleStatus.GameVictory;
                }
                if ( _puzzleCells.MineWasRevealed )
                {
                    _puzzleStatus = PuzzleStatus.GameDefeat;
                }
            }
            return updatedTiles;
        }



        public PuzzleCell TileWasRightClicked ( int row, int col )
        {
            if ( PuzzleStatusCheck () )
            {
                // Using PuzzleCellsEvent
                _puzzleCells.ToggleCellStatusAndSusCellsCount ( row, col );
                //Fired a delegate to update the suspect cells countdown label.

                //// OR
                //var puzzleCell = _puzzleCells.ToggleCellStatusAndSusCellsCount ( row, col );
            }
            var puzzleCell = PuzzleCellArray [row, col];
            return puzzleCell;
        }
        #endregion


        #region  Private   Methods
        //  *****       Private   Methods        *****          *****          *****          *****          *****       Private   Methods        *****          *****          *****      


        private void DispatcherTimer_Tick ( object sender, EventArgs e )
        {
            if ( _puzzleStatus == PuzzleStatus.GameOn )
            {
                _elapsedTime++;
                if ( _elapsedTime > 999 )
                {
                    _elapsedTime = 999;
                    _dispatcherTimer.Stop ();
                }
                UpdateTimeDisplay?.Invoke ( _elapsedTime.ToString () );
            }
            else
            {
                _dispatcherTimer.Stop ();
            }
        }



        /// <summary>
        /// Is the game active?
        /// </summary>
        /// <returns></returns>
        private bool PuzzleStatusCheck ()
        {
            var proceed = false;
            if ( !(( _puzzleStatus == PuzzleStatus.GameVictory ) || ( _puzzleStatus == PuzzleStatus.GameDefeat )) )
            {
                proceed = true;

                if ( ( _puzzleStatus == PuzzleStatus.GamePaused ) || ( _puzzleStatus == PuzzleStatus.GameNew ) )
                {
                    _puzzleStatus = PuzzleStatus.GameOn;
                    _dispatcherTimer.Start ();
                }
            }
            return proceed;
        }



        #endregion

    }
}

