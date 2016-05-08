//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HimonoLib
{
    public class GameManager : MonoBehaviour
    {

        #region Variable

        [SerializeField]
        private Renderer    m_blockPrefab   = null;

        private BlockViewer m_viewer    = new BlockViewer();

        private int[,]      m_field     = null;
        private Dictionary< ETetrimino, int[,] >    m_tetrimonoTypes    = new Dictionary< ETetrimino, int[,] >();

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


        #region Private

        private int[,] ApplyField( int[,] i_field, Tetrimino i_tetrimino )
        {
            Debug.Assert( i_field != null, "invalid i_field!" );
            Debug.Assert( i_tetrimino != null, "invalid i_tetrimino!" );

            var newField    = (int[,])i_field.Clone();


            for( int x = 0, sizeX = i_tetrimino.Width; x < sizeX; ++x )
            {
                for( int y = 0, sizeY = i_tetrimino.Height; y < sizeY; ++y )
                {
                    int colorIndex  = i_tetrimino.ArrayData[ y, x ];
                    if( colorIndex == (int)EBlockColor.None )
                    {
                        continue;
                    }

                    int xIndex  = x + i_tetrimino.PositionX;
                    int yIndex  = y + i_tetrimino.PositionY;

                    if( xIndex < newField.GetLength( 1 ) && yIndex < newField.GetLength( 0 ) )
                    {
                        newField[ yIndex, xIndex ]  = colorIndex;
                    }
                }
            }
            return newField;
        }

        private bool CanMove( int[ , ] i_field, Tetrimino i_tetrimino, int i_offsetX, int i_offsetY )
        {
            Debug.Assert( i_field != null, "invalid i_field!" );
            Debug.Assert( i_tetrimino != null, "invalid i_tetrimino!" );

            for( int x = 0, sizeX = i_tetrimino.Width; x < sizeX; ++x )
            {
                for( int y = 0, sizeY = i_tetrimino.Height; y < sizeY; ++y )
                {
                    int colorIndex = i_tetrimino.ArrayData[ y, x ];
                    if( colorIndex == (int)EBlockColor.None )
                    {
                        continue;
                    }

                    int xIndex = x + i_tetrimino.PositionX + i_offsetX;
                    int yIndex = y + i_tetrimino.PositionY + i_offsetY;

                    if( xIndex >= i_field.GetLength( 1 ) ||
                        yIndex >= i_field.GetLength( 0 ) ||
                        xIndex < 0 ||
                        yIndex < 0 )
                    {
                        return false;
                    }

                    int currentColorIndex   = i_field[ yIndex, xIndex ];
                    if( currentColorIndex != (int)EBlockColor.None )
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void ClearLine( int i_lineIndex )
        {
            for( int y = i_lineIndex - 1; y >= 0; --y )
            {
                for( int x = 0, sizeX = m_field.GetLength( 1 ); x < sizeX; ++x )
                {
                    var colorIndex = m_field[ y, x ];
                    if( colorIndex != (int)EBlockColor.White && m_field[ y + 1, x ] != (int)EBlockColor.White )
                    {
                        m_field[ y + 1, x ] = colorIndex;
                    }
                    
                }
            }
        }

        #endregion // Private


        #region State

        private IEnumerator InitializeState()
        {
            yield return new WaitForSeconds( 1.0f );

            m_field = BlockTypes.DefaultField;

            StartCoroutine( GameState() );
            yield return null;
        }

        private IEnumerator GameState()
        {
            // 新ブロック生成
            var prevField = (int[ , ])m_field.Clone();

            var tetrimino = new Tetrimino();
            tetrimino.PositionX = START_X;
            tetrimino.PositionY = START_Y;

            m_field = ApplyField( prevField, tetrimino );

            // 生成位置にブロックが置けなければゲームオーバー
            if( !CanMove( prevField, tetrimino, 0, 0 ) )
            {
                StartCoroutine( GameOverState() );
                yield break;
            }


            // ブロック操作及び落下
            while( true )
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
                        tetrimino.Rotate( ETetriminoRotation.Left );
                        if( CanMove( prevField, tetrimino, offsetX, 0 ) )
                        {
                            m_field = ApplyField( prevField, tetrimino );
                        }
                        else
                        {
                            tetrimino.Rotate( ETetriminoRotation.Right );
                        }
                    }

                    if( offsetX != 0 )
                    {
                        if( CanMove( prevField, tetrimino, offsetX, 0 ) )
                        {
                            tetrimino.PositionX += offsetX;
                            m_field = ApplyField( prevField, tetrimino );
                        }
                    }
                    
                    selfFallen = offsetY > 0;
                    yield return null;
                }

                if( !CanMove( prevField, tetrimino, 0, 1 ) )
                {
                    break;
                }
                tetrimino.PositionY++;

                m_field = ApplyField( prevField, tetrimino );
            }


            {
                int clearLineCount  = 0;

                for( int y = m_field.GetLength( 0 ) - 1; y >= 0; )
                {
                    bool isClear    = true;
                    for( int x = 1, sizeX = m_field.GetLength( 1 ) - 1; x < sizeX; ++x )
                    {
                        var colorIndex = (EBlockColor)m_field[ y, x ];
                        if( colorIndex == EBlockColor.None || colorIndex == EBlockColor.White )
                        {
                            isClear = false;
                            break;
                        }
                    }

                    if( isClear )
                    {
                        ClearLine( y );
                    }
                    else
                    {
                        --y;
                    }
                }
            }

            StartCoroutine( GameState() );

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

    } // class GameManager

} // namespace HimonoLib

