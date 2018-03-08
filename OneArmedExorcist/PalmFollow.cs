using UnityEngine;
using System.Collections;

public class PalmFollow : MonoBehaviour {

	public Transform player;
	public float speed;
	bool playerSeen;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!playerSeen) {
			return;
		}
		float step = speed * Time.deltaTime;
		transform.position = Vector3.MoveTowards (transform.position, player.position, step);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other == player) {
			playerSeen = true;
		}

	}
}
