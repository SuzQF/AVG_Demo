using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

/*
* Project Name: OurSmallWorld
* Create Date: 2020/11/3
* Author: Suz
* Update Record: 
* 11/4	已完成部分的主线脚本转移至本类，下属模块的事件调用改为调用基类的事件触发方法。
* 11/16	将主线脚本相关数据转移到本类，本类只处理涉及主线脚本的功能 
* 11/18	角色移动相关的功能调用由事件触发改为直接调用对应角色RoleController对象的对应方法
* 11/19	删除所有事件方法的调用，所有功能调用改为方法调用（NextLine无法用方法调用，只能保留StartCoroutine形式）
* 4/11 清除数组形式的对话内容存储，改由文件存储并在演出中读取。
*/

/// <summary>
///
/// </summary>
public class MainStoryScript : PerformanceManagerBase {
	//主线脚本单例
	public static MainStoryScript instance;

	//Sprite资源
	public Sprite backgroundSprite;
	public Sprite middlegroundSprite;

	//当前场景的摄像机
	public CameraController cameraController;

	//已演出章节
	public int chapterCount = 0;

	//当前演出章节名
	public string currentChapter;

	//反射
	string mainStory = "MainStoryScript";

	/// <summary>
	/// 初始化
	/// </summary>
	private void Awake() {
		InstanceInit();
	}

