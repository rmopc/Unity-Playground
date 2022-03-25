using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour 
{

	public bool hasBlueKeyCard = false;
	public bool hasGreenKeyCard = false;
	public bool hasCarKey = false;
	public bool hasOfficeKeys = false;

	public GameObject doorToOpen;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(hasOfficeKeys == true)
        {
			doorToOpen.GetComponent<RayCastDoor>().hasKey = true;
			doorToOpen.GetComponent<RayCastDoor>().isLocked = false;
		}
	}
}
