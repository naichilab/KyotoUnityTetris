//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;

namespace HimonoLib
{
    // ブロックの色
    public enum EBlockColor
    {
        None,
        White,

        Cyan,
        Yellow,
        Magenta,
        Blue,
        Orange,
        Green,
        Red,

        Black,
    }

    // テトリミノの型
    public enum ETetrimino
    {
        I,
        O,
        T,
        J,
        L,
        S,
        Z,
    }


    public static class BlockTypes
    {
        #region Variable

        private static Dictionary< ETetrimino, int[,] >    m_tetrimonoTypes    = null;

        public const int EMPTY_COLOR    = (int)EBlockColor.None;
        public const int WALL_COLOR     = (int)EBlockColor.White;
        public const int CLASH_COLOR    = (int)EBlockColor.Black;

        #endregion // Variable


        #region Field

        private static readonly int[,] DEFAULT_FIELD = new int[,]
        {
            { 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        };

        #endregion // Field


        #region Tetrimono

        // 各テトリミノの4*4の配列
        // 配列内の数字はEBlockColorと合わせる必要があります

        private static readonly int[,] I_TETRIMINO = new int[,]
        {
            {2,2,2,2},
            {0,0,0,0},
            {0,0,0,0},
            {0,0,0,0},
        };

        private static readonly int[,] O_TETRIMINO = new int[,]
        {
            {0,0,0,0},
            {0,3,3,0},
            {0,3,3,0},
            {0,0,0,0},
        };

        private static readonly int[,] T_TETRIMINO = new int[,]
        {
            {0,0,0,0},
            {0,4,0,0},
            {4,4,4,0},
            {0,0,0,0},
        };

        private static readonly int[,] J_TETRIMINO = new int[,]
        {
            {0,0,0,0},
            {5,0,0,0},
            {5,5,5,0},
            {0,0,0,0},
        };

        private static readonly int[,] L_TETRIMINO = new int[,]
        {
            {0,0,0,0},
            {0,0,6,0},
            {6,6,6,0},
            {0,0,0,0},
        };

        private static readonly int[,] S_TETRIMINO = new int[,]
        {
            {0,0,0,0},
            {0,7,7,0},
            {7,7,0,0},
            {0,0,0,0},
        };

        private static readonly int[,] Z_TETRIMINO = new int[,]
        {
            {0,0,0,0},
            {8,8,0,0},
            {0,8,8,0},
            {0,0,0,0},
        };


        #endregion // Tetrimono


        #region Property

        public static int[ , ] DefaultField
        {
            get
            {
                // 配列は参照渡しのため、そのままデータを渡すと元データが変更されてしまいます.
                // そのため、Clone()関数で別データにしてから渡します.
                return (int[,])DEFAULT_FIELD.Clone();
            }
        }

        #endregion // Property


        #region Public

        public static ETetrimino GetRandomTetriminoType()
        {
            var typeList    = (ETetrimino[])System.Enum.GetValues( typeof( ETetrimino ) );
            if( typeList.Length == 0 )
            {
                Debug.LogWarning( "empty ETetrimino!" );
                return default( ETetrimino );
            }

            int index   = Random.Range( 0, typeList.Length );
            return typeList[ index ];
        }

        /// <summary>
        /// 指定したテトリミノ型の配列を返す
        /// </summary>
        /// <param name="i_type">取得したテトリミノ型</param>
        /// <returns>4*4の配列 指定したテトリミノ型はない場合はnull</returns>
        public static int[ , ] GetTetriminoArray( ETetrimino i_type )
        {
            // テトリミノ型リストの初期化が行われていない場合は、初期化を行う.
            if( m_tetrimonoTypes == null )
            {
                InitializeTetriminoArray();
            }

            // テトリミノ型リストに指定された型が存在しない場合は、Warningを出してnullを返します.
            // このif文内の処理が行われた場合は、InitializeTetriminoArray()を見直す必要があります.
            if( !m_tetrimonoTypes.ContainsKey( i_type ) )
            {
                Debug.LogWarningFormat( "invalid i_type! type={0}", i_type );
                return null;
            }

            // 配列は参照渡しのため、そのままデータを渡すと元データが変更されてしまいます.
            // そのため、Clone()関数で別データにしてから渡します.
            return (int[,])m_tetrimonoTypes[ i_type ].Clone();
        }

        #endregion // Public


        #region Private

        /// <summary>
        /// テトリミノ型リストの初期化を行う
        /// </summary>
        private static void InitializeTetriminoArray()
        {
            m_tetrimonoTypes    = new Dictionary<ETetrimino, int[,]>();

            m_tetrimonoTypes.Add( ETetrimino.I, I_TETRIMINO );
            m_tetrimonoTypes.Add( ETetrimino.O, O_TETRIMINO );
            m_tetrimonoTypes.Add( ETetrimino.S, S_TETRIMINO );
            m_tetrimonoTypes.Add( ETetrimino.Z, Z_TETRIMINO );
            m_tetrimonoTypes.Add( ETetrimino.J, J_TETRIMINO );
            m_tetrimonoTypes.Add( ETetrimino.L, L_TETRIMINO );
            m_tetrimonoTypes.Add( ETetrimino.T, T_TETRIMINO );
        }

        #endregion // Private


    } // class BlockTypes

} // namespace HimonoLib

