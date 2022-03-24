using System.Collections;
using UnityEngine;

public class RaycastSend : MonoBehaviour {

    //Tämä liitetään tyhjään objektiin hierarkiassa.
    //Tämä ampuu säteen, joka triggerin perusteella suorittaa toiminnon

   	public int rayLength = 2;
    public GameObject keyController;

    bool guiShow = false;

    RaycastHit hit;

    private KeyController kc;

    void Start()
    {
        kc = keyController.GetComponent<KeyController>();
    }

    void Update() 
	{                
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, rayLength))       
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
                Debug.Log("hitting button");
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

            if (hit.collider.tag == "interaction")
            {
                Debug.Log("hitting interactable object");
                guiShow = true;
                {
                    if (Input.GetKeyDown("f"))
                    {
                        hit.collider.transform.GetComponent<RayCastInteract>().Interact();
                    }
                }
            }

            if (hit.collider.tag == "generator")
            {
                Debug.Log("hitting interactable object");
                guiShow = true;
                {
                    if (Input.GetKeyDown("f"))
                    {
                        hit.collider.transform.GetComponent<GeneratorManager>().Interact();
                    }
                }
            }

            if (hit.collider.tag == "key")
            {
                Debug.Log("hitting interactable object");
                guiShow = true;
                {
                    if (Input.GetKeyDown("f"))
                    {
                        //Destroy(hit.collider.gameObject);
                        hit.collider.gameObject.SetActive(false);
                        kc.hasCarKey = true;                        
                    }
                }
            }

            if (hit.collider.tag == "cardoor")
            {
                guiShow = true;
                //{
                //    if (Input.GetKeyDown("f"))
                //    {
                //        hit.collider.transform.parent.GetComponent<RayCastDoor>().OpenDoor();
                //    }
                //}
            }

            if (Time.timeScale == 0.0f)
            {
                guiShow = false;
            }
        }
        else
        {
            guiShow = false; //ei tällä hetkellä tee mitään?
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
                    //GUI.skin.box.normal.textColor = Color.red; ÄLÄ KÄYTÄ! muuttaa pysyvästi muidenkin värit.
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

        if (hit.collider.tag == "interaction")
        {
            if (guiShow == true && hit.collider.transform.GetComponent<RayCastInteract>().usedOnce == false) //Huomioi boolean-tarkistus
            {
                GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 100, 25), "Use");                
            }
        }

        if (hit.collider.tag == "generator")
        {
            if (guiShow == true && hit.collider.GetComponent<GeneratorManager>().isInteractable == true)
            {
                GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 100, 25), "Turn on");
            }
        }

        if (hit.collider.tag == "key")
        {
            if (guiShow == true /*&& hit.collider.GetComponent<GeneratorManager>().isInteractable == true*/)
            {
                GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 100, 25), "Pick up");
            }
        }

        if (hit.collider.tag == "cardoor")
        {
            if (guiShow == true)
            {
                GUI.Box(new Rect(Screen.width / 2, Screen.height / 2, 100, 25), "Pick up");
            }
        }
    }
}

