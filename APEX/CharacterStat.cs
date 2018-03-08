using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;


public class CharacterStat {

		[XmlAttribute("Strength")]
		public int p_Strength;
		[XmlAttribute("Speed")]
		public int p_Speed;
		[XmlAttribute("Stealth")]
		public int p_Stealth;
		[XmlAttribute("Endurance")]
		public int p_Endurance;

	}


