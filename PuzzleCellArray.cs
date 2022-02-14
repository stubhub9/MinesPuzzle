using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesPuzzle
{
    /// <summary>
    /// Provides an IEnumerable 2D array of the PuzzleCell model.
    /// </summary>
    class PuzzleCellArray : IEnumerable
    //, IEnumerable<PuzzleCell>
    //?? : Array, IEnumerable  ??
    {
        //  Field
        PuzzleCell [,] _puzzleCellArray;

        //  Add an indexer.
        public PuzzleCell this [int indexRow, int indexCol]
        {
            get
            {
                return _puzzleCellArray [indexRow, indexCol];
            }
            set
            {
                _puzzleCellArray [indexRow, indexCol] = (PuzzleCell) value;
            }
        }

        //  Constructor
        public PuzzleCellArray ( int row = 10, int col = 10 )
        {
            _puzzleCellArray = new PuzzleCell [row, col];

            for ( int iRow = 0; iRow < row; iRow++ )
            {
                for ( int iCol = 0; iCol < col; iCol++ )
                {
                    _puzzleCellArray [iRow, iCol] = new PuzzleCell ()
                    {
                        Col = iCol,
                        Row = iRow,
                    };
                }
            }
        }

        #region  IEnumerable  Method  Group
        public PuzzleCellArray_Enum GetEnumerator ()
        {
            return new PuzzleCellArray_Enum ( _puzzleCellArray );
        }


        IEnumerator IEnumerable.GetEnumerator ()
        {
            return (IEnumerator)GetEnumerator ();
        }
        #endregion

        #region  Public  PuzzleArray  &  Collection  Method  Group
        //TODO:??  Add PuzzleCells methods                            ??????????????????????????????????????????
        //??  Add client helper methods.                       ????????????????????????????????????????????
        public void UpdateByList ( List<PuzzleCell> cells )
        {

        }


        public void UpdateByItem ( PuzzleCell cell )
        {

        }


        public PuzzleCell ReturnItemByRowCol ( int row, int col )
        {
            return new PuzzleCell ();
        }


    }

    #endregion


    //  *****        class PuzzleCellArray_Enum : IEnumerator          *****          *****          *****          *****          class PuzzleCellArray_Enum : IEnumerator          *****           *****
    /// <summary>
    /// IEnumerator helper class
    /// </summary>
    public class PuzzleCellArray_Enum : IEnumerator
    {
        #region   Fields
        public PuzzleCell [,] _puzzleCells;
        int positionRow = -1;
        int positionCol = -1;
        #endregion



        #region   IEnumerator Property Group
        public PuzzleCell Current
        {
            get
            {
                try
                {
                    return _puzzleCells [positionRow, positionCol];
                }
                catch ( IndexOutOfRangeException )
                {
                    throw new InvalidOperationException ();
                }
            }
        }


        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }
        #endregion


        //  *****          Constructor          *****          *****          *****          *****          *****          *****           *****          Constructor          *****           *****
        public PuzzleCellArray_Enum ( PuzzleCell [,] array )
        {
            _puzzleCells = array;
        }


        #region  IEnumerator  Method  Group
        public bool MoveNext ()
        {
            if ( positionRow == -1 )
            {
                positionRow = 0;
            }

            positionCol++;
            if ( positionCol == _puzzleCells.GetLength ( 1 ) )
            {
                positionCol = 0;
                positionRow++;
            }


            return ( positionRow < _puzzleCells.GetLength ( 0 ) );
        }


        public void Reset ()
        {
            positionRow = -1;
            positionCol = -1;
        }
        #endregion
    }

}
