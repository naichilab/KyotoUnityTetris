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

        private List<Renderer>  m_blockObjects  = new List<Renderer>();

        /// <summary>
        /// ブロックの色型と色情報のリンク用リスト.
        /// 色を塗る必要のないものはリストには入れていない.
        /// </summary>
        private readonly Dictionary< EBlockColor, Color >   COLOR_LIST = new Dictionary< EBlockColor, Color >
        {
            { EBlockColor.White,    Color.white },
            { EBlockColor.Cyan,     Color.cyan },
            { EBlockColor.Yellow,   Color.yellow },
            { EBlockColor.Magenta,  Color.magenta },
            { EBlockColor.Blue,     Color.blue },
            { EBlockColor.Orange,   new Color( 1.0f, 0.5f, 0.0f ) },
            { EBlockColor.Green,    Color.green },
            { EBlockColor.Red,      Color.red },
            { EBlockColor.Black,    Color.black },
        };   

        #endregion // Variable

        
        #region Public

        /// <summary>
        /// フィールド配列分のブロックを生成し初期化する
        /// </summary>
        /// <param name="i_blockCount">生成するブロック数</param>
        /// <param name="i_blockPrefab">生成するブロックPrefab</param>
        /// <param name="i_parent">生成したブロックの親transform</param>
        public void Initialize( int i_blockCount, Renderer i_blockPrefab, Transform i_parent )
        {
            // 元となるPrefabが設定されていないのなら警告を出し、処理を行わない.
            if( i_blockPrefab == null )
            {
                Debug.LogWarning( "missing m_blockPrefab!" );
                return;
            }

            // すでにブロックが作られていたら余計にブロックを作成してしまうため、一度削除.
            if( m_blockObjects != null && m_blockObjects.Count > 0 )
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
            m_blockObjects.Clear();
            for( int i = 0; i < i_blockCount; ++i )
            {
                var blockObj    = GameObject.Instantiate( i_blockPrefab );
                blockObj.gameObject.SetActive( false );

                // 親のtransformが指定されていたら、そのtransformの子に配置する.
                // ゲームのロジックには影響はないが、EditorのHierarchyがきれいになります.
                if( i_parent != null )
                {
                    blockObj.transform.SetParent( i_parent, false );
                }

                m_blockObjects.Add( blockObj );
                
            }
        }

        /// <summary>
        /// ブロックを指定されたフィールドの状態に合わせて描画します.
        /// </summary>
        /// <param name="i_array">２次元配列を持つフィールド情報</param>
        public void RenderBlock( int[,] i_array )
        {
            // ブロックオブジェクトリストがない場合は、描画しようがないので警告を出して処理を終了させる.
            if( m_blockObjects == null || m_blockObjects.Count == 0 )
            {
                Debug.LogWarning( "not created m_blockObjects yet!" );
                return;
            }

            // ２次元配列がない場合はも、描画しようがないので警告を出して処理を終了させる.
            var arrayData   = i_array;
            if( arrayData == null || arrayData.Length == 0 )
            {
                Debug.LogWarning( "invalid i_field!" );
                return;
            }

            // フィールド配列の数とブロックオブジェクトリストの数は同じになるはずなので、違う場合は警告を出す.
            if( arrayData.Length != m_blockObjects.Count )
            {
                Debug.LogAssertionFormat( "i_field is different from m_blockObjects in a number. field={0}, blockObjects={1}", arrayData.Length, m_blockObjects.Count );
            }



            int objIndex    = 0;

            for( int x = 0, sizeX = arrayData.GetLength( 1 ); x < sizeX; ++x )
            {
                for( int y = 0, sizeY = arrayData.GetLength( 0 ); y < sizeY; ++y )
                {
                    var blockObj    = m_blockObjects[ objIndex ];

                    // フィールドの配列Y軸は数字が大きくなるほど下に向かっていくが、
                    // Unityの座標系Y軸は数字が大きくなるほど上に向かっていくずれがあるため、指定する数値を負数にすることでごまかしている.
                    blockObj.transform.position = new Vector3( x, -y, 0.0f );

                    // 指定した引数情報が色リストにある場合はGameObjectを表示して色を変更する.
                    // それ以外はGameObjectを非表示にする.
                    var colorIndex  = (EBlockColor)arrayData[ y, x ];
                    if( COLOR_LIST.ContainsKey( colorIndex ) )
                    {
                        blockObj.gameObject.SetActive( true );
                        blockObj.material.color = COLOR_LIST[ colorIndex ];
                    }
                    else
                    {
                        blockObj.gameObject.SetActive( false );
                    }

                    // ブロックオブジェクトが足りなくなったら処理終了.
                    ++objIndex;
                    if( objIndex >= m_blockObjects.Count )
                    {
                        return;
                    }
                }
            }

        }

        #endregion // Public

    } // class BlockViewer
    
} // namespace HimonoLib

