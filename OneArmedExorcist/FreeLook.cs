using UnityEngine;
using System.Collections;

public class FreeLook : MonoBehaviour {

	// How much rotation the mouse is inputing.
	private float camXAngle, camYAngle;

	public Vector3 curRot;

	// Limits for turning the player's head.
	public float camUpClamp, camDownClamp;
	// Different bodies to rotate for free look/ non-free look head movement.
	public GameObject cam, head, body, gun;
	// Whether or not the player is free looking.
	private bool freeLook;
	// Variables for freelook to smoothly snap back to the body's center.
	private Quaternion intitialRot;
	public float snapTime;

	private Quaternion tempRotation;

	private Vector3 gunPos;

	void Start ()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		curRot = transform.localEulerAngles;
	}

	void Update ()
	{
		// Set the bool to equal whether or not the player is pressing the free look button.
		freeLook = Input.GetButton("Free Look");
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}


		// If the player is pressing the freelook button.
		if (freeLook)
		{
			
			gun.transform.SetParent (null);
			SetLayerRecursively (gun, 0);
			// Get the mouse input on the X-Axis.
			if (Input.GetAxisRaw("Mouse X") != 0)
			{
				// CamXAngle = how far the mouse moved on the X-Axis.
				camXAngle = Input.GetAxisRaw("Mouse X");
				// Rotate the HEAD freely by camXAngle.

				head.transform.Rotate(Vector3.up * camXAngle);
				
			}
		}
		// If the player is not pressing the freelook button.
		else if (freeLook == false)
		{
			

			// Set Slerp initial rotation.
			intitialRot = head.transform.rotation;
			// Slerp rotation of the head back to the body's rotation.
			head.transform.rotation = Quaternion.Slerp(intitialRot, body.transform.rotation, snapTime);

			if (head.transform.rotation == intitialRot) {
				gun.transform.SetParent (transform);
				SetLayerRecursively (gun, 11);
			}

			// Get the mouse input on the X-Axis.
			if (Input.GetAxisRaw("Mouse X") != 0)
			{
				// CamXAngle = how far the mouse moved on the X-Axis.
				camXAngle = Input.GetAxisRaw("Mouse X");
				// Rotate the BODY freely by camXAngle.
				body.transform.Rotate(Vector3.up * camXAngle);
			}
		}


		// Get the mouse input on the Y-Axis.
		if (Input.GetAxisRaw("Mouse Y") != 0)
		{
			// CamYAngle = how far the mouse moved on the Y-Axis.
			camYAngle = Input.GetAxisRaw("Mouse Y");
			// Rotate the CAMERA freely by camYAngle.
			cam.transform.Rotate(Vector3.right * -camYAngle);
			//head.transform.Rotate (Vector3.right * -camYAngle);

			if (cam.transform.eulerAngles.x > 100) 
			{
				cam.transform.eulerAngles = new Vector3 (Mathf.Clamp (cam.transform.eulerAngles.x, 300, 360), cam.transform.eulerAngles.y, 0);
			}
			else
			{
				cam.transform.eulerAngles = new Vector3 (Mathf.Clamp (cam.transform.eulerAngles.x, 0, 60), cam.transform.eulerAngles.y, 0);
			}


		//	Vector3 rot = transform.eulerAngles;
		//	rot.x = Mathf.Clamp(rot.x + camYAngle, -camUpClamp, camUpClamp);
		//	cam.transform.Rotate (rot);
		}


	}

	public static void SetLayerRecursively(GameObject go, int layerNumber)
	{
		foreach (Transform trans in go.GetComponentsInChildren<Transform>(true))
		{
			trans.gameObject.layer = layerNumber;
		}
	}
}
		
