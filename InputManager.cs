using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerInput
{
	public Vector3 MoveInput;
	public Vector3 RightStickInput;
    public Vector3 DpadInput;
	public bool JumpInput;
    public bool SprintInput;
	public bool DiggingInput;
	public bool JumpHeld;
	public bool Interact;
	public bool Pause;

    public bool PickupItem;
	public float FlowerMode;
    public float RadialInput;
	public Vector3 CursorInput;
}

public class InputManager : MonoBehaviour 
{
	public PlayerInput p_Input;

	bool keyboard;
	bool ps4controller = true;

	void Start ()
	{
        // Setting relevent options
        if (!PlayerPrefs.HasKey("cameraX"))
        {
            PlayerPrefs.SetInt("cameraX", 0);
        }

        if (!PlayerPrefs.HasKey("cameraY"))
        {
            PlayerPrefs.SetInt("cameraY", 0);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
		p_Input = new PlayerInput();
        Debug.developerConsoleVisible = false;
	}

	void Update ()
    {   
        // For switching control schemes on the fly, checked profiler and doesn't seem to have a noticable impact on the framerate
        CheckControlScheme();

        // Debug methods for save data testing (only PC)
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SaveManager.instance.RelaySaveData();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            SaveManager.instance.DeleteSaveFile();
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            SaveManager.instance.ClearPlayerPrefs();
        }

        if (Input.GetKeyDown(KeyCode.F10))
        {
            Debug.developerConsoleVisible = false;
        }

//        if (UIManager.instance.activeMenu == null)
//        {
//            Cursor.lockState = CursorLockMode.Locked;
//        }

		if (!keyboard)
		{
			if (ps4controller)
            {
				Vector3 moveInput = new Vector3(Input.GetAxisRaw("LSX"), 0, -Input.GetAxisRaw("LSY"));
				Vector3 camInput = new Vector3(Input.GetAxisRaw("RSX"), Input.GetAxisRaw("RSY"), 0);
				Vector3 cursorInput = new Vector3(Input.GetAxisRaw("RSX"), 0, Input.GetAxisRaw("RSY"));
                Vector3 dpadInput = new Vector3 (Input.GetAxisRaw ("PS4DpadHorizontal"), Input.GetAxisRaw ("PS4DpadVertical"), 0);

				bool jumpInput = Input.GetButtonDown("Cross");
				bool diggingInput = Input.GetButtonDown("Square");
				bool jumpHeld = Input.GetButton("Cross");
				bool interact = Input.GetButtonDown("Triangle");
				bool pause = Input.GetButtonDown("Options");
                bool sprintInput = Input.GetButton("L1");

                float radialInput = Input.GetAxisRaw("PS4LT");
				float flowerMode = Input.GetAxisRaw("PS4RT");
                bool pickupItem = Input.GetButtonDown("Square");

                // Start not inverted (will need changing)
                if (PlayerPrefs.GetInt("cameraY") == 0)
                {
                    camInput.y = -camInput.y;
                }

                if (PlayerPrefs.GetInt("cameraX") == 1)
                {
                    camInput.x = -camInput.x;
                }
                    
                p_Input = new PlayerInput()
				{
					MoveInput = moveInput,
					RightStickInput = camInput,
                    DpadInput = dpadInput,
					JumpInput = jumpInput,
                    SprintInput = sprintInput,
					JumpHeld = jumpHeld,
					DiggingInput = diggingInput,
                    RadialInput = radialInput,
					FlowerMode = flowerMode,
					Interact = interact,
					CursorInput = cursorInput,
					PickupItem = pickupItem,
					Pause = pause
				};

			}
			else if (!ps4controller)
			{
				Vector3 moveInput = new Vector3(Input.GetAxisRaw("LSX"), 0, -Input.GetAxisRaw("LSY"));
				Vector3 camInput = new Vector3(Input.GetAxisRaw("xboxRSX"), Input.GetAxisRaw("xboxRSY"), 0);
				Vector3 cursorInput = new Vector3(Input.GetAxisRaw("xboxRSX"), 0, -Input.GetAxisRaw("xboxRSY"));
                Vector3 dpadInput = new Vector3 (Input.GetAxisRaw ("xboxDpadHorizontal"), Input.GetAxisRaw ("xboxDpadVertical"), 0);

				bool jumpInput = Input.GetButtonDown("A");
				bool diggingInput = Input.GetButtonDown("X");
				bool jumpHeld = Input.GetButton("A");
				bool interact = Input.GetButtonDown("Y");
				bool pause = Input.GetButtonDown("Start");
                bool sprintInput = Input.GetButton("LB");

                float radialInput = Input.GetAxisRaw("xboxLT");
				float flowerMode = Input.GetAxisRaw("xboxRT");
                bool pickupItem = Input.GetButtonDown("Y");

                if (PlayerPrefs.GetInt("cameraY") == 1)
                {
                    camInput.y = -camInput.y;
                }

                if (PlayerPrefs.GetInt("cameraX") == 1)
                {
                    camInput.x = -camInput.x;
                }

				p_Input = new PlayerInput()
				{
					MoveInput = moveInput,
					RightStickInput = camInput,
                    DpadInput = dpadInput,
					JumpInput = jumpInput,
                    SprintInput = sprintInput,
					JumpHeld = jumpHeld,
					DiggingInput = diggingInput,
                    RadialInput = radialInput,
					FlowerMode = flowerMode,
					Interact = interact,
					CursorInput = cursorInput,
					PickupItem = pickupItem,
					Pause = pause
				};
			}
		}
		else
		{
			Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
			Vector3 camInput = new Vector3(Input.GetAxisRaw("Mouse X"), -Input.GetAxisRaw("Mouse Y"), 0);
			Vector3 cursorInput = new Vector3(Input.GetAxisRaw("Mouse X"), 0, Input.GetAxisRaw("Mouse Y"));
            Vector3 dpadInput = new Vector3(Input.GetAxisRaw("ArrowKeysHorizontal"), Input.GetAxisRaw("ArrowKeysVertical"), 0);

			bool jumpInput = Input.GetKeyDown(KeyCode.Space);
			bool diggingInput = Input.GetKeyDown(KeyCode.Q);
			bool jumpHeld = Input.GetKey(KeyCode.Space);
			bool interact = Input.GetKeyDown(KeyCode.E);
			bool pause = Input.GetKeyDown(KeyCode.Escape);
            bool sprintInput = Input.GetKey(KeyCode.LeftShift);

            float radialInput = 0;
            if (Input.GetMouseButton(1))
            {
                radialInput = 1f;
            }
            else
            {
                radialInput = 0;
            }

			float flowerMode = 0;
			if (Input.GetKey(KeyCode.Tab))
			{
				flowerMode = 1f;
			}
			else
			{
				flowerMode = 0;
			}
                
            bool pickupItem = Input.GetKeyDown(KeyCode.E);
            if (PlayerPrefs.GetInt("cameraY") == 1)
            {
                camInput.y = -camInput.y;
            }

            if (PlayerPrefs.GetInt("cameraX") == 1)
            {
                camInput.x = -camInput.x;
            }

			p_Input = new PlayerInput()
			{
				MoveInput = moveInput,
				RightStickInput = camInput,
				JumpInput = jumpInput,
				JumpHeld = jumpHeld,
				DiggingInput = diggingInput,
                RadialInput = radialInput,
				FlowerMode = flowerMode,
				Interact = interact,
				CursorInput = cursorInput,
				PickupItem = pickupItem,
				Pause = pause,
                SprintInput = sprintInput,
                DpadInput = dpadInput
			};
		}
	}

