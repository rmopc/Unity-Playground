using System.Collections;
using UnityEngine;
// Tämä skripti liitetään siihen objektiin, josta halutaan avata ovi. GameObject door:iin määritellään inspectorissa se kohde, mikä avataan
public class RayCastLift : MonoBehaviour 
{

	public bool isOpen = false;
    public bool isLocked = false;
    public AudioSource audioSource; //public siksi, että voi määritellä jos esim napista painaa oven auki, joka on toisaalla
    public AudioClip doorOpen;
	public AudioClip doorClose;
    public AudioClip doorLocked;
    public GameObject lift; // voiko tän skippaa jos startissa määrittelee audiosourcen lailla oven? tuskinpa lol

	

      
	void Start () 
	{
       audioSource = GetComponent<AudioSource>(); // vertaa tätä nyt tohon publiciin
 	}

    public void UseLift()
    {
        if (lift.GetComponent<Animation>().isPlaying == false && isLocked == false)
        {
            isOpen = !isOpen;
            if (isOpen)
            {
                audioSource.PlayOneShot(doorOpen, 0.25f);
                lift.GetComponent<Animation>().Play("Up");
                isOpen = true;
            }
            else
            {
                audioSource.PlayOneShot(doorClose, 0.25f);
                lift.GetComponent<Animation>().Play("Down");
                isOpen = false;
            }
        }
        if (isLocked && !audioSource.isPlaying)
        {
        audioSource.PlayOneShot(doorLocked, 0.25f);               
        }
    }
}﻿
