using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour 
{
	GameObject player;
	Vector3 offset;
    Vector3 camVelocity;
    Vector3 pivotVelocity;
	GameObject pivotPoint;
	CharacterControl cont;

	private InputManager input;

	[SerializeField]
	private float dampSpeed = 0.3f;

	public float camSensitivity;
	public float topAngle;
	public float bottomAngle;

    float distToCam;

    bool transformNearCamera;

    public LayerMask layerMask;

    float hitDist;

    float distBetweenPandC = 8.5f;

	void Start () 
	{
		player = CharacterManager.playerRef;
		pivotPoint = PivotPoint.pPoint;
		input = player.GetComponent<InputManager> ();
		offset = pivotPoint.transform.position - player.transform.position;
        distToCam = (transform.position - player.transform.position).magnitude;
		camVelocity = Vector3.zero;
		cont = player.GetComponent<CharacterControl> ();
	}

	void Update()
	{
        if (input.p_Input.RightStickInput != Vector3.zero && UIManager.instance.lockCamera == false) 
		{
			AdjustCam ();
		}

       // float distBetweenPandC = Vector3.Distance(pivotPoint.transform.position, transform.position);
        RaycastHit hit;
        if (Physics.SphereCast(pivotPoint.transform.position, 0.2f, transform.position - pivotPoint.transform.position, out hit, distBetweenPandC, layerMask))
        {
            hitDist = hit.distance;
            transformNearCamera = true;
        }
        else
        {
            transformNearCamera = false;
        }
    }

	// Adjust the camera when leaving the ground or when the player has input
	void AdjustCam()
	{
		Vector3 camRotation = new Vector3 (input.p_Input.RightStickInput.y, input.p_Input.RightStickInput.x, 0);
		if (cont.bottomSide == true)
		{
			camRotation = -camRotation;
		}
		pivotPoint.transform.localEulerAngles = pivotPoint.transform.localEulerAngles + (camRotation * camSensitivity * Time.fixedDeltaTime);

		float x = pivotPoint.transform.localEulerAngles.x;

		if (x < topAngle || x > bottomAngle)
		{

		}
		else
		{
			// Snap to top angle
			if (x - topAngle < 10) 
			{
				pivotPoint.transform.localEulerAngles = new Vector3 (topAngle, pivotPoint.transform.localEulerAngles.y, pivotPoint.transform.localEulerAngles.z);
			} 
			// Snap to bottom angle
			else
			{
				pivotPoint.transform.localEulerAngles = new Vector3 (bottomAngle, pivotPoint.transform.localEulerAngles.y, pivotPoint.transform.localEulerAngles.z);
			}
		}
	}

	// Flip the Camera Angles when transitioning, (called from CharacterControl)
	public void FlipCameraAngles()
	{
		float hold = bottomAngle;
        offset = -offset;
		bottomAngle = 360 - topAngle;
		topAngle = 360 - hold;

		AdjustCam ();
	}

	// Moving the camera to follow the player
	void LateUpdate ()
	{
        pivotPoint.transform.position = Vector3.SmoothDamp(pivotPoint.transform.position, player.transform.position + offset, ref pivotVelocity, dampSpeed);
        //pivotPoint.transform.position = player.transform.position + offset;
        if (transformNearCamera)
        {
            
            //pivotPoint.transform.position = Vector3.SmoothDamp (pivotPoint.transform.position, player.transform.position + (pivotPoint.transform.forward * hitDist), ref velocity, dampSpeed);
            //transform.position = Vector3.SmoothDamp (transform.position, pivotPoint.transform.position - (pivotPoint.transform.forward * hitDist), ref velocity, dampSpeed);
            transform.position = pivotPoint.transform.position - (pivotPoint.transform.forward * hitDist);
                
        }
        else
        {
            //pivotPoint.transform.position = Vector3.SmoothDamp (pivotPoint.transform.position, player.transform.position, ref velocity, dampSpeed);
            transform.position = Vector3.SmoothDamp (transform.position, pivotPoint.transform.position - (pivotPoint.transform.forward * distToCam), ref camVelocity, dampSpeed);
        }
    }
}