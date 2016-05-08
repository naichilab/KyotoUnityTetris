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

        private FieldData   m_field     = null;
        private Dictionary< ETetrimino, int[,] >    m_tetrimonoTypes    = new Dictionary< ETetrimino, int[,] >();




        #endregion // Variable


        #region Fixed


        private const int   START_X = 0;
        private const int   START_Y = 5;

        private readonly int[,] DEFAULT_FIELD = new int[,]
        {
            { 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 1 },
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
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
        };

        private readonly int[,] I_TETRIMINO = new int[,]
        {
            {2,2,2,2},
            {0,0,0,0},
            {0,0,0,0},
            {0,0,0,0},
        };

        private readonly int[,] O_TETRIMINO = new int[,]
        {
            {0,0,0,0},
            {0,3,3,0},
            {0,3,3,0},
            {0,0,0,0},
        };

        private readonly int[,] S_TETRIMINO = new int[,]
        {
            {0,0,0,0},
            {0,4,4,0},
            {4,4,0,0},
            {0,0,0,0},
        };

        private readonly int[,] Z_TETRIMINO = new int[,]
        {
            {0,0,0,0},
            {3,3,0,0},
            {0,3,3,0},
            {0,0,0,0},
        };

        private readonly int[,] J_TETRIMINO = new int[,]
	    {
		    {0,0,0,0},
		    {2,0,0,0},
		    {2,2,2,0},
		    {0,0,0,0},
	    };

        private readonly int[,] L_TETRIMINO = new int[,]
        {
            {0,0,0,0},
            {0,0,3,0},
            {3,3,3,0},
            {0,0,0,0},
        };

        private readonly int[,] T_TETRIMINO = new int[,]
        {
            {0,0,0,0},
            {0,4,0,0},
            {4,4,4,0},
            {0,0,0,0},
        };

        #endregion // Fixed


        #region Property

        #endregion // Property


        #region Public

        #endregion // Public


        #region UnityMessage

        void Awake()
        {
            m_field         = new FieldData();
            m_field.Field   = DEFAULT_FIELD;

            m_tetrimonoTypes.Add( ETetrimino.I, I_TETRIMINO );
            m_tetrimonoTypes.Add( ETetrimino.O, O_TETRIMINO );
            m_tetrimonoTypes.Add( ETetrimino.S, S_TETRIMINO );
            m_tetrimonoTypes.Add( ETetrimino.Z, Z_TETRIMINO );
            m_tetrimonoTypes.Add( ETetrimino.J, J_TETRIMINO );
            m_tetrimonoTypes.Add( ETetrimino.L, L_TETRIMINO );
            m_tetrimonoTypes.Add( ETetrimino.T, T_TETRIMINO );


            m_viewer.Initialize( m_field.Field.Length, m_blockPrefab, transform );
        }

        void Start()
        {
            StartCoroutine( GameState() );
        }

        void Update()
        {
            m_viewer.ShowBlock( m_field );
        }

        #endregion // UnityMessage


        #region Protected

        #endregion // Protected


        #region Private

        #endregion // Private


        #region State

        private IEnumerator GameState()
        {


            yield return null;
        }

        private IEnumerator BlockUpdate()
        {
            while( true )
            {

            }
            
        }

        #endregion // State


        #region SubClass

        public class FieldData : IFieldData
        {
            private int [,] m_field     = null;

            public int[,] Field
            {
                get
                {
                    return m_field;
                }
                set
                {
                    m_field = value;
                }
            }

        }

        #endregion // SubClass

    } // class GameManager




    public enum EBlockColor
    {
        None,
        White,

        Red,
        Green,
        Blue,
    }

    public enum ETetrimino
    {
        I,
        O,
        S,
        Z,
        J,
        L,
        T,
    }

    public interface IFieldData
    {
        int[,]  Field
        {
            get;
        }
    }

    

} // namespace HimonoLib