	/// <summary>
	/// 单例初始化
	/// </summary>
	private void InstanceInit() {
		if (instance == null) {
			instance = this;
		}

		else if (instance != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
	}

	/// <summary>
	/// 剧情章节入口
	/// </summary>
	/// <param name="ChapterNumber">第几章</param>
	public void ChooseStoryChapter(string chapterTopic) {
		ChapterStart("Story");
		GM.Log("Chapter " + chapterTopic);
		currentChapter = chapterTopic;
		StartCoroutine(chapterTopic);
	}

	/// <summary>
	/// 章节结束整理
	/// </summary>
	protected override void ChapterEnd() {

		GM.Log("Story Chapter " + chapterCount + " End");

		//章节结束flag，跳出章节演出循环
		isChapterEnd = true;

		//重置演出指针
		currentPerformance = 0;

		//章节+1
		chapterCount += 1;

		waitSeconds = 0;

		//重置章节类型Flag
		isStoryChapter = false;

		//章节结束，退出交互模式
		GM.instance.OutFromInteractMode();
	}



	//脚本———————————————————————————————————————————————————————

	/// <summary>
	/// 序章
	/// </summary>
	/// <returns></returns>
	public IEnumerator FirstToVillage() {
		while (!isChapterEnd) {
			switch (currentPerformance) {
				case 0:
					GM.Log("初始化当前场景CameraController");
					cameraController = FindObjectOfType<CameraController>();

					GM.Log("设置摄像机位置");
					cameraController.SetCameraPosition(new Vector3(0, 20.73f, -10));

					GM.Log("设置摄像机视野");
					cameraController.SetCameraSize(5);

					GM.Log("设置背景位置");
					ScenesManager.instance.SetBackGroundPosition(new Vector3(0, 19.03f, 0), new Vector3(0, 9.19f, 0));

					GM.Log("移动摄像机");
					StartCoroutine(cameraController.MoveCamera(new Vector3(0, -5.4f, -10), 2.6f));

					GM.Log("移动背景");
					StartCoroutine(ScenesManager.instance.MoveBackGround(new Vector3(0, -1.65f, 0), new Vector3(0, -4, 0), 2.1f, 1.4f));

					GM.Log("开启Aside文件流");
					AsidesController.instance.asidesReader = GM.instance.InitLineFileReader("Opening");

					GM.Log("第一句旁白");
					AsidesController.instance.NextAside(new Vector3(0, 2, 0) + cameraController.transform.position);
					waitSeconds = 2.61f;
					break;
				case 1:
					GM.Log("第二句旁白");
					AsidesController.instance.NextAside(new Vector3(0, 2, 0) + cameraController.transform.position);
					break;
				case 2:
					GM.Log("第三句旁白");
					AsidesController.instance.NextAside(new Vector3(0, 2, 0) + cameraController.transform.position);
					break;
				case 3:
					GM.Log("第四句旁白");
					AsidesController.instance.NextAside(new Vector3(0, 2, 0) + cameraController.transform.position);
					break;
				case 4:
					GM.Log("第五句旁白");
					AsidesController.instance.NextAside(new Vector3(0, 2, 0) + cameraController.transform.position);

					waitSeconds = 2.51f;
					break;
				case 5:
					GM.Log("移动相机");
					StartCoroutine(cameraController.MoveCamera(new Vector3(-1.8f, -6.64f, -10), 2.2f));

					GM.Log("缩小视野");
					StartCoroutine(cameraController.CameraShrink(4f, 1));
					waitSeconds = 2f;
					break;
				case 6:
					GM.Log("设置角色位置");
					RoleManager.instance.FindRoleController(RoleManager.Arvin).SetRolePosition(new Vector3(-9.3f, -8.89f, 0));

					GM.Log("主角入场");
					StartCoroutine(RoleManager.instance.FindRoleController(RoleManager.Arvin).RoleMove(new Vector3(-5, -8.89f, 0)));
					break;
				case 7:
					GM.Log("关闭Aside文件流");
					GM.instance.CloseLineFileReader();

					GM.Log("打开Line文件流");
					LinesController.instance.linesReader = GM.instance.InitLineFileReader("FirstToVillage");

					GM.Log("Arvin台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Arvin台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Arvin台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					waitSeconds = 0.5f;
					break;
				case 8:
					GM.Log("关闭Line文件流");
					GM.instance.CloseLineFileReader();

					GM.Log("移动相机");
					StartCoroutine(cameraController.MoveCamera(cameraController.positionMark, 2.2f));

					GM.Log("放大相机");
					yield return StartCoroutine(cameraController.CameraMagnify(5, 1));

					waitSeconds = 0f;
					break;
				case 9:
					GM.Log("摄像机跟随");
					cameraController.SetTarget(RoleManager.instance.FindRoleController(RoleManager.Arvin).transform);

					ChapterEnd();
					yield break;

				default:
					break;
			}
			currentPerformance += 1;
			yield return new WaitForSeconds(waitSeconds);
		}
	}


	/// <summary>
	/// TalkWithBearded脚本
	/// </summary>
	/// <returns></returns>
	public IEnumerator TalkWithBearded() {
		while (!isChapterEnd) {
			switch (currentPerformance) {
				case 0:
					GM.Log("Arvin向右移动");
					yield return StartCoroutine(RoleManager.instance.FindRoleController(RoleManager.Arvin).RoleMove(new Vector3(11.4f, -8.88f, 0),2.4f));

					StartCoroutine(cameraController.MoveCamera(new Vector3(11.4f, -6.64f, -10), 2.2f));

					StartCoroutine(cameraController.CameraShrink(4, 1));
					break;
				case 1:
					GM.Log("Bearded右转");
					RoleManager.instance.FindRoleController(RoleManager.Bearded).RoleTurn(RoleController.RIGHT);

					GM.Log("开启Line文件流");
					LinesController.instance.linesReader = GM.instance.InitLineFileReader("TalkWithBearded");

					GM.Log("Arvin停下");
					RoleManager.instance.FindRoleController(RoleManager.Arvin).RoleStop();

					GM.Log("Bearded台词");
					yield return StartCoroutine(LinesController.instance.NextLine());
					waitSeconds = 0;
					break;
				case 2:
					
					GM.Log("Arvin左转");
					RoleManager.instance.FindRoleController(RoleManager.Arvin).RoleTurn(RoleController.LEFT);

					GM.Log("Arvin台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Bearded台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Arvin台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Bearded台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Bearded台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Arvin台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Arvin台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Bearded台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Bearded台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Arvin台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("关闭Line文件流");
					GM.instance.CloseLineFileReader();

					StartCoroutine(cameraController.MoveCamera(cameraController.positionMark, 2.2f));

					yield return StartCoroutine(cameraController.CameraMagnify(5, 1));

					ChapterEnd();
					yield break;

				default:
					break;
			}
			currentPerformance += 1;
			yield return new WaitForSeconds(waitSeconds);
		}

	}

	/// <summary>
	/// MeetWithAkauri脚本
	/// </summary>
	/// <returns></returns>
	public IEnumerator MeetWithAkauri() {
		while (!isChapterEnd) {
			switch (currentPerformance) {
				case 0:
					cameraController = FindObjectOfType<CameraController>();

					RoleManager.instance.FindRoleController(RoleManager.Arvin).SetRolePosition(new Vector3(7.325f, -0.714f, 0));

					RoleManager.instance.FindRoleController(RoleManager.Arvin).RoleStop();

					StartCoroutine(cameraController.MoveCamera(new Vector3(6.13f, 2.05f, -10), 2.2f));

					StartCoroutine(cameraController.CameraShrink(4, 1));

					GM.Log("开启Line文件流");
					LinesController.instance.linesReader = GM.instance.InitLineFileReader("MeetWithAkauri");

					GM.Log("Arvin台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Arvin台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("设置Akauri位置");
					RoleManager.instance.FindRoleController(RoleManager.Akauri).SetRolePosition(new Vector3(14f, -0.819f, 0));

					yield return StartCoroutine(RoleManager.instance.FindRoleController(RoleManager.Akauri).RoleMove(new Vector3(9, -0.819f, 0)));
					break;
				case 1:

					GM.Log("Akauri台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Arvin台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Akauri台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Akauri台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					StartCoroutine(RoleManager.instance.FindRoleController(RoleManager.Akauri).RoleMove(new Vector3(10.3f, -0.816f, 0)));
					waitSeconds = 0f;
					break;
				case 2:
					StartCoroutine(RoleManager.instance.FindRoleController(RoleManager.Arvin).RoleMove(new Vector3(8.5f, -0.714f, 0)));

					GM.Log("Arvin台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					waitSeconds = 0.1f;
					break;
				case 3:
					RoleManager.instance.FindRoleController(RoleManager.Akauri).RoleTurn(RoleController.LEFT);

					GM.Log("Akauri台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Akauri台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Akauri台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Akauri台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Akauri台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Akauri台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Akauri台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Akauri台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("关闭Line文件流");
					GM.instance.CloseLineFileReader();

					LinesController.instance.ChooseSideLine("GoWithAkauri", "BackToVillage");

					ChapterEnd();
					yield break;

				default:
					break;
			}
			currentPerformance += 1;
			yield return new WaitForSeconds(waitSeconds);
		}

	}

	/// <summary>
	/// GoWithAkauri脚本模板
	/// </summary>
	/// <returns></returns>
	public IEnumerator GoWithAkauri() {
		while (!isChapterEnd) {
			switch (currentPerformance) {
				case 0:
					GM.Log("打开Line文件流");
					LinesController.instance.linesReader = GM.instance.InitLineFileReader("GoWithAkauri");

					StartCoroutine(RoleManager.instance.FindRoleController(RoleManager.Arvin).RoleMove(new Vector3(9.2f, -0.714f, 0)));

					GM.Log("Arvin台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Arvin台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Akauri台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Akauri台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("关闭Line文件流");
					GM.instance.CloseLineFileReader();

					StartCoroutine(RoleManager.instance.FindRoleController(RoleManager.Akauri).RoleMove(new Vector3(14f, -0.819f, 0)));

					yield return StartCoroutine(RoleManager.instance.FindRoleController(RoleManager.Arvin).RoleMove(new Vector3(14f, -0.714f, 0)));

					ChapterEnd();
					yield break;

				default:
					break;
			}
			currentPerformance += 1;
			yield return new WaitForSeconds(waitSeconds);
		}

	}

	/// <summary>
	/// BackToVillage脚本模板
	/// </summary>
	/// <returns></returns>
	public IEnumerator BackToVillage() {
		while (!isChapterEnd) {
			switch (currentPerformance) {
				case 0:
					GM.Log("打开Line文件流");
					LinesController.instance.linesReader = GM.instance.InitLineFileReader("BackToVillage");

					GM.Log("Arvin台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Arvin台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Arvin台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Akauri台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					StartCoroutine(RoleManager.instance.FindRoleController(RoleManager.Akauri).RoleMove(new Vector3(14f, -0.819f, 0)));

					GM.Log("重定位");
					Exit[] temp = FindObjectsOfType<Exit>();
					foreach (var exit in temp) {
						if (exit.targetSceneName=="Village") {
							exit.targetSceneName = "EndScene2";
							break;
						}
					}

					GM.Log("关闭Line文件流");
					GM.instance.CloseLineFileReader();

					GM.Log("放大相机");
					StartCoroutine(cameraController.CameraMagnify(cameraController.sizeMark, 1));

					GM.Log("移动相机");
					yield return StartCoroutine(cameraController.MoveCamera(cameraController.positionMark, 2.2f));

					ChapterEnd();
					yield break;

				default:
					break;
			}
			currentPerformance += 1;
			yield return new WaitForSeconds(waitSeconds);
		}

	}

	/// <summary>
	/// End1
	/// </summary>
	/// <returns></returns>
	public IEnumerator End1() {
		while (!isChapterEnd) {
			switch (currentPerformance) {
				case 0:
					AsidesController.instance.asidesReader = GM.instance.InitLineFileReader("End1");

					cameraController = FindObjectOfType<CameraController>();

					GM.Log("旁白1");
					AsidesController.instance.NextAside(new Vector3(0, 3, 0) + cameraController.transform.position, isDistroy: false);
					waitSeconds = 2.61f;
					break;
				case 1:
					GM.Log("旁白2");
					AsidesController.instance.NextAside(new Vector3(0, 2, 0) + cameraController.transform.position, isDistroy: false);
					break;
				case 2:
					GM.Log("旁白3");
					AsidesController.instance.NextAside(new Vector3(0, 1, 0) + cameraController.transform.position, isDistroy: false);
					break;
				case 3:
					GM.Log("旁白4");
					AsidesController.instance.NextAside(new Vector3(0, 0, 0) + cameraController.transform.position, isDistroy: false);
					break;
				case 4:
					GM.Log("旁白");
					AsidesController.instance.NextAside(new Vector3(0, -1, 0) + cameraController.transform.position, isDistroy: false);
					break;
				case 5:
					GM.Log("旁白");
					AsidesController.instance.NextAside(new Vector3(0, -2, 0) + cameraController.transform.position, isDistroy: false);
					break;
				case 6:
					GM.Log("旁白");
					AsidesController.instance.NextAside(new Vector3(0, -3, 0) + cameraController.transform.position, isDistroy: false);
					break;
				case 7:
					GM.Log("旁白");
					AsidesController.instance.NextAside(new Vector3(0, -4, 0) + cameraController.transform.position, isDistroy: false);

					GM.instance.CloseLineFileReader();
					break;
				case 8:
					ScenesManager.instance.ChangeScene(Color.black, "MainMenu");
					ChapterEnd();
					yield break;
				default:
					break;
			}
			currentPerformance += 1;
			yield return new WaitForSeconds(waitSeconds);
		}

	}

	/// <summary>
	/// End2
	/// </summary>
	/// <returns></returns>
	public IEnumerator End2() {
		while (!isChapterEnd) {
			switch (currentPerformance) {
				case 0:
					AsidesController.instance.asidesReader = GM.instance.InitLineFileReader("End2");

					cameraController = FindObjectOfType<CameraController>();

					GM.Log("旁白1");
					AsidesController.instance.NextAside(new Vector3(0, 3, 0) + cameraController.transform.position, isDistroy: false);
					waitSeconds = 2.61f;
					break;
				case 1:
					GM.Log("旁白2");
					AsidesController.instance.NextAside(new Vector3(0, 2, 0) + cameraController.transform.position, isDistroy: false);
					break;
				case 2:
					GM.Log("旁白3");
					AsidesController.instance.NextAside(new Vector3(0, 1, 0) + cameraController.transform.position, isDistroy: false);
					break;
				case 3:
					GM.Log("旁白4");
					AsidesController.instance.NextAside(new Vector3(0, 0, 0) + cameraController.transform.position, isDistroy: false);
					break;
				case 4:
					GM.Log("旁白");
					AsidesController.instance.NextAside(new Vector3(0, -1, 0) + cameraController.transform.position, isDistroy: false);
					break;
				case 5:
					GM.Log("旁白");
					AsidesController.instance.NextAside(new Vector3(0, -2, 0) + cameraController.transform.position, isDistroy: false);
					break;
				case 6:
					GM.Log("旁白");
					AsidesController.instance.NextAside(new Vector3(0, -3, 0) + cameraController.transform.position, isDistroy: false);
					break;
				case 7:
					GM.Log("旁白");
					AsidesController.instance.NextAside(new Vector3(0, -4, 0) + cameraController.transform.position, isDistroy: false);

					GM.instance.CloseLineFileReader();
					break;
				case 8:
					ScenesManager.instance.ChangeScene(Color.black, "MainMenu");

					ChapterEnd();
					yield break;
				default:
					break;
			}
			currentPerformance += 1;
			yield return new WaitForSeconds(waitSeconds);
		}

	}

	/// <summary>
	/// 脚本模板
	/// </summary>
	/// <returns></returns>
	public IEnumerator Chapter() {
		while (!isChapterEnd) {
			switch (currentPerformance) {
				case 0:
					//yield return StartCoroutine(LinesController.instance.NextLine());
					ChapterEnd();
					yield break;

				default:
					break;
			}
			currentPerformance += 1;
			yield return new WaitForSeconds(waitSeconds);
		}

	}

}
