using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClickController : MonoBehaviour {

	public static string correctCode = "0016";
	public static string playerCode = "";
	public static int totalDigits = 0;
	public bool unlocked = false;
	private bool clicked;

	public GameObject objectToUnlock;
	public AudioSource audioSource;
	public AudioClip clickSound;
	public AudioClip correctCodeSound;
	public AudioClip wrongCodeSound;
	public Material lightEmission;

	void Start () 
	{
        lightEmission.DisableKeyword("_EMISSION");
		clicked = false;
    }
	
	
	void Update () 
	{        
        if (clicked == true)
        {
			lightEmission.DisableKeyword("_EMISSION");
			clicked = false;
		}		

		if (totalDigits == 4)
        {
			if (playerCode == correctCode)
            {
				Debug.Log("Code is Correct!");
				audioSource.PlayOneShot(correctCodeSound, 0.25f);
				objectToUnlock.GetComponent<RayCastDoor>().isLocked = false;
				playerCode = "";
				totalDigits = 0;
				unlocked = true;
				
			}

            else
            {
				Debug.Log("Wrong code entered");
				audioSource.PlayOneShot(wrongCodeSound, 0.25f);
				playerCode = "";
				totalDigits = 0;				
			}
			clicked = true;
		}		
	}

	void OnMouseDown()
    {
		audioSource.PlayOneShot(clickSound, 0.25f);
		playerCode += gameObject.name;
		totalDigits += 1;		
		StartCoroutine(ClickTime());
	}

	IEnumerator ClickTime()
    {
		lightEmission.EnableKeyword("_EMISSION");
		yield return new WaitForSeconds(1f);
		clicked = true;
	}
}
