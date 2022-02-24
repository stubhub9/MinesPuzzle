using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesPuzzle
{
    /// <summary>
    /// TODO:  Develop and Integrate:  Provides an IEnumerable 2D array of the PuzzleCell model.
    /// </summary>
    class PuzzleCellArray : ICollection<PuzzleCell>,  IEnumerable, IEnumerable<PuzzleCell>
    //?? : Array  ??

    //  IPuzzleCells Collection:
    //  Properties:  RowCount, ColCount  (?xy?)
    //  Write/ Update from List<Puzz>)  & a
    //  Read with List<Puzz> return
    {

        //  Field
        readonly PuzzleCell [,] _puzzleCellArray;

        public int Count =>   (int)_puzzleCellArray.LongLength;

        public bool IsReadOnly => throw new NotImplementedException ();

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

        #region  IEnumerables  Method  Group
        public PuzzleCellArray_Enumerator GetEnumerator ()
        {
            return new PuzzleCellArray_Enumerator ( _puzzleCellArray );
        }


        IEnumerator IEnumerable.GetEnumerator ()
        {
            return (IEnumerator)GetEnumerator ();
        }


        IEnumerator<PuzzleCell> IEnumerable<PuzzleCell>.GetEnumerator ()
        {
            return (IEnumerator <PuzzleCell>)GetEnumerator ();
            //throw new NotImplementedException ();
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

        public void Add ( PuzzleCell item )
        {
            throw new NotImplementedException ();
        }

        public void Clear ()
        {
            throw new NotImplementedException ();
        }

        public bool Contains ( PuzzleCell item )
        {
            throw new NotImplementedException ();
        }

        public void CopyTo ( PuzzleCell [] array, int arrayIndex )
        {
            throw new NotImplementedException ();
        }

        public bool Remove ( PuzzleCell item )
        {
            throw new NotImplementedException ();
        }
    }

    #endregion


    //  *****        class PuzzleCellArray_Enum : IEnumerator          *****          *****          *****          *****          class PuzzleCellArray_Enum : IEnumerator          *****           *****
    /// <summary>
    /// IEnumerator helper class
    /// </summary>
    public class PuzzleCellArray_Enumerator : IEnumerator, IEnumerator<PuzzleCell>
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
        public PuzzleCellArray_Enumerator ( PuzzleCell [,] array )
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



        //  For IEnumerable<T>
        void IDisposable.Dispose () { }
    }


}
