using UnityEngine;
using System.Collections;

public class WhipAttack : MonoBehaviour {

	private Vector3 whipPoint;
	public float maxDistance;
	private Animator anim;
	public GameObject whip;


	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
	
		whipPoint = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		whipPoint.z = 0f;

		if (Input.GetMouseButtonDown (1)) {
			RaycastHit2D hit = Physics2D.Raycast (transform.position, whipPoint - transform.position, maxDistance);
			anim.SetBool ("attacking", true);
			whip.transform.rotation = Quaternion.LookRotation(Vector3.forward, whipPoint - transform.position);
			Instantiate(whip, hit.point, whip.transform.rotation);
			//Debug.DrawRay (transform.position, whipPoint - transform.position, Color.red);
		} else {
			anim.SetBool ("attacking", false);
		} 
		}
}
