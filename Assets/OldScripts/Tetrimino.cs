using UnityEngine;
using System.Collections;

namespace Old
{


public class Tetrimino : MonoBehaviour {

	private GameManager mGameManager;
	private bool mStopFlag = false;
	//private float mPhaseTimer = 0f;
	private int mCurrentTet;
	private Type mType;
	private Field mField;
	private int x;
	private int y;

	//テトリミノのタイプ
	public enum Type
	{
		ITet,
		OTet,
		STet,
		ZTet,
		JTet,
		LTet,
		TTet,
		Cube,
		Num,
	};

	private int[,] ITetArray = new int[,]
	{
		{2,2,2,2},
		{0,0,0,0},
		{0,0,0,0},
		{0,0,0,0},
	};

	private int[,] OTetArray = new int[,]
	{
		{0,0,0,0},
		{0,2,2,0},
		{0,2,2,0},
		{0,0,0,0},
	};

	private int[,] STetArray = new int[,]
	{
		{0,0,0,0},
		{0,2,2,0},
		{2,2,0,0},
		{0,0,0,0},
	};

	private int[,] ZTetArray = new int[,]
	{
		{0,0,0,0},
		{2,2,0,0},
		{0,2,2,0},
		{0,0,0,0},
	};

	private int[,] JTetArray = new int[,]
	{
		{0,0,0,0},
		{2,0,0,0},
		{2,2,2,0},
		{0,0,0,0},
	};

	private int[,] LTetArray = new int[,]
	{
		{0,0,0,0},
		{0,0,2,0},
		{2,2,2,0},
		{0,0,0,0},
	};

	private int[,] TTetArray = new int[,]
	{
		{0,0,0,0},
		{0,2,0,0},
		{2,2,2,0},
		{0,0,0,0},
	};



	public void FallTet(Tetrimino tetrimino)
	{
		for (int i = 0; i < 10; ++i)
		{
			Vector3 tetPos = tetrimino.transform.position;
			tetPos.y -= 1;
			tetrimino.transform.position = tetPos;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Bottom" )
		{
			mStopFlag = true;
		}
	}

	//生成されたテトリミノを受け取る
	public void GetTet(int num)
	{
		mType = (Type)num;
		/*switch (mType)
		{
		case Type.ITet:
			mField.mFieldCell = ITetArray;
			break;
		case Type.OTet:
			mField.mFieldCell = OTetArray;
			break;
		case Type.STet:
			mField.mFieldCell = STetArray;
			break;
		case Type.ZTet:
			mField.mFieldCell = ZTetArray;
			break;
		case Type.JTet:
			mField.mFieldCell = JTetArray;
			break;
		case Type.LTet:
			mField.mFieldCell = LTetArray;
			break;
		case Type.TTet:
			mField.mFieldCell = TTetArray;
			break;
		}*/

	}

	public bool SetBottomStopFlag()
	{
		return mStopFlag;
	}
		
}

}