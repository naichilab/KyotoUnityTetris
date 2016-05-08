using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Old
{


public class Player : MonoBehaviour
{

}

public class Field : MonoBehaviour
{

}

public class GameManager : MonoBehaviour {

    

	//フェイズ
	private enum Phase
	{
		Init,
		Play,
		Gameover,
		Num,
	}

	//プレハブ
	[SerializeField] private Player mPlayerPrefab;
	//フィールドプレハブ
	[SerializeField] private Field mFieldPrefab;
	//テトリミノプレハブ
	[SerializeField] private Tetrimino mITPrefab;
	[SerializeField] private Tetrimino mOTPrefab;
	[SerializeField] private Tetrimino mSTPrefab;
	[SerializeField] private Tetrimino mZTPrefab;
	[SerializeField] private Tetrimino mJTPrefab;
	[SerializeField] private Tetrimino mLTPrefab;
	[SerializeField] private Tetrimino mTTPrefab;
	//サンプルで作ったTetrimino
	[SerializeField] private Tetrimino mCubePrefab;
	//フィールドのセルのXの個数
	public static readonly int CellsXNum = 10;
	//フィールドのセルのYの個数
	public static readonly int CellsYNum = 20;
	//ゲームの進行
	private Phase mPhase;
	//ゲームプレー中の進行
	private Phase mPhasePlay;
	//フェイズの初期化
	private int mCheckPhase = 2;
	//プレイヤー
	private List<Player> mPlayer = new List<Player>();
	//フィールド
	private List<Field> mField = new List<Field>();
	//テトリミノ
	private List<Tetrimino> mTetrimino = new List<Tetrimino>();
	//プレイヤーの人数
	private const int PlayerNum = 1;
	//レベル
	//private int mLevel = 0;
	//テトリミノ
	Tetrimino tetrimino;
	//フィールド
	Field field;
	//落ちる距離
	//private int mFallCell = 1;
	//タイマー
	private float mTimer = 0;
	private int mCount = 0;
	//落ちる速さ
	private float mLimitTime = 1f;
	//落ちるのをストップさせる
	private bool mBottomStop = false;
	//テトリミノの場所
	Vector3 tetPos;
	//移動可能フラグ
	bool mCanMove = true;
	//移動距離
	int mFallDistance = 0;

	private int[,] mFieldCell = new int[,] {
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
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
		{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
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

	private int[,] CubeArray = new int[,]
	{
		{0,2,2,0},
		{0,0,0,0},
		{0,0,0,0},
		{0,0,0,0},
	};

	private void Start()
	{
		mTetrimino.Add (mITPrefab);
		mTetrimino.Add (mOTPrefab);
		mTetrimino.Add (mSTPrefab);
		mTetrimino.Add (mZTPrefab);
		mTetrimino.Add (mJTPrefab);
		mTetrimino.Add (mLTPrefab);
		mTetrimino.Add (mTTPrefab);
		mTetrimino.Add (mCubePrefab);

		mPhase = Phase.Init;
	}

	private void Update()
	{
		//フェイズの切り替え
		switch (mPhase)
		{
		case Phase.Init:
			PhaseInit ();
			break;
		case Phase.Play:
			PhasePlay ();
			break;
		/*case Phase.Gameover:
			PhaseGameover;
			break;*/
		}

		CheckMoveBlock ();
	}

	private void PhaseInit()
	{
		//前回のプレイヤーを削除し、今回のプレイヤーを生成する
		CreatePlayer();
		//前回のテトリミノを削除
		DeleteLastTet();
		//フィールドの生成
		CreateField();

		mPhase = Phase.Play;
	}

	//プレイヤーを生成する
	private void CreatePlayer()
	{
		//前回のプレイヤーを削除
		for (int i = 0; i < mPlayer.Count; ++i)
		{
			Destroy (mPlayer [i].gameObject);
		}
		mPlayer.Clear ();

		//プレイヤーを作る
		for(int i = 0; i < PlayerNum; ++i)
		{
			Player player = Instantiate (mPlayerPrefab);
			mPlayer.Add (player);
			//TODO mPlayer [i].Init;
		}

	}


	//前回のテトリミノを全て削除する
	private void DeleteLastTet()
	{
		GameObject[] Tets;
		//タグでTetriminoとなっているものを全て削除
		Tets = GameObject.FindGameObjectsWithTag ("Tetrimino");
		foreach (GameObject Tetrimino in Tets)
		{
			Destroy (Tetrimino);
		}
	}

	//フィールドを生成する
	private void CreateField()
	{
		//プレイヤーの数だけフィールドがあるので、プレイヤーの数だけフィールドを削除する
		for (int i = 0; i < mField.Count; ++i)
		{
			Destroy (mField [i].gameObject);
		}
		mField.Clear ();

		for (int i = 0; i < PlayerNum; ++i)
		{
			Field field = Instantiate (mFieldPrefab);
			mField.Add (field);
			//TODO mField[i].Init;
			field.transform.parent = GameObject.Find("FieldManager").transform;
		}
	}

	private void PhasePlay()
	{
		switch(mCheckPhase)
		{
		case 0:
			//GameOverCheck ();
			break;
		case 1:
			//LevelCheck ();
			break;
		case 2:
			CreateTet ();
			break;
		case 3:
			mBottomStop = tetrimino.SetBottomStopFlag ();
			if (mBottomStop == true)
			{
				mCheckPhase = 2;
				break;
			}
			FallDown ();
			break;
		case 4:
			mTimer += Time.deltaTime;
			if (mTimer > mLimitTime)
			{
				mCheckPhase = 3;
				mTimer = 0f;
			}
			break;
		case 5:
			//AddPoint ();
			break;
		/*case 6:
			LevelUpCheck ();
			break;*/
		}
	}
		
	private void CreateTet()
	{
		int tetIndex = Random.Range (7, 7);
		tetrimino = (Tetrimino)Instantiate (mTetrimino [tetIndex]);
		tetrimino.GetTet (tetIndex);
		mTetrimino.Add (tetrimino);
		tetrimino.transform.parent = GameObject.Find("FieldManager").transform;
		tetrimino.transform.localPosition = new Vector3 (1, 18.5f, 0);

		MakeField (0, 4);

		mCheckPhase = 4;

	}
		
	private void FallDown()
	{
		//tetrimino.FallTet (tetrimino);
		//++mTetCounter;

		/*if (IsHighArea (tetPos.y) == false)
		{
			//何もしない
		}
		else if(IsHighArea (tetPos.y) == true)
		{*/
		//ClearField (mFallDistance, 0);
		MakeField (mFallDistance + 1, 0);
		++mFallDistance;
		if (mCanMove == true)
		{
			tetPos = tetrimino.transform.position;
			tetPos.y = tetPos.y - 1f;
			tetrimino.transform.position = tetPos;
			mCheckPhase = 4;
		}
		else
		{
			mCheckPhase = 5;
		}

		Debug.Log (mCanMove);	
		Debug.Log (mFieldCell [0, 5]);
		Debug.Log (mFieldCell [0, 6]);
		Debug.Log (mFieldCell [1, 5]);
		Debug.Log (mFieldCell [1, 6]);


	}

	private void CheckMoveBlock()
	{
		if (mBottomStop == false)
		{
			if (Input.GetKeyDown (KeyCode.Space))
			{
				tetrimino.transform.Rotate (new Vector3 (0, 0, 90));
			}
			if (Input.GetKeyDown (KeyCode.RightArrow))
			{
				if (IsLeftArea (tetPos.x) == false)
				{
					//何もしない
				}
				else if(IsLeftArea (tetPos.x) == true)
				{
					tetPos = tetrimino.transform.position;
					tetPos.x = tetPos.x + 1f;
					tetrimino.transform.position = tetPos;
				}
			}
			if (Input.GetKeyDown (KeyCode.LeftArrow))
			{
				if (IsRightArea (tetPos.x) == false)
				{
					//何もしない
				}
				else if(IsRightArea (tetPos.x) == true)
				{
					tetPos = tetrimino.transform.position;
					tetPos.x = tetPos.x - 1f;
					tetrimino.transform.position = tetPos;
				}
			}
			if (Input.GetKeyDown (KeyCode.DownArrow))
			{
				FallDown ();
			}
		}
	}

	//範囲内かを確認して、範囲内のみ動くようにしたい
	private bool IsRightArea(float x)
	{
		bool mIsArea = true;
		//テトリミノが範囲内であればtrueを返す
		if (x < -3)
		{
			mIsArea = false;
		}

		return mIsArea;
	}

	private bool IsLeftArea(float x)
	{
		bool mIsArea = true;
		//テトリミノが範囲内であればtrueを返す
		if (x > 3)
		{
			mIsArea = false;
		}

		return mIsArea;
	}

	private bool IsHighArea(float y)
	{
		bool mIsArea = true;
		//テトリミノが範囲内であればtrueを返す
		if (y < 0)
		{
			mIsArea = false;
		}

		return mIsArea;
	}

	private void MakeField(int i, int j)
	{
		for (int y = 0; y < 4; ++y)
		{
			for (int x = 0; x < 4; ++x)
			{
				mFieldCell [i + y, j + x] = mFieldCell [i + y, j + x] + CubeArray [y, x];
				int check = mFieldCell [i + y, j + x];

				if (check == 3 || check == 4)
				{
					mCanMove = false;
				}

			}
		}


	}

	private void ClearField(int i, int j)
	{
		for (int y = 0; y < 4; ++y)
		{
			for (int x = 0; x < 4; ++x)
			{
				mFieldCell [i + y, j + x] = 0;

			}
		}
	}

		
}


}
