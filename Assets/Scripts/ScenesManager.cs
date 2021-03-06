using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
* Project Name: OurSmallWorld
* Create Date: 2020/11/3
* Author: Suz
* Update Record: 
* 11/17	删除冗余代码，优化代码结构
* 11/19	删除事件相关，简化调用
*/


public class ScenesManager : MonoBehaviour {

	//场景管理员类单例
	public static ScenesManager instance;

	[SerializeField] private Transform backgroundTransform;
	[SerializeField] private Transform middlegroundTransform;

	//标准场景名
	public const string MAIN_MENU = "MainMenu";
	public const string VILLAGE = "Village";
	public const string FOREST = "Forest";
	public const string ENDSCENE1 = "EndScene1";
	public const string ENDSCENE2 = "EndScene2";

	//透明度
	[SerializeField] private float alpha;

	//渐变速度
	[SerializeField] private float rendSpeed = 2f;

	//避免淡出淡入冲突flag
	private bool isRenderOut = false;

	//开局演出flag
	private bool isOpening = true;

	//当前场景名字
	public string currentSceneName;

	//背景跟随摄像机
	private bool isbackgroundFollowCamera = false;

	private void Awake() {
		if (instance == null) {
			instance = this;
		}

		else if (instance != this) {
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);
	}

	void FixedUpdate() {
		if (currentSceneName != SceneManager.GetActiveScene().name) {
			EnterNewSceneSet(currentSceneName, SceneManager.GetActiveScene().name);
			currentSceneName = SceneManager.GetActiveScene().name;
		}
	}

	private void LateUpdate() {
		if (isbackgroundFollowCamera) {
			backgroundTransform.SetParent(FindObjectOfType<CameraController>().transform);
			middlegroundTransform.SetParent(FindObjectOfType<CameraController>().transform);

			//backgroundTransform.position = new Vector3(currentSceneCamera.transform.position.x, backgroundTransform.position.y, backgroundTransform.position.z);
			//middlegroundTransform.position = new Vector3(currentSceneCamera.transform.position.x, middlegroundTransform.position.y, middlegroundTransform.position.z);
		}
	}

	/// <summary>
	/// 切换场景
	/// </summary>
	/// <param name="sceneName">场景名</param>
	public void ChangeScene(Color renderColor, string sceneName) {
		GM.Log("FadeIn");
		GM.instance.IntoInteractMode();
		StartCoroutine(FadeTo(renderColor, sceneName));
	}

	/// <summary>
	/// 进入新场景事件处理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public void EnterNewSceneSet(string lastSceneName, string currentSceneName) {

		//淡入视效设置
		instance.SetFadeIn(lastSceneName,currentSceneName);

		//退出交互模式
		GM.instance.OutFromInteractMode();

