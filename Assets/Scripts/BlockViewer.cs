//------------------------------------------------------------------------
//
// (C) Copyright 2016 Urahimono Project Inc.
//
//------------------------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;

namespace HimonoLib
{
    public class BlockViewer
    {
    
        #region Variable

        private Renderer[]  m_blockObjects  = null;


        private readonly Dictionary< EBlockColor, Color >   COLOR_LIST = new Dictionary< EBlockColor, Color >
        {
            { EBlockColor.White,    Color.black },
            { EBlockColor.Red,      Color.red },
            { EBlockColor.Green,    Color.green },
            { EBlockColor.Blue,     Color.blue },
        };   

        #endregion // Variable

        
        #region Public

        public void Initialize( int i_blockCount, Renderer i_blockPrefab, Transform i_parent )
        {
            // 元となるPrefabが設定されていないのなら処理を行わない
            if( i_blockPrefab == null )
            {
                Debug.LogWarning( "missing m_blockPrefab!" );
                return;
            }

            // すでにブロックが作られていたら削除
            if( m_blockObjects != null && m_blockObjects.Length > 0 )
            {
                foreach( var obj in m_blockObjects )
                {
                    if( obj != null )
                    {
                        GameObject.Destroy( obj );
                    }
                }
            }

            // 新しいブロックを再生成
            var list = new List< Renderer >();
            for( int i = 0; i < i_blockCount; ++i )
            {
                var blockObj    = GameObject.Instantiate( i_blockPrefab );
                blockObj.gameObject.SetActive( false );
                if( i_parent != null )
                {
                    blockObj.transform.SetParent( i_parent, false );
                }

                list.Add( blockObj );
                
            }
            m_blockObjects  = list.ToArray();
        }
        

        public void ShowBlock( IFieldData i_field )
        {
            if( m_blockObjects == null || m_blockObjects.Length == 0 )
            {
                Debug.LogWarning( "not created m_blockObjects yet!" );
                return;
            }

            var arrayData   = i_field.Field;
            if( arrayData == null || arrayData.Length == 0 )
            {
                Debug.LogWarning( "invalid i_field!" );
                return;
            }



            int objIndex    = 0;
            for( int x = 0, sizeX = i_field.Field.GetLength( 1 ); x < sizeX; ++x )
            {
                for( int y = 0, sizeY = i_field.Field.GetLength( 0 ); y < sizeY; ++y )
                {
                    var blockObj    = m_blockObjects[ objIndex ];

                    blockObj.gameObject.SetActive( false );
                    blockObj.transform.position = new Vector3( x, -y, 0.0f );

                    var colorIndex  = (EBlockColor)arrayData[ y, x ];
                    if( COLOR_LIST.ContainsKey( colorIndex ) )
                    {
                        blockObj.gameObject.SetActive( true );
                        blockObj.material.color = COLOR_LIST[ colorIndex ];
                    }

                    ++objIndex;
                    if( objIndex >= m_blockObjects.Length )
                    {
                        return;
                    }
                }
            }

        }

        #endregion // Public

    } // class BlockViewer
    
} // namespace HimonoLib

