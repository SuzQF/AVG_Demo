using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
* Project Name: OurSmallWorld
* Create Date: 2020/11/3
* Author: Suz
* Update Record: 
* 
*/

/// <summary>
/// 场景出口类
/// </summary>
public class Exit : MonoBehaviour
{

	//当前场景名
	public string nativeSceneName;

	//目标场景名
	public string targetSceneName;

	private void Awake() {
		nativeSceneName = SceneManager.GetActiveScene().name;
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.tag == "Player") {
			PlayerController.instance.sceneChangePassword = nativeSceneName;
			ScenesManager.instance.ChangeScene(Color.black,targetSceneName);
		}
	}
}
