using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesPuzzle
{
    /// <summary>
    ///  Provides data for PuzzleGrid button tags and the PuzzleLogic matrix.
    ///  ___int Row, Col,
    ///  ___enum CellValue, CellStatus
    /// </summary>
    public struct PuzzleCell : IEquatable <PuzzleCell> , IComparable<PuzzleCell>
        //  IEquatable for List.Contains, ...; cell equality based on Row & Col of item.
    {
        public int Row { get; set; }
        public int Col { get; set; }
        internal CellValue CellValue { get; set; }
        public CellStatus CellStatus { get; set; }

        //  Constructors  ******************************************************************************

        //  Used for mine placement.
        internal PuzzleCell ( int row, int col, CellValue value, CellStatus status )
        {
            Row = row;
            Col = col;
            CellValue = value;
            CellStatus = status;
        }

        #region  Interface Methods Group
        //  Implement IComparable<this> based on Row and Col as primary keys.
        public int CompareTo ( PuzzleCell other )
        {
            //if ((this.Row*1000 + this.Col) < (other.Row * 1000 + other.Col))
            if ( this.Row < other.Row )
            {
                return -1;
            }
            else if ( this.Row > other.Row )
            {
                return 1;
            }
            else if ( this.Col > other.Col )
            {
                return 1;
            }
            else if ( this.Col < other.Col )
            {
                return -1;
            }
            else return 0;
        }


        //  Implement IEquatable
        public bool Equals ( PuzzleCell other )
        {
            return ( ( this.Row == other.Row ) && ( this.Col == other.Col ) );

            //throw new NotImplementedException ();

            //  ?? Because PuzzleCell is a struct, checking:
            //  "other== null" was an error?? 
            //  and "if ( other is PuzzleCell )" isn't neccesary.  ??
        }

        public override bool Equals ( object otherObject )
        {
            //Error:  PuzzleCell is non-nullable.
            //PuzzleCell other = otherObject as PuzzleCell;
            if ( otherObject is PuzzleCell )
            {
                PuzzleCell other = (PuzzleCell) otherObject;
                return ( ( this.Row == other.Row ) && ( this.Col == other.Col ) );
            }
            else
            { return false; }

        }

        //  Provide hashcode that tracks with Equals (object).
        public override int GetHashCode ()
        {
            return Tuple.Create ( Row, Col ).GetHashCode ();
            //return base.GetHashCode ();

        }

        public static bool operator == ( PuzzleCell cell1, PuzzleCell cell2 )
        {
            return ( ( cell1.Row == cell2.Row ) && ( cell1.Col == cell2.Col ) );

        }

        public static bool operator != ( PuzzleCell cell1, PuzzleCell cell2 )
        {
            return !( ( cell1.Row == cell2.Row ) && ( cell1.Col == cell2.Col ) );

        }
        //  End of IEquatable Method Group
        #endregion


    }
}
