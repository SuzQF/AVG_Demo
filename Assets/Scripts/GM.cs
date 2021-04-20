using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
* Project Name: OurSmallWorld
* Create Date: 2020/11/3
* Author: Suz
* Update Record: 
* 11/18	封装了isInteractMode的访问
* 11/19	将ScenesManager类的创建Dolly实例的相关方法转移至本类
* 4/7将原本的实例化NPC方法泛化
* 4/11添加文件读取方法，在演出中读取文件获取对话、旁白数据。
*/

public class GM : MonoBehaviour {

	//游戏管理员类单例
	public static GM instance;

	//交互模式
	[SerializeField]private bool isInteractMode = true;

	public FileStream fileStream = null;
	public StreamReader linesReader = null;

	//交互用显示名称
	public Text InteractTargetName;
	public Vector3 highLightOffset = new Vector3(0,3f,0);


	private void Awake() {
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);

	}

	private void FixedUpdate() {
		HighLightTarget();
	}
	/// <summary>
	/// 开始游戏按键按钮事件
	/// </summary>
	public void StartBtnClicked() {
		Log("GM Change Scene To Village");
		ScenesManager.instance.ChangeScene(Color.black, ScenesManager.VILLAGE);
	}

	public void LoadBtnClicked() {
		ScenesManager.instance.ChangeScene(Color.black, ScenesManager.FOREST);
	}

	public void ExitBtnClicked() {
		Log("GM Quit");
		Application.Quit();
	}

	/// <summary>
	/// 进入交互模式
	/// </summary>
	public void IntoInteractMode() {
		Log("Is Interact Mode True");
		isInteractMode = true;
	}

	/// <summary>
	/// 结束交互模式
	/// </summary>
	public void OutFromInteractMode() {
		Log("Is Interact Mode False");
		isInteractMode = false;
	}

	/// <summary>
	/// 获取GM的InteractMode
	/// </summary>
	/// <returns></returns>
	public bool GetInteractMode() {
		return isInteractMode;
	}

	public StreamReader InitLineFileReader(string convesationName) {
		fileStream = null;
		linesReader = null;
		string path = "Assets\\Texts\\"+  convesationName + ".txt";

		Log(path);

		if (!File.Exists(path)) {
			Log("File don't exist: " + path);
		}
		else {
			fileStream = File.Open(path, FileMode.Open);
			GM.Log("File Open" + convesationName);
		}
		linesReader = new StreamReader(fileStream);
		
		return linesReader;
	}

	public void CloseLineFileReader() {
		fileStream.Close();
		linesReader.Close();

		if (LinesController.instance.linesReader!=null) {
			LinesController.instance.linesReader.Close();
		}

		if (AsidesController.instance.asidesReader!=null) {
			AsidesController.instance.asidesReader.Close();
		}
	}

	public static bool JudgeReachGoalPosition(Vector3 currentPosition, Vector3 goalPosition) {
		if (Mathf.Abs(currentPosition.x - goalPosition.x) < 0.1f
			&& Mathf.Abs(currentPosition.y - goalPosition.y) < 0.1f
			&& Mathf.Abs(currentPosition.z - goalPosition.z) < 0.1f) {
			return true;
		}
		return false;
	}

	/// <summary>
	/// 高亮目标
	/// </summary>
	/// <param name="collision"></param>
	/// <param name="offset"></param>
	public void HighLightTarget() {
		if (PlayerController.instance==null || PlayerController.instance.interactTarget == null || isInteractMode) {
			instance.InteractTargetName.enabled = false;
			return;
		}
		if (PlayerController.instance.interactTarget != null) {
			instance.InteractTargetName.enabled = true;
			instance.InteractTargetName.text = PlayerController.instance.interactTarget.name.Split('(')[0];
			instance.InteractTargetName.transform.position = Camera.main.WorldToScreenPoint(PlayerController.instance.interactTarget.transform.position + instance.highLightOffset);
		}
	}

	public static void Log(object msg) {
		Debug.Log(msg + "--" + Time.time);
	}

}
