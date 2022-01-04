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
        ///   Could inherit from Button and add enum Properties.
        /// </summary>
        private Button [,] _puzzleGridTiles;

        PuzzleLogic _puzzleLogic;

        #endregion


        #region  Properties
        //  *****          Properties          *****          *****          *****          *****          *****          Properties          *****          *****          *****          *****

        //  Set by MainWindow default 500.0, 500.0
        public Size PuzzleSize
        {
            get;
            set;
        }


        public PuzzleLogic ThePuzzleLogic
        { get => _puzzleLogic; }

        #endregion


        #region  Events
        //  *****         Events          *****          *****          *****          *****          *****          Events          *****          *****          *****          *****

        //  Event Handlers  *********************************************************************************************************************

        private void OnPuzzleButtonClick ( object sender, RoutedEventArgs e )
        {
            //  ?Only buttons are clickable, in the PuzzleGrid; so var b is always button.
            var b = e.Source as Button;
            var row = (int)b.GetValue ( RowProperty );
            var col = (int)b.GetValue ( ColumnProperty );
            _puzzleLogic.TileWasSelected ( row, col );
        }




        //  Flag or unflag a hidden cell as being presumed as a mine. 
        private void OnPuzzleButtonRightMouseDown ( object sender, RoutedEventArgs e )
        {
            Button button = e.Source as Button;

            //  Mouse right button down, was targeting non-button elements; re grid through margins..
            if ( button is null )
            { return; }

            var row = (int)button.GetValue ( RowProperty );
            var col = (int)button.GetValue ( ColumnProperty );
            _puzzleLogic.TileWasRightClicked ( row, col );
        }



        private void PuzzleGridLoaded ( object sender, RoutedEventArgs e )
        {
        }
        //  End of Events  Methods group
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
            AddHandler ( ButtonBase.MouseRightButtonDownEvent, new RoutedEventHandler ( OnPuzzleButtonRightMouseDown ) );

            _puzzleLogic = new PuzzleLogic ( numberOfRows, numberOfMines );
            _puzzleLogic.ThePuzzleCells.UpdateGrid += UpdateTiles;

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

                    button.SetValue ( RowProperty, row );
                    button.SetValue ( ColumnProperty, col );

                    //  Panel.Children
                    Children.Add ( button );
                    _puzzleGridTiles [row, col] = button;
                }
            }

            //  End SetupThePuzzleGridStructure method.
        }
        #endregion





        #region  Private   Methods
        //  *****       Private   Methods        *****          *****          *****          *****          *****       Private   Methods        *****          *****          *****    

        #region  UpdateTiles  Method Group
        void UpdateTiles ( object sender, PuzzleCellsEventArgs e )
        {
            var cells = e.Cells;
            if ( cells.Count == 0 )
            { return; }

            UpdateTiles_Cells ( e );

            if ( ( _puzzleLogic.GameStatus == PuzzleStatus.GameVictory ) || ( _puzzleLogic.GameStatus == PuzzleStatus.GameDefeat ) )
            {
                UpdateTiles_IsVictory ( _puzzleLogic.GameStatus == PuzzleStatus.GameVictory );
            }
        }


        void UpdateTiles_Cells ( PuzzleCellsEventArgs e )
        {
            foreach ( var cell in e.Cells )
            {
                var tile = _puzzleGridTiles [cell.Row, cell.Col];
                tile.Tag = cell;

                if ( _puzzleLogic.GameStatus == PuzzleStatus.GameDefeat )
                { UpdateTiles_Boom ( tile, cell ); }

                else
                {
                    switch ( cell.CellStatus )
                    {
                        case CellStatus.Hidden:
                            tile.Background = PuzzleColors.TileBrush_Unknown;
                            tile.Content = "";
                            break;
                        case CellStatus.Suspected:
                            tile.Background = PuzzleColors.TileBrush_Suspected;
                            tile.Content = "?";
                            break;
                        case CellStatus.Revealed:
                        default:
                            tile.Content = ( (int)cell.CellValue ).ToString ();
                            tile.Background = PuzzleColors.TileBrush_Revealed;
                            break;
                    }
                }
                //  End of foreach.
            }
        }

        /// <summary>
        /// TODO:  REDO:  using LINQ & IEnumerables  
        /// ?save this old method as an alternate?? or is that what git is for??  re:  main branch!!!
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="cell"></param>
        void UpdateTiles_Boom ( Button tile, PuzzleCell cell )
        {
            //  This should be the mine; followed by the mine list.
            switch ( cell.CellStatus )
            {
                case CellStatus.Boom:
                    tile.Content = "";
                    tile.Background = PuzzleColors.TileBrush_Boom;
                    break;

                case CellStatus.Hidden:
                case CellStatus.Suspected:
                    tile.Content = "?";
                    tile.Background = PuzzleColors.TileBrush_Safed;
                    break;

                case CellStatus.Revealed:
                    //default:
                    tile.Content = "*";
                    tile.Background = PuzzleColors.TileBrush_Mined;
                    break;
            }
            //  End of Defeat method.
        }

        /// <summary>
        /// TODO:  REDO:  LINQ
        /// </summary>
        /// <param name="isVictory"></param>
        private void UpdateTiles_IsVictory ( bool isVictory )
        {
            foreach ( var button in _puzzleGridTiles )
            {
                var tag = (PuzzleCell)button.Tag;

                if ( isVictory )
                {

                    if ( tag.CellValue == CellValue.Mine )
                    {
                        button.Background = PuzzleColors.TileBrush_Safed;
                        button.Content = "!";
                    }

                    else
                    {
                        button.Background = PuzzleColors.TileBrush_Victory;
                        button.Content = "";
                    }
                    //  End of, isVictory.
                }

                //  else, WAS NOT Victory.
                else
                {
                    if ( button.Background == PuzzleColors.TileBrush_Unknown )
                    {
                        button.Background = PuzzleColors.TileBrush_Defeat;
                    }
                }
            }
        }
        #endregion

        #endregion


    }
}
