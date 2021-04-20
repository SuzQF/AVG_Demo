using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/*
*Project Name: OurSmallWorld
*Create Date: 2020/11/3
*Author: Suz
*Update Record: 
*11/16		将主线剧情相关数据移至MainStoryScript类，纯化本类
*11/16		本类将只负责与台词相关的服务提供
*/

public class LinesController : MonoBehaviour {

	//台词控制器单例
	public static LinesController instance;

	//用于台词UI跟随
	public Camera LinesControllerCamera;
	public Canvas LinesControllerCanvas;

	public StreamReader linesReader;

	//台词显示所用UI
	[SerializeField]private Text linesTxt;

	//台词对应跟随目标
	[SerializeField] private Transform target;

	//物品交互台词
	public string[] itemScripts;
	public int itemCurrentLine = 0;

	//等待玩家按键继续
	private bool iswaitSpace = false;

	//分支选项UI
	public Transform SideChooseTable;

	//分支选项UI预制体
	public GameObject sidePrefab;

	//分支对应序号
	int side = 0;

	/// <summary>
	/// 初始化
	/// </summary>
	void Awake() {
		if (instance == null) {
			instance = this;
		}

		else if (instance != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
	}

	// Update is called once per frame
	void Update() {
		TxtPositionFollow();
	}

	/// <summary>
	/// 角色台词位置跟随
	/// </summary>
	private void TxtPositionFollow() {

		//Text跟随
		if (target==null) {
			return;
		}
		var temp = Camera.main.WorldToScreenPoint(target.transform.position);
		Vector2 pos = new Vector2();
		bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(LinesControllerCanvas.transform as RectTransform, temp, LinesControllerCamera, out pos);
		if (isRect) {
			//根据当前说话角色调整台词UI显示偏移量
			linesTxt.rectTransform.anchoredPosition = new Vector2(pos.x, pos.y+ 100);
		}
	}


	/// <summary>
	/// 角色下一句台词
	/// </summary>
	/// <returns></returns>
	public IEnumerator NextLine() {
		GM.Log("NextLine Start");
		bool isOver = false;
		while (!isOver) {

			//获取下一句台词信息
			GetNextLine();

			//台词跟随角色
			TxtPositionFollow();

			//等待台词淡入协程结束
			yield return StartCoroutine(LinesFadeIn(5f));
			isOver = true;

			//持续显示并等待玩家按键继续
			yield return StartCoroutine(WaitForSpace());
		}

		//等待台词淡出协程结束
		yield return StartCoroutine(LinesFadeOut(4f));
		GM.Log("HideLines");

		yield return new WaitForSeconds(0.5f);

		target = null;
		GM.Log("NextLine End" );
	}

	/// <summary>
	/// 确定下一句台词
	/// </summary>
	/// <returns>如果是剧情则返回true，否则为false</returns>
	private void GetNextLine() {

		//获取并处理文件中的台词信息
		string[] role_sentence = linesReader.ReadLine().Split('#');
		string role = role_sentence[0];
		string sentence = role_sentence[1];

		//设置台词UIText
		linesTxt.text = sentence;
		
		//设置台词UI图标位置
		linesTxt.transform.GetChild(0).transform.localPosition = new Vector3(65 + 100*(sentence.Length / 2.0f), 4.5f, -5);

		//设置台词跟随目标
		target = RoleManager.instance.FindRoleController(role).transform;

		////如果与场景交互
		//if (PerformanceManagerBase.isItemChapter) {
		//	linesTxt.text = itemScripts[itemCurrentLine];
		//	target = PlayerController.instance.transform;
		//	itemCurrentLine += 1;
		//}

		//////如果与多莉交互
		////else if (PerformanceManager.instance.isDollyChapter) {
		////	linesTxt.text = DollyScript[DollyScriptCurrentLine];
		////}

		////如果是剧情
		////根据剧本的当前台词flag选择跟随目标和确定台词
		//else if (PerformanceManagerBase.isStoryChapter) {
		//	if (MainStoryScript.instance.mainStoryScript[MainStoryScript.instance.mainStoryScriptCurrentLine] == 0) {
		//		linesTxt.text = MainStoryScript.instance.ArvinLines[MainStoryScript.instance.ArvinCurrentLine];
		//		target = PlayerController.instance.transform;
		//		MainStoryScript.instance.ArvinCurrentLine += 1;
		//	}
		//	else if (MainStoryScript.instance.mainStoryScript[MainStoryScript.instance.mainStoryScriptCurrentLine] == 1) {
		//		linesTxt.text = MainStoryScript.instance.DollyLines[MainStoryScript.instance.DollyCurrentLine];
		//		target = MainStoryScript.instance.DollyRoleController.transform;
		//		MainStoryScript.instance.DollyCurrentLine += 1;
		//	}
		//}

	}

	/// <summary>
	/// 台词淡入协程
	/// </summary>
	/// <param name="rendSpeed"></param>
	/// <returns></returns>
	private IEnumerator LinesFadeIn(float rendSpeed) {
		instance.ShowLines();
		float alpha = 0;
		while (alpha < 1) {
			alpha += rendSpeed * Time.deltaTime;
			linesTxt.GetComponent<Text>().color = new Vector4(1, 1, 1, alpha);
			yield return 0;
		}
		GM.Log("Line Fade In Over");
	}

	/// <summary>
	/// 显示台词UI
	/// </summary>
	private void ShowLines() {
		linesTxt.gameObject.SetActive(true);
	}

	/// <summary>
	/// 台词淡出协程
	/// </summary>
	/// <param name="rendSpeed"></param>
	/// <returns></returns>
	private IEnumerator LinesFadeOut(float rendSpeed) {
		float alpha = 1;
		while (alpha > 0) {
			alpha -= rendSpeed * Time.deltaTime;
			linesTxt.GetComponent<Text>().color = new Vector4(1, 1, 1, alpha);
			yield return new WaitForSeconds(0);
		}
		instance.HideLines();
	}

	/// <summary>
	/// 隐藏台词UI
	/// </summary>
	private void HideLines() {
		linesTxt.gameObject.SetActive(false);
	}

	/// <summary>
	/// 等待玩家按键继续
	/// </summary>
	/// <returns></returns>
	IEnumerator WaitForSpace() {
		iswaitSpace = true;
		while (iswaitSpace) {
			if (Input.GetKey(KeyCode.Space)) {
				iswaitSpace = false;
				yield break;
			}
			yield return 0;
		}
	}

	public void ChooseSideLine(params string[] sides) {
		StartCoroutine("ChooseSideLineCoroutine",sides);
	}

	public IEnumerator ChooseSideLineCoroutine(params string[] sides) {
		GM.Log(sides.Length+"Sides length");
		InitTable(sides);
		
		//协程持续判定WS按钮事件
		//WS决定int增减，int对应按钮序号，以及UI边框闪烁
		//判定Space按钮事件——获取当前按钮的序号，调用SideLineChoosed方法。
		while (true) {
			
			if (Input.GetKeyDown(KeyCode.S)) {
				if (side<SideChooseTable.childCount-1) {
					side += 1;
					SetButtonOutLine(side);
				}
				GM.Log("GetKey S " + side);
			}
			if (Input.GetKeyDown(KeyCode.W)) {
				if (side>0) {
					side -= 1;
					SetButtonOutLine(side);
				}
				GM.Log("GetKey W " + side);
			}
			
			if (Input.GetKeyDown(KeyCode.Space)) {
				Debug.Log("GetKey Space" + side);
				SideLineChoosed();
			}
			yield return 0.1f;
		}
	}

	/// <summary>
	/// 初始化分支选择UI
	/// </summary>
	/// <param name="sides"></param>
	private void InitTable(string[] sides) {
		GM.Log(sides.Length + "Sides length");
		//显示UI
		SideChooseTable.gameObject.SetActive(true);

		//根据side数量设置side按钮排布
		float distance = 300.0f / sides.Length;
		for (int i = 0; i < sides.Length; i++) {
			
			//实例化按钮
			var temp = Instantiate(sidePrefab, new Vector3(0, 0, 0), Quaternion.identity);

			//设置按钮位置
			temp.transform.SetParent(SideChooseTable.transform);
			temp.transform.localPosition = new Vector3(0, 150f - distance/2.0f - distance*i, 0);

			//设置按钮文本
			SideChooseTable.GetChild(i).GetChild(0).GetComponent<Text>().text = sides[i];

			SetButtonOutLine(side);
		}
	}

	/// <summary>
	/// 设置按钮Outline以高亮当前选择
	/// </summary>
	/// <param name="side"></param>
	private void SetButtonOutLine(int side) {
		for (int i = 0; i < SideChooseTable.childCount; i++) {
			SideChooseTable.GetChild(i).GetComponent<Outline>().enabled = false;
		}
		SideChooseTable.GetChild(side).GetComponent<Outline>().enabled = true;
	}

	public void SideLineChoosed() {
		//关闭选择协程
		StopCoroutine("ChooseSideLineCoroutine");

		//隐藏UI
		SideChooseTable.gameObject.SetActive(false);

		//开启对应的脚本
		MainStoryScript.instance.ChooseStoryChapter(SideChooseTable.GetChild(side).GetChild(0).GetComponent<Text>().text);

		side = 0;
	}
}