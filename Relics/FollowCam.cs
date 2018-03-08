using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour {

	public float dampTime = 0.15f;
	private Vector3 velocity = Vector3.zero;
	public Transform player;
	//public BoxCollider2D LevelBoundaries;
	private Vector3 minBound;
	private Vector3 maxBound;
	float camVertExtent;
	float camHorzExtent; 

	void Start ()
	{
		//minBound = LevelBoundaries.bounds.min;
		//maxBound = LevelBoundaries.bounds.max;
		camVertExtent = GetComponent<Camera> ().orthographicSize;
		camHorzExtent = GetComponent<Camera> ().aspect * camVertExtent;
	}
	

	void Update () 
	{
		if (player)
		{
			Vector3 point = GetComponent<Camera>().WorldToViewportPoint(player.position);
			Vector3 delta = player.position - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
			Vector3 destination = transform.position + delta;
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
		}

		//float leftBound   = LevelBoundaries.bounds.min.x + camHorzExtent;
		//float rightBound  = LevelBoundaries.bounds.max.x - camHorzExtent;
		//float bottomBound = LevelBoundaries.bounds.min.y + camVertExtent;
		//float topBound    = LevelBoundaries.bounds.max.y - camVertExtent;
		
//		float camX = Mathf.Clamp(player.transform.position.x, leftBound, rightBound);
//		float camY = Mathf.Clamp(player.transform.position.y, bottomBound, topBound);

		float camX = player.transform.position.x;
		float camY = player.transform.position.y;

		transform.position = new Vector3(camX, camY, transform.position.z);
	}
}
