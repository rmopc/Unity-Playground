using UnityEngine;
using System.Collections;

public class DoorHandler : MonoBehaviour {

    private Animator _animator = null;
	public AudioClip door_open;

 // Use this for initialization
 void Start () {
        _animator = GetComponent<Animator>();
 }

    void OnTriggerStay(Collider collider)
    {
    	if (Input.GetKeyDown ("f")) {
       		_animator.SetBool("isopen", true);
			AudioSource.PlayClipAtPoint (door_open, Camera.main.transform.position, 0.25f);
		}
    }

    void OnTriggerExit(Collider collider)
    {
        _animator.SetBool("isopen", false);
    }
}﻿