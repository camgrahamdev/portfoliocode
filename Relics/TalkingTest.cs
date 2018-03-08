using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class TalkingTest : MonoBehaviour
{
	public TalkScript _talkScript;
	public TextAsset npcDialogue;
	// public Texture2D textBox;
	public string npcName;
	string[] dialogue;
	Rect dialogueRect = new Rect(Screen.width / 2 - 285, Screen.height - 135, 590, 0);
	Rect npcnameRect = new Rect(Screen.width / 2 - 295, Screen.height - 182, 150, 20);
	public bool talking = false;
	int index = 0;
	public Font myFont;
	GUIStyle myStyle = new GUIStyle();
	GUIStyle npcNameStyle = new GUIStyle();
	
	public float letterDelay = 0.1f;
	
	// Use this for initialization
	void Start()
	{
		_talkScript = _talkScript.GetComponent<TalkScript> (); 
		myStyle.font = myFont;
		myStyle.fontSize = 20;
		myStyle.normal.textColor = Color.black;
		myStyle.wordWrap = true;
		
		npcNameStyle.font = myFont;
		npcNameStyle.fontSize = 20;
		npcNameStyle.normal.textColor = Color.black;
		npcNameStyle.alignment = TextAnchor.MiddleCenter;
		
		if (npcDialogue != null)
		{
			dialogue = (npcDialogue.text.Split('\n'));
		}
		index = 0;
	}
	
	// Update is called once per frame
	void Update()
	{
		if (talking)
		{
			if (Input.GetKeyDown("return"))
			{
				index++;
			}
		}
	}
	
	//	public void isInDialogue(bool _talking){
	//		Debug.Log ("talking = " + talking);
	//		talking = _talking;
	//
	//	}
	
	public void OnGUI()
	{
		if (talking)
		{
			
			if (index < dialogue.Length)
			{              
				GUI.Label(dialogueRect, dialogue [index], myStyle);
				GUI.Label(npcnameRect, npcName, npcNameStyle);
			} else
			{
				TalkScript.inDialogue = false;
				_talkScript.endCutscene = true;
				//isInDialogue(false);
				talking = false;
				index = 0;
			}
		}		
	}
}