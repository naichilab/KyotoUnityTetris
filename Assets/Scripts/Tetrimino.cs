//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;

namespace HimonoLib
{
    public enum ETetriminoRotation
    {
        Right,
        Left,
    }


    public class Tetrimino
    {
        #region Constructor

        public Tetrimino()
            : this( BlockTypes.GetRandomTetriminoType() )
        {

        }

        public Tetrimino( ETetrimino i_type )
        {
            ArrayData   = BlockTypes.GetTetriminoArray( i_type );
            Debug.AssertFormat( ArrayData != null && ArrayData.Length > 0, "missing tetrimino arrayData! type={0}", i_type );
        }

        #endregion // Constructor


        #region Variable

        #endregion // Variable




        #region Property

        public int[ ,] ArrayData
        {
            get;
            private set;
        }

        public int PositionX
        {
            get;
            set;
        }

        public int PositionY
        {
            get;
            set;
        }

        public int Width
        {
            get
            {
                return ArrayData != null ? ArrayData.GetLength( 1 ) : 0;
            }
        }

        public int Height
        {
            get
            {
                return ArrayData != null ? ArrayData.GetLength( 0 ) : 0;
            }
        }

        #endregion // Property


        #region Public

        public void Rotate( ETetriminoRotation i_type )
        {
            switch( i_type )
            {
                case ETetriminoRotation.Left:
                    Transposed();
                    VFlip();
                    break;

                case ETetriminoRotation.Right:
                    VFlip();
                    Transposed();
                    break;

                default:
                    Debug.LogAssertionFormat( "invalid type! type={0}", i_type );
                    break;
            }

        }

        #endregion // Public


        #region Private

        private void HFlip()
        {
            for( int x = 0, sizeX = Width; x < sizeX; ++x )
            {
                for( int y = 0, sizeY = Height / 2; y < sizeY; ++y )
                {
                    int bottom  = Height - 1 - x;
                    int temp    = ArrayData[ y, x ];
                    ArrayData[ y, x ]       = ArrayData[ bottom, x ];
                    ArrayData[ bottom, x ]  = temp;

                }
            }
        }

        private void VFlip()
        {
            for( int y = 0, sizeY = Height; y < sizeY; ++y )
            {
                for( int x = 0, sizeX = Width / 2; x < sizeX; ++x )
                {
                    int right   = Width - 1 - x;
                    int temp    = ArrayData[ y, x ];
                    ArrayData[ y, x ]       = ArrayData[ y, right ];
                    ArrayData[ y, right ]   = temp;
                }
            }
        }

        private void Transposed()
        {
            for( int x = 0, sizeX = Width; x < sizeX; ++x )
            {
                for( int y = x, sizeY = Height; y < sizeY; ++y )
                {
                    int temp    = ArrayData[ x, y ];
                    ArrayData[ x, y ]   = ArrayData[ y, x ];
                    ArrayData[ y, x ]   = temp;

                }
            }
        }

        #endregion // Private


    } // class Tetrimino
    
} // namespace HimonoLib

