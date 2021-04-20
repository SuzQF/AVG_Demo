using UnityEngine;


/*
* Project Name: OurSmallWorld
* Create Date: 2020/11/3
* Author: Suz
* Update Record: 
* 11/4	将本类抽象化为三个脚本类的基类，完成了已有事件的封装
*			Awake方法体封装成两个方法便于派生类调用
*			保留了章节开始方法，章节结束方法抽象化，由派生类实现
* 11/16	去除Awake方法，本类不需要初始化，本类的Awake方法会默认成为派生类的默认Awake方法，无益
* 11/18	删除了角色移动相关的事件声明与事件触发方法，改为直接调用角色RoleController方法
* 11/19	彻底删除所有类型的事件相关代码，所有事件触发均改为直接调用
*/

public abstract class PerformanceManagerBase : MonoBehaviour {

	//脚本类型
	public static bool isStoryChapter = false;
	public static bool isRoleChapter = false;
	public static bool isItemChapter = false;

	//演出迭代等待时间，随时调整
	public float waitSeconds = 0f;

	//当前演出指针
	[SerializeField] protected int currentPerformance = 0;

	//章节结束flag，用于维持和跳出章节演出循环
	[SerializeField] protected bool isChapterEnd = true;

	/// <summary>
	/// 章节开始处理
	/// </summary>
	/// <param name="chapterType"></param>
	protected void ChapterStart(string chapterType) {

		//章节开始，进入交互模式（玩家无法操作角色）
		GM.instance.IntoInteractMode();

		RoleManager.instance.FindRoleController(RoleManager.Arvin).RoleStop();

		//设置脚本类型
		SetChapterType(chapterType);

		//脚本开启flag
		isChapterEnd = false;
	}

	/// <summary>
	/// 章节结束处理
	/// </summary>
	protected abstract void ChapterEnd();

	/// <summary>
	/// 设置章节脚本类型
	/// </summary>
	/// <param name="type"></param>
	private void SetChapterType(string type) {
		isStoryChapter = false;
		isRoleChapter = false;
		isItemChapter = false;
		switch (type) {
			case "Story":
				isStoryChapter = true;
				break;
			case "Item":
				isItemChapter = true;
				break;
			case "Role":
				isRoleChapter = true;
				break;
			default:
				break;
		}
	}

	
}
