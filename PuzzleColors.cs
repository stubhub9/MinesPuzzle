using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MinesPuzzle
{
    static class PuzzleColors
    {

        #region  Private  Fields
        //  *****          Private    Fields          *****          *****          *****          *****          *****          Private    Fields          *****          *****          
        static Color _color_Unknown = Color.FromRgb ( 60, 80, 170 );
        static Color _color_Defeat = Color.FromRgb ( 180, 80, 40 );
        static Color _color_Victory = Color.FromRgb ( 140, 225, 255 );


        static readonly SolidColorBrush _tileBrush_Revealed = new SolidColorBrush ( Color.FromRgb ( 130, 150, 240 ) );
        static readonly SolidColorBrush _tileBrush_Unknown = new SolidColorBrush ( Color.FromRgb ( 60, 80, 170 ) );
        static readonly SolidColorBrush _tileBrush_Defeat = new SolidColorBrush ( _color_Defeat );
        static readonly SolidColorBrush _tileBrush_Victory = new SolidColorBrush ( _color_Victory );

        static readonly LinearGradientBrush _tileBrush_Safed;
        static readonly RadialGradientBrush _tileBrush_Boom;
        static readonly RadialGradientBrush _tileBrush_Suspected;
        //  End of Private Fields.
        #endregion

        #region  Properties
        //  *****          Properties          *****          *****          *****          *****          *****          Properties          *****          *****          *****        

        static public Color Color_Defeat
        { get => _color_Defeat; }

        static public Color Color_Unknown
        { get => _color_Unknown; }

        static public Color Color_Victory
        { get => _color_Victory; }

        static public RadialGradientBrush TileBrush_Boom
        { get => _tileBrush_Boom; }

        static public RadialGradientBrush TileBrush_Suspected
        { get => _tileBrush_Suspected; }

        static public LinearGradientBrush TileBrush_Mined
        { get => _tileBrush_Safed; }

        static public LinearGradientBrush TileBrush_Safed
        { get => _tileBrush_Safed; }

        static public SolidColorBrush TileBrush_Revealed
        { get => _tileBrush_Revealed; }

        static public SolidColorBrush TileBrush_Unknown
        { get => _tileBrush_Unknown; }

        static public SolidColorBrush TileBrush_Defeat
        { get => _tileBrush_Defeat; }

        static public SolidColorBrush TileBrush_Victory
        { get => _tileBrush_Victory; }

        #endregion

        #region  Static Constructor
        //  *****          Static    Constructor          *****          *****          *****          *****          *****          Static    Constructor          *****          *****          *****          *****

        static PuzzleColors ()
        {

            #region  Gradstops
            var gradStops_Boom = new GradientStopCollection ();
            var gradStop = new GradientStop ( Colors.Red, 1.0 );
            gradStops_Boom.Add ( gradStop );
            gradStop = new GradientStop ( Colors.Yellow, .9 );
            gradStops_Boom.Add ( gradStop );
            gradStop = new GradientStop ( Colors.Red, .8 );
            gradStops_Boom.Add ( gradStop );
            gradStop = new GradientStop ( Colors.Yellow, .7 );
            gradStops_Boom.Add ( gradStop );
            gradStop = new GradientStop ( Colors.Red, .5 );
            gradStops_Boom.Add ( gradStop );
            gradStop = new GradientStop ( Colors.Yellow, .3 );
            gradStops_Boom.Add ( gradStop );


            var gradStops_Suspected = new GradientStopCollection ();
            gradStop = new GradientStop ( Color.FromRgb ( 60, 80, 170 ), .9 );
            gradStops_Suspected.Add ( gradStop );
            gradStop = new GradientStop ( Color.FromRgb ( 100, 120, 190 ), .7 );
            gradStops_Suspected.Add ( gradStop );


            var gradStops_Safed = new GradientStopCollection ();
            gradStop = new GradientStop ( Color.FromRgb ( 60, 80, 170 ), 1.0 );
            gradStops_Safed.Add ( gradStop );
            gradStop = new GradientStop ( Color.FromRgb ( 60, 80, 170 ), .82 );
            gradStops_Safed.Add ( gradStop );
            gradStop = new GradientStop ( Color.FromRgb ( 255, 225, 70 ), .8 );
            gradStops_Safed.Add ( gradStop );
            gradStop = new GradientStop ( Color.FromRgb ( 60, 80, 170 ), .72 );
            gradStops_Safed.Add ( gradStop );
            gradStop = new GradientStop ( Color.FromRgb ( 60, 80, 170 ), .62 );
            gradStops_Safed.Add ( gradStop );
            gradStop = new GradientStop ( Color.FromRgb ( 255, 225, 70 ), .6 );
            gradStops_Safed.Add ( gradStop );
            gradStop = new GradientStop ( Color.FromRgb ( 60, 80, 170 ), .58 );
            gradStops_Safed.Add ( gradStop );
            gradStop = new GradientStop ( Color.FromRgb ( 60, 80, 170 ), .42 );
            gradStops_Safed.Add ( gradStop );
            gradStop = new GradientStop ( Color.FromRgb ( 255, 225, 70 ), .4 );
            gradStops_Safed.Add ( gradStop );
            gradStop = new GradientStop ( Color.FromRgb ( 60, 80, 170 ), .38 );
            gradStops_Safed.Add ( gradStop );
            gradStop = new GradientStop ( Color.FromRgb ( 60, 80, 170 ), .22 );
            gradStops_Safed.Add ( gradStop );
            gradStop = new GradientStop ( Color.FromRgb ( 255, 225, 70 ), .2 );
            gradStops_Safed.Add ( gradStop );
            gradStop = new GradientStop ( Color.FromRgb ( 60, 80, 170 ), .18 );
            gradStops_Safed.Add ( gradStop );
            gradStop = new GradientStop ( Color.FromRgb ( 60, 80, 170 ), 0.0 );
            gradStops_Safed.Add ( gradStop );
            //  End of GradStops
            #endregion

            //var _color_Unknown = Color.FromRgb ( 60, 80, 170 );
            //var _color_Defeat = Color.FromRgb ( 180, 80, 40 );
            //var _color_Victory = Color.FromRgb ( 120, 140, 210 );


            _tileBrush_Safed = new LinearGradientBrush ( gradStops_Safed );
            _tileBrush_Boom = new RadialGradientBrush ( gradStops_Boom );
            _tileBrush_Suspected = new RadialGradientBrush ( gradStops_Suspected );
            //_tileBrush_Revealed = new SolidColorBrush ( Colors.CornflowerBlue );
            //_tileBrush_Unknown = new SolidColorBrush ( Color.FromRgb ( 60, 80, 170 ) );

        }
        //  End of Static Constructor
        #endregion









    }
}
