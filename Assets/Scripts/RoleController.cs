using System.Collections;
using UnityEngine;

/*
* Project Name: OurSmallWorld
* Create Date: 2020/11/3
* Author: Suz
* Update Record: 
* 11/18	明确了RoleController定位，功能精简优化，去掉了多余的事件相关模块
* 11/20	添加了人物朝向常量，便于设置人物朝向，关于根据坐标自动设置朝向的想法待定
*/

public class RoleController : MonoBehaviour {

	public string RoleName;

	//移动速度
	public static float moveSpeed = 3.2f;

	//由导演传来的移动指令信息
	private bool isMoveOrder;

	//人物朝向常量（人物所有图片默认向右）
	public const bool RIGHT = false;
	public const bool LEFT = true;

	public Animator animator;

	//移动目标位置
	private Vector3 goalPosition;

	private void Awake() {
		animator = GetComponent<Animator>();	
	}

	/// <summary>
	/// 角色持续移动方法
	/// </summary>
	private void Move() {
		animator.SetBool("isWalking", true);
		if (goalPosition.x>=transform.position.x) {
			RoleTurn(RIGHT);
		}
		else {
			RoleTurn(LEFT);
		}
		transform.position = Vector3.MoveTowards(transform.position, goalPosition, moveSpeed * Time.deltaTime);
	}

	/// <summary>
	/// 角色位置设置
	/// </summary>
	/// <param name="position"></param>
	public void SetRolePosition(Vector3 position) {
		transform.position = position;
	}

	/// <summary>
	/// 角色转身
	/// </summary>
	/// <param name="turnDirection">true为左 false为右 </param>
	public void RoleTurn(bool turnDirection) {
		GetComponent<SpriteRenderer>().flipX = turnDirection;
	}

	/// <summary>
	/// 角色移动
	/// </summary>
	/// <param name="goalPosition"></param>
	public IEnumerator RoleMove(Vector3 goalPosition, float speed = 3.2f) {
		this.goalPosition = goalPosition;
		isMoveOrder = true;
		moveSpeed = speed;
		while (!GM.JudgeReachGoalPosition(transform.position, goalPosition)) {
			Move();
			yield return 0;
		}
		RoleStop();

	}

	/// <summary>
	/// 角色停止移动
	/// </summary>
	public void RoleStop() {
		isMoveOrder = false;
		moveSpeed = 3.2f;
		goalPosition = transform.position;
		PlayerController.instance.rg.velocity = new Vector2(0, 0);
		animator.SetBool("isWalking", false);
	}


}
	