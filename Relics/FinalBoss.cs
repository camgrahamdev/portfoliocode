using UnityEngine;
using System.Collections;

/* 
 * Enter area, trigger spawns first boss state
 * When boss is hit once or several times change to second state
 * While vines rotate at different speeds with speed variable
 * Vines speed up after state change
 * At final state it dies
 * Play ending scene or carry on
*/

public class FinalBoss : MonoBehaviour 
{

	public GameObject [] bossStates;
	public Collider2D flowerCol;
	public Collider2D exitCol;
	
	private bool isColliding;

	[SerializeField]
	private States currentState;
	enum States
	{
		Boss1,
		Boss2,
		Boss3,
		Boss4,
		Boss5,
		Boss6
	}

	void Start () 
	{
		currentState = States.Boss1;
	}
	

	void Update () 
	{
		isColliding = false;
		switch (currentState) 
		{
		case States.Boss1:
			FirstState();
			break;
		case States.Boss2:
			SecondState();
			break;
		case States.Boss3:
			ThirdState();
			break;
		case States.Boss4:
			FourthState();
			break;
		case States.Boss5:
			FifthState();
			break;
		case States.Boss6:
			SixthState();
			break;
		}
	}
	

	void FirstState ()
	{
		//bossStates[0].SetActive(true);
		//default spinning animation
		//Reference animation to set speed
		//add hit counter to go to next state
	}

	void SecondState ()
	{
		bossStates[0].SetActive(false);
		bossStates[1].SetActive(true);
		bossStates [1].GetComponent<Animator> ().speed = 0.4f;
		//Reference animation to set speed
		//add hit counter to go to next state
	}

	void ThirdState ()
	{
		bossStates[0].SetActive(false);
		bossStates[1].SetActive(false);
		bossStates[2].SetActive(true);
		bossStates [2].GetComponent<Animator> ().speed = 0.6f;
		//Reference animation to set speed
		//add hit counter to go to next state
	}

	void FourthState ()
	{
		bossStates[0].SetActive(false);
		bossStates[2].SetActive(false);
		bossStates[3].SetActive(true);
		bossStates [3].GetComponent<Animator> ().speed = 0.8f;
		//Reference animation to set speed
		//add hit counter to go to next state
	}

	void FifthState ()
	{
		bossStates[0].SetActive(false);
		bossStates[3].SetActive(false);
		bossStates[4].SetActive(true);
		bossStates [4].GetComponent<Animator> ().speed = 1f;
		//Reference animation to set speed
		//add hit counter to go to next state
	}

	void SixthState ()
	{
		bossStates[0].SetActive(false);
		bossStates[4].SetActive(false);
		bossStates[5].SetActive(true);
		bossStates [5].GetComponent<Animator> ().speed = 1.2f;
		//Reference animation to set speed
		//add hit counter to go to next state
	}
	

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.tag == "Whip") 
		{
			//Bug: switches two state because two colliders on whip or triggerenter is called twice
			if (isColliding) return;
			{
				currentState ++;
				isColliding = true;

			}

		} 
		else 
		{
			return;
		}
		//bossStates[0].SetActive(false);

	}


	void FlowerDeath ()
	{
		//Run end game sequence
	}


}
