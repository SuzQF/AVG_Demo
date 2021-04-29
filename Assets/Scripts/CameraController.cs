using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
*Project Name: OurSmallWorld
*Create Date: 2020/11/3
*Author: Suz
*Update Record: 
*
*/

public class CameraController : MonoBehaviour
{

	public new Camera camera; 
	public Transform target;

	public Vector3 positionMark;
	public float sizeMark;

	private void Awake() {
		camera = GetComponent<Camera>();
		if (PlayerController.instance!=null) {
			target = PlayerController.instance.transform;
		}
	}

	void LateUpdate(){
		CameraFollow();
    }



	/// <summary>
	/// 摄像机移动协程
	/// </summary>
	/// <returns></returns>
	public IEnumerator MoveCamera(Vector3 goalPosition, float speed) {
		//记录移动之前位置坐标
		positionMark = transform.position;

		//设跟随目标为空（取消跟随）
		SetTarget(null);

		//避免输入坐标时误将z轴坐标设为0
		goalPosition = new Vector3(goalPosition.x, goalPosition.y, -10);

		while (!GM.JudgeReachGoalPosition(transform.position,goalPosition)) {
			transform.position = Vector3.MoveTowards(transform.position, goalPosition, speed*Time.deltaTime);
			yield return 0;
		}
		SetCameraPosition(goalPosition);
	}


	/// <summary>
	/// 摄像机跟随
	/// </summary>
	public void CameraFollow() {
		if (target==null) {
			return;
		}
		else {
			transform.position = new Vector3(target.position.x, target.position.y + 3.6f, transform.position.z);
		}
		if (transform.position.x<-1.8&&camera.orthographicSize<5) {
			transform.position=new Vector3(-1.8f, target.position.y + 3.6f, transform.position.z);
		}
		if (camera.orthographicSize == 5) {
			if (transform.position.x < 0) {
				transform.position = new Vector3(0, target.position.y + 3.6f, transform.position.z);
			}
			if (transform.position.x>10.5f) {
				transform.position = new Vector3(10.5f, target.position.y + 3.6f, transform.position.z);
			}
		}
	}

	/// <summary>
	/// 设置摄像机跟随目标
	/// </summary>
	/// <param name="target"></param>
	public void SetTarget(Transform target) {
		this.target = target;
		GM.Log("Camera Set Target" + target);
	}

	/// <summary>
	/// 缩小摄像机视野协程
	/// </summary>
	/// <param name="size"></param>
	/// <param name="speed"></param>
	/// <returns></returns>
	public IEnumerator CameraShrink(float size, float speed = 1) {
		sizeMark = camera.orthographicSize;

		while (camera.orthographicSize > size) {
			camera.orthographicSize -= speed * Time.deltaTime;
			yield return 0;
		}
		camera.orthographicSize = size;
		yield break;
	}

	/// <summary>
	/// 放大摄像机视野协程
	/// </summary>
	/// <param name="size"></param>
	/// <param name="speed"></param>
	/// <returns></returns>
	public IEnumerator CameraMagnify(float size, float speed = 1) {
		sizeMark = camera.orthographicSize;

		while (camera.orthographicSize<size) {
			camera.orthographicSize += speed * Time.deltaTime;
			yield return 0;
		}
		camera.orthographicSize = size;
		yield break;
	}

	public void SetCameraPosition(Vector3 position) {
		camera.transform.position = position;
	}

	public void SetCameraSize(float size) {
		camera.orthographicSize = size;
	}
}