		//场景初始化
		instance.SceneInit();
	}

	/// <summary>
	/// 设置淡入视效
	/// </summary>
	/// <param name="e"></param>
	private void SetFadeIn(string lastSceneName, string currentSceneName) {

		//根据上一场景选择淡入颜色
		instance.StartCoroutine(instance.FadeIn(Color.black));

		//更新当前场景名
		instance.currentSceneName = currentSceneName;
		GM.Log("Current Scene: " + instance.currentSceneName);

		//根据当前场景选择淡入速度
		instance.rendSpeed = 2f;
		
	}

	/// <summary>
	/// 新场景设置
	/// </summary>
	private void SceneInit() {

		//绑定当前场景摄像机
		//LinesController.instance.GetComponent<Canvas>().worldCamera = FindObjectOfType<Camera>();

		if (currentSceneName==MAIN_MENU) {
			BGMController.instance.ChangeAudio(BGMController.instance.MainMenuBGM,0.25f,0.25f);
		}

		if (currentSceneName  == VILLAGE&&MainStoryScript.instance.chapterCount==0) {

			BGMController.instance.ChangeAudio(BGMController.instance.VillageBGM, 0.5f,0.5f);

			GM.Log("instantiate Hatman");
			RoleManager.instance.CreateRoleInstance(RoleManager.Hatman, new Vector3(0.41f,-8.88f,0));

			GM.Log("instantiate Bearded");
			RoleManager.instance.CreateRoleInstance(RoleManager.Bearded, new Vector3(9.44f, -9f, 0));

			GM.Log("instantiate Oldman");
			RoleManager.instance.CreateRoleInstance(RoleManager.Oldman, new Vector3(15.25f, -7.216f, 0));

			if (isOpening) {
				isOpening = false;
				MainStoryScript.instance.ChooseStoryChapter("FirstToVillage");
			}
		}

		if (currentSceneName==FOREST) {
			BGMController.instance.ChangeAudio(BGMController.instance.ForestBGM, 0.5f,0.5f);
			GM.Log("instantiate Bearded");
			RoleManager.instance.CreateRoleInstance(RoleManager.Akauri, new Vector3(14f, -0.819f, 0));
		}

		if (currentSceneName==ENDSCENE1) {
			MainStoryScript.instance.ChooseStoryChapter("End1");
		}

		if (currentSceneName==ENDSCENE2) {
			MainStoryScript.instance.ChooseStoryChapter("End2");
		}
	}

	//IEnumerator 枚举器笔记 
	//姑且先当作是一个简单需要迭代的的异步吧

	//协程，微线程，在一个线程间完成并发异步的程序结构
	//原理与多线程类似，不过协程始终在一个线程内，比多线程高效，也不存在跨线程通信之类的问题

	/// <summary>
	/// 淡入协程
	/// </summary>
	/// <param name="renderColor">淡入颜色</param>
	/// <returns></returns>
	IEnumerator FadeIn(Color renderColor) {
		while (isRenderOut) {
			yield return new WaitForSeconds(0);
		}
		alpha = 1;
		while (alpha > 0) {

			alpha -= Time.deltaTime * rendSpeed;
			transform.GetChild(0).GetComponent<Image>().color = new Color(renderColor.r, renderColor.g, renderColor.b, alpha);
			//yield return 必须返回 IEnumberable<T>、IEnumberable、IEnumberator<T>和IEnumberator中的一个
			//yield return是一次迭代的结束，如果要中途退出迭代，则使用yield break
			//WaitForSeconds是符合yield return返回值类型的函数，表示等待一定时间（单位：ms）后进行下一次迭代

			yield return new WaitForSeconds(0);
		}
	}

	/// <summary>
	/// 淡出到场景协程
	/// </summary>
	/// <param name="renderColor">淡出颜色</param>
	/// <param name="sceneName">目标场景名</param>
	/// <returns></returns>
	IEnumerator FadeTo(Color renderColor, string sceneName) {
		alpha = 0;
		while (alpha < 1) {
			alpha += Time.deltaTime * rendSpeed;
			transform.GetChild(0).GetComponent<Image>().color = new Vector4(renderColor.r, renderColor.g, renderColor.b, alpha);
			yield return new WaitForSeconds(0);
		}
		GM.Log("Fade To Scene:" + sceneName);
		isbackgroundFollowCamera = false;
		SceneManager.LoadScene(sceneName);
		yield break;
	}

	/// <summary>
	/// 淡出协程
	/// </summary>
	/// <param name="renderColor">淡出颜色</param>
	/// <returns></returns>
	IEnumerator FadeOut(Color renderColor) {
		isRenderOut = true;
		GM.Log("Render Out");
		alpha = 0;
		while (alpha < 1) {
			alpha += Time.deltaTime * rendSpeed;
			transform.GetChild(0).GetComponent<Image>().color = new Vector4(renderColor.r, renderColor.g, renderColor.b, alpha);
			yield return new WaitForSeconds(0);
		}
		GM.Log("Render Out End");
		isRenderOut = false;
	}

	public IEnumerator MoveBackGround(Vector3 backGoalPos, Vector3 midGoalPos, float backSpeed, float midSpeed) {
		while (true) {
			if (!GM.JudgeReachGoalPosition(backgroundTransform.position, backGoalPos)) {
				backgroundTransform.position = Vector3.MoveTowards(backgroundTransform.position, backGoalPos, backSpeed*Time.deltaTime);
			}
			if (!GM.JudgeReachGoalPosition(middlegroundTransform.position, midGoalPos)) {
				middlegroundTransform.position = Vector3.MoveTowards(middlegroundTransform.position, midGoalPos, midSpeed*Time.deltaTime);
			}
			else {
				break;
			}
			yield return 0;
		}
		GM.Log("背景移动 Over" );
		isbackgroundFollowCamera = true;
	}

	public void SetBackGroundPosition(Vector3 backPosition, Vector3 midPosition) {
		backgroundTransform = GameObject.Find("background").transform;
		middlegroundTransform = GameObject.Find("middleground").transform;

		backgroundTransform.position = backPosition;
		middlegroundTransform.position = midPosition;
	}

	/// <summary>
	/// 修改背景图协程
	/// </summary>
	/// <param name="e"></param>
	public void ChangeBackground(Color outColor, Sprite imageSource, Vector3 imageScale, Vector3 imagePosition, Color inColor) {
		StartCoroutine(ChangeBackgroundCoroutine(outColor, imageSource, imageScale, imagePosition, inColor));
	}

	/// <summary>
	/// 修改背景图协程
	/// </summary>
	/// <param name="outColor">当前背景图淡出速度</param>
	/// <param name="imageSource">目标背景图Image</param>
	/// <param name="imageScale">目标背景图缩放</param>
	/// <param name="imagePosition">目标背景图位置</param>
	/// <param name="inColor">目标背景图淡入速度</param>
	/// <returns></returns>
	IEnumerator ChangeBackgroundCoroutine(Color outColor, Sprite imageSource, Vector3 imageScale, Vector3 imagePosition, Color inColor) {
		yield return StartCoroutine(FadeOut(inColor));

		FindObjectOfType<SpriteRenderer>().sprite = imageSource;
		FindObjectOfType<SpriteRenderer>().transform.localScale = imageScale;
		FindObjectOfType<SpriteRenderer>().transform.position = imagePosition;

		GM.Log("Change Background Over");
		yield return StartCoroutine(FadeIn(outColor));
		yield break;
	}
}
