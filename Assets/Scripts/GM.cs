using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
	[SerializeField] private bool isInteractMode = true;

	public FileStream fileStream = null;
	public StreamReader linesReader = null;
	//交互用显示名称
	public Text InteractTargetName;
	public Vector3 highLightOffset = new Vector3(0, 3f, 0);

	public GameObject PauseTable;
	public float defaultTimeScale;

	public CameraController cameraController;
	private void Awake() {
		if (instance == null) {
			instance = this;
		}
		else if (instance != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
		defaultTimeScale = Time.timeScale;
	}

	private void Update() {
		HighLightTarget();
		if (!isInteractMode) {
			if (Input.GetKeyDown(KeyCode.Escape)) {
				SetCanvas("GM");
				PauseTable.SetActive(!PauseTable.activeSelf);
				Time.timeScale = Time.timeScale > 0 ? 0 : defaultTimeScale;
			}
		}
	}

	public void SetCanvas(string name) {
		Log(name);
		transform.GetChild(0).GetComponent<Canvas>().sortingOrder = 0;
		transform.GetChild(1).GetComponent<Canvas>().sortingOrder = 0;
		transform.GetChild(2).GetComponent<Canvas>().sortingOrder = 0;

		transform.GetChild(1).GetComponent<Canvas>().sortingOrder = 5;
		//transform.Find(name).GetComponent<Canvas>().sortingOrder = 5;
	}



	/// <summary>
	/// 主菜单开始游戏按键按钮事件
	/// </summary>
	public void MM_StartBtnClicked() {
		Log("GM Change Scene To Village");
		SetCanvas("TextController");
		StartCoroutine( ScenesManager.instance.ChangeScene(Color.black, ScenesManager.VILLAGE));
	}

	/// <summary>
	/// 主菜单读档按键按钮事件
	/// </summary>
	public void MM_LoadBtnClicked() {
		SetCanvas("TextController");
		StartCoroutine(Load());
	}

	/// <summary>
	/// 离开游戏
	/// </summary>
	public void MM_ExitBtnClicked() {
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

	/// <summary>
	/// 初始化文件流
	/// </summary>
	/// <param name="convesationName"></param>
	/// <returns></returns>
	public StreamReader InitLineFileReader(string convesationName) {
		fileStream = null;
		linesReader = null;
		string path = Application.persistentDataPath +"\\" + convesationName + ".txt";

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

	/// <summary>
	/// 关闭文件流
	/// </summary>
	public void CloseLineFileReader() {
		fileStream.Close();
		linesReader.Close();

		if (LinesController.instance.linesReader != null) {
			LinesController.instance.linesReader.Close();
		}

		if (AsidesController.instance.asidesReader != null) {
			AsidesController.instance.asidesReader.Close();
		}
	}


	/// <summary>
	/// 高亮目标
	/// </summary>
	/// <param name="collision"></param>
	/// <param name="offset"></param>
	public void HighLightTarget() {
		if (PlayerController.instance == null) {
			return;
		}
		if (PlayerController.instance.interactTarget == null || isInteractMode) {
			instance.InteractTargetName.enabled = false;
			return;
		}
		if (PlayerController.instance.interactTarget != null) {
			instance.InteractTargetName.enabled = true;
			instance.InteractTargetName.text = PlayerController.instance.interactTarget.name.Split('(')[0];
			instance.InteractTargetName.transform.position = Camera.main.WorldToScreenPoint(PlayerController.instance.interactTarget.transform.position + instance.highLightOffset);
		}
	}


	/// <summary>
	/// 继续游戏按钮
	/// </summary>
	public void PT_ResumeClicked() {
		PauseTable.SetActive(false);
		Time.timeScale = defaultTimeScale;
		OutFromInteractMode();
	}

	/// <summary>
	/// 存档按钮
	/// </summary>
	public void PT_SaveClicked() {
		CreateSaveFile();
		OutFromInteractMode();
		Time.timeScale = defaultTimeScale;
		PauseTable.SetActive(false);
	}

	/// <summary>
	/// 创建存档
	/// </summary>
	public void CreateSaveFile() {
		Save save = RecordSaveInfo();

		BinaryFormatter bf = new BinaryFormatter();
		string path = Application.persistentDataPath + "/gamesave.save";
		if (File.Exists(path)) {
			File.Delete(path);
		}
		FileStream file = File.Create(path);
		bf.Serialize(file, save);
		file.Close();

		Debug.Log("Saved");
	}

	/// <summary>
	/// 记录存档信息
	/// </summary>
	/// <returns></returns>
	public Save RecordSaveInfo() {
		Save save = new Save();

		save.chapterCount = MainStoryScript.instance.chapterCount;
		save.currentSceneName = ScenesManager.instance.currentSceneName;
		save.playerPosition[0] = PlayerController.instance.transform.position.x;
		save.playerPosition[1] = PlayerController.instance.transform.position.y;
		save.playerPosition[2] = PlayerController.instance.transform.position.z;

		save.cameraPosition[0] = FindObjectOfType<CameraController>().transform.position.x;
		save.cameraPosition[1] = FindObjectOfType<CameraController>().transform.position.y;
		save.cameraPosition[2] = FindObjectOfType<CameraController>().transform.position.z;

		if (FindObjectOfType<CameraController>().target==null) {
			save.isFollowing = false;
		}
		else {
			save.isFollowing = true;
		}

		return save;
	}

	/// <summary>
	/// 暂停菜单读档按键按钮事件
	/// </summary>
	public void PT_LoadClicked() {
		PauseTable.SetActive(false);
		StartCoroutine(Load());
		Time.timeScale = defaultTimeScale;
	}

	/// <summary>
	/// 读档
	/// </summary>
	public IEnumerator Load() {

		if (File.Exists(Application.persistentDataPath + "/gamesave.save")) {
			// 2
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
			Save save = (Save)bf.Deserialize(file);
			file.Close();

			Debug.Log("Game Loaded");
			MainStoryScript.instance.chapterCount = save.chapterCount;

			if (ScenesManager.instance.currentSceneName != save.currentSceneName) {
				StartCoroutine(ScenesManager.instance.ChangeScene(Color.black, save.currentSceneName));
			}
			else {
				StartCoroutine(ScenesManager.instance.FadeOutThenIn(Color.black, Color.black));
			}

			if (PlayerController.instance == null) {
				var temp = Instantiate(RoleManager.instance.ArvinPrefab, new Vector3(
					save.playerPosition[0], save.playerPosition[1], save.playerPosition[2]), 
					Quaternion.identity);
				PlayerController.instance = temp.GetComponent<PlayerController>();
			}
			else {
				PlayerController.instance.transform.position = new Vector3(
					save.playerPosition[0], save.playerPosition[1], save.playerPosition[2]);
			}

			yield return new WaitForSeconds(0.6f);
			GM.Log("Find Camera");
			Log(FindObjectOfType<CameraController>().transform.position);
			if (save.isFollowing) {
				FindObjectOfType<CameraController>().SetTarget(PlayerController.instance.gameObject.transform);
			}
			else {
				FindObjectOfType<CameraController>().SetTarget(null);
				FindObjectOfType<CameraController>().transform.position = new Vector3(
					save.cameraPosition[0], save.cameraPosition[1], save.cameraPosition[2]);
			}
			Log(FindObjectOfType<CameraController>().transform.position);
		}
		else {
			Debug.Log("No game saved!");
		}
		OutFromInteractMode();
	}

	public void PT_ExitClicked() {
		PauseTable.SetActive(false);
		OutFromInteractMode();
		Time.timeScale = defaultTimeScale;
		Log("Exit");
		StartCoroutine(ScenesManager.instance.ChangeScene(Color.black, ScenesManager.MAIN_MENU));
	}

	public static bool JudgeReachGoalPosition(Vector3 currentPosition, Vector3 goalPosition) {
		if (Mathf.Abs(currentPosition.x - goalPosition.x) < 0.1f
			&& Mathf.Abs(currentPosition.y - goalPosition.y) < 0.1f
			&& Mathf.Abs(currentPosition.z - goalPosition.z) < 0.1f) {
			return true;
		}
		return false;
	}

	public static void Log(object msg) {
		Debug.Log(msg + "--" + Time.time);
	}

	public void SetGame() {
		GameObject.Find("MainMenu").transform.Find("Start").GetComponent<Button>().onClick.AddListener(MM_StartBtnClicked);
		GameObject.Find("MainMenu").transform.Find("Load").GetComponent<Button>().onClick.AddListener(MM_LoadBtnClicked);
		GameObject.Find("MainMenu").transform.Find("Exit").GetComponent<Button>().onClick.AddListener(MM_ExitBtnClicked);
	}
}
