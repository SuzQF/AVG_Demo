using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Project Name: OurSmallWorld
* Create Date: 2020/11/3
* Author: Suz
* Update Record: 
* 11/20	删除事件相关，转为直接调用
*/

/// <summary>
/// 场景入口
/// </summary>
public class Enterance : MonoBehaviour {
	//场景进入通行证
	[SerializeField] private string sceneEnterPassword;

	void Start() {

		//从Exit获取
		sceneEnterPassword = transform.parent.GetComponent<Exit>().targetSceneName;
		if (PlayerController.instance==null) {
			return;
		}
		if (PlayerController.instance.sceneChangePassword == sceneEnterPassword) {
			PlayerController.instance.transform.position = transform.position;
			PlayerController.instance.animator.SetBool("isWalking", false);

			//调用ScenesManager类的进入新场景设置方法
			ScenesManager.instance.EnterNewSceneSet(
				transform.parent.GetComponent<Exit>().targetSceneName,
				transform.parent.GetComponent<Exit>().nativeSceneName);
		}
	}

}
