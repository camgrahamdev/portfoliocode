using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class AbilitiesEquipped : MonoBehaviour, IHasChanged {

	[SerializeField]
	Transform slots;
	[SerializeField]
	Evolution _Evolution;




	void Start() {
		HasChanged();
	}

	public void HasChanged() {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Apex_Greybox_v2"))
        {
            _Evolution.ResetAbilities();
            foreach (Transform slotTransform in slots)
            {
                GameObject item = slotTransform.GetComponent<Slot>().item;
                if (item)
                {
                    _Evolution.EquipAbilities(item.name);
                }

            }
        }


	}

}

namespace UnityEngine.EventSystems {
	public interface IHasChanged : IEventSystemHandler {
		void HasChanged();
	}

}