	void ControllerType()
	{
		ps4controller = false;
	}

	void OnEnable()
	{
		DetectJoyStick.OnDetect += ControllerType;
	}

	void OnDisable()
	{
		DetectJoyStick.OnDetect -= ControllerType;
	}

    // Only 'Controller Pressed' exists, as many of the buttons overlap on the controllers
    void CheckControlScheme()
    {
        if (keyboard == false)
        {
            if (Input.anyKeyDown && ControllerPressed() == false)
            {
                keyboard = true;
                print("Switched to keyboard controls");
                return;
            }
        }

        if (keyboard == true)
        {
            if (ControllerPressed() == true)
            {
                keyboard = false;
                print("Switched to controller controls");
                return;
            }
        }
    }

    bool ControllerPressed()
    {
        if (Input.GetButtonDown("A") || Input.GetButtonDown("X") || Input.GetButtonDown("Y") || Input.GetButtonDown("Start") || Input.GetButtonDown("LB"))
        {
            return true;
        }

        if (Input.GetAxisRaw("xboxRT") != 0 || Input.GetAxisRaw("xboxLT") != 0 || Input.GetAxisRaw("xboxRSX") != 0 || Input.GetAxisRaw("xboxRSY") != 0)
        {
            return true;
        }

        if (Input.GetAxisRaw("xboxDpadHorizontal") != 0 || Input.GetAxisRaw("xboxDpadVertical") != 0)
        {
            return true;
        }

        return false;
    }
}
