using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Project Name: OurSmallWorld
* Create Date: 2020/11/19
* Author: Suz
* Update Record: 
* 11/19	将BGM控制从AudioController类中延伸出来单独成类
*/

/// <summary>
///BGM控制器类
/// </summary>
public class BGMController : AudioControllerBase
{
	//BGM控制器类单例
	public static BGMController instance;

	//BGM音频列表
	public AudioClip MainMenuBGM;
	public AudioClip VillageBGM;
	public AudioClip ForestBGM;

	void Awake() {
		if (instance == null) {
			instance = this;
		}

		else if (instance != this) {
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);

		source = GetComponent<AudioSource>();
	}

}
