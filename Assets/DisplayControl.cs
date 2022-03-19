using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayControl : MonoBehaviour {

	void Start () 
	{ 
			
	}
	
	void Update () 
	{
		GetComponent<TextMesh>().text = ButtonClickController.playerCode;
	}
}
