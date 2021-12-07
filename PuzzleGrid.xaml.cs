using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media.Animation;

namespace MinesPuzzle
{
    /// <summary>
    /// Interaction logic for PuzzleGrid.xaml
    ///  Create a grid with buttons that can added to the visual. 
    ///  Update the buttons as the game progresses, with the results from PuzzleLogic..
    /// </summary>
    //public partial class PuzzleGrid : UserControl
    public partial class PuzzleGrid : Grid
    {


        //  Private Data      ********************************************************************************************************

        /// <summary>
        ///   Enables updating (PuzzleGrid) UI buttons based on row and column.
        ///   Could inherit from Button and add enum Properties
        /// </summary>
        private Button [,] _puzzleGridTiles;
        public PuzzleLogic _puzzleLogic;

        private int _numberOfMines;
        private int _numberOfRows;
        private Size _gridSize;

        private Brush _boomBrush;
        private Brush _revealedBrush;
        private Brush _fogBrush;

        //  Properties     ********************************************************************************************

        //public int NumberOfRows
        //{ get => _numberOfRows; }

        //  Set by MainWindow default 500.0, 500.0
        public Size PuzzleSize
        {
            get;
            set;
        }



        //  Events     **********************************************************************************

        public event EventHandler SuspectedMinesCountdown_Changed;

        //  Provide event handlers for the MainWindow to tie into.
        public event EventHandler PuzzleWon;
        public event EventHandler<HandledEventArgs> MoveMade;
        public event EventHandler ClockTick;


        //  Event Handlers  *********************************************************************************************************************
        //TODO:  ??Move OnClick to MainWindow??  
        // Reveal a cell, ignore if cell is already revealed or suspected..
        private void OnPuzzleButtonClick ( object sender, RoutedEventArgs e )
        {
            // Have PuzzleLogic return a collection of cells for button tags that need to be updated.
            var b = e.Source as Button;
            var row = (int)b.GetValue ( RowProperty );
            var col = (int)b.GetValue ( ColumnProperty );
            var updatedCells = _puzzleLogic.TileWasSelected ( row, col );
            if ( updatedCells.Count == 0 ) return;
            else
            {
                foreach ( var item in updatedCells )
                {
                    var updatedTile = _puzzleGridTiles [item.Row, item.Col];
                    updatedTile.Tag = item;
                    //_puzzleGridTiles [item.Row, item.Col].Tag = item;
                    //:TODO     Add Style triggers to auto update properties;
                    //      or override Button to add Properties for Tag.Props, and Dependency those;
                    //      Do it Here, for now. Plus a GameOver would have to be communicated when necessary.
                    var cellStatus = item.CellStatus;
                    if ( cellStatus == CellStatus.Boom )
                    {
                        updatedTile.Background = _boomBrush;
                    }
                    else
                    {
                        updatedTile.Background = _revealedBrush;
                    }

                    updatedTile.Content = item.CellValue.ToString ();


                }
            }

            //  Trigger animations for reveal, already revealed or suspected.

            //  Update minecount?
        }

        //  Flag or unflag a hidden cell as being presumed as a mine. 
        private void OnPuzzleButtonRightClick ( object sender, RoutedEventArgs e )
        {
            var b = e.Source as Button;
            var row = (int)b.GetValue ( RowProperty );
            var col = (int)b.GetValue ( ColumnProperty );
            var updatedCell = _puzzleLogic.TileWasRightClicked ( row, col );

            //TODO:  Assuming the tag will trigger a style change!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            b.Tag = updatedCell;

            //TODO:  Need PuzzleLogic to update the suspected tiles countdown label!!!!!!!!!!!!!!!!!!!!!
        }

        private void PuzzleGridLoaded ( object sender, RoutedEventArgs e )
        {

        }


        #region  ?Failed Constructor?
        //  Constructor   ***********************************************************************************************************
        //// Update constructor
        //public PuzzleGrid ( Size puzzleSize, int numRows, int numMines )
        //{
        //    InitializeComponent ();

        //    //  Centralize handling of all clicks in PuzzleGrid.
        //    AddHandler ( ButtonBase.ClickEvent, new RoutedEventHandler ( OnPuzzleButtonClick ) );

