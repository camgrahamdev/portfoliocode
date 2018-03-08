//Cameron Graham
//Handles reading of stat based XML files to the player

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class Evolution : MonoBehaviour {

	public int strength, speed, stealth, endurance;
	public EvolveUI _EvolveUI;
	List<string> abilitiesOnPlayer = new List<string>();
	List<string> abilitiesEquippedOnPlayer = new List<string> ();
	List<string> strengthAbilities = new List<string> ();
	List<string> speedAbilities = new List<string> ();
	List<string> stealthAbilities = new List<string>(); 

	bool firstStatMenu = false, firstSkillMenu = false, firstSleep = false;

	public GameObject item {
		get {
			if (transform.childCount > 0) {
				return transform.GetChild (0).gameObject;
			}
			return null;
		}
	}

	[SerializeField]
	GameObject abilityPrefab;

	[SerializeField]
	int exp_RequiredToLevel = 10;

	float strength_Experience, speed_Experience, stealth_Experience; //, endurance_Experience;

	public enum abilityTypeToAdd {
		strengthAbility,
		speedAbility,
		stealthAbility,
//		enduranceAbility
	}

	[SerializeField]
	int CurrentLevel;

	[SerializeField]
	GameObject statUI;
	[SerializeField]
	GameObject skillUI;

	[SerializeField]
	Transform parentSlot;

	bool alreadyAtStatCap = false;
	bool calledAlready = false;


    public void ResetSingleton()
    {
		ResetEXP ();
        Debug.LogError("GM name: " + GameManager.instance.gameObject.name);
        Debug.LogError("UI script obj name: " + GameManager.instance.uiScript.gameObject.name);

        _EvolveUI = GameManager.instance.uiScript.gameObject.transform.GetChild(7).GetComponent<EvolveUI>();

        if(_EvolveUI == null)
        {
            Debug.LogError("No script found on evolveUI object");
        }

        statUI = _EvolveUI.gameObject.transform.GetChild(0).gameObject;
        skillUI = _EvolveUI.gameObject.transform.GetChild(1).gameObject;
        parentSlot = _EvolveUI.gameObject.transform.GetChild(1).GetChild(1);

		abilitiesOnPlayer = new List<string>();
		abilitiesEquippedOnPlayer = new List<string> ();
		strengthAbilities = new List<string> ();
		speedAbilities = new List<string> ();
		stealthAbilities = new List<string>(); 

		strengthAbilities.Add ("Roar");
		strengthAbilities.Add ("Cleave");
		speedAbilities.Add ("Dash");
		speedAbilities.Add ("MudSpray");
		stealthAbilities.Add ("Vanish");


    }

	void Awake() {
		
	}

	// Use this for initialization
	void Start () {
		
		//_EvolveUI.UpdateStats (strength, speed, stealth, endurance);
	//	UpdateStats();
	//	UpdateExpValues ();
	}

	public void InitialiseScript()
	{
		abilitiesOnPlayer = new List<string>();
		abilitiesEquippedOnPlayer = new List<string> ();
		strengthAbilities = new List<string> ();
		speedAbilities = new List<string> ();
		stealthAbilities = new List<string>();

		_EvolveUI = GameManager.instance.uiScript.gameObject.transform.GetChild(7).GetComponent<EvolveUI>();

		//From awake
		if (_EvolveUI == null) {
			Debug.LogError ("No UI attached");
		}
		else
		{
			statUI = _EvolveUI.gameObject.transform.GetChild(0).gameObject;
			skillUI = _EvolveUI.gameObject.transform.GetChild(1).gameObject;
			parentSlot = _EvolveUI.gameObject.transform.GetChild(1).GetChild(1);
		}

		//if (!System.IO.File.Exists (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/teststats.xml")) {
		//	SaveStats ();
		//}
		//CharacterStat stats = XMLReader.Deserialize<CharacterStat> (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/teststats.xml");
		strength = 0;
		speed = 0;
		stealth = 0;
		endurance = 0;


		//From start
		strengthAbilities.Add ("Roar");
		strengthAbilities.Add ("Cleave");
		speedAbilities.Add ("Dash");
		speedAbilities.Add ("MudSpray");
		stealthAbilities.Add ("Vanish");

	}

	// PURELY FOR DEBUGGING, WILL BE INCORPERATED INTO INPUT MANAGER AT A LATER TIME

	void Update()
	{
		//Moved update stats and xp to on key down so it's not called every frame
        //is this a bad idea?


		UpdateStats();
		UpdateExpValues();

		if (!GameManager.instance.player.status.IsSleeping) {
			if (skillUI.gameObject.activeSelf == false) {
				if (Input.GetKeyDown (KeyCode.I)) {
					if (!firstStatMenu) {
						GameManager.instance.uiScript.messageLog.CreateNewMessage ("Here you can see your progress towards levelling up.");
						GameManager.instance.uiScript.messageLog.CreateNewMessage ("When one is full you will be able to evolve and pick a new skill.");
						firstStatMenu = true;
					}
					UpdateStats ();
					UpdateExpValues ();

					if (_EvolveUI.gameObject.activeSelf == false) {				
						_EvolveUI.gameObject.SetActive (true);
						statUI.gameObject.SetActive (true);
						UpdateStats ();
						UpdateExpValues ();

						Cursor.visible = true;
						Cursor.lockState = CursorLockMode.None;

						//Rory adding disabling for player movement
						GameManager.instance.player.status.LockPlayerMovement = true;

					} else {
						_EvolveUI.gameObject.SetActive (false);
						statUI.gameObject.SetActive (false);

						Cursor.visible = false;
						Cursor.lockState = CursorLockMode.Locked;

						//Rory adding enabling for player movement
						GameManager.instance.player.status.LockPlayerMovement = false;

					}
				}
			}
		}
		if (Input.GetKeyDown (KeyCode.L)) {
			SaveStats ();
		}


		if (statUI.gameObject.activeSelf == false) {
			if (Input.GetKeyDown (KeyCode.K)) {
				if (!firstSkillMenu) {
					GameManager.instance.uiScript.messageLog.CreateNewMessage ("Click on evolve to gain a new skill.");
					GameManager.instance.uiScript.messageLog.CreateNewMessage ("Pick the skill you want and then drag it into one of your empty slots.");
					firstSkillMenu = true;
				}
				UpdateStats ();
				UpdateExpValues ();

				if (_EvolveUI.gameObject.activeSelf == false) {				
					_EvolveUI.gameObject.SetActive (true);
					skillUI.gameObject.SetActive (true);
					UpdateStats ();
					UpdateExpValues ();

					Cursor.visible = true;
					Cursor.lockState = CursorLockMode.None;

					GameManager.instance.player.status.LockPlayerMovement = true;
				} else {
					_EvolveUI.gameObject.SetActive (false);
					skillUI.gameObject.SetActive (false);

					Cursor.visible = false;
					Cursor.lockState = CursorLockMode.Locked;

					GameManager.instance.player.status.LockPlayerMovement = false;
					GameManager.instance.player.status.CanChangeAbilities = false;
					GameManager.instance.player.status.IsSleeping = false;
				}
			}
		}
	}


	//Passes through stats to the UI script

	void UpdateStats()
	{
		Mathf.Clamp (strength, 0f, 100f);
		Mathf.Clamp (speed, 0f, 100f);
		Mathf.Clamp (stealth, 0f, 100f);

		if (SceneManager.GetActiveScene () == SceneManager.GetSceneByName ("Apex_Greybox_v2")) {
			if (!alreadyAtStatCap) {
				if (strength >= 100 && !GameManager.instance.player.status.StrengthStatMaxed || speed >= 100 && !GameManager.instance.player.status.SpeedStatMaxed || stealth >= 100 && !GameManager.instance.player.status.StealthStatMaxed) {

					EventManager.TriggerEvent ("UnlockBed");
					alreadyAtStatCap = true;
				}
			}
		}

		if (stealthAbilities.Count == 0) {
			GameManager.instance.player.status.StealthStatMaxed = true;
		}
		if (speedAbilities.Count == 0) {
			GameManager.instance.player.status.SpeedStatMaxed = true;
		}
		if (strengthAbilities.Count == 0) {
			GameManager.instance.player.status.StrengthStatMaxed = true;
		}
	
		_EvolveUI.UpdateStats (strength, speed, stealth, endurance);
	}



	// Saves stats back to XML
	void SaveStats (){
		CharacterStat stats = new CharacterStat ();



		strength = stats.p_Strength;
		speed = stats.p_Speed;
		stealth = stats.p_Stealth;
		endurance = stats.p_Endurance;

		XMLReader.Serialize (stats, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/teststats.xml");

		print (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
	}

	// void AssignSkill (string KeyValue) {
	//TODO apply abilities to the player and store them in an XML
	//	abilities.Add(KeyValue);
	//}

	//TODO: refactor code pls thnx

	public void AddStrengthXP (float expAmount)
	{
		strength_Experience += expAmount;
        UpdateStats();
        UpdateExpValues();

    }
	public void AddSpeedXP (float expAmount)
	{
		speed_Experience += expAmount;
        UpdateStats();
        UpdateExpValues();

    }
	public void AddStealthXP (float expAmount)
	{
		stealth_Experience += expAmount;
        UpdateStats();
        UpdateExpValues();

    }
//	public void AddEnduranceXP (float expAmount)
//	{
//		endurance_Experience += expAmount;

//	}

	public void AddAbility (abilityTypeToAdd _abilityTypeToAdd){
		switch (_abilityTypeToAdd) {
		case abilityTypeToAdd.strengthAbility:
			ResetEXP ();

			if (strengthAbilities.Count != 0) {
		
				abilitiesOnPlayer.Add (strengthAbilities [0]);

				strengthAbilities.RemoveAt (0);

			}
			CurrentLevel++;
			AddAbilitiesToUI ();
			UpdateStats();
			UpdateExpValues();
			break;
		case abilityTypeToAdd.speedAbility:
			ResetEXP ();
			if (speedAbilities.Count != 0) {
				abilitiesOnPlayer.Add (speedAbilities [0]);
				speedAbilities.RemoveAt (0);
			}
			CurrentLevel++;
			AddAbilitiesToUI ();
			UpdateStats();
			UpdateExpValues();
			break;
		case abilityTypeToAdd.stealthAbility:
			ResetEXP ();
			if (stealthAbilities.Count != 0) {
				abilitiesOnPlayer.Add (stealthAbilities [0]);
				stealthAbilities.RemoveAt (0);
			}
			CurrentLevel++;
			AddAbilitiesToUI ();
			UpdateStats();
			UpdateExpValues();
			break;
//		case abilityTypeToAdd.enduranceAbility:
//			endurance_Experience = 0;
//			abilitiesOnPlayer.Add (abilities [0]);
//			abilities.RemoveAt (0);
//			CurrentLevel++;
//			break;
		

		}
		AddAbilitiesToUI ();
		UpdateStats();
		UpdateExpValues();
		alreadyAtStatCap = false;
	}

	public void ResetEXP()
	{
		strength_Experience = 0;
		speed_Experience = 0;
		stealth_Experience = 0;
	}

	void AddAbilitiesToUI() {
		for (int i = 0; i < abilitiesOnPlayer.Count; i++) {
			if (!abilitiesEquippedOnPlayer.Contains(abilitiesOnPlayer[i])) {
				abilitiesEquippedOnPlayer.Add (abilitiesOnPlayer [i]);
				GameObject tempObj = Instantiate (abilityPrefab, new Vector3(0,0,0), Quaternion.identity) as GameObject;
				tempObj.name = abilitiesOnPlayer [i];
				tempObj.GetComponent<Image> ().sprite = Resources.Load<Sprite> ("Images/Skill Images/" + tempObj.name);

				if (parentSlot.GetChild (i) != item) {
					tempObj.transform.SetParent (parentSlot.GetChild (i));
				}
			}


			

		}
			
	}

	void UpdateExpValues ()
	{
		
//		int strength_Experience_temp = Mathf.FloorToInt ((strength_Experience / exp_RequiredToLevel));
//		int speed_Experience_temp = Mathf.FloorToInt ((speed_Experience / exp_RequiredToLevel));
//		int stealth_Experience_temp = Mathf.FloorToInt ((stealth_Experience / exp_RequiredToLevel));
//		int endurance_Experience_temp = Mathf.FloorToInt ((endurance_Experience / exp_RequiredToLevel));



		strength = Mathf.FloorToInt ((strength_Experience / exp_RequiredToLevel));
		speed =  Mathf.FloorToInt ((speed_Experience / exp_RequiredToLevel));
		stealth = Mathf.FloorToInt ((stealth_Experience / exp_RequiredToLevel));
        //		endurance = Mathf.FloorToInt ((endurance_Experience / exp_RequiredToLevel));

        //Debug.Log("Strength " + strength);

	}

	public void EquipAbilities(string ability){
		Debug.Log (ability + " equipped?");
		switch (ability) {
		default:
			GameManager.instance.player.status.MudSpray_Equipped = false;
			GameManager.instance.player.status.Dash_Equipped = false;
			GameManager.instance.player.status.Roar_Equipped = false;
			break;
		case "Dash":
			GameManager.instance.player.status.Dash_Equipped = true;
			Debug.Log ("Dash equipped");
			break;
		case "Roar":
			GameManager.instance.player.status.Roar_Equipped = true;
			Debug.Log ("Roar equipped");
			break;
		case "MudSpray":
			GameManager.instance.player.status.MudSpray_Equipped = true;
			Debug.Log ("MudSpray equipped");
			break;
		case "Slowmo":
			GameManager.instance.player.status.Slowmo_Equipped = true;
			Debug.Log ("Slowmo equipped");
			break;
		case "Vanish":
			GameManager.instance.player.status.Vanish_Equipped = true;
			Debug.Log ("Vanish equipped");
			break;
		case "Cleave":
			GameManager.instance.player.status.Cleave_Equipped = true;
			Debug.Log ("Cleave equipped");
			break;
		}
			




	}

	public void ResetAbilities() {
		GameManager.instance.player.status.MudSpray_Equipped = false;
		GameManager.instance.player.status.Dash_Equipped = false;
		GameManager.instance.player.status.Roar_Equipped = false;
		GameManager.instance.player.status.Slowmo_Equipped = false;
		GameManager.instance.player.status.Vanish_Equipped = false;
		GameManager.instance.player.status.Cleave_Equipped = false;
	}

	void PlayerSleepUI()
	{

		GameManager.instance.player.status.IsSleeping = true;
		GameManager.instance.player.status.LockPlayerMovement = true;
	
		_EvolveUI.gameObject.SetActive (true);
		statUI.gameObject.SetActive (true);
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;		
	}

	void SwitchToSkillUI()
	{
		EventManager.TriggerEvent ("LockBed");
		statUI.gameObject.SetActive (false);
		skillUI.gameObject.SetActive (true);
		GameManager.instance.player.status.CanChangeAbilities = true;
		calledAlready = false;
		if (!firstSleep) {
			GameManager.instance.uiScript.messageLog.CreateNewMessage ("When you sleep the world around you can change.");
			GameManager.instance.uiScript.messageLog.CreateNewMessage ("Be careful to look around the map to see what’s happened.");
			firstSleep = true;
		}

	}
		
	void OnEnable()
	{
		EventManager.StartListening ("playerSleeping", PlayerSleepUI);
		EventManager.StartListening ("LevelUpPressed", SwitchToSkillUI);
	}

	void OnDisable()
	{
		EventManager.StopListening ("playerSleeping", PlayerSleepUI);
		EventManager.StopListening ("LevelUpPressed", SwitchToSkillUI);
	}
}
		