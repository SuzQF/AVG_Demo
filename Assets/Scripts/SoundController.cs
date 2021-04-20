using System;
using System.Collections;
using System.Collections.Generic;
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
public class SoundController : AudioControllerBase
{
	//BGM控制器类单例
	public static SoundController instance;

	private Transform soundFrom;

	public AudioClip BearBottle;
	public AudioClip Door1;
	public AudioClip Door2;
	public AudioClip Herbs;
	public AudioClip OldmanCough;
	public AudioClip RunInTown;
	public AudioClip Walk_RunInForest;
	public AudioClip WalkAround;
	public AudioClip WomanBreath;

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

	public void PlaySound(Transform sounder, string soundName, float volume = 1) {
		AudioSource audio = sounder.transform.Find(soundName).GetComponent<AudioSource>();
		audio.volume = volume;
		audio.Play();
	}

	public void StopSound(Transform sounder, string soundName) {
		sounder.transform.Find(soundName).GetComponent<AudioSource>().Stop();
	}
}
