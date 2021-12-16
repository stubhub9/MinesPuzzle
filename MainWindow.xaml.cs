using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MinesPuzzle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //  Using the next line can tell me what the current C# revision is (7.3).
        //#error version

//  Set the puzzle tiles to   <FontFamily ="Arial Black"/>

        //  Private Fields
        private PuzzleGrid _puzzleGrid;

        // Provide default values for now.
        // Used to set Puzzle: Grid, Logic and Cells
        private Size _puzzleSize = new Size ( 500.0, 500.0 );
        private int _numberOfRows = 10;
        private int _numberOfMines = 15;



        //  The slidepuzzle included drop shadow effects here, for the status label.
        public MainWindow ()
        {
            InitializeComponent ();
//TODO:  Need to play with this>>>>>>>>>>>>>>>>>>>>>>>>>>>
            //this.SizeToContent = SizeToContent.WidthAndHeight;
            NewPuzzleGrid ();
            //_puzzleGrid._puzzleLogic.ThePuzzleCells.UpdateTilesTillClear += UpdateMinesDisplay;
            //_puzzleGrid._puzzleLogic.UpdateTimeDisplay += UpdateTimeDisplay;

        }


        private void NewGameButton_Click ( object sender, RoutedEventArgs e )
        {
            NewPuzzleGrid ();
        }

        private void OnMoveMade ( object sender, HandledEventArgs e )
        { }


        private void OnResetPuzzle ( object sender, RoutedEventArgs e )
        {
            NewPuzzleGrid ();
        }

        //  On new game.
        private void NewPuzzleGrid ()
        {
            if ( _puzzleGrid != null )
            {
                PuzzleHostingPanel.Children.Remove ( _puzzleGrid );
                //TODO:  I need to do this?
                //_puzzleGrid._puzzleLogic.UpdateTimeDisplay -= UpdateTimeDisplay;
            }


            _puzzleGrid = new PuzzleGrid ( _puzzleSize, _numberOfRows, _numberOfMines )
            {
                PuzzleSize = _puzzleSize,
            };
            //_puzzleGrid.PuzzleSize = _puzzleSize;

            PuzzleHostingPanel.Children.Add ( _puzzleGrid );
            _puzzleGrid.Height = 600;
            _puzzleGrid.Width = 600;


            _puzzleGrid._puzzleLogic.ThePuzzleCells.UpdateGrid += UpdateMinesDisplay;
            _puzzleGrid._puzzleLogic.UpdateTimeDisplay += UpdateTimeDisplay;
            _puzzleGrid._puzzleLogic.Ready ();
        }



        private void UpdateTimeDisplay ( string elapsedTime )
        {
            //  Button content is passed to the label.
            timerDisplay.Content = elapsedTime;
        }

        //private void UpdateMinesDisplay ( string hiddenTiles, PuzzleCell cell )
        private void UpdateMinesDisplay ( object sender, PuzzleCellsEventArgs e )
        {
            mineCountDisplay_Label.Content = e.Mines.ToString () ;
        }


    }
}
