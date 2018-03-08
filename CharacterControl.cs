using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(InputManager))]
[RequireComponent (typeof(CharacterController))]
[RequireComponent (typeof(DetectJoyStick))]

public class CharacterControl : MonoBehaviour
{
    float timer = 5;
    Vector3 hitNormal;

    public float slopeLimit = 80.0f;
    public float plumSlopeLimit = 30.0f;

	private InputManager input;
    private InterpolatedTransform interpTrans;

	// current velocity
	public Vector3 moveDirection;
	// current direction our character's art is facing
	public Vector3 lookDirection { get; private set; }

	public float WalkSpeed;
	public float WalkAcceleration;
    public float sprintSpeed;
    public float sprintAcceleration;
	public float JumpAcceleration;
	public float jumpSpeedLimit;
	public float JumpHeight;
    public float JumpInitialSpeedReduction = 1.3f;
	public float Gravity;
	public float FallAcceleration = 0.2f;
    public float GlidingSpeed = 1.1f;
	public float GlidingAcceleration = 30.0f;
    public float GlidingFallingAcceleration = 0.1f;
    public float GlidingFallingSpeed = 50f;
	public float GlidingTime = 1.0f;
    public float GlideGoFast = 1.5f;
	public float fallingClampSpeed = 0.4f;
    public float slopeFriction = 0.9f;
    public float knockbackDistance = 2f;

	//Plum speed modifiers
	public float PlumWalkSpeed;
	public float PlumWalkAcceleration;

	public float diggingSpeed = 10.0f;

    public float SpeedClamp = 1f;

	float groundedGrav = -0.3f;

	public float rayDist = 1.0f;
	Vector3 rayDir = new Vector3 (0, -1, 0);

    PlayerAnimations animScript;
	CharacterController cController;
	Animator anim;
	Camera cam;

    public bool isGrounded;
	private bool canDig;
	public bool currentlyDigging;
	public bool bottomSide = false;

	private bool currentlyJumping = false;
	bool hasStartedJump;

	GameObject pivotPoint;
	public Transform childObj;

	float yLimit;
    Transform currentBlock;
    public bool newDiggingMethod;
    List<Transform> alteredblocks = new List<Transform>();
	float jumpForce;
    Vector3 jumpDirection;

	bool bounced;

    Quaternion originalRotation;

    private bool _Occupied; 

    public bool occupied
    {
        get { return _Occupied; }
        set
        {
            if (_Occupied != value)
            {
                _Occupied = value;
                UpdateOccupied();
            }
        }
    }
  
	public GameObject normalModel;
	public GameObject plumrollingModel;
    public GameObject leafGlideObj;

	public GameObject plumPrefab;

	public ParticleSystem pSystem;
	public ParticleSystem pSystemLanded;

	Vector3 objSlopeRot;
	float returnDegrees;

	bool hasNotJumped;
	bool firstEnterGlide;
	bool disableJumpOnSlope;
	bool canGlide;
	bool alreadyInGlideCoroutine;
    bool damageTaken;
    bool currentlyTakingDamage = false;

    private bool _CurrentlyOnPlum;
    public bool currentlyOnPlum
    {
        get { return _CurrentlyOnPlum; }
        set
        {
            if (_CurrentlyOnPlum != value)
            {
                _CurrentlyOnPlum = value;
                UpdateCurrentlyOnPlum();
            }
        }
    }

    Vector3 knockbackTarget;
    bool knockedBack;
   
    Vector3 levelStartPos;
    Vector3 enemyVectorDirection;
    Vector3 dir;

    Holding holdingRef;
    public float changeIdleTimer;
    float idleTimer;

    public float changePositionTimer;
    float positionTimer;
    public Vector3 savedPosition;

	enum PlayerState
	{
		_Idle,
		_Walking,
		_Jumping,
        _Sprinting,
		_Falling,
		_Digging,
		_Sliding,
		_Gliding,
        _Damaged,
        _Dying,
		_Plumbing
	}

	PlayerState currentState;

	float buttonHeldTimer = 0;
	float buttonHeldJumpTime = 0.2f;

    float startTime;
    float journeyLength;

    Vector3 startPos;
    Vector3 startMarker;
    Vector3 endMarker;

    public float moveOffset;
    public float journeyTime = 5f;

    public bool bobbing = false;
    bool isCurrent = false;
    Vector3 currentDirection;

    public RaycastHit hitGrounded;

    [SerializeField]
    Transform plumTopPoint;


	void Start ()
	{
        holdingRef = GetComponent<Holding>();
        idleTimer = changeIdleTimer;
		cam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
		pivotPoint = PivotPoint.pPoint;
        UpdateCameraForward();
        transform.forward = dir;
		input = gameObject.GetComponent<InputManager> ();
		cController = gameObject.GetComponent<CharacterController> ();
		currentState = PlayerState._Idle;
		lookDirection = transform.forward;
        normalModel.SetActive(true);
		anim = childObj.GetComponent<Animator> ();
        levelStartPos = transform.position;
        interpTrans = GetComponent<InterpolatedTransform>();
        animScript = GameObject.FindObjectOfType<PlayerAnimations>();



        occupied = false;
        GetComponent<Planting>().inPlantingMode = false;
	}

    void UpdateCameraForward()
    {
        //Get Forward face
        dir = cam.transform.forward;
        //Convert to local Space
        //dir = transform.InverseTransformDirection(dir);
        //Reset/Ignore y axis
        dir.y = 0;

    }

    void UpdateOccupied()
    {
        GameManager.instance.playerOccupied = occupied;
    }

     void UpdateCurrentlyOnPlum()
    {
        GameManager.instance.playerOnPlum = currentlyOnPlum;
    }

