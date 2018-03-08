using UnityEngine;
using System.Collections;

public class FrogScript : MonoBehaviour
{

	public Rigidbody2D rb;
	public Collider2D[] exitCols;
	public static int health = 3;
	[SerializeField]
	private BossState
		bossState;
	private enum BossState
	{
		idle,
		moving,
		damage
	}
	;


	public Vector2 velocity;
	public float distanceStop;
	public float speed;
	private int destPoint = 0;
	public Vector3 destination;
	Vector3 targetDestination;
	Vector3 originalPos;
	private Animator animator;
	public Transform[] waypoints;
	SpriteRenderer rend;
	


	// Use this for initialization
	void Start ()
	{
		rend = GetComponent<SpriteRenderer> ();
		animator = GetComponent<Animator> ();
		originalPos = transform.position;
		rb = GetComponent<Rigidbody2D> ();
		GoToNextPoint ();
		bossState = BossState.idle;

		rend.enabled = false;
		rend.enabled = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
		velocity = rb.velocity;

		switch (bossState) {
		case BossState.idle:
				//StopAllCoroutines();
			StartCoroutine ("IdleState");
			break;
		case BossState.moving:
				//StopAllCoroutines();
			StartCoroutine ("MovingState");
			break;
		case BossState.damage:
				//StopAllCoroutines();
				//StartCoroutine("DamageState");
			DamageState ();
			break;
		}
			
	}

	IEnumerator IdleState ()
	{


		rb.velocity = Vector2.zero;
		this.animator.SetBool ("walking", false);
		yield return new WaitForSeconds (3f);
		bossState = BossState.moving;
		StopCoroutine ("IdleState");

	}

	IEnumerator MovingState ()
	{

		targetDestination = (destination - transform.position);
		rb.AddForce (targetDestination.normalized * speed * Time.fixedDeltaTime);
		this.animator.SetBool ("walking", true);
		if (targetDestination.x < 0)
			transform.localScale = new Vector3 (1, transform.localScale.y, transform.localScale.z);
		else
			transform.localScale = new Vector3 (-1, transform.localScale.y, transform.localScale.z);
		if (Vector3.Distance (transform.position, destination) <= 0.5f) {
			GoToNextPoint ();
		}


		yield return new WaitForSeconds (15f);

		bossState = BossState.idle;
		StopCoroutine ("MovingState");
	}

	void DamageState ()
	{

		StartCoroutine (FlashColour ());

		health -= 1;




		if (health == 0) {
			StopAllCoroutines ();
			DeadState ();
		}

	


		bossState = BossState.idle;
		//yield return new WaitForSeconds(.1f);
	}

	void DeadState ()
	{

		// put in dying state here, im too dumb
		Destroy (this.gameObject);

	}

	void GoToNextPoint ()
	{
		

		// Returns if no points have been set up
		if (waypoints.Length == 0)
			return;
		
		// Set the next destination point.
		destination = waypoints [destPoint].position;

		
		// Choose the next point in the array as the destination,
		// cycling to the start if necessary.
		destPoint = (destPoint + 1) % waypoints.Length;
		
		
	}

	void OnTriggerEnter2D (Collider2D other)
	{

		if (other.gameObject.tag == "Grabbable Colliders") {
			Destroy (other.gameObject);
			bossState = BossState.damage;
		}

	}

	IEnumerator FlashColour ()
	{
		for (int i = 0; i < 5; i++) {
			this.rend.enabled = false;
			yield return new WaitForSeconds (0.1f);
			this.rend.enabled = true;
			yield return new WaitForSeconds (0.1f);
		}
		StopAllCoroutines ();


	}

}
