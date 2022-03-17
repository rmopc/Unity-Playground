using System.Collections;
using UnityEngine;
/*
 Käytä tätä ensisijassa sellaiseen objektiin, jossa Animator-komponentti.
Tämä skripti liitetään siihen objektiin, josta halutaan avata ovi. GameObject door:iin määritellään inspectorissa se kohde, mikä avataan
Remote-script, eli nappiin/vipuun menee TÄMÄ script, ja inspectorista määrätään se kohde, mikä avataan
Huom secondButton, eli jos on tarve saada useasta napista kontrolloida samaa ovea.
 
*/
public class RayCastDoorRemote : MonoBehaviour 
{

	public bool isOpen = false;
    public bool isLocked = false;
    public AudioClip buttonClick;
    public AudioClip doorOpen;
	public AudioClip doorClose;
    public AudioClip doorLocked;
    public GameObject door;
    public GameObject secondButton; //mikäli on kaksi erillistä nappia ovelle

    private AudioSource audioSourceDoor; // ovesta kuuluva ääni
    private AudioSource audioSourceButton; //oven avaamisnapista kuuluva ääni
    private Animator dooranim;


    void Start () 
	{
        audioSourceDoor = door.GetComponent<AudioSource>();
        audioSourceButton = GetComponent<AudioSource>();
        dooranim = door.GetComponent<Animator>();
    }


    public void OpenDoor()
    {
        isOpen = !isOpen;
        if (isOpen)
        {
            audioSourceDoor.PlayOneShot(doorOpen, 0.25f);
            audioSourceButton.PlayOneShot(buttonClick, 0.25f);
            dooranim.SetBool("open",true);
            isOpen = true;
            secondButton.GetComponent<RayCastDoorRemote>().isOpen = true;

            //koita saada tää toimimaan!
            //if (door.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("opened"))
            //{
            //    audioSourceDoor.Stop();
            //}
        }
        else
        {
            audioSourceDoor.PlayOneShot(doorClose, 0.25f);
            audioSourceButton.PlayOneShot(buttonClick, 0.25f);
            dooranim.SetBool("open", false);
            isOpen = false;
            secondButton.GetComponent<RayCastDoorRemote>().isOpen = false;
        }


    }
}﻿
/* 
TODO!
-GUI osalta koodia siistittävä reilusti
-Lukkoääni/toiminto lisättävä (kts! normi door-scripti!)
-Selvitettävä tarvitseeko sulkemistoimintoa ja/tai onko se inspectorista valittava toiminto (helppo määrittää booleanilla :P )

*/