    public void ChangeToPlum()
    {

        moveDirection = Vector3.zero;
        RaycastHit hit;
       
        currentState = PlayerState._Plumbing;
       
        if (Physics.Raycast(transform.position, -transform.up, out hit))
        {
            if (!bottomSide)
            {
                plumrollingModel.transform.position = new Vector3(hit.point.x, hit.point.y - 1, hit.point.z);
                transform.position = new Vector3(hit.point.x, hit.point.y - 1, hit.point.z);
                interpTrans.ForgetPreviousTransforms();
            }
            else
            {
                plumrollingModel.transform.position = new Vector3(hit.point.x, hit.point.y + 1, hit.point.z);
                transform.position = new Vector3(hit.point.x, hit.point.y + 1, hit.point.z);
                interpTrans.ForgetPreviousTransforms();
            }
        
        }
    }

	void ChangeToPlayer()
	{

		normalModel.SetActive (true);
		plumrollingModel.SetActive (false);
		currentState = PlayerState._Idle;
	}

    void ChangeToPlayer2()
    {

        normalModel.SetActive (true);
        plumrollingModel.SetActive (false);
    }
        

    void CycleIdleAnim()
    {
        anim.SetInteger("idleAlt", Random.Range(1, 4));
        animScript.SetTorsoWeight(1);
        // Give extra delay for the playing animation
        idleTimer = changeIdleTimer + 5;
    }

    // Called from PlayerSounds as its only script attached to the anim object, and returns the idle anim back to the original
    public void ReturnToIdle()
    {
        anim.SetInteger("idleAlt", 0);
        idleTimer = changeIdleTimer;
    }

    void SavePosition()
    {
        Vector3 dir = Vector3.zero;
        if (bottomSide == true)
        {
            dir = Vector3.up;
        }
        else
        {
            dir = Vector3.down;
        }

        Ray ray = new Ray(transform.position, dir);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, transform.localScale.y /  + 0.5f))
        {
            if (hit.transform.tag == "NotRespawnable")
            {
                print("Terrain is marked as non respawnable");
                return;
            }
            else
            {
                savedPosition = transform.position;
                positionTimer = changePositionTimer;
            }
        }
    }

	void Update ()
	{
      

        UpdateCameraForward();

        if (Input.GetKeyDown(KeyCode.F1))
        {
            JumpingPower();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            anim.Play("BeginSit");
            anim.SetBool("isSitting", true);
            occupied = true;
        }

        if (input.p_Input.MoveInput.magnitude != 0 || input.p_Input.JumpInput)
        {
            anim.SetBool("isSitting", false);
        }

        // Check to save a respawn position 
        if (isGrounded == true && !bobbing)
        {
            if (positionTimer > 0)
            {
                positionTimer -= Time.deltaTime;
            }
            else
            {
                SavePosition();
            }
        }

        if (knockedBack == true)
        {
            float step = knockbackDistance * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, knockbackTarget, step); 
        }

		if (occupied == false)
		{
			lookDirection = dir;

			if (bottomSide == true)
            {
				childObj.transform.position = new Vector3 (transform.position.x, transform.position.y + 1, transform.position.z);
                plumrollingModel.transform.position = new Vector3(transform.position.x, transform.position.y -0.75f, transform.position.z);
			} 
            else 
            {
				childObj.transform.position = new Vector3 (transform.position.x, transform.position.y - 1, transform.position.z);
                plumrollingModel.transform.position = new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z);
			}
                

			Debug.DrawRay(transform.position, rayDir * rayDist, Color.green);

			if (!bottomSide)
			{
                

                if (Physics.SphereCast(transform.position, cController.radius / 1.4f, rayDir, out hitGrounded, rayDist) && bounced == false)
				{
                    if (hitGrounded.transform.tag == "Player" || hitGrounded.transform.tag == "Plum") {
						return;
					}
					isGrounded = true;


					moveDirection.y = groundedGrav;
				}
				else
				{
					isGrounded = false;
				}
			}
			else
			{
                if (Physics.SphereCast(transform.position, cController.radius / 1.4f, -rayDir, out hitGrounded, rayDist) && bounced == false)
				{
                    if (hitGrounded.transform.tag == "Player" || hitGrounded.transform.tag == "Plum") {
                        return;
                    }

					isGrounded = true;
					moveDirection.y = groundedGrav;
				}
				else
				{
					isGrounded = false;
				}
			}
               
			if (!bottomSide) {
				returnDegrees = Vector3.Angle (Vector3.up, hitNormal);
			} else {
				returnDegrees = Vector3.Angle (Vector3.down, hitNormal);
			}

			switch (currentState)
			{
				case PlayerState._Idle:
					Idle();
					break;
				case PlayerState._Walking:
					Walking();
					break;
                case PlayerState._Sprinting:
                    Sprinting();
                    break;
				case PlayerState._Jumping:
					if (!currentlyJumping)
					{
						Jumping();
					}
					break;
				case PlayerState._Falling:
					Falling();
					break;
				case PlayerState._Digging:
					Digging();
					break;
				case PlayerState._Sliding:
					Sliding();
					break;
				case PlayerState._Gliding:
					Gliding();
					break;
				case PlayerState._Plumbing:
                    
					Plumbing();
					break;
                case PlayerState._Damaged:
                    Damaged();
                    break;
                case PlayerState._Dying:
                    Dying();
                    break;
				default:
					Idle();
					break;
			}

			CheckForExtendedJump();

			Vector3 clampMovement = transform.position;

			if (currentState == PlayerState._Walking && isGrounded || currentState == PlayerState._Sprinting && isGrounded)
			{
				var emission = pSystem.emission;
				emission.enabled = true;
			}
			else
			{
				var emission = pSystem.emission;
				emission.enabled = false;

			}
//			if (Input.GetButtonDown("Circle"))
//			{
//				GameObject plumGO = Instantiate (plumPrefab, this.transform.position, Quaternion.identity);
//				Rigidbody plumRB = plumGO.GetComponent<Rigidbody> ();
//				plumRB.AddForce(childObj.transform.forward * 100, ForceMode.Impulse);
//			}
		}
	}

    void FixedUpdate()
    {
        //moveDirection.y = Mathf.Clamp (moveDirection.y, -fallingClampSpeed, fallingClampSpeed);
        //moveDirection.x = Mathf.Clamp(moveDirection.x, -SpeedClamp, SpeedClamp);
        //moveDirection.z = Mathf.Clamp(moveDirection.z, -SpeedClamp, SpeedClamp);

        // Test to stop the player moving if they enter dialogue
        if (occupied == false)
        {
            cController.Move(moveDirection * 50 * Time.fixedDeltaTime);
        }
    }
        
	void CheckForExtendedJump ()
	{
		if (isGrounded == false && hasStartedJump == true && buttonHeldTimer < buttonHeldJumpTime) 
		{
			if (input.p_Input.JumpHeld)
			{
				buttonHeldTimer += Time.deltaTime;
                if (buttonHeldTimer >= buttonHeldJumpTime)
                {
                   // hasReachedTopofJump = true;
                }
				if (bottomSide == true)
				{
                    moveDirection.y -= jumpForce * Time.deltaTime;
				}
				else
				{
                    moveDirection.y += jumpForce * Time.deltaTime;
				}

				if (jumpForce <= 0) 
				{
                    //hasReachedTopofJump = true;
					jumpForce -= 1f * Time.deltaTime;
				}
			}
			else
			{
				hasStartedJump = false;
			}
        }
	}

	private Vector3 LocalMovement ()
	{
		Vector3 right = Vector3.Cross (transform.up, lookDirection);

		Vector3 local = Vector3.zero;

		if (input.p_Input.MoveInput.x != 0) {
			local += right * input.p_Input.MoveInput.x;


		}

        if (input.p_Input.MoveInput.z != 0) {
			local += lookDirection * input.p_Input.MoveInput.z;
      
		}

		return local;
	}

    private Vector3 LocalMovementSideWaysOnly ()
    {
        Vector3 right = Vector3.Cross (transform.up, lookDirection);

        Vector3 local = Vector3.zero;

        if (input.p_Input.MoveInput.x != 0) {
            local = right * input.p_Input.MoveInput.x;
        }

        return local;
    }

	void Idle ()
	{

        if (leafGlideObj != null)
        {
            leafGlideObj.SetActive(false);
        }

        if (idleTimer > 0)
        {
            idleTimer -= Time.deltaTime;
        }
        else
        {
            CycleIdleAnim();

        }

        currentlyOnPlum = false;
        //hasReachedTopofJump = false;
		hasNotJumped = false;
		alreadyInGlideCoroutine = false;
		firstEnterGlide = true;
        anim.SetBool("isSprinting", false);
		anim.SetBool ("isRunning", false);
        anim.SetBool("isWalking", false);

		moveDirection = Vector3.MoveTowards (moveDirection, Vector3.zero, 10.0f * Time.deltaTime);
		//cController.slopeLimit = 45;
		if (!bottomSide) {
			groundedGrav = -0.3f;
		} else {
			groundedGrav = 0.3f;
		}

		if (input.p_Input.JumpInput && isGrounded) {
			currentState = PlayerState._Jumping;
			return;
		}

        if (input.p_Input.DiggingInput && isGrounded && !holdingRef.holding) 
		{
			CheckToBeginDig();
		}

		if (!isGrounded) {
			hasNotJumped = true;
            originalRotation = childObj.rotation;
			currentState = PlayerState._Falling;
			return;
		}

		if (returnDegrees >= slopeLimit) {
			currentState = PlayerState._Sliding;
		}

		if (input.p_Input.MoveInput != Vector3.zero) {
			currentState = PlayerState._Walking;
			return;
		}

        if (input.p_Input.MoveInput != Vector3.zero && input.p_Input.SprintInput)
        {
            currentState = PlayerState._Sprinting;
        }

        if (damageTaken == true)
        {
            currentState = PlayerState._Damaged;
            return;
        }
	}

	void Jumping ()
	{
        originalRotation = childObj.transform.rotation;
        moveDirection = new Vector3(moveDirection.x / JumpInitialSpeedReduction,  moveDirection.y, moveDirection.z / JumpInitialSpeedReduction);
        SavePosition();
        ReturnToIdle();
		anim.SetBool ("hasJumped", true);
		//cController.slopeLimit = 0;
		pSystemLanded.Stop();
       

		if (isGrounded) 
		{
            JumpingPower();
		}

		if (!isGrounded) {
			currentState = PlayerState._Falling;
			return;
		}

		if (input.p_Input.MoveInput != Vector3.zero) {
			//moveDirection = Vector3.MoveTowards (moveDirection, LocalMovement () * WalkSpeed, WalkAcceleration * Time.deltaTime);
		}

        if (damageTaken == true)
        {
            currentState = PlayerState._Damaged;
            return;
        }
	}

	void Walking ()
	{
        ReturnToIdle();
		var emission = pSystem.emission;
        if (input.p_Input.MoveInput.magnitude > 0.3f)
        {
            anim.SetBool("isSprinting", false);
            anim.SetBool("isRunning", true);
            anim.SetBool("isWalking", false);
			emission.rateOverTime = input.p_Input.MoveInput.magnitude * 10;
        }
        else
        {
            anim.SetBool("isSprinting", false);
            anim.SetBool("isWalking", true);
            anim.SetBool ("isRunning", false);
			emission.rateOverTime = 2;
        }

        if (damageTaken == true)
        {
            currentState = PlayerState._Damaged;
            return;
        }

		if (input.p_Input.JumpInput && isGrounded) {
			currentState = PlayerState._Jumping;
			return;
		}

        if (input.p_Input.DiggingInput && isGrounded && !holdingRef.holding) 
		{
			CheckToBeginDig();
		}

		if (returnDegrees >= slopeLimit) {
			currentState = PlayerState._Sliding;
            return;
			//Debug.Log ("Slope limit reached");
		}

		if (!isGrounded) {
			hasNotJumped = true;
            originalRotation = childObj.rotation;
			currentState = PlayerState._Falling;
			return;
		}

        if (input.p_Input.MoveInput != Vector3.zero && input.p_Input.SprintInput)
        {
            currentState = PlayerState._Sprinting;
            return;
        }

		if (input.p_Input.MoveInput != Vector3.zero)
		{
			Vector3 targetRot;
			moveDirection = Vector3.MoveTowards (moveDirection, LocalMovement () * WalkSpeed, WalkAcceleration * Time.deltaTime);
			targetRot = new Vector3 (moveDirection.x, 0, moveDirection.z);
			targetRot = targetRot.normalized;

			if (!bottomSide)
			{
				childObj.transform.rotation = Quaternion.LookRotation (targetRot);
                
			}
			else
			{
				childObj.transform.rotation = Quaternion.LookRotation (targetRot, Vector3.down);
			}
		}
		else
		{
			currentState = PlayerState._Idle;
			return;
		}
	}

    void Sprinting()
    {
        ReturnToIdle();
        anim.SetBool ("isSprinting", true);

        if (damageTaken == true)
        {
            currentState = PlayerState._Damaged;
            return;
        }

        if (input.p_Input.JumpInput && isGrounded && !disableJumpOnSlope)
        {
            currentState = PlayerState._Jumping;
            return;
        }

		if (returnDegrees >= slopeLimit) {
			disableJumpOnSlope = true;
            
		} else {
			disableJumpOnSlope = false;
		}

        if (!isGrounded) {
			hasNotJumped = true;
            originalRotation = childObj.rotation;
            currentState = PlayerState._Falling;
            return;
        }

        if (input.p_Input.MoveInput != Vector3.zero && input.p_Input.SprintInput)
        {
            Vector3 targetRot;
            moveDirection = Vector3.MoveTowards (moveDirection, LocalMovement () * sprintSpeed, sprintAcceleration * Time.deltaTime);
            targetRot = new Vector3 (moveDirection.x, 0, moveDirection.z);

            targetRot = targetRot.normalized;
            if (!bottomSide)
            {
                childObj.transform.rotation = Quaternion.LookRotation (targetRot);

            }
            else
            {
                childObj.transform.rotation = Quaternion.LookRotation (targetRot, Vector3.down);
            }
        }
        else
        {
            currentState = PlayerState._Idle;
            return;
        }
    }

	void Falling ()
	{
		Vector3 fallDirection = moveDirection;
   

		if (!isGrounded)
        {
			//Debug.Log ("Not grounded, applying gravity");
			//cController.slopeLimit = 0;
			if (!bottomSide) 
            {
				moveDirection.y -= Gravity / fallingClampSpeed * Time.deltaTime;
				fallDirection.y = moveDirection.y;
			}
            else 
            {
				moveDirection.y -= -Gravity / fallingClampSpeed * Time.deltaTime;
				fallDirection.y = moveDirection.y;
			}
		}

		if (input.p_Input.MoveInput != Vector3.zero) 
        {
			moveDirection = Vector3.MoveTowards (moveDirection, LocalMovement(), FallAcceleration * Time.deltaTime);
         
		}
            
        if (isGrounded && !currentlyOnPlum) 
		{
            anim.SetBool("isGliding", false);
			anim.SetBool ("hasJumped", false);
			pSystemLanded.Play();

            Quaternion tempRot = childObj.rotation;
            tempRot.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, childObj.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
           // Debug.Log("tempRot : " + tempRot.eulerAngles);
            childObj.transform.rotation = tempRot;


            if (input.p_Input.MoveInput != Vector3.zero)
            {
                moveDirection = moveDirection / 2;
                currentState = PlayerState._Walking;
            }
            else {
                currentState = PlayerState._Idle;
            }
			
			return;
		}
        else if (isGrounded && currentlyOnPlum)
        {
            currentState = PlayerState._Plumbing;
            return;
        }


        if (input.p_Input.JumpInput && input.p_Input.JumpHeld && !holdingRef.holding && !currentlyOnPlum|| input.p_Input.JumpHeld && hasNotJumped && input.p_Input.JumpInput && !holdingRef.holding && !currentlyOnPlum) {
			
			currentState = PlayerState._Gliding;
			return;

		}

        if (damageTaken == true)
        {
            currentState = PlayerState._Damaged;
            return;
        }
	}

	void CheckToBeginDig()
	{
		RaycastHit hit;
		if (!bottomSide)
		{
			if (Physics.Raycast (transform.position, Vector3.down, out hit, 3)) 
			{
                if (TerrainManager.instance.CanDigHere(hit.transform.tag))
				{
					yLimit = hit.transform.position.y + -(hit.transform.localScale.y / 2);
                    currentBlock = hit.transform;

                    if (newDiggingMethod == false)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            StackCheck();
                        }
                    }

					currentState = PlayerState._Digging;
					currentlyDigging = true;
				}
			}
		} 
		else
		{
			if (Physics.Raycast (transform.position, Vector3.up, out hit, 3)) 
			{
                if (TerrainManager.instance.CanDigHere(hit.transform.tag))
				{
					yLimit = hit.transform.position.y + (hit.transform.localScale.y / 2);	
					currentState = PlayerState._Digging;
                    for (int i = 0; i < 10; i++)
                    {
                        StackCheck();
                    }
					currentlyDigging = true;
				}
			}
		}

        if (alteredblocks.Count > 0)
        {
            for (int i = 0; i < alteredblocks.Count; i++)
            {
                alteredblocks[i].gameObject.layer = 0;
            }

            alteredblocks.Clear();
        }

		return;
	}

    void StackCheck()
    {
        print("Current block: " + currentBlock.name);
        print("yLimit: " + yLimit);
        RaycastHit hit;
        float length = currentBlock.localScale.y / 2;
        if (bottomSide == false)
        {
			if (Physics.Raycast(new Vector3(transform.position.x, currentBlock.position.y + length + 1f, transform.position.z), Vector3.down, out hit, length + 1))
            {
                if (hit.transform == currentBlock)
                {
                    // ignore raycast
                    currentBlock.gameObject.layer = 2;
                    alteredblocks.Add(currentBlock);
                    return;
                }

                if (yLimit > hit.transform.position.y - hit.transform.localScale.y / 2)
                {
                    print("New Current block: " + hit.transform.name);
                    currentBlock = hit.transform;
                    yLimit = currentBlock.position.y - currentBlock.localScale.y / 2;
                    return;
                }
                else
                {
                    print("Found " + hit.transform.name + ", but it was smaller than the current block");
                    alteredblocks.Add(hit.transform);
                    hit.transform.gameObject.layer = 2;
                }
            }

            else if (Physics.Raycast(new Vector3(transform.position.x, currentBlock.position.y, transform.position.z), Vector3.down, out hit, length + 1))
            {
                if (hit.transform == currentBlock)
                {
                    // ignore raycast
                    currentBlock.gameObject.layer = 2;
                    alteredblocks.Add(currentBlock);
                    return;
                }

                if (yLimit > hit.transform.position.y - hit.transform.localScale.y / 2)
                {
                    print("New Current block: " + hit.transform.name);
                    currentBlock = hit.transform;
                    yLimit = currentBlock.position.y - currentBlock.localScale.y / 2;
                    return;
                }
                else
                {
                    print("Found " + hit.transform.name + ", but it was smaller than the current block");
                    alteredblocks.Add(hit.transform);
                    hit.transform.gameObject.layer = 2;
                }
            }
        }
        else
        {
			if (Physics.Raycast(new Vector3(transform.position.x, currentBlock.position.y - length - 1f, transform.position.z), Vector3.up, out hit, length + 1))
            {
                if (hit.transform == currentBlock)
                {
                    // ignore raycast
                    currentBlock.gameObject.layer = 2;
                    alteredblocks.Add(currentBlock);
                    return;
                }

                if (yLimit < hit.transform.position.y + hit.transform.localScale.y / 2)
                {
                    print("New Current block: " + hit.transform.name);
                    currentBlock = hit.transform;
                    yLimit = currentBlock.position.y + currentBlock.localScale.y / 2;
                    return;
                }
                else
                {
                    print("Found " + hit.transform.name + ", but it was smaller than the current block");
                    alteredblocks.Add(hit.transform);
                    hit.transform.gameObject.layer = 2;
                }
            }
            else if (Physics.Raycast(new Vector3(transform.position.x, currentBlock.position.y, transform.position.z), Vector3.up, out hit, length + 1))
            {
                if (hit.transform == currentBlock)
                {
                    // ignore raycast
                    currentBlock.gameObject.layer = 2;
                    alteredblocks.Add(currentBlock);
                    return;
                }

                if (yLimit < hit.transform.position.y + hit.transform.localScale.y / 2)
                {
                    print("New Current block: " + hit.transform.name);
                    currentBlock = hit.transform;
                    yLimit = currentBlock.position.y + currentBlock.localScale.y / 2;
                    return;
                }
                else
                {
                    print("Found " + hit.transform.name + ", but it was smaller than the current block");
                    alteredblocks.Add(hit.transform);
                    hit.transform.gameObject.layer = 2;
                }
            }
        }
    }

	void Digging ()
	{
        ReturnToIdle();
		groundedGrav = 0.0f;
		moveDirection = Vector3.zero;
		if (currentlyDigging == true)
		{
			anim.SetBool ("isDigging", true);
			if (anim.GetCurrentAnimatorStateInfo(0).IsName("DigStart")) 
            {	
				moveDirection = Vector3.SlerpUnclamped (moveDirection, childObj.transform.up * 2, 2 * Time.deltaTime);
			}
			if (anim.GetCurrentAnimatorStateInfo(0).IsName("SpinDig")) 
            {
                transform.Translate (0, -diggingSpeed * 100 * Time.deltaTime, 0);
                interpTrans.ForgetPreviousTransforms();
			}

            Collider hitCollider = currentBlock.GetComponent<Collider>();
            if (newDiggingMethod == true)
            {  
                if (bottomSide == true)
                {
                    if (!hitCollider.bounds.Contains(transform.position) && transform.position.y > yLimit)
                    {
                        FinishDigging();
                    }
                }
                else
                {
                    if (!hitCollider.bounds.Contains(transform.position) && transform.position.y < yLimit)
                    {
                        FinishDigging();
                    }
                }
            }
            
            else
            {
                if (bottomSide == false && transform.position.y < yLimit || bottomSide == true && transform.position.y > yLimit)
                {
                    FinishDigging();
                }
            }
		}
	}

    void FinishDigging()
    {
        transform.Rotate (180, 180, 0);
        pivotPoint.transform.Rotate (180, 180, 0);
        bottomSide = !bottomSide;
        lookDirection = transform.forward;
        currentState = PlayerState._Idle;
        cam.GetComponent<CameraController> ().FlipCameraAngles ();
        currentlyDigging = false;
        anim.SetBool ("isDigging", false);
        interpTrans.ForgetPreviousTransforms();
    }

	bool CheckForStackedBlock()
	{
		bool continueDigging = false;
		RaycastHit hit;
		if (!bottomSide) 
		{
            // Slightly extended to account for slopes, which should now work (original was 0.5f)
            if (Physics.Raycast (new Vector3 (transform.position.x, transform.position.y + 0.75f, transform.position.z), Vector3.down, out hit, 2.5f) || 
                Physics.Raycast (new Vector3 (transform.position.x, transform.position.y + 8, transform.position.z), Vector3.down, out hit, 3))
			{
                print(hit.transform.name);
                if (TerrainManager.instance.CanDigHere(hit.transform.tag))
				{
					continueDigging = true;
					yLimit = hit.transform.position.y + -(hit.transform.localScale.y / 2);
				}
			}
		} 
		else
		{
			if (Physics.Raycast (new Vector3(transform.position.x, transform.position.y - 0.75f, transform.position.z), Vector3.up, out hit, 2.5f)) 
            {
                if (TerrainManager.instance.CanDigHere(hit.transform.tag)) 
				{
					continueDigging = true;
					yLimit = hit.transform.position.y + (hit.transform.localScale.y / 2);	
				}
			}
		}

		if (continueDigging == true) 
		{
			print ("can continue digging");
			currentState = PlayerState._Digging;
			currentlyDigging = true;
		}

		return continueDigging;
	}

	void Sliding()
	{
		//float returnDegrees = Vector3.Angle(hitNormal, cController.transform.forward);
		if (!bottomSide) {
            if (!currentlyOnPlum)
            {
                if (returnDegrees < slopeLimit || !isGrounded) {              
                    currentState = PlayerState._Idle;        
                    return;
                } else {
                    moveDirection.x += (1f - hitNormal.y) * hitNormal.x * (1f - slopeFriction) * 100 * Time.deltaTime;
                    moveDirection.z += (1f - hitNormal.y) * hitNormal.z * (1f - slopeFriction) * 100 * Time.deltaTime;
                }  
            }
            else
            {
                if (returnDegrees < plumSlopeLimit || !isGrounded) {              
                    currentState = PlayerState._Plumbing;        
                    return;
                } else {
                    moveDirection.x += (1f - hitNormal.y) * hitNormal.x * (1f - slopeFriction) * 100 * Time.deltaTime;
                    moveDirection.z += (1f - hitNormal.y) * hitNormal.z * (1f - slopeFriction) * 100 * Time.deltaTime;
                }
            }	
		} else {
            if (!currentlyOnPlum)
            {
                if (returnDegrees < slopeLimit || !isGrounded)
                {               
                    currentState = PlayerState._Idle;    
                    return;
                }
                else
                {
                    moveDirection.x += (1f + hitNormal.y) * hitNormal.x * (1f - slopeFriction) * 100 * Time.deltaTime;
                    moveDirection.z += (1f + hitNormal.y) * hitNormal.z * (1f - slopeFriction) * 100 * Time.deltaTime;
                }
            }
            else
            {
                if (returnDegrees < plumSlopeLimit || !isGrounded)
                {               
                    currentState = PlayerState._Plumbing;    
                    return;
                }
                else
                {
                    moveDirection.x += (1f + hitNormal.y) * hitNormal.x * (1f - slopeFriction) * 100 * Time.deltaTime;
                    moveDirection.z += (1f + hitNormal.y) * hitNormal.z * (1f - slopeFriction) * 100 * Time.deltaTime;
                }
            }
		}

        if (input.p_Input.MoveInput != Vector3.zero)
        {
            moveDirection = Vector3.MoveTowards(moveDirection, LocalMovement() * WalkSpeed, WalkAcceleration * Time.deltaTime);
        }
            
        if (damageTaken == true)
        {
            currentState = PlayerState._Damaged;
            return;
        }

        if (input.p_Input.JumpInput && isGrounded && !currentlyOnPlum)
		{
			currentState = PlayerState._Jumping;
			return;
		}

        if (input.p_Input.JumpInput && isGrounded && currentlyOnPlum) 
        {
            GameObject plumGO = Instantiate (plumPrefab, this.transform.position, Quaternion.identity);
            if (bottomSide == true)
            {
                plumGO.GetComponent<RBGravity>().underside = true;
            }
            else
            {
                plumGO.GetComponent<RBGravity>().underside = false;
            }
            Rigidbody plumRB = plumGO.GetComponent<Rigidbody> ();
            plumRB.AddForce (cController.velocity * 100);
            currentlyOnPlum = false;
            ChangeToPlayer ();
        }
	}

	void Gliding()
	{
        anim.SetBool("isGliding", true);

        if (leafGlideObj != null)
        {
            leafGlideObj.SetActive(true);
        }

		if (!alreadyInGlideCoroutine) {
			StartCoroutine (GlideTimer());
		}

        Vector3 fallDirection = Vector3.zero;
		if (firstEnterGlide) {
			moveDirection.y = 0;
			firstEnterGlide = false;
		}

        if (damageTaken == true)
        {
            currentState = PlayerState._Damaged;
            return;
        }

		if (canGlide) {
            Vector3 targetRot;
			if (input.p_Input.JumpHeld) {
					if (!bottomSide) {
                    
                        fallDirection.y =  -Gravity / GlidingFallingSpeed;

					} else {
                        
                        fallDirection.y = Gravity / GlidingFallingSpeed;
                     
					}								


                             
 
                if (input.p_Input.MoveInput != Vector3.zero)
                {
                    Quaternion tempRot = Quaternion.identity;
                    if (!bottomSide)
                    {
                        tempRot.eulerAngles = new Vector3(45, 0, childObj.rotation.eulerAngles.z);
                    }
                    else
                    {
                        tempRot.eulerAngles = new Vector3(-45, 0, childObj.rotation.eulerAngles.z);
                    }

                    targetRot = new Vector3 (moveDirection.x, 0, moveDirection.z);
                    childObj.rotation = Quaternion.RotateTowards(childObj.rotation, Quaternion.LookRotation(targetRot) * tempRot, 150 * Time.deltaTime);
                    //moveDirection = Vector3.MoveTowards(moveDirection, LocalMovement() * GlidingSpeed + childObj.forward *  GlideGoFast  * Time.deltaTime, GlidingFallingAcceleration * Time.deltaTime) + fallDirection * Time.deltaTime;
                    moveDirection = Vector3.MoveTowards(moveDirection, LocalMovement() * GlidingSpeed, GlidingFallingAcceleration * Time.deltaTime) + fallDirection * Time.deltaTime;
                }
                else
                {
                    childObj.transform.rotation = Quaternion.RotateTowards(childObj.transform.rotation, originalRotation, 150 * Time.deltaTime);
                    moveDirection = Vector3.MoveTowards(moveDirection, LocalMovement() * GlidingSpeed, GlidingFallingAcceleration * Time.deltaTime) + fallDirection * Time.deltaTime;
                }
               
              

         

			} else {
                leafGlideObj.SetActive(false);
				currentState = PlayerState._Falling;
			}
		} else {
            leafGlideObj.SetActive(false);
			currentState = PlayerState._Falling;
		}

		if (isGrounded) 
        {
            anim.SetBool("isGliding", false);
			anim.SetBool ("hasJumped", false);
			pSystemLanded.Play ();
            if (leafGlideObj != null)
            {
                leafGlideObj.SetActive(false);
            }

			currentState = PlayerState._Idle;
			return;
		}
	}

    float Clamp(float angle)
    {
        if (angle < 180)
        {
            angle = Mathf.Clamp(angle, 0, 20);
        }

        if (angle > 180)
        {
            angle = Mathf.Clamp(angle, 340, 360);
        }

        return angle;
    }

	IEnumerator GlideTimer()
	{
		alreadyInGlideCoroutine = true;
		canGlide = true;
		yield return new WaitForSeconds (GlidingTime);
		canGlide = false;
	}


        
    void Damaged()
    {
        if (!currentlyTakingDamage)
        {
            if (GameManager.instance.currentHealth == 0)
            {
                if (currentlyOnPlum)
                {
                    ChangeToPlayer2();
                }
                currentState = PlayerState._Dying;
                return;
            }

            // Only called once to get the positions of the enemy and the player
            if (knockedBack == false)
            {
                knockbackTarget = transform.position - (enemyVectorDirection * 2);
            }

            StartCoroutine(TakingDamage());

            moveDirection = Vector3.MoveTowards(moveDirection, (moveDirection + (enemyVectorDirection * knockbackDistance)), Time.deltaTime);
        }
    }

    IEnumerator TakingDamage()
    {
        if (currentlyOnPlum)
        {
            ChangeToPlayer2();
        }
        knockedBack = true;
       currentlyTakingDamage = true;
        yield return new WaitForSeconds(1f);
        currentlyTakingDamage = false;
        damageTaken = false;
        knockedBack = false;
       
        currentState = PlayerState._Idle;
    }

    void Dying()
    {
        moveDirection = Vector3.zero;

        //reset rotation
        Quaternion tempRot = childObj.rotation;
        tempRot.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, childObj.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        childObj.transform.rotation = tempRot;

        anim.Play("Death");
        anim.SetBool("isDead", true);
        occupied = true;
    }

    // Called from PlayerSounds, to relay the information that the player needs to respawn (once the animation is finished)
    public void Respawn()
    {
        print("respawned");
        anim.SetBool("isDead", false);
        occupied = false;

        GameManager.instance.ChangeLives(-1);
        GameManager.instance.ChangeHealth(3);
        transform.position = levelStartPos;
        currentState = PlayerState._Idle;
    }

    public void Current(Vector3 _currentDirection)
    {
        isCurrent = true;
        currentDirection = _currentDirection;
    }

    public void StopCurrent()
    {
        isCurrent = false;
        currentDirection = Vector3.zero;
    }
		
	void Plumbing()
	{
        Vector3 currentV3 = moveDirection;
        currentlyOnPlum = true;

		// Play animation between swapping to plum plant
		if (normalModel.activeSelf)
		{
			normalModel.SetActive(false);
           
         
			plumrollingModel.SetActive(true);
         
		}

        if (!isGrounded) {
            originalRotation = childObj.rotation;
            currentState = PlayerState._Falling;
            return;
        }

        if (bobbing)
        {
            Vector3 tempVec = new Vector3(0, transform.position.y, 0);
            transform.position = new Vector3(transform.position.x, (Mathf.Sin(Time.time * moveOffset) / 3) + tempVec.y - 0.3f, transform.position.z);      
        }
            
        if (isCurrent && input.p_Input.MoveInput == Vector3.zero)
        {
			moveDirection = Vector3.MoveTowards (moveDirection, LocalMovement () * PlumWalkSpeed, PlumWalkAcceleration * Time.deltaTime) + currentDirection * Time.deltaTime;
        }


		if (input.p_Input.MoveInput != Vector3.zero)
        {
			Vector3 targetRot;
            if (!isCurrent)
            {
                moveDirection = Vector3.MoveTowards (moveDirection, LocalMovement () * PlumWalkSpeed, PlumWalkAcceleration * Time.deltaTime);
            }
            else if (isCurrent)
            {
				moveDirection = Vector3.MoveTowards (moveDirection, LocalMovement () * PlumWalkSpeed, PlumWalkAcceleration * Time.deltaTime) + currentDirection * Time.deltaTime;
            }
			
       

			targetRot = new Vector3 (moveDirection.x, 0, moveDirection.z);


			targetRot = targetRot.normalized;
			if (!bottomSide)
			{
				plumrollingModel.transform.rotation = Quaternion.LookRotation (targetRot);
			}
			else
			{
				plumrollingModel.transform.rotation = Quaternion.LookRotation (targetRot, Vector3.down);
			}

		}

        if (returnDegrees >= plumSlopeLimit)
        {
            currentState = PlayerState._Sliding;
            return;
            //Debug.Log ("Slope limit reached");
        }

    


		if (input.p_Input.JumpInput && isGrounded) 
		{
            GameObject plumGO = Instantiate (plumPrefab, this.transform.position, Quaternion.identity);
            if (bottomSide == true)
            {
                plumGO.GetComponent<RBGravity>().underside = true;
            }
            else
            {
                plumGO.GetComponent<RBGravity>().underside = false;
            }
            Rigidbody plumRB = plumGO.GetComponent<Rigidbody> ();
            plumRB.AddForce (cController.velocity * 100);
            Vector3 tempvec;
            tempvec = plumTopPoint.position;
            transform.position = tempvec;
            interpTrans.ForgetPreviousTransforms();
            ChangeToPlayer2 ();
            moveDirection = new Vector3(moveDirection.x / 1.2f,  moveDirection.y, moveDirection.z / 1.1f);
            JumpingPower();
	
            currentlyOnPlum = false;
            currentState = PlayerState._Jumping;
            return;
       
          
		}

        if (damageTaken == true)
        {
            currentState = PlayerState._Damaged;
            return;
        }

		if (!bottomSide) {
			moveDirection.y -= Gravity / fallingClampSpeed * Time.deltaTime;

		} else {
			moveDirection.y -= -Gravity / fallingClampSpeed * Time.deltaTime;
		}
	}

    void JumpingPower()
	{
        jumpForce = JumpHeight / 2;
        moveDirection.x += (moveDirection.x / 1.4f) * Time.deltaTime;
        moveDirection.z += (moveDirection.z / 1.4F) * Time.deltaTime;
        buttonHeldTimer = 0;
        currentlyJumping = true;
        hasStartedJump = true;
        jumpDirection = Vector3.MoveTowards (moveDirection, LocalMovement () * WalkSpeed, JumpAcceleration * Time.deltaTime);
        jumpDirection = jumpDirection / jumpSpeedLimit;
        moveDirection += transform.up * (JumpHeight / 1.1f) + jumpDirection * Time.deltaTime;
        //moveDirection = new Vector3(jumpDirection.x,  transform.up.y * (JumpHeight / 1.1f), jumpDirection.z);


        currentlyJumping = false;
	}

	public void Bounce(float _bouncePower)
	{
        moveDirection = new Vector3(0,  moveDirection.y, 0);
		bounced = true;
		currentlyJumping = true;
		//hasNotJumped = true;
        anim.SetBool("hasJumped", true);

		if (bottomSide == true) 
		{
			moveDirection.y = -_bouncePower;
		}
		else 
		{
			moveDirection.y = _bouncePower;
		}
		StartCoroutine(Unbounce ());
		currentlyJumping = false;
	}

    public void Bob()
    {
        bobbing =  true;
    }

    public void StopBob()
    {
        bobbing = false;
    }

   
  

	IEnumerator Unbounce()
	{
		yield return new WaitForSeconds (0.1f);
		bounced = false;
	}

    void OnControllerColliderHit (ControllerColliderHit hit) 
    {
        hitNormal = hit.normal;
        if (currentState == PlayerState._Jumping)
        {
            if (!bottomSide)
            {
                if ((cController.collisionFlags & CollisionFlags.Above) != 0)
                {
                    moveDirection.y = -moveDirection.y;
                    currentState = PlayerState._Falling;
                }   
            }
            else
            {
                if ((cController.collisionFlags & CollisionFlags.Below) != 0)
                {
                    moveDirection.y = -moveDirection.y;
                    currentState = PlayerState._Falling;
                }
            }
                  
        }

        if (currentState == PlayerState._Falling)
        {
            if (!bottomSide)
            {
                if ((cController.collisionFlags & CollisionFlags.Below) != 0)
                {
                    moveDirection = new Vector3 (moveDirection.x + hit.normal.x / 10, moveDirection.y, moveDirection.z + hit.normal.z / 10);
                }
            }
            else
            {
                if ((cController.collisionFlags & CollisionFlags.Above) != 0)
                {
                    moveDirection = new Vector3 (moveDirection.x + hit.normal.x / 10, moveDirection.y, moveDirection.z + hit.normal.z / 10);
                }
            }
        
        }
   }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "DamageTrigger")
        {
            enemyVectorDirection = other.transform.position - transform.position;
            print("got enemy direction");
        }
    }
        
  
    void ChangeToDamageState()
    {
        damageTaken = true;
    }

    // Called from conversation trigger to stop the player moving in dialogue
    public void OccupyPlayer()
    {
        occupied = true;
        anim.SetBool("isWalking", false);
        anim.SetBool("isRunning", false);
        anim.SetBool("isSprinting", false);

        pSystemLanded.Stop();
        pSystem.Stop();
    }

	void OnEnable()
	{
        GameManager.TakeDamage += ChangeToDamageState;
	}

	void OnDisable()
	{
        GameManager.TakeDamage -= ChangeToDamageState;
	}
}