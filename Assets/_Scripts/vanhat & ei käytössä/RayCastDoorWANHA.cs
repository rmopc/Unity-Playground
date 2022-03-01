using System.Collections;
using UnityEngine;

public class RayCastDoorWANHA : MonoBehaviour {
    

    bool guiShow = false;
	bool isOpen = false;

    [Header("LUE SCRIPTIN NOTET!!")]
    public int rayLength = 2;

    //Tätä scriptiä on nyt muokattu niin, että PITÄISI toimia simppelimmin kuin kahden scritpin tekniikalla
    //Muista että tämä riittää liitettynä cameraan. Vanhalla tyylillä jokaisessa ovessa oli oma vasta-scritpi
    //ja kameran scripti könköllä tavalla kutsui oven scriptiä avaamaan.

    //SELVITÄ: MISTÄ VITUSTA JOHTUU, että animaatio menee vituiks eikä meinaa pysyä oikeassa kulmassa jne?

    //TODO: laita äänet toimimaan ja harkitse tuleeko ne tähän, tästä kutsuttuna vaiko oveen itseensä suoraan?

    void Update()
	{
		RaycastHit hit;
		Vector3 fwd = transform.TransformDirection(Vector3.forward);

		if(Physics.Raycast(transform.position, fwd, out hit, rayLength))
		{
			if (hit.collider.CompareTag ("door"))
			{
				guiShow = true;                
                {
					if(Input.GetKeyDown("f") && isOpen==false)
					{
						hit.collider.transform.GetComponent<Animator>().SetBool("isopen", true);
                        OpenDoor();
                        isOpen = true;
                        guiShow = false;
					}

					else if(Input.GetKeyDown("f") && isOpen==true)
					{
						hit.collider.transform.GetComponent<Animator>().SetBool("isopen", false);
                        OpenDoor();
                        isOpen = false;
                        guiShow = false;
					}
				}
			}
		}
		else
		{
			guiShow = false;
		}
	}
    public void OpenDoor()
    {
        isOpen = !isOpen;
        if (isOpen)
        {
            //audioSource.PlayOneShot(doorOpen, 0.25f);
        }
        else
        {
            //audioSource.PlayOneShot(doorClose, 0.25f);
        }
    }
    void OnGUI()
	{
		if(guiShow == true && isOpen == false)
		{
			GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 100, 25), "Open Door");
            Debug.Log("Available to open door");
        }
		else if(guiShow == true && isOpen == true)
		{
			GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 100, 25), "Close Door");
            Debug.Log("Available to close door");
        }
	}
}﻿
