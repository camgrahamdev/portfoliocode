using UnityEngine;
using System.Collections;

public class characterController : MonoBehaviour
{

	public Rigidbody2D rb;

	//private KeyCode horizontal;
	//private KeyCode vertical;

	public float speed = 100f;
	private float forwardMovement;
	private float sideMovement;
	private Animator animator;
	private Vector2 lastMove;
	private bool freezeMovement;
	private BoxCollider2D bc2d;

	void Start ()
	{
		rb = GetComponent<Rigidbody2D>();
		animator = this.GetComponent<Animator>();
		bc2d = this.GetComponent<BoxCollider2D> ();
	}
	
	void Update ()
	{
		animator.SetBool ("walking", false);
		if (Time.timeScale != 0) {
			if (!freezeMovement) {
				GetInput ();
				CharacterMove ();
			} else {
				rb.velocity = Vector3.zero;
			}
		}
		}

	void FixedUpdate ()
	{
		//CharacterMove();
	}

	void GetInput ()
	{

		forwardMovement = Input.GetAxisRaw("Vertical") * speed;
		sideMovement = Input.GetAxisRaw("Horizontal") * speed;

		animator.SetFloat ("SpeedX", sideMovement);
		animator.SetFloat ("SpeedY", forwardMovement);

		Vector3 moveHorizontal = transform.right * sideMovement;
		Vector3 moveVertical = transform.forward * forwardMovement;

	
	}

	void CharacterMove ()
	{


		//if (Input.GetKeyDown(horizontal))
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
		{
			rb.AddForce(new Vector2 (sideMovement, 0f) * Time.fixedDeltaTime * speed);
			animator.SetBool ("walking", true);
			lastMove = new Vector2 (sideMovement, 0f);
		}

		//if (Input.GetKeyDown(vertical))
		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
		{
			rb.AddForce(new Vector2 (0f, forwardMovement) * Time.fixedDeltaTime * speed);
			animator.SetBool ("walking", true);
			lastMove = new Vector2 (0f, forwardMovement);
		}

		animator.SetFloat ("LastMoveX", lastMove.x);
		animator.SetFloat ("LastMoveY", lastMove.y);
			
	}

	public void SkyscraperColl(){
		bc2d.size = new Vector2 (0.113096f, 0.05045298f);
		bc2d.offset = new Vector2 (0.007929802f, -0.08260012f);
	}
	
	public void DefaultColl(){
		bc2d.size = new Vector2 (0.08f, 0.2f);
		bc2d.offset = new Vector2 (0, 0);
	}

	void FreezeMovement ()
	{
		freezeMovement = true;
	}

	void UnFreezeMovement()
	{
		freezeMovement = false;
	}
	
}