        //    //_puzzleLogic = new PuzzleLogic ( numberOfRows, numberOfMines );
        //    SetupThePuzzleGridStructure ( numRows );
        //    //private void PuzzleGridLoaded ( object sender, RoutedEventArgs e )
        //}

        ////Create a grid, with dedicated left & right click handlers.
        //public PuzzleGrid ()
        //{
        //    InitializeComponent ();

        //    //  Are these properties ever used?
        //    //NumberOfRows = numberOfRows;
        //    //NumberOfMines = numberOfMines;


        //    //  Centralize handling of all clicks in PuzzleGrid.
        //    AddHandler ( ButtonBase.ClickEvent, new RoutedEventHandler ( OnPuzzleButtonClick ) );
        //    //AddHandler ( MouseRightButtonDownEvent, new RoutedEventHandler ( OnPuzzleButtonRightClick ) );
        //}
        #endregion

        //  Constructor      ***************************************************************************************************************************************

        public PuzzleGrid ( Size gridSize, int numberOfRows, int numberOfMines )
        {
            InitializeComponent ();
            Width = gridSize.Width;
            Height = gridSize.Height;

            //  Centralize handling of all clicks in PuzzleGrid.
            AddHandler ( ButtonBase.ClickEvent, new RoutedEventHandler ( OnPuzzleButtonClick ) );

            _puzzleLogic = new PuzzleLogic ( numberOfRows, numberOfMines );


            SetupTheBrushes ();
            SetupThePuzzleGridStructure ( numberOfRows );
        }



        private void SetupTheBrushes ()
        {
            var gradStops = new GradientStopCollection ();
            var gradStop = new GradientStop ( Colors.Red, 1.0 );
            gradStops.Add ( gradStop );
            gradStop = new GradientStop ( Colors.Yellow, .9 );
            gradStops.Add ( gradStop );
            gradStop = new GradientStop ( Colors.Red, .8 );
            gradStops.Add ( gradStop );
            gradStop = new GradientStop ( Colors.Yellow, .7 );
            gradStops.Add ( gradStop );
            _boomBrush = new RadialGradientBrush ( gradStops );
            _revealedBrush = new SolidColorBrush ( Colors.CornflowerBlue );
            _fogBrush = new SolidColorBrush ( Color.FromRgb ( 60, 80, 170 ) );
        }

        //PuzzleLogicInstance = new PuzzleLogic ( numberOfRows, numberOfMines );
        ////_puzzleLogic = new PuzzleLogic ( numberOfRows, numberOfMines );
        //SetupThePuzzleGridStructure ( numberOfRows );

        private void SetupThePuzzleGridStructure ( int numberOfRows )
        {
            _puzzleGridTiles = new Button [numberOfRows, numberOfRows];
            //  Define rows and columns in the grid.
            for ( var i = 0; i < numberOfRows; i++ )
            {
                var row = new RowDefinition { Height = GridLength.Auto };
                RowDefinitions.Add ( row );

                var column = new ColumnDefinition { Width = GridLength.Auto };
                ColumnDefinitions.Add ( column );
            }


            var buttonStyle = (Style)Resources ["PuzzleButtonStyle"];

            //var tags = _puzzleLogic.PuzzleCellArray;
            var puzzleCellArray = _puzzleLogic.PuzzleCellArray;
            //var tag = new PuzzleCell ();
            var brush = new SolidColorBrush ( Colors.SlateBlue );
            //  Now add the buttons in.
            for ( var row = 0; row < numberOfRows; row++ )
            {
                for ( var col = 0; col < numberOfRows; col++ )
                {
                    var tag = puzzleCellArray [row, col];
                    var button = new Button
                    {
                        FontSize = 24,
                        Tag = tag,
                        /*Style = buttonStyle,*/
                        Background = _fogBrush,
                        Height = 60,
                        Width = 56,
                        Content =tag.CellValue.ToString() ,
                    };


                    //b.Tag = tags [row, col];
                    //b.Tag = tag ;
                    //b.Style = buttonStyle;
                    button.SetValue ( RowProperty, row );
                    button.SetValue ( ColumnProperty, col );
                    //      SlidePuzzle doesn't specify Children either.

                    Children.Add ( button );
                    _puzzleGridTiles [row, col] = button;
                }
            }


        }



    }
}
