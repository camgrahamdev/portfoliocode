using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public Menu activeMenu;
    public Menu pauseMenu;
	public Menu controlsMenu;
    public Menu loadingSceneMenu;
    public static UIManager instance;
    InputManager input;

    public bool savedOccupied;
    public bool lockCamera;

    int uiLives;

	// Needs to be a float to handle the image fill amount
	float uiChi;
	Image uiChiImg;

    [SerializeField]
    Image[] healthPetals;

    [SerializeField]
    Image centrePetal;

    [SerializeField]
    Image[] chiNumbersEnd;
    [SerializeField]
    Image[] chiNumbersMid;
    [SerializeField]
    Image[] chiNumbersStart;


    Text uiLivesText;
	Text uiChiText;

    public Text healthText;
    public Text pikaText;

    public Text timerText;


    public float loadingProgress;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

	void Start ()
    {
        input = GameObject.FindObjectOfType<InputManager>();
        //timerText.enabled = false;

        uiLivesText = this.transform.GetChild(0).GetComponent<Text>();
		uiChiText = this.transform.GetChild (1).GetComponent<Text> ();

		// Default to showing the mushroom as true, which for this demo is true
        if (SceneManager.GetActiveScene().name != "MainMenu")
            UpdateUI();
    		SelectPlant(1);
	}
        
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            GameManager.instance.IncreasePlayerMaxHealth();
            GameManager.instance.currentChi += 1;
            UpdateUI();
        }

        if (activeMenu != null)
        {   
            lockCamera = true;
            activeMenu.UpdateMenu();
        }

        // Dont allow pausing on the MainMenu
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            if (input.p_Input.Pause)
            {
                if (activeMenu == pauseMenu)
                {
                    CloseActiveMenu(false);
                }
                else if (activeMenu != loadingSceneMenu)
                { 
                    SaveOccupiedValue();
                    AssignActiveMenu(pauseMenu);
                }
            }
        }
    }

    public void LoadLevelUI()
    {
        AssignActiveMenu(loadingSceneMenu);
    }

    public void UpdateUI () 
    {
        uiLives = GameManager.instance.lives;
		uiChi = GameManager.instance.currentChi;

        uiLivesText.text = "Current Lives: " + uiLives;
		uiChiText.text = "x " + uiChi.ToString();

        healthText.text = "Current Health: " + GameManager.instance.currentHealth.ToString();
        pikaText.text = "Pikas Returned: " + GameManager.instance.pikasReturned;

        // Chi counter
        float chi_1 = Mathf.Floor(uiChi / 100);
        float chi_2 = Mathf.Floor(uiChi / 10);
        float chi_3 = Mathf.Floor(uiChi % 10);

        if (chi_2 >= 10)
        {
            chi_2 = Mathf.Floor(chi_2 % 10);
        }

        ChangeChiNumberImg(chiNumbersEnd, chi_3);
        ChangeChiNumberImg(chiNumbersMid, chi_2);
        ChangeChiNumberImg(chiNumbersStart, chi_1);

        // Health counter
        float angleInterval = 0;
        float currentAngle = 0;
        angleInterval = 360 / GameManager.instance.healthLimit;

        for (int i = 0; i < 5; i++)
        {   
            if (i == 0)
            {
                // first petal here
                healthPetals[i].enabled = true;

                healthPetals[i].transform.position = centrePetal.transform.position;
                healthPetals[i].transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                currentAngle += angleInterval;
                healthPetals[i].enabled = true;

                healthPetals[i].transform.position = centrePetal.transform.position;
                healthPetals[i].transform.rotation = Quaternion.Euler(0, 0, currentAngle);
            }

            if (i >= GameManager.instance.healthLimit)
            {
                healthPetals[i].enabled = false;
            }

            if (i >= GameManager.instance.currentHealth)
            {
                healthPetals[i].enabled = false;
            }
        }
    }

    void ChangeChiNumberImg(Image[] _numbers, float _chiAmount)
    {
        for (int i = 0; i < _numbers.Length; i++)
        {
            if (i == _chiAmount)
            {
                _numbers[i].enabled = true;
            }
            else
            {
                _numbers[i].enabled = false;
            }
        }
    }

	void SelectPlant(int _plantNum)
	{
	}

    void OnEnable()
    {
        GameManager.LivesChanged += UpdateUI;
    }

    void OnDisable()
    {
        GameManager.LivesChanged -= UpdateUI;
    }

    // Called whenever the player first enters a Menu. This currently works with the Pause Menu but should also be applied to any quick access menus
    public void SaveOccupiedValue()
    {
        savedOccupied = GameManager.instance.playerOccupied;
    }

    // Called when another menu is being opened
    public void CloseActiveMenu(bool _assigningNewMenu)
    {
		if (activeMenu != null)
		{
			activeMenu.Close ();
			activeMenu = null;

            // If the player has not open another menu and is just returning to the game, re apply the Occupied value saved when the player entered the Menu
            if (_assigningNewMenu == false)
            {
                savedOccupied = GameManager.instance.player_cControl.occupied;
            }

            lockCamera = false;
		}
    }

    // Usually called just after CloseActive(), to open another menu 
    public void AssignActiveMenu(Menu _active)
    {
        CloseActiveMenu (true);
        activeMenu = _active;
        activeMenu.Open();
    }
}