using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorTriggerRight : MonoBehaviour {

    private bool inTrigger;
    private GameObject player;
	private Animator animator;

    public carManager2 carMan;
	public GameObject doorHolderRight;
	public AudioClip doorOpenSound;

	void Start(){
		animator = doorHolderRight.GetComponent<Animator> ();
	}

    void Update()
    {
        if(inTrigger == true)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
				animator.SetBool("isopen", true);
				AudioSource.PlayClipAtPoint (doorOpenSound, transform.position);
				StartCoroutine(EnterCar());
            }
        }
    }

	IEnumerator EnterCar() {
		yield return new WaitForSeconds (0.9f);
		carMan.vehicleControl(player);
		inTrigger = false;
		animator.SetBool("isopen", false);
	}

	void OnTriggerEnter (Collider col) {
        inTrigger = true;
        player = col.gameObject;
	}
    void OnTriggerExit()
    {
        inTrigger = false;
        player = null;
    }
}