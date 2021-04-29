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
public class SoundController : MonoBehaviour
{
	//BGM控制器类单例
	public static SoundController instance;

	public AudioClip BearBottle;
	public AudioClip Door1;
	public AudioClip Door2;
	public AudioClip Herbs;
	public AudioClip OldmanCough;
	public AudioClip RunInTown;
	public AudioClip WalkInForest;
	public AudioClip RunInForest;
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
	}

	public void PlaySound(Transform sounder, string soundName,float volume = 1) {

		//AudioSource audio = Instantiate(SoundPrefab, sounder.position, Quaternion.identity).GetComponent<AudioSource>();
		//audio.transform.SetParent(sounder);
		//audio.clip = clip;
		//audio.loop = isLoop;
		if (sounder.transform.Find(soundName).GetComponent<AudioSource>().isPlaying) {
			return;
		}
		AudioSource audio = sounder.transform.Find(soundName).GetComponent<AudioSource>();
		audio.volume = volume;
		audio.Play();
	}

	public void SetSoundClip(Transform sounder, string soundName,AudioClip clip,float volume) {
		AudioSource audio =  sounder.Find(soundName).GetComponent<AudioSource>();
		audio.clip = clip;
		audio.volume = volume;
	}

	public void StopSound(Transform sounder, string soundName) {
		sounder.transform.Find(soundName).GetComponent<AudioSource>().Stop();
	}
}
