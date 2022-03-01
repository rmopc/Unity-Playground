using UnityEngine;
using System.Collections;

public class EnterVehicle : MonoBehaviour
{
    private bool inVehicle = false;
 	car vehicleScript;
    //public GameObject guiObj;
    GameObject player;


    void Start()
    {
        vehicleScript = GetComponent<car>(); //jos käytän unityn omaa, eli tähänkö voi laittaa ton pickupin oman?
        player = GameObject.Find ("FPS Player");  //tähän GameObject.Find ja nimi???
        //guiObj.SetActive(false);
    }

    // Update is called once per frame
    void OnTriggerStay(Collider other)
    {
		if (other.gameObject.name == "FPS Player" && inVehicle == false)
        {
            //guiObj.SetActive(true);
            if (Input.GetKey(KeyCode.F))
            {
                //guiObj.SetActive(false);
                player.transform.parent = gameObject.transform;              
                vehicleScript.enabled = true;
                player.SetActive(false);
                inVehicle = true;
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
		if (other.gameObject.name == "FPS Player")
        {
            //guiObj.SetActive(false);
        }
    }
    void Update()
    {
        if (inVehicle == true && Input.GetKey(KeyCode.F))
        {
            vehicleScript.enabled = false;
            player.SetActive(true);
            player.transform.parent = null;
            inVehicle = false;
        }
    }
}