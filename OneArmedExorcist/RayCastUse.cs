using UnityEngine;
using System.Collections;

public class RayCastUse : MonoBehaviour {

	Camera cam;


	// Use this for initialization
	void Start () {
		cam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetKeyDown(KeyCode.E)) {
			Ray ray = new Ray (cam.transform.position, cam.transform.forward);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, 500)) {
				if (hit.transform.gameObject.tag == "Edible") {
					hit.transform.gameObject.GetComponent<Edible> ().EatMe ();
				}
			}
		}

	}
}
