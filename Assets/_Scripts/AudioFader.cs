﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFader : MonoBehaviour {

	public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
	{
		float startVolume = audioSource.volume;
		while (audioSource.volume > 0)
		{
			audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
			yield return null;
		}
		audioSource.Stop();
	}

	public static IEnumerator FadeIn(AudioSource audioSource, float FadeTime)
	{
		audioSource.Play();
		audioSource.volume = 0f;
		while (audioSource.volume < 0.2f)
		{
			audioSource.volume += Time.deltaTime / FadeTime;
			yield return null;
		}
	}
}