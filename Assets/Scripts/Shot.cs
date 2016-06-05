using UnityEngine;
using System.Collections;

public class Shot : Token {
	//ショットオブジェクト管理
	public static TokenMgr<Shot> parent;
	//ショットを打つ
	public static Shot Add(float px,float py ,float direction,float speed,int power)
	{
		//初期化処理(Init)を呼び出すように修正
		Shot s = parent.Add(px,py, direction, speed);
		if( s == null)
		{
			return null;
		}
		s.Init(power);
		return s;
	}

	//ショットの威力
	int _power;
	public int Power
	{
		get { return _power;}
	}	

	//初期化
	public void Init(int power)
	{
		_power = power;
	}


	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if(IsOutside())
		{
			//画面外に出たので消滅
			Vanish();
		}
	}



	//消滅
	public override void Vanish()
	{

		//ボールエフェクト生成
		for(int i=0;i<4;i++)
		{
			//消滅フレーム数
			int timer = Random.Range(30,50);
			//反対方向に飛ばす
			float dir = Direction -180+ Random.Range(-60,60);
			float spd =Random.Range(1.0f,1.5f);
			Particle p =Particle.Add(Particle.eType.Ball,timer,X,Y,dir,spd);
			if(p)
			{
				//大きさ
				p.Scale = 0.8f;
				//赤色を設定
				p.SetColor(1.0f,0.0f,0.0f);
			}
		}
		//親の消滅処理を呼び出す
		base.Vanish();
	}



}

