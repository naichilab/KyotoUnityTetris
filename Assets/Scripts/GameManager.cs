//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace HimonoLib
{
    public class GameManager : MonoBehaviour
    {

        #region Variable

        [SerializeField, Range( 0.0f, 5.0f )]
        private float m_clearEffectTime = 0.0f;
        [SerializeField, Range( 0.0f, 2.0f )]
        private float m_clearEffectIntervalTime = 0.0f;

        [SerializeField]
        private Renderer    m_blockPrefab   = null;

        private BlockViewer m_viewer    = new BlockViewer();

        private int[,]      m_field     = null;

        #endregion // Variable


        #region Fixed

        private const int   START_X = 5;
        private const int   START_Y = 0;

        #endregion // Fixed


        #region UnityMessage

        void Awake()
        {
            m_field = BlockTypes.DefaultField;
            m_viewer.Initialize( m_field.Length, m_blockPrefab, transform );
        }

        void Start()
        {
            StartCoroutine( InitializeState() );
        }

        void Update()
        {
            m_viewer.RenderBlock( m_field );
        }

        #endregion // UnityMessage


        #region State

        /// <summary>
        /// 初期化状態
        /// </summary>
        /// <returns></returns>
        private IEnumerator InitializeState()
        {
            yield return new WaitForSeconds( 1.0f );

            m_field = BlockTypes.DefaultField;

            StartCoroutine( GameState() );
            yield return null;
        }

        /// <summary>
        /// ゲーム
        /// </summary>
        /// <returns></returns>
        private IEnumerator GameState()
        {
            while( true )
            {
                // 新ブロック生成
                var sourceField = (int[ , ])m_field.Clone();
                var tetrimino = new Tetrimino( START_X, START_Y );

                // 生成位置にブロックが置けなければゲームオーバー
                if( !SetArray( ref m_field, sourceField, tetrimino ) )
                {
                    break;
                }

                yield return StartCoroutine( ControlTetriminoCoroutine( sourceField, tetrimino ) );
                yield return StartCoroutine( ClearTetriminoCoroutine() );
            }
            
            StartCoroutine( GameOverState() );
        }

        private IEnumerator ControlTetriminoCoroutine( int[ , ] i_sourceField, Tetrimino i_tetrimino )
        {
            while( SetArray( ref m_field, i_sourceField, i_tetrimino ) )
            {
                float startTime = Time.timeSinceLevelLoad;
                bool selfFallen = false;
                while( Time.timeSinceLevelLoad - startTime < 0.5f && !selfFallen )
                {
                    int     offsetX     = 0;
                    int     offsetY     = 0;

                    if( Input.GetKeyDown( KeyCode.LeftArrow ) )
                    {
                        offsetX--;
                    }

                    if( Input.GetKeyDown( KeyCode.RightArrow ) )
                    {
                        offsetX++;
                    }

                    if( Input.GetKeyDown( KeyCode.DownArrow ) )
                    {
                        offsetY++;
                    }

                    if( Input.GetKeyDown( KeyCode.UpArrow ) )
                    {
                        i_tetrimino.Rotate( ETetriminoRotation.Left );

                        if( !SetArray( ref m_field, i_sourceField, i_tetrimino, offsetX, 0 ) )
                        {
                            i_tetrimino.Rotate( ETetriminoRotation.Right );
                        }
                    }

                    if( offsetX != 0 )
                    {
                        if( SetArray( ref m_field, i_sourceField, i_tetrimino, offsetX, 0 ) )
                        {
                            i_tetrimino.PositionX += offsetX;
                        }
                    }
                    
                    selfFallen = offsetY > 0;
                    yield return null;
                }

                i_tetrimino.PositionY++;
            }
        }

        private IEnumerator ClearTetriminoCoroutine()
        {
            var clearLineList   = GetClearLineList( m_field );
            if( clearLineList == null || clearLineList.Length == 0 )
            {
                yield break;
            }

            var normalArray = (int[ , ] ) m_field.Clone();
            var effectArray = ChangeIndex( m_field, (int)EBlockColor.White, clearLineList );


            var blinkCoroutine  = StartCoroutine( BlinkTetriminoCoroutine( normalArray, effectArray, m_clearEffectIntervalTime ) );
            yield return new WaitForSeconds( m_clearEffectTime );
            StopCoroutine( blinkCoroutine );

            ClearLine( clearLineList );




//             for( int y = m_field.GetLength( 0 ) - 1; y >= 0; )
//             {
//                 bool isClear = true;
//                 for( int x = 1, sizeX = m_field.GetLength( 1 ) - 1; x < sizeX; ++x )
//                 {
//                     var colorIndex = (EBlockColor)m_field[ y, x ];
//                     if( colorIndex == EBlockColor.None || colorIndex == EBlockColor.White )
//                     {
//                         isClear = false;
//                         break;
//                     }
//                 }
// 
//                 if( isClear )
//                 {
//                     ClearLine( y );
//                 }
//                 else
//                 {
//                     --y;
//                 }
//             }
        }

        private IEnumerator BlinkTetriminoCoroutine( int[ , ] i_normalArray, int[ , ] i_effectArray, float i_intervalTime )
        {
            bool isEffect   = true;

            m_field = i_effectArray;
            while( true )
            {
                yield return new WaitForSeconds( i_intervalTime );

                isEffect    = !isEffect;
                m_field     = isEffect ? i_effectArray : i_normalArray;
            }
        }

        private IEnumerator GameOverState()
        {
            for( int y = m_field.GetLength( 0 ) - 1; y >= 0; --y )
            {
                for( int x = 0, sizeX = m_field.GetLength( 1 ) ; x < sizeX; ++x )
                {
                    var colorIndex = (EBlockColor)m_field[ y, x ];
                    if( colorIndex != EBlockColor.None && colorIndex != EBlockColor.White )
                    {
                        m_field[ y, x ] = (int)EBlockColor.Black;
                    }
                }

                yield return new WaitForSeconds( 0.05f );
            }


        }

        #endregion // State


        #region Private

        /// <summary>
        /// 指定した２次元配列に指定したテトリミノの２次元配列を加える.
        /// </summary>
        /// <param name="o_destination">作成された２次元配列</param>
        /// <param name="i_sourceArray">元となる２次元配列</param>
        /// <param name="i_tetrimino">加えるテトリミノ</param>
        /// <returns>加えることができたかの真偽</returns>
        private bool SetArray( ref int[ , ] o_destination, int[ , ] i_sourceArray, Tetrimino i_tetrimino, int i_offsetX = 0, int i_offsetY = 0 )
        {
            Debug.Assert( i_tetrimino != null, "invalid i_tetrimino!" );
            return SetArray( ref o_destination, i_sourceArray, i_tetrimino.ArrayData, i_tetrimino.PositionX + i_offsetX, i_tetrimino.PositionY + i_offsetY );
        }

        /// <summary>
        /// 指定した２次元配列をもとに指定した位置に２次元配列を加える
        /// </summary>
        /// <param name="o_destination">作成された２次元配列</param>
        /// <param name="i_sourceArray">元となる２次元配列</param>
        /// <param name="i_addArray">加えたい２次元配列</param>
        /// <param name="i_offsetX">加える２次元配列のオフセットx位置</param>
        /// <param name="i_offsetY">加える２次元配列のオフセットy位置</param>
        /// <returns></returns>
        private bool SetArray( ref int[ , ] o_destination, int[ , ] i_sourceArray, int[ , ] i_addArray, int i_offsetX = 0, int i_offsetY = 0 )
        {
            Debug.Assert( i_sourceArray != null, "invalid i_sourceArray!" );
            Debug.Assert( i_addArray != null, "invalid i_addArray!" );

            var destination = (int[ , ])i_sourceArray.Clone();

            for( int y = 0, sizeY = i_addArray.GetLength( 0 ); y < sizeY; ++y )
            {
                for( int x = 0, sizeX = i_addArray.GetLength( 1 ); x < sizeX; ++x )
                {
                    int value   = i_addArray[ y, x ];

                    // 空のブロックを当てはめる場合は、元がどのブロックでも当てはめれるためスルー.
                    if( value == BlockTypes.EMPTY_COLOR )
                    {
                        continue;
                    }

                    int posX    = x + i_offsetX;
                    int posY    = y + i_offsetY;

                    // 配列外に設定しようとした場合は加えることができないと判断する.
                    if( IsOutOfRange( destination, posX, posY ) )
                    {
                        return false;
                    }

                    // 指定した２次元配列の場所が空のブロックでないのなら設定できないと判断する.
                    int sourceValue = destination[ posY, posX ];
                    if( sourceValue != BlockTypes.EMPTY_COLOR )
                    {
                        return false;
                    }

                    destination[ posY, posX ]   = value;
                }
            }

            o_destination   = destination;

            return true;
        }

        /// <summary>
        /// 指定した位置が２次元配列内に収まるのかを調べる
        /// </summary>
        /// <param name="i_array">調べる２次元配列内</param>
        /// <param name="i_x">x位置</param>
        /// <param name="i_y">y位置</param>
        /// <returns></returns>
        private bool IsOutOfRange( int[ , ] i_array, int i_x, int i_y )
        {
            Debug.Assert( i_array != null, "invalid i_array!" );

            if( i_y < 0 )
            {
                return true;
            }

            if( i_x < 0 )
            {
                return true;
            }

            if( i_y >= i_array.GetLength( 0 ) )
            {
                return true;
            }

            if( i_x >= i_array.GetLength( 1 ) )
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 揃っている列を取得する
        /// </summary>
        /// <param name="i_array">調べる2次元配列</param>
        /// <returns>揃っている列のインデックスの配列</returns>
        private int[] GetClearLineList( int[ , ] i_array )
        {
            Debug.Assert( i_array != null, "invalid i_array!" );

            var clearList   = new List< int >();

            for( int y = 0, sizeY = i_array.GetLength( 0 ); y < sizeY; ++y )
            {
                bool isClear = false;
                for( int x = 0, sizeX = i_array.GetLength( 1 ); x < sizeX; ++x )
                {
                    int index   = i_array[ y, x ];

                    if( index == BlockTypes.WALL_COLOR )
                    {
                        continue;
                    }

                    if( index == BlockTypes.EMPTY_COLOR )
                    {
                        isClear = false;
                        break;
                    }

                    // 1列全て壁だった場合の対応として、1つでも通常のブロックがある場合のみクリア可能フラグを立てるようにする.
                    isClear = true;
                }

                if( isClear )
                {
                    clearList.Add( y );
                }
            }

            return clearList.ToArray();
        }

        /// <summary>
        /// 指定したインデックスに変更する.
        /// 壁インデックスを無視する.
        /// </summary>
        /// <param name="i_sourceArray">元なる2次元配列</param>
        /// <param name="i_changedIndex">変更するインデックス</param>
        /// <param name="i_lines">変更する列のリスト</param>
        /// <returns>変更事2次元配列</returns>
        private int[ , ] ChangeIndex( int [ , ] i_sourceArray, int i_changedIndex, params int[] i_lines )
        {
            Debug.Assert( i_sourceArray != null, "invalid i_sourceArray!" );

            if( i_lines == null || i_lines.Length == 0 )
            {
                return i_sourceArray;
            }

            var destinationArray    = (int[ , ])i_sourceArray.Clone();

            for( int y = 0, sizeY = i_sourceArray.GetLength( 0 ); y < sizeY; ++y )
            {
                // Linqを使って、指定した列以外の列は弾く.
                if( !i_lines.Any( value => value == y ) )
                {
                    continue;
                }

                for( int x = 0, sizeX = i_sourceArray.GetLength( 1 ); x < sizeX; ++x )
                {
                    if( destinationArray[ y, x ] == BlockTypes.WALL_COLOR )
                    {
                        continue;
                    }
                    destinationArray[ y, x ]    = i_changedIndex;
                }
            }

            return destinationArray;
        }

        /// <summary>
        /// 指定した列を消してそれ以上上にある列を下に詰める
        /// </summary>
        /// <param name="i_lines"></param>
        private void ClearLine( params int[] i_lines )
        {
            if( i_lines == null || i_lines.Length == 0 )
            {
                return;
            }

            // 昇順に並べ替える.
            // 画面から見て上の列から消していかないと、消すべき列のインデックスがずれてしまうから.
            var lines = i_lines.OrderBy( value => value );

            foreach( int clearLine in lines )
            {
                for( int y = clearLine; y > 0; --y )
                {
                    for( int x = 0, sizeX = m_field.GetLength( 1 ); x < sizeX; ++x )
                    {
                        if( m_field[ y, x ] == BlockTypes.WALL_COLOR ||
                            m_field[ y - 1, x ] == BlockTypes.WALL_COLOR )
                        {
                            continue;
                        }
                        m_field[ y, x ] = m_field[ y - 1, x ];
                    }
                }
            }
        }

        #endregion // Private

    } // class GameManager

} // namespace HimonoLib

