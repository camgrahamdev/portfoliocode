using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TalkScript : MonoBehaviour {
	
    bool canTalk;
	public bool endCutscene;
    public Image textbox;
	public static bool inDialogue;
	public TalkingTest _TalkingTest;
	public bool cutscene;
	public GameObject backstoryMarker;
    

	void Start(){
		_TalkingTest = _TalkingTest.GetComponent<TalkingTest> (); 
	
	}

	void DoDialogue(){
		inDialogue = true;
		textbox.gameObject.SetActive(true);		
		Debug.Log ("I am talking");
		Time.timeScale = 0;
		_TalkingTest.talking = true;
		backstoryMarker.SetActive(false);
	}

	void Update(){
		if (!inDialogue) {
			if(!cutscene){
			if (Input.GetKeyDown (KeyCode.Return) & canTalk == true) {
					DoDialogue ();
				}
				else{
					textbox.gameObject.SetActive(false);
					Time.timeScale = 1;
				}
			}
            else{
				if (canTalk == true && !endCutscene){
					DoDialogue();
				}
				else{
					textbox.gameObject.SetActive(false);
					Time.timeScale = 1;
				}
		}
		}
	}

	void OnTriggerEnter2D(Collider2D other){
		Debug.Log (other);
		canTalk = true;
	}

	void OnTriggerExit2D(){
		canTalk = false;
		inDialogue = false;
	}
	
}
