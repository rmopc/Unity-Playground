using System.Collections;
using UnityEngine;

public class MoveTo : MonoBehaviour {

	public Transform goal;

	void Start () {
		UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		agent.destination = goal.position;
	}
}
