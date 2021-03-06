﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tower : Token {


	public AudioSource  audioClip1;  //


	/// コスト
	// 射程範囲
	public int CostRange
	{
		get { return Cost.TowerUpgrade(eUpgrade.Range, _lvRange); }
	}
	// 連射速度
	public int CostFirerate
	{
		get { return Cost.TowerUpgrade(eUpgrade.Firerate, _lvFirerate); }
	}
	// 攻撃威力
	public int CostPower
	{
		get { return Cost.TowerUpgrade(eUpgrade.Power, _lvPower); }
	}
	/// アップグレード種別に対応したコストを取得する
	public int GetCost(eUpgrade type)
	{
		switch (type)
		{
		case eUpgrade.Range: return CostRange;
		case eUpgrade.Firerate: return CostFirerate;
		case eUpgrade.Power: return CostPower;
		}
		return 0;
	}

	//アップグレードの種類
	public enum eUpgrade {
		Range,    //射程範囲
		Firerate, //連射速度
		Power,    //攻撃威力
	}


	//タワー管理
	public static TokenMgr<Tower> parent;
	//タワー生成
	public static Tower Add(float px,float py){
		Tower t =parent.Add(px,py);
		if( t == null){
			return null;
		}
		t.Init();
		return t;
	}


	//ショットの速度
	const float SHOT_SPEED = 5.0f;
	//射程範囲
	float _range;
	//連射速度
	float _firerate;
	//連射速度インターバルタイマー
	float _tFirerate;
	//攻撃威力
	int _power;

	//射程範囲
	int _lvRange;
	public int LvRange
	{
		get { return _lvRange;}
	}
	//射程速度
	int _lvFirerate;
	public int LvFirerate
	{
		get { return _lvFirerate ;}
	}
	//攻撃力
	int _lvPower;
	public int LvPower
	{
		get { return _lvPower ;}
	}

	//パラメータを更新
	void UpdateParam()
	{
		//射程範囲
		_range = TowerParam.Range(_lvRange);
		//連射速度
		_firerate = TowerParam.Firerate(_lvFirerate);
		//攻撃威力
		_power = TowerParam.Power(_lvPower);


		// レベルに対する色を設定
		// 平均レベルを計算
		float avg = (_lvRange + _lvFirerate + _lvPower) / 3.0f;
		// 小数点以下を切り上げする
		int avgLv = Mathf.CeilToInt(avg);
		Color c;
		switch (avgLv)
		{
		case 1: c = Color.white; break; // 白色
		case 2: c = Color.cyan; break; // シアン
		case 3: c = Color.green; break; // 緑色
		case 4: c = Color.yellow; break; // 黄色
		default: c = Color.red; break; // 赤色
		}
		// 少し明るくする
		c.r += 0.3f;
		c.g += 0.3f;
		c.b += 0.3f;
		SetColor(c);

	}


	// 初期化
	//	void Start () 
	void Init () 
	{

		audioClip1 = gameObject.AddComponent<AudioSource>();  // 

		_range = Field.GetChipSize()*1.5f;	
		_firerate = 2.0f;
		//連射速度インターバル初期化
		_tFirerate = 0;
		//攻撃威力を設定
		//_power=1;

		//レベル初期化
		_lvRange =1;
		_lvFirerate =1;
		_lvPower =1;
		UpdateParam();

	}

	// Update is called once per frame
	void Update () {
		//インターバルタイマー更新
		_tFirerate +=Time.deltaTime;
		Enemy e = Enemy.parent.Nearest(this);
		if( e == null)
		{
			//敵がいないので何にしない
			return;
		}
		//敵への距離を取得する
		float dist = Util.DistanceBetween(this,e);
		if( dist> _range)
		{
			//射程範囲外
			return;
		}
		//敵への角度を取得
		float targetAngle = Util.AngleBetween(this,e);
		//現在向いている角度との差を求める
		float dAngle = Mathf.DeltaAngle(Angle,targetAngle);
		//差の0.2だけ回転する。
		Angle += dAngle * 0.2f;
		//もう一度角度差を求める
		float dAngle2 =	Mathf.DeltaAngle(Angle,targetAngle);
		if(Mathf.Abs(dAngle2) >16)
		{
			//角度が大きい場合は撃てない
			return;
		}

		if(_tFirerate < _firerate)
		{
			return;
		}


		//ショットを撃つ
		Shot.Add(X,Y,Angle,SHOT_SPEED,_power);
		audioClip1.PlayOneShot(audioClip1.clip);  //


		_tFirerate = 0;
	}

	/// アップグレードする
	public void Upgrade(eUpgrade type)
	{
		switch (type)
		{
		case eUpgrade.Range:
			// 射程範囲のレベルアップ
			_lvRange++;
			break;

		case eUpgrade.Firerate:
			// 連射速度のレベルアップ
			_lvFirerate++;
			break;

		case eUpgrade.Power:
			// 攻撃威力のレベルアップ
			_lvPower++;
			break;
		}
		// パラメータ更新
		UpdateParam();

		// アップグレードエフェクト生成
		Particle p = Particle.Add(Particle.eType.Ellipse, 20, X, Y, 0, 0);
		if (p)
		{
			p.SetColor(0.2f, 0.4f, 0.8f);
		}
	}



}
