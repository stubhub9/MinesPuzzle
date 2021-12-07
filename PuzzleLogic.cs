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
    /// <summary>
    ///   Processes game changes.
    /// </summary>
    public class PuzzleLogic //: INotifyPropertyChanged
    {
        public delegate void TimerDisplayHandler ( string time );
        public event TimerDisplayHandler UpdateTimeDisplay;

        #region Field Data


        private PuzzleCells _puzzleCells;

//Make the timer private; by adding a public Event for Updating the Display Clock>>>>>>>>>>>>>>>>>>>>
        public DispatcherTimer _dispatcherTimer;
        private int _elapsedTime;
        private PuzzleStatus _puzzleStatus;

        //private bool _isGameLost;
        //private bool _isGameOver;
        //private bool _isGamePaused;
        //private bool _isGameWon;

        #endregion

        //  Properties  *********************************************************************************

        public int ElapsedTime
        {
            get => _elapsedTime;
            set
            {
                _elapsedTime = value;
                //OnPropertyChanged ("ElapsedTime");
            }
        }


        public PuzzleStatus PuzzleStatus
        {
            get => _puzzleStatus;
            //set
            //{ _puzzleStatus = value; }
        }

        //public bool IsGameLost
        //{ get => _isGameLost; }

        //Do I need this for visuals or error checking? Game is won or lost; true?
        //public bool IsGameOver
        //{
        //    get => _isGameOver;
        //}

        //public bool IsGamePaused
        //{ get => _isGamePaused; }

        //public bool GameWon
        //{ get => _isGameWon; }

       
        //  Provides the tags for the PuzzleGrid buttons.
        public PuzzleCell [,] PuzzleCellArray
        {
            get => _puzzleCells.PuzzleCellArray;
        }

        /// <summary>
        ///  Count of mines minus suspected mines.
        /// </summary>
        public int SuspectedMinesCountdown
        { get => _puzzleCells.SuspectedMinesCountdown; }

        //  Constructor      *******************************************************************************************

        public PuzzleLogic ( int numberOfRows, int numberOfMines )
        {
            _puzzleCells = new PuzzleCells ( numberOfRows, numberOfMines );

            //System.NullReferenceException: 'Object reference not set to an instance of an object.'
            //if ( _dispatcherTimer.Interval != (new TimeSpan ( 0, 0, 1 )) )
            //{
            //    BuildTimer ();
            //}

            BuildTimer ();
            ResetFieldVars ();

        }

        private void ResetFieldVars ()
        {
            _puzzleStatus = PuzzleStatus.GameNew;
            _dispatcherTimer.Start ();
            _elapsedTime = 0;

            //_isGameLost = false;
            //_isGameOver = false;
            //_isGamePaused = true;
            //_isGameWon = false;
        }


        private void BuildTimer ()
        {
            _dispatcherTimer = new DispatcherTimer ();
            _dispatcherTimer.Tick += /*new*/ DispatcherTimer_Tick;
            _dispatcherTimer.Interval = new TimeSpan ( 0, 0, 1 );
        }


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


        //  Public Methods     **********************************************************************************************

        //  On new game
        //public void TimerStart ()
        //{
        //    if ( _elapsedTime < 999 )
        //    {
        //        _dispatcherTimer.Start ();
        //    }
        //}

        //  Lose focus, paused, pre or post game; whenever an incrementing clock isn't needed.
        //public void TimerStop ()
        //{
        //    _dispatcherTimer.Stop ();
        //}


        public List<PuzzleCell> TileWasSelected ( int row, int col )
        {
            //_isGamePaused = false;
            var updatedTiles = new List<PuzzleCell> ();

//As is, this would work for UNpausing; IF A SAFE TILE (eg: last tile selected) is used.
//Or have the pause condition related to Win Minimized and restore, lost focus, ....; and use different methods.
            if (( _puzzleStatus == PuzzleStatus.GamePaused ) || (  _puzzleStatus == PuzzleStatus.GameNew ) )
            {
                _puzzleStatus = PuzzleStatus.GameOn;
                _dispatcherTimer.Start ();
            }
            if ( ( _puzzleStatus == PuzzleStatus.GameOn ) )
            {

                updatedTiles = _puzzleCells.UpdateSelectedCell ( row, col );
                if ( _puzzleCells.HiddenSafeCells == 0 )
                {
                    _puzzleStatus = PuzzleStatus.GameWon;
                }
                if ( _puzzleCells.HasBoom )
                {
                    _puzzleStatus = PuzzleStatus.GameLost;
                }
            }
            //  If puzzlecell's hiddenSafeCell count is 0, then game is won and done.
            //  If puzzlecell has boom then game is lost and done.
//UI will react to won/ loss status Properties.
            return updatedTiles;
        }

        /// <summary>
        ///  Alternate between Hidden and Supected CellStatus.
        /// </summary>
        /// <param name="puzzleCell"></param>
        /// <returns></returns>
        public PuzzleCell TileWasRightClicked ( int row, int col )
        {
            var puzzleCell = _puzzleCells.ToggleCellStatusAndSusCellsCount ( row, col );

            //  Fire a delegate to update the suspect cells countdown label.

            return puzzleCell;
        }



        //event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        //public event PropertyChangedEventHandler PropertyChanged;
        //{
        //    add
        //    {
        //        throw new NotImplementedException ();
        //    }

        //    remove
        //    {
        //        throw new NotImplementedException ();
        //    }
        //}

        //protected void OnPropertyChanged ( [CallerMemberName] string name = null )
        //{
        //    PropertyChanged?.Invoke ( this, new PropertyChangedEventArgs ( name ) );
        //}


    }
}
