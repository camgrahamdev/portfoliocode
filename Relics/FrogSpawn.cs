using UnityEngine;
using System.Collections;

public class FrogSpawn : MonoBehaviour
{
	
	[SerializeField]
	private GameObject boss;
	[SerializeField]
	private Collider2D enterCol;
	[SerializeField]
	private Collider2D exitCol;
	[SerializeField]
	private Collider2D exitCol2;

	private float time = 1.5f;

	void Update ()
	{
		if (FrogScript.health == 0f) 
		{
			exitCol.enabled = false;
			exitCol2.enabled = false;
		}
	}
	
	IEnumerator OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.tag == "Player") 
		{	
			yield return new WaitForSeconds (time);
			boss.SetActive(true);
			exitCol.enabled = true;
			exitCol2.enabled = true;
			yield return null;
		} 
		else 
		{
			boss.SetActive(false);
		}
		
	}
	
}