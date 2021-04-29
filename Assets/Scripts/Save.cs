using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

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

[System.Serializable]
public class Save{

	//MainStoryScript
	public int chapterCount;

	//SceneManager
	public string currentSceneName;

	//PlayerController
	public float[] playerPosition = new float[3];

	//CameraController
	public float[] cameraPosition = new float[3];
	public bool isFollowing;


}
