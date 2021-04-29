using System.Collections;
using UnityEngine;

/*
*Project Name: 
*Create Date: 
*Author: 
*Update Record: 
*
*/

/// <summary>
///
/// </summary>
public class RoleScript : PerformanceManagerBase {
	//主线脚本单例
	public static RoleScript instance;

	//当前场景的摄像机
	//public CameraController cameraController;

	//当前演出章节名
	public string currentChapter;

	public Transform currentRoleTransform;

	//反射
	string mainStory = "RoleScript";

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
	/// 角色章节入口
	/// </summary>
	/// <param name="ChapterNumber">第几章</param>
	public void ChooseRoleChapter(Transform roleTransform) {
		ChapterStart("Role");

		currentRoleTransform = roleTransform;

		//清除生成Perfab的“(Clone)”后缀
		string chapterTopic = currentRoleTransform.name.Split('(')[0];
		GM.Log("Chapter " + chapterTopic);

		//如果在巡逻则停止移动
		if (currentRoleTransform.GetComponent<RoleController>().isPatrol) {
			currentRoleTransform.GetComponent<RoleController>().RoleStop();
		}

		currentChapter = chapterTopic;
		StartCoroutine(chapterTopic);
	}

	/// <summary>
	/// Hatman脚本
	/// </summary>
	/// <returns></returns>
	public IEnumerator Hatman() {
		while (!isChapterEnd) {
			switch (currentPerformance) {
				case 0:
					GM.Log("开启Line文件流");
					LinesController.instance.linesReader = GM.instance.InitLineFileReader(currentChapter);

					GM.Log("Arvin台词");
					yield return StartCoroutine(LinesController.instance.NextLine());
					break;
				case 1:
					GM.Log("Hatman台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("关闭Line文件流");
					GM.instance.CloseLineFileReader();

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

	/// <summary>
	/// Oldman脚本
	/// </summary>
	/// <returns></returns>
	public IEnumerator Oldman() {
		while (!isChapterEnd) {
			switch (currentPerformance) {
				case 0:
					GM.Log("开启Line文件流");
					LinesController.instance.linesReader = GM.instance.InitLineFileReader(currentChapter);

					SoundController.instance.PlaySound(currentRoleTransform, "OldmanCough");
					waitSeconds = 1f;
					break;
				case 1:
					GM.Log("Oldman台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Oldman台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Oldman台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Oldman台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Oldman台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Arvin台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("关闭Line文件流");
					GM.instance.CloseLineFileReader();

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
	/// 章节结束整理
	/// </summary>
	protected override void ChapterEnd() {

		GM.Log("Role Chapter End");

		//章节结束flag，跳出章节演出循环
		isChapterEnd = true;

		//重置演出指针
		currentPerformance = 0;

		if (currentRoleTransform.GetComponent<RoleController>().canPatrol) {
			currentRoleTransform.GetComponent<RoleController>().isPatrol = true;
			currentRoleTransform.GetComponent<RoleController>().goalPosition = currentRoleTransform.GetComponent<RoleController>().PatrolPosition1;
		}

		waitSeconds = 0;

		//重置章节类型Flag
		isRoleChapter = false;

		//章节结束，退出交互模式
		GM.instance.OutFromInteractMode();
	}
}
