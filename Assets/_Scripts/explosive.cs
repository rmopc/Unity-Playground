using UnityEngine;
using System.Collections;

public class explosive : MonoBehaviour {

	public float hitPoints = 100f;
	public Transform spawnobject;
	public GameObject explosion;
	public float radius = 3.0f;
	public float power = 100.0f;

	void Update () 
	{
		if (hitPoints <= 0)
		{
			Instantiate(spawnobject, transform.position, transform.rotation);
			Instantiate(explosion, transform.position, Quaternion.identity);
			Vector3 explosionPos = transform.position;
			Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
			foreach (Collider hit in colliders) 
			{
				if (hit.GetComponent<Rigidbody>() != null)
				{
					Rigidbody rb = hit.GetComponent<Rigidbody>();
					rb.AddExplosionForce(power, explosionPos, radius, 3.0f);

				}
			}
			Destroy (gameObject);
		}	
	}

	public void ApplyDamage (float damage) 
	{
		hitPoints = hitPoints - damage;
	}
}
