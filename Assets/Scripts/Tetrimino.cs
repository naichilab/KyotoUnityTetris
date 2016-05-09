//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;

namespace HimonoLib
{
    /// <summary>
    /// テトリミノ回転方向
    /// </summary>
    public enum ETetriminoRotation
    {
        Right,
        Left,
    }

    public class Tetrimino
    {
        #region Constructor

        /// <summary>
        /// 作りテトリミノ型を指定しない場合は、ランダムで設定される
        /// </summary>
        public Tetrimino( int i_x, int i_y )
            : this( BlockTypes.GetRandomTetriminoType(), i_x, i_y )
        {

        }

        public Tetrimino( ETetrimino i_type, int i_x, int i_y )
        {
            // テトリミノ型に合わせて4*4の配列を取得する.
            // 配列が取得できない場合はゲームにならないので、Assert()でプログラムを強制的に止める.
            ArrayData   = BlockTypes.GetTetriminoArray( i_type );
            Debug.AssertFormat( ArrayData != null && ArrayData.Length > 0, "missing tetrimino arrayData! type={0}", i_type );

            PositionX   = i_x;
            PositionY   = i_y;
        }

        #endregion // Constructor


        #region Property

        /// <summary>
        /// 配列情報
        /// </summary>
        public int[ ,] ArrayData
        {
            get;
            private set;
        }

        /// <summary>
        /// x座標
        /// </summary>
        public int PositionX
        {
            get;
            set;
        }

        /// <summary>
        /// y座標
        /// </summary>
        public int PositionY
        {
            get;
            set;
        }

        /// <summary>
        /// 横幅
        /// </summary>
        public int Width
        {
            get
            {
                return ArrayData != null ? ArrayData.GetLength( 1 ) : 0;
            }
        }

        /// <summary>
        /// 縦幅
        /// </summary>
        public int Height
        {
            get
            {
                return ArrayData != null ? ArrayData.GetLength( 0 ) : 0;
            }
        }

        #endregion // Property


        #region Public

        /// <summary>
        /// 回転する
        /// </summary>
        /// <param name="i_type">回転方向タイプ</param>
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

        /// <summary>
        /// 左右反転
        /// </summary>
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

        /// <summary>
        /// 上下反転
        /// </summary>
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

        /// <summary>
        /// 転置
        /// </summary>
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

