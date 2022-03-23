using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindAudioManager : MonoBehaviour
{

	private AudioSource audioSource;
	public float fadeTime = 2f;

	void Start()
	{
		audioSource = GetComponent<AudioSource>();
	}



	void OnTriggerEnter(Collider collider)
	{
		if (collider.tag == "Player")
		{
			//audioSource.Stop();
			StartCoroutine(AudioFader.FadeOut(audioSource, fadeTime));
		}
	}
	void OnTriggerExit(Collider collider)
	{
		if (collider.tag == "Player")
		{
			//audioSource.Play();
			StartCoroutine(AudioFader.FadeIn(audioSource, fadeTime));
		}
	}
}