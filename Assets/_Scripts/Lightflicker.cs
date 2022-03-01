using UnityEngine;
using System.Collections;

public class Lightflicker : MonoBehaviour {

	public float minFlickerIntensity = 0.5f; //NÄÄ ARVOT MÄÄRITTELEE VALON MÄÄRÄN
	public float maxFlickerIntensity = 2.5f;

	private Light mylight;
	private float randomintensity;
	void Start()
	{
		randomintensity = (Random.Range (0.0f,6f)); //TÄÄ MÄÄRITTELEE NOPEUDEN
	}
	void Update()
	{
		float noise = Mathf.PerlinNoise(randomintensity,Time.time); //MODAA TÄNNE TOTA AIKAA JOTTA SAIS SÄPSIMÄÄN?
		mylight = GetComponent<Light>();
		mylight.intensity = Mathf.Lerp(minFlickerIntensity,maxFlickerIntensity,noise);
	}





}
