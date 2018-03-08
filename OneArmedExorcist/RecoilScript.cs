using UnityEngine;
using System.Collections;

public class RecoilScript : MonoBehaviour {

	[SerializeField]
	private float maxRecoil_x = -20;
	[SerializeField]
	private float recoilSpeed = 10;
	[SerializeField]
	private Transform recoilTrans;
	float recoil;
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			recoil += 0.1f;
		}

		if (recoil > 0) {
			Quaternion maxRecoil = Quaternion.Euler (maxRecoil_x, 0, 0);
			recoilTrans.rotation = Quaternion.Slerp (recoilTrans.rotation, maxRecoil, Time.deltaTime * recoilSpeed);
			recoil -= Time.deltaTime;
		} else {
			recoil = 0;
			Quaternion minRecoil = Quaternion.Euler (0, 0, 0);
			float angle = Quaternion.Angle (recoilTrans.rotation, minRecoil);
			if (angle < 1) {
				return;
			}
			recoilTrans.rotation = Quaternion.Slerp (recoilTrans.rotation, minRecoil, Time.deltaTime * recoilSpeed / 2);

		}
	}
}
