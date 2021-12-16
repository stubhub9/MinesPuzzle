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


        #region  Private Fields
        //  *****          Private Fields          *****          *****          *****          *****          *****        Private Fields          *****          *****          *****          ******

        /// <summary>
        ///   Enables updating (PuzzleGrid) UI buttons based on row and column.
        ///   Could inherit from Button and add enum Properties
        /// </summary>
        private Button [,] _puzzleGridTiles;
        public PuzzleLogic _puzzleLogic;

        //private int _numberOfMines;
        //private int _numberOfRows;
        //private Size _gridSize;

        ////private Brush _tileBrush_Boom;
        ////private Brush _tileBrush_Revealed;
        ////private Brush _tileBrush_Suspected;
        ////private Brush _tileBrush_Unknown;
        ////private Brush _tileBrush_Safed;
        //  End of Private Fields
        #endregion


        #region  Properties
        //  *****          Properties          *****          *****          *****          *****          *****          Properties          *****          *****          *****          *****

        //public int NumberOfRows
        //{ get => _numberOfRows; }

        //  Set by MainWindow default 500.0, 500.0
        public Size PuzzleSize
        {
            get;
            set;
        }

        #endregion


        #region  Events
        //  *****         Events          *****          *****          *****          *****          *****          Events          *****          *****          *****          *****

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
            var b = e.Source as Button;
            var row = (int)b.GetValue ( RowProperty );
            var col = (int)b.GetValue ( ColumnProperty );

            var updatedCells = _puzzleLogic.TileWasSelected ( row, col );

            if ( updatedCells.Count == 0 )
                return;

            else
            {
                foreach ( var item in updatedCells )
                {
                    var updatedTile = _puzzleGridTiles [item.Row, item.Col];
                    updatedTile.Tag = item;

                    if ( _puzzleLogic.PuzzleStatus == PuzzleStatus.GameDefeat )
                    {
                        //  This should be the mine; followed by the mine list.
                        if ( item.CellStatus == CellStatus.Boom )
                        {
                            updatedTile.Content = "";
                            updatedTile.Background = PuzzleColors.TileBrush_Boom;
                        }
                        else
                        if ( item.CellStatus == CellStatus.Suspected )
                        {
                            updatedTile.Content = "?";
                            updatedTile.Background = PuzzleColors.TileBrush_Safed;

                        }
                        else
                        {
                            updatedTile.Content = "*";
                            updatedTile.Background = PuzzleColors.TileBrush_Mined;
                            ////UpdateTiles_IsVictory ( false );
                        }
                    }
                    //  End of Defeat branch.

                    //else if ( _puzzleLogic.PuzzleStatus == PuzzleStatus.GameVictory )
                    //{

                    //}
                    else
                    {
                        updatedTile.Content = ( (int)item.CellValue ).ToString ();
                        updatedTile.Background = PuzzleColors.TileBrush_Revealed;
                        //updatedTile.Background = _tileBrush_Revealed;
                    }
                }
            }
            if ( ( _puzzleLogic.PuzzleStatus == PuzzleStatus.GameVictory ) || ( _puzzleLogic.PuzzleStatus == PuzzleStatus.GameDefeat ) )
            { UpdateTiles_IsVictory ( _puzzleLogic.PuzzleStatus == PuzzleStatus.GameVictory ); }
            //  End OnPuzzleButtonClick
        }

        private void UpdateTiles_IsVictory ( bool isVictory )
        {
            foreach ( var button in _puzzleGridTiles )
            {
                if ( isVictory )
                {
                    var tag = (PuzzleCell)button.Tag;
                    if ( tag.CellValue == CellValue.Mine )
                    {   button.Background = PuzzleColors.TileBrush_Safed;  }

                    else
                    { button.Background = PuzzleColors.TileBrush_Victory; }
                }

                else
                {
                    if ( button.Background == PuzzleColors.TileBrush_Unknown )
                    {
                        button.Background = PuzzleColors.TileBrush_Defeat;
                    }
                }
            }
        }



        //  Flag or unflag a hidden cell as being presumed as a mine. 
        private void OnPuzzleButtonRightClick ( object sender, RoutedEventArgs e )
        {
            var button = e.Source as Button;
            var row = (int)button.GetValue ( RowProperty );
            var col = (int)button.GetValue ( ColumnProperty );
            var updatedCell = _puzzleLogic.TileWasRightClicked ( row, col );
            button.Tag = updatedCell;

            if ( updatedCell.CellStatus == CellStatus.Suspected )
            { button.Background = PuzzleColors.TileBrush_Suspected; }
            else
            { button.Background = PuzzleColors.TileBrush_Unknown; }

            //b.Background = ( updatedCell.CellStatus == CellStatus.Suspected ) ? PuzzleColors.TileBrush_Suspected : PuzzleColors.TileBrush_Unknown;
            button.Content = ( updatedCell.CellStatus == CellStatus.Suspected ) ? "?" : "";
        }



        private void PuzzleGridLoaded ( object sender, RoutedEventArgs e )
        {

        }
        //  End of Events &  Methods group
        #endregion

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



        #region  Constructor Method Group
        //  *****          Constructor          *****          *****          *****          *****          *****          Constructor          *****          *****          *****

        public PuzzleGrid ( Size gridSize, int numberOfRows, int numberOfMines )
        {
            InitializeComponent ();
            Width = gridSize.Width;
            Height = gridSize.Height;

            //  Centralize handling of all clicks in PuzzleGrid.
            AddHandler ( ButtonBase.ClickEvent, new RoutedEventHandler ( OnPuzzleButtonClick ) );
            AddHandler ( ButtonBase.MouseRightButtonDownEvent, new RoutedEventHandler ( OnPuzzleButtonRightClick ) );

            _puzzleLogic = new PuzzleLogic ( numberOfRows, numberOfMines );


            SetupThePuzzleGridStructure ( numberOfRows );
        }





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

            var puzzleCellArray = _puzzleLogic.PuzzleCellArray;
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
                        Background = PuzzleColors.TileBrush_Unknown,
                        Height = 30,
                        Width = 30,
                        /*TODO:  Remove troubleshooting. */
                        //Content = ((int) tag.CellValue).ToString (),
                        Margin = new Thickness ( 1, 1, 1, 1 ),
                    };
                    //////TODO:  Remove TEST
                    ////if ( tag.CellValue == CellValue.Mine )
                    ////{
                    ////    button.Background = PuzzleColors.TileBrush_Boom;
                    ////    button.Content = "";
                    ////}

                    button.SetValue ( RowProperty, row );
                    button.SetValue ( ColumnProperty, col );
                    //      SlidePuzzle doesn't specify Children (shared property?) either.

                    Children.Add ( button );
                    _puzzleGridTiles [row, col] = button;
                }
            }

            //  End SetupThePuzzleGridStructure method.
        }
        #endregion


    }
}
