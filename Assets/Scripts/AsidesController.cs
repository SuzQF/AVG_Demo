using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/*
*Project Name: OurSmallName
*Create Date: 2020/11/16
*Author: Suz
*Update Record: 
* 11/16		将原本杂糅在LInesController类中的旁白相关成员独立成类
* 11/16		本类将只负责所有旁白相关的服务提供
*/

/// <summary>
/// 旁白控制器类
/// </summary>
public class AsidesController : MonoBehaviour {

	//旁白控制器单例
	public static AsidesController instance;

	//旁白用UI
	public Text asideText;

	//暂存旁白UI
	private Text tempAsideBox;

	public StreamReader asidesReader;

	//上一句旁白是否添加至活动旁白列表
	bool isAdded = true;

	//当前旁白指针
	[SerializeField] private List<Text> activeAsides = new List<Text>();

	private void Awake() {
		if (instance == null) {
			instance = this;
		}

		else if (instance != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);

	}

	/// <summary>
	/// 下一句旁白协程——默认旁白显示时间为2.5s
	/// </summary>
	/// <param name="asidePos">旁白坐标</param>
	/// <param name="rendInSpeed">旁白淡入速度</param>
	/// <param name="rendOutSpeed">旁白淡出速度</param>
	/// <param name="cycleRound">旁白显示循环次数</param>
	/// <param name="showSeconds">持续显示时间</param>
	/// <returns></returns>
	public void NextAside(Vector3 asidePos,
													float rendInSpeed = 1f,//淡入时间  实际时间=1/淡入速度
													float rendOutSpeed = 1f,//淡出时间  实际时间=1/淡出速度
													int cycleRound = 1,//循环次数
													float showSeconds = 0.5f,//持续显示时间
													float cycleInternal = 0f,//循环间隔
													bool isDistroy = true
		) {
		StartCoroutine(NextAsideCoroutine(asidePos, rendInSpeed, rendOutSpeed, cycleRound, showSeconds, cycleInternal,isDistroy));
	}
	
	/// <summary>
	/// 下一句旁白协程
	/// </summary>
	/// <param name="asidePos"></param>
	/// <param name="rendInSpeed"></param>
	/// <param name="rendOutSpeed"></param>
	/// <param name="cycleRound"></param>
	/// <param name="showSeconds"></param>
	/// <param name="cycleInternal"></param>
	/// <returns></returns>
	private IEnumerator NextAsideCoroutine(Vector3 asidePos,float rendInSpeed,float rendOutSpeed,int cycleRound,float showSeconds,float cycleInternal,bool isDestroy) {
		for (int round = 0; round < cycleRound; round++) {
			bool isOver = false;
			while (!isOver) {
				while (!isAdded) {
					yield return new WaitForSeconds(0);
				}
				ShowNextAside(rendInSpeed, asidePos);
				isAdded = false;
				//开启旁白淡入协程
				GM.Log("Next Aside Fade In  " + round );
				yield return StartCoroutine(AsidesFadeIn(tempAsideBox, rendInSpeed));

				isOver = true;
				yield return new WaitForSeconds(showSeconds);
			}
			if (isDestroy) {
				GM.Log("Next Aside Fade Out  " + round);
				yield return StartCoroutine(AsidesFadeOut(tempAsideBox, rendOutSpeed));
				yield return new WaitForSeconds(cycleInternal);
			}
			
		}
		isAdded = true;
	}

	/// <summary>
	/// 实例化下一句旁白
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void ShowNextAside(float fadeInSpeed, Vector3 asidePos) {

		//新建旁白游戏对象
		instance.tempAsideBox = Instantiate(instance.asideText, transform);
		DontDestroyOnLoad(instance.tempAsideBox.gameObject);

		//设置旁白内容
		string aside = asidesReader.ReadLine();
		GM.Log("Current Aside: " + aside);
		instance.tempAsideBox.GetComponent<Text>().text = aside;

		//将当前旁白添加到活动旁白列表
		instance.activeAsides.Add(instance.tempAsideBox);

		//设置旁白位置
		instance.tempAsideBox.transform.position = Camera.main.WorldToScreenPoint(asidePos);
	}

	/// <summary>
	/// 旁白淡入协程
	/// </summary>
	/// <param name="rendSpeed"></param>
	/// <returns></returns>
	IEnumerator AsidesFadeIn(Text aside, float rendSpeed) {
		float alpha = 0;
		while (alpha < 1) {
			alpha += rendSpeed * Time.deltaTime;
			aside.GetComponent<Text>().color = new Vector4(0, 0, 0, alpha);
			yield return 0;
		}
	}

	/// <summary>
	/// 旁白淡出协程
	/// </summary>
	/// <param name="rendSpeed"></param>
	/// <returns></returns>
	IEnumerator AsidesFadeOut(Text aside, float rendSpeed) {
		float alpha = 1;
		while (alpha > 0) {
			alpha -= rendSpeed * Time.deltaTime;
			aside.GetComponent<Text>().color = new Vector4(0, 0, 0, alpha);
			yield return 0;
		}
		GM.Log("Destroy Aside: " + aside.GetComponent<Text>().text );
		Destroy(aside.gameObject);
		instance.activeAsides.RemoveAt(0);
	}

	public void ClearActiveAside() {
		foreach (var aside in activeAsides) {
			Destroy(aside.gameObject);
		}
		activeAsides.Clear();
	}
}
