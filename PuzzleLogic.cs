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

        internal delegate void PuzzleStatusHandler ( PuzzleStatus gameStatus );
        internal event PuzzleStatusHandler UpdateGameStyle;


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

        internal PuzzleStatus GameStatus
        { get => _puzzleStatus; }

        //  Provides the tags for the PuzzleGrid buttons.
        public PuzzleCell [,] PuzzleCellArray
        { get => _puzzleCells.PuzzleCellArray; }

        internal PuzzleCells ThePuzzleCells
        { get => _puzzleCells; }

        #endregion

        #region  Constructor Method Group
        //  *****          Constructor          *****          *****          *****          *****          *****          Constructor          *****          *****          *****
        public PuzzleLogic ( int numberOfRows, int numberOfMines )
        {
            InitializeTimer ();
            InitializeVars ( numberOfRows, numberOfMines );
        }


        private void InitializeTimer ()
        {
            _dispatcherTimer = new DispatcherTimer ();
            _dispatcherTimer.Tick += /*new*/ DispatcherTimer_Tick;
            _dispatcherTimer.Interval = new TimeSpan ( 0, 0, 1 );
        }

        private void InitializeVars ( int numberOfRows, int numberOfMines )
        {
            _puzzleCells = new PuzzleCells ( numberOfRows, numberOfMines );
            _puzzleCells.UpdateGrid += UpdatePuzzleStatus;

            _puzzleStatus = PuzzleStatus.GameNew;
            _elapsedTime = 0;
            _dispatcherTimer.Start ();

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




        
        public void TileWasSelected ( int row, int col )
        {
            if ( PuzzleStatusCheck () )
            {
                _puzzleCells.UpdateSelectedCell ( row, col );
            }
        }



        public void TileWasRightClicked ( int row, int col )
        {
            if ( PuzzleStatusCheck () )
            {
                _puzzleCells.ToggleCellStatusAndSusCellsCount ( row, col );
            }
        }
        #endregion


        #region  Private   Methods
        //  *****       Private   Methods        *****          *****          *****          *****          *****       Private   Methods        *****          *****          *****      


        void UpdatePuzzleStatus ( object sender, PuzzleCellsEventArgs e )
        {
            //  Checking for state changes.
            if ( e.AllCellsRevealed )
            { _puzzleStatus = PuzzleStatus.GameVictory; }

            else if ( e.HasBoom )
            { _puzzleStatus = PuzzleStatus.GameDefeat; }
        }




        void DispatcherTimer_Tick ( object sender, EventArgs e )
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
        /// Is the game active or activate?
        /// </summary>
        /// <returns></returns>
        bool PuzzleStatusCheck ()
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

