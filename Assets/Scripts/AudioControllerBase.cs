using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Project Name: OurSmallWorld
* Create Date: 2020/11/3
* Author: Suz
* Update Record: 
* 11/19	删除了冗余的事件相关代码，优化方法封装
*/

/// <summary>
/// 音频控制器基类
/// </summary>
public class AudioControllerBase : MonoBehaviour {

	//音频控制器直接操作的音源
	protected AudioSource source;
	

	/// <summary>
	/// 播放音频
	/// </summary>
	/// <param name="rendSpeed">淡入速度</param>
	public void PlayAudio(AudioClip audio, float rendInSpeed = 0.125f,float volume = 1) {
		StartCoroutine(FadeInCoroutine(audio, rendInSpeed,volume));
	}

	/// <summary>
	/// 暂停音频
	/// </summary>
	/// <param name="rendSpeed">淡出速度</param>
	public void PauseAudio(float rendSpeed) {
		StartCoroutine(FadeOutCoroutine(rendSpeed));
	}

	/// <summary>
	/// 切换BGM
	/// </summary>
	/// <param name="goalAudio">切换目标BGM</param>
	/// <param name="outSpeed">当前BGM淡出速度</param>
	/// <param name="inSpeed">目标BGM淡入速度</param>
	public void ChangeAudio(AudioClip goalAudio, float outSpeed, float inSpeed) {
		if (!source.isPlaying) {
			PlayAudio(goalAudio, inSpeed);
			return;
		}
		StartCoroutine(ChangeAudioCoroutine(goalAudio,outSpeed,inSpeed));
	}

	/// <summary>
	/// 音频淡入
	/// </summary>
	/// <param name="rendSpeed"></param>
	/// <param name="clipSource"></param>
	/// <returns></returns>
	IEnumerator FadeInCoroutine(AudioClip clipSource, float rendSpeed = 0.125f,float maxvolume=1) {
		float volume = 0;
		source.volume = 0;
		source.clip = clipSource;
		source.Play();
		while (volume<maxvolume) {
			volume += rendSpeed * Time.deltaTime;
			source.volume = volume;
			yield return new WaitForSeconds(0);
		}
		GM.Log("Audio Fade In:" + clipSource.name);
	}

	/// <summary>
	/// 音频淡出
	/// </summary>
	/// <param name="rendSpeed"></param>
	/// <param name="audioSource"></param>
	/// <returns></returns>
	IEnumerator FadeOutCoroutine(float rendSpeed = 0.125f) {
		GM.Log("Audio Fade Out 0: " + source.clip.name);
		float volume = 1;
		while (volume > 0) {
			volume -= rendSpeed * Time.deltaTime;
			source.volume = volume;
			yield return new WaitForSeconds(0);
		}
		source.Pause();
		GM.Log("Audio Fade Out: " + source.clip.name );
	}

	/// <summary>
	/// 切换BGM协程
	/// </summary>
	/// <param name="e"></param>
	/// <returns></returns>
	IEnumerator ChangeAudioCoroutine(AudioClip goalAudio, float outSpeed,float inSpeed) {
		yield return StartCoroutine(FadeOutCoroutine(outSpeed));
		yield return StartCoroutine(FadeInCoroutine(goalAudio, inSpeed));
	}

	/// <summary>
	/// 设置音频声道
	/// </summary>
	/// <param name="stereo"></param>
	public void SetAudioStereo(float stereo) {
		source.panStereo = stereo;
	}

}
