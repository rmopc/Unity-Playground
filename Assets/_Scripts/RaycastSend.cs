using System.Collections;
using UnityEngine;

public class RaycastSend : MonoBehaviour {

    //Tää liitetään FPS controllerin Main cameraan
    //ampuu säteen, joka triggerin perusteella suorittaa toiminnon

   	public int rayLength = 2;

    bool guiShow = false;

    RaycastHit hit;

    void Update() 
	{        
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        if (Physics.Raycast(transform.position, fwd, out hit, rayLength))
        { 
            if (hit.collider.tag =="door")
            {
                guiShow = true;
                {
                    if (Input.GetKeyDown("f"))
                    {
                        hit.collider.transform.parent.GetComponent<RayCastDoor>().OpenDoor();
                    }
                }
            }
            if (hit.collider.tag== "button")
            {
                Debug.Log("hitting");
                guiShow = true;
                {
                    if (Input.GetKeyDown("f"))
                    {
                        hit.collider.transform.GetComponent<RayCastDoorRemote>().OpenDoor();
                    }
                }
            }

            if (hit.collider.tag == "lift")
            {
                Debug.Log("hitting");
                guiShow = true;
                {
                    if (Input.GetKeyDown("f"))
                    {
                        hit.collider.transform.parent.GetComponent<RayCastLift>().UseLift();
                    }
                }
            }
        }
        else
        {
            guiShow = false;
        }
    }
    void OnGUI() //heittää tota "object not set to an instance" välillä, mistä johtuu?
    {
        if (hit.collider.tag == "door")
        {
            if (guiShow == true && hit.collider.transform.parent.GetComponent<RayCastDoor>().isOpen == false)
            {
                if (hit.collider.GetComponent<Animation>().isPlaying == false && hit.collider.transform.parent.GetComponent<RayCastDoor>().isLocked == false) // tällä estetään, ettei UI-boksi näy koko ajan ja muutu lennosta "open" ja "close" välillä
                {
                    GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 100, 25), "Open");
                    //Debug.Log("Available to open door");
                }
                else if (hit.collider.GetComponent<Animation>().isPlaying == false)
                {
                    GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 100, 25), "Locked");
                }
            }

            else if (guiShow == true && hit.collider.transform.parent.GetComponent<RayCastDoor>().isOpen == true)
            {
                if (hit.collider.GetComponent<Animation>().isPlaying == false)
                {
                    GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 100, 25), "Close");
                    //Debug.Log("Available to close door");
                }
            }
        }

        if (hit.collider.tag =="button")
        {
            if (guiShow == true && hit.collider.GetComponent<RayCastDoorRemote>().isOpen == false && hit.collider.GetComponent<RayCastDoorRemote>().door.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("closed"))
            {
                GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 100, 25), "Open");
                //Debug.Log("Available to open door");
            }

            else if (guiShow == true && hit.collider.GetComponent<RayCastDoorRemote>().isOpen == true && hit.collider.GetComponent<RayCastDoorRemote>().door.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("opened"))
            {

                GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 100, 25), "Close");
                //Debug.Log("Available to close door");
            }
        }
    }
}

