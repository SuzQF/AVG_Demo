using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Project Name: OurSmallWorld
* Create Date: 2020/11/3
* Author: Suz
* Update Record: 
* 11/4	完成的脚本转移到本类
* 11/12	完成了章节选择，目前将所有脚本的调用暂时用统一方法实现，待改进
* 
*/

/// <summary>
///
/// </summary>
public class ItemScript : PerformanceManagerBase {

	//物品脚本单例
	public static ItemScript instance;

	//当前场景的摄像机
	public CameraController cameraController;

	//当前演出章节名
	public string currentChapter;

	public Transform currentItemTransform;

	//反射
	string mainStory = "ItemScript";

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
	/// 场景物品章节入口
	/// </summary>
	/// <param name="ChapterNumber">章节名称</param>
	public void ChooseItemChapter(Transform transform) {

		currentItemTransform = transform;

		ChapterStart("Item");
		GM.Log("Chapter " + transform.name);

		currentChapter = transform.name;
		StartCoroutine(transform.name);
	}


	/// <summary>
	/// Smithy脚本
	/// </summary>
	/// <returns></returns>
	public IEnumerator Smithy() {
		while (!isChapterEnd) {
			switch (currentPerformance) {
				case 0:
					GM.Log("开启Line文件流");
					LinesController.instance.linesReader = GM.instance.InitLineFileReader(currentChapter);

					GM.Log("Arvin台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Arvin台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Arvin台词");
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
	/// RoadSigh脚本
	/// </summary>
	/// <returns></returns>
	public IEnumerator RoadSign() {
		while (!isChapterEnd) {
			switch (currentPerformance) {
				case 0:
					GM.Log("开启Line文件流");
					LinesController.instance.linesReader = GM.instance.InitLineFileReader(currentChapter);

					GM.Log("Arvin台词");
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
	/// Herbs脚本
	/// </summary>
	/// <returns></returns>
	public IEnumerator Herbs() {
		while (!isChapterEnd) {
			switch (currentPerformance) {
				case 0:
					GM.Log("开启Line文件流");
					LinesController.instance.linesReader = GM.instance.InitLineFileReader(currentChapter);

					GM.Log("Arvin台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Arvin台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Arvin台词");
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
	/// Pot脚本
	/// </summary>
	/// <returns></returns>
	public IEnumerator Pot() {
		while (!isChapterEnd) {
			switch (currentPerformance) {
				case 0:
					GM.Log("开启Line文件流");
					LinesController.instance.linesReader = GM.instance.InitLineFileReader(currentChapter);

					GM.Log("Arvin台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Arvin台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Arvin台词");
					yield return StartCoroutine(LinesController.instance.NextLine());

					GM.Log("Arvin台词");
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

		GM.Log("Item Chapter End");

		//章节结束flag，跳出章节演出循环
		isChapterEnd = true;

		//重置演出指针
		currentPerformance = 0;

		waitSeconds = 0;

		//重置章节类型Flag
		isItemChapter = false;

		//章节结束，退出交互模式
		GM.instance.OutFromInteractMode();
	}
}
