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
    /// </summary>
    public partial class PuzzleGrid : UserControl
    {
        //  Private Data

        private PuzzleLogic puzzleLogic;
        //  Was -1; but setting for a default value.
        private int _numRows = 10;

        //  Unsure about needing these.
        private Size _masterPuzzlesize = Size.Empty;
        private UIElement _elementForPuzzle;

        public int NumRows
        {
            get => _numRows;
        }



        //  Grid Setup logic.............
        public PuzzleGrid ()
        {
            InitializeComponent ();

            //  Centralize handling of all clicks in the puzzle grid.
            AddHandler ( ButtonBase.ClickEvent, new RoutedEventHandler ( OnPuzzleButtonClick ) );
        }

        //  Was for brush tiling logic.......................
        private void OnPuzzleGridLoaded (object sender, RoutedEventArgs e)
        {

        }


        private void OnPuzzleButtonClick (object sender, RoutedEventArgs e)
        {

        }


        private void SetupThePuzzleGridStructure ()
        {


        }

        //private void CheckToSetup ()
        //{
        //  
                    //throw new InvalidOperationException ("NFI");

        //    //  Unused, will set a default numRow, visual  and PuzzleSize

        //    /*  Originally  ***************************************************
        //    //  Verify that _numRows != -1, but is a valid choice; 
        //    //  and IsApplyingStyle or a visual element has been chosen;
        //    //  and masterPuzzleSize has been chosen.  */
        //}



    }
}
