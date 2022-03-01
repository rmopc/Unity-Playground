using UnityEngine;
using System.Collections;

public class breakablelight : MonoBehaviour {

	public float hitPoints = 50f;
	public Transform brokenobject;
   

	
	// Update is called once per frame
	void Update () 
	{
		if (hitPoints <= 0) {
			Instantiate (brokenobject, transform.position, transform.rotation);
			Destroy (gameObject);
		}
	}
	public void ApplyDamage (float damage) 
	{
		hitPoints = hitPoints - damage;
	}
}
