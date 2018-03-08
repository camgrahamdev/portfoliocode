using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EvolveUI : MonoBehaviour {

	public Evolution _evolution;

	public Image strength_Bar;
	public Image speed_Bar;
	public Image stealth_Bar;

	public Button strength_Button, speed_Button, stealth_Button;

	public enum uiButtonPressed {
		strengthButtonPressed,
		speedButtonPressed,
		stealthButtonPressed,
//		enduranceButtonPressed
	}

	int strength, speed, stealth, endurance;


	void Awake () {
		this.gameObject.SetActive (false);
	}

	// Use this for initialization
	void Start () {
        strength_Bar.fillAmount = 0f;
        speed_Bar.fillAmount = 0f;
        stealth_Bar.fillAmount = 0f;

		strength_Button.gameObject.SetActive (false);
		speed_Button.gameObject.SetActive (false);
		stealth_Button.gameObject.SetActive (false);
}

	void UpdateUI()
	{
		// Update level of stats


		// Update bars to show level
		if (GameManager.instance.player.status.StrengthStatMaxed) {
			strength_Bar.fillAmount = 100f;
		} else {
			strength_Bar.fillAmount = strength / 100f;
		}

		if (GameManager.instance.player.status.SpeedStatMaxed) {
			speed_Bar.fillAmount = 100f;
		} else {
			speed_Bar.fillAmount = speed / 100f;
		}

		if (GameManager.instance.player.status.StealthStatMaxed) {
			stealth_Bar.fillAmount = 100f;
		} else {
			stealth_Bar.fillAmount = stealth / 100f;
		}




		if (GameManager.instance.player.status.IsSleeping == true) {
			strength_Button.gameObject.SetActive (strength >= 100 && !GameManager.instance.player.status.StrengthStatMaxed);
			speed_Button.gameObject.SetActive (speed >= 100 && !GameManager.instance.player.status.SpeedStatMaxed);
			stealth_Button.gameObject.SetActive (stealth >= 100 && !GameManager.instance.player.status.StealthStatMaxed);
		}


	}

	// Updates stats and then updates the UI
	public void UpdateStats(int p_Strength, int p_Speed, int p_Stealth, int p_Endurance)
	{
		strength = p_Strength;
		speed = p_Speed;
		stealth = p_Stealth;
		endurance = p_Endurance;

		UpdateUI ();
	}

	public void ButtonPressedUI(string nameOfButton){
		switch (nameOfButton) {
		case "Strength Button":
			_evolution.AddAbility (Evolution.abilityTypeToAdd.strengthAbility);
			break;
		case "Speed Button":
			_evolution.AddAbility (Evolution.abilityTypeToAdd.speedAbility);
			break;
		case "Stealth Button":
			_evolution.AddAbility (Evolution.abilityTypeToAdd.stealthAbility);
			break;
//		case "Endurance Button":
//			_evolution.AddAbility (Evolution.abilityTypeToAdd.enduranceAbility);
//			break;
		}
		EventManager.TriggerEvent ("LevelUpPressed");
	}

	void AllowButtons()
	{
		GameManager.instance.player.status.IsSleeping = true;
	}

	void OnEnable()
	{
		EventManager.StartListening ("playerSleeping", AllowButtons);
	}

	void OnDisable()
	{
		EventManager.StopListening ("playerSleeping", AllowButtons);
	}

}
