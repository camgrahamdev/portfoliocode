using UnityEngine;
using System.Collections;

public class SpawnEnemies : MonoBehaviour {

	public GameObject enemySpawns;
	public bool spawnEnemies;
	public bool deSpawnEnemies;



	void OnTriggerEnter2D (Collider2D other){
		if (spawnEnemies == true)
			enemySpawns.SetActive(true);

		if (deSpawnEnemies == true)
			enemySpawns.SetActive(false);
	}

}
