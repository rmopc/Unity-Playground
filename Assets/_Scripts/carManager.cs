using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class carManager : MonoBehaviour {//TÄMÄ EI OLE KÄYTÖSSÄ TÄMÄ SCRIPTI, CARMANAGER2 ON OIKEA :)

    public Camera carCam;
    public CarUserControl userCtrl;

    private bool inVeh;
    private GameObject player;
    //private GameObject visibleBody;

	void Start () {
        userCtrl.enabled = false;
        carCam.enabled = false;
        inVeh = false;
	}
	
	void Update () {
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
            carCam.enabled = true;
            userCtrl.enabled = true;
            player.SetActive(false);
            //visibleBody.SetActive(false);
            player.transform.parent = this.transform;
			//visibleBody.transform.parent = this.transform;

            StartCoroutine(Time(true));
        }
        else
        {
            player.SetActive(true);
			//visibleBody.SetActive(true);
            carCam.enabled = false;
            userCtrl.enabled = false;
            player.transform.parent = null;
			//visibleBody.transform.parent = null;
            player = null;
			//visibleBody = null;

            StartCoroutine(Time(false));
        }
    }

    private IEnumerator Time(bool inVehicle)
    {
        yield return new WaitForSeconds(1);
        inVeh = inVehicle;
    }
}