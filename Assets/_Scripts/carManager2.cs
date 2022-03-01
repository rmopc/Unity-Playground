using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carManager2 : MonoBehaviour 
{
    public Camera carCam;
    public car userCtrl;
    public AudioClip starttimoottori;
	public AudioClip sammutus;
	public GameObject aseet; //tää oltava jotta aseet ei näy auton rinnalla, jos on aseita inventoryssä, oltava valittuna siitä FPS controllerista, joka on aktiivinen
	public GameObject kädet; //tää laitettu tähän siksi, että pidetään startissa poiskytkettynä ettei näy haamukädet auton sisällä ennenku auton käynnistää
	public GameObject lights; //tämän alle laitettu kaikki auton valot, eli ajovalot ja mittariston valot

    //todo: jos taskulamppu on päällä, ei se de-aktivoidu autoon astuessa. Selvitä miten koko pelaajan voi kytkeä pois niin, että autosta pääsee myös pelaajaan takaisin, aseiden tilalle ei meinaan koko prefabia voi laittaa.

    private bool inVeh;
    private GameObject player;

	void Start () 
	{
		kädet.SetActive(false);
		lights.SetActive(false);
        userCtrl.enabled = false;
        userCtrl.controlled = false; //tämä lisätty koska äänet on päällä jos tämä on true, modaa alta kaikki jos otat tän pois!
        carCam.enabled = false;
        inVeh = false;
	}
	
	void Update () 
	{
        if (Input.GetKeyDown(KeyCode.F))
        {
            if(inVeh == true)
            {
                vehicleControl(null);
            }
        }
	}

    public void vehicleControl(GameObject playerObj)
    {
        if(inVeh == false)
        {
            player = playerObj;
			kädet.SetActive(true);
			lights.SetActive(true);
            carCam.enabled = true;
            userCtrl.enabled = true;
			userCtrl.controlled = true;
			AudioSource.PlayClipAtPoint (starttimoottori, transform.position);
            player.SetActive(false);
			aseet.SetActive(false);
            player.transform.parent = this.transform;

            StartCoroutine(Time(true));
        }
        else
        {
            player.SetActive(true);
			kädet.SetActive(false);
			lights.SetActive(false);
			aseet.SetActive(true);
            carCam.enabled = false;
            userCtrl.enabled = false;
			userCtrl.controlled = false;
			AudioSource.PlayClipAtPoint (sammutus, transform.position);
            player.transform.parent = null;
            player = null;

            StartCoroutine(Time(false));
        }
    }

    private IEnumerator Time(bool inVehicle)
    {
        yield return new WaitForSeconds(1);
        inVeh = inVehicle;
    }
}