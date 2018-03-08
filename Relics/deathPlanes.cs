using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class deathPlanes : PlayerSave
{
	[SerializeField]
	private BoxCollider2D plane;
	[SerializeField]
	private GameObject saveTest;

	public GUITexture overlay;
	public float fadeTime = 1f;

	private Rigidbody2D rb;

	bool death;
	

	void Start ()
	{
		GetComponent<Rigidbody2D> ();
	}
	
	void OnTriggerEnter2D (Collider2D other)
	{

		if (other.gameObject.tag == "Player") 
		{
			death = true;

			if(death == true)
			{
				StartCoroutine(FadeToClear());
				LoadPos();
			}

		}

	}
	IEnumerator FadeToClear ()
	{

		overlay.gameObject.SetActive (true);
		overlay.color = Color.black;
		
		float rate = 1f / fadeTime;
		float progress = 0.0f;
		
		while (progress < 1.0f) {
			overlay.color = Color.Lerp (Color.black, Color.clear, progress);
			
			progress += rate * Time.deltaTime;
			
			yield return null;
		}
		
		overlay.color = Color.clear;
		overlay.gameObject.SetActive (false);
	}	
}
