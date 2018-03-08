using UnityEngine;
using System.Collections;

public class Pathing : MonoBehaviour {
	public float distanceStop;
	public float speed;
	private int destPoint = 0;
	public Vector3 destination;
	Vector3 targetDestination;
	float distanceFromPlayer;
	Vector3 originalPos;
	GameObject player;
	private Animator animator;
	public Rigidbody2D rb;
	public Transform[] waypoints;



	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
		originalPos = transform.position;
		player = GameObject.FindGameObjectWithTag ("Player");
		rb = GetComponent<Rigidbody2D> ();
		GoToNextPoint ();
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 direction = (player.transform.position - transform.position).normalized;
		Vector3 originalDirection = (originalPos - transform.position).normalized;
		animator.SetFloat ("SpeedX", direction.x);
		animator.SetFloat ("SpeedY", direction.y);

		distanceFromPlayer = Vector3.Distance (transform.position, player.transform.position);


		if (transform.position == originalPos)
			animator.SetBool ("walking", false);



		if (distanceFromPlayer >= distanceStop) {
			// If distance from player is larger than the stopping distance return to original position
			//GoToNextPoint();
			ReturnToPatrol();
			rb.AddForce(targetDestination.normalized * speed * Time.fixedDeltaTime);
			animator.SetBool ("walking", true);
			animator.SetFloat("SpeedX", targetDestination.x);
			animator.SetFloat("SpeedY", targetDestination.y);
			if (Vector3.Distance (transform.position, destination) <= 0.1f)
			{
				GoToNextPoint();
			}

		} else {
			// add force in the direction of the player
			animator.SetBool ("walking", true);
			rb.AddForce(direction.normalized * speed * Time.fixedDeltaTime);
		}	
	
	}


	void ReturnToPatrol() {
		rb.AddForce (Vector3.zero);
		if (waypoints.Length != 0) {
			destination = waypoints [destPoint].position;
			targetDestination = (destination - transform.position);
		}
	}

	void GoToNextPoint() {


		// Returns if no points have been set up
		if (waypoints.Length == 0)
			return;
		
		// Set the next destination point.
		destination = waypoints[destPoint].position;
		targetDestination = (destination - transform.position);
		
		// Choose the next point in the array as the destination,
		// cycling to the start if necessary.
		destPoint = (destPoint + 1) % waypoints.Length;




	}
	                     
}
