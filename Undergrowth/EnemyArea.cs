using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyArea : MonoBehaviour {
    
    NavMeshSurface navMeshSurface;
    public int maxEnemiesForArea;
    public int enemyIndex;
    GameObject[] enemies;



    public Transform spawnPos;


	// Use this for initialization
    void Start () {
        StartCoroutine(CheckForEnemies());
        navMeshSurface = GetComponent<NavMeshSurface>();
        enemies = new GameObject[maxEnemiesForArea];
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i] = SpawnEnemy(enemies[i], enemyIndex);
            if (enemies[i] != null)
            {
                enemies[i].transform.position = spawnPos.transform.position;
                enemies[i].SetActive(true); 
            }
        }
	}

    IEnumerator CheckForEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);
            for (int i = 0; i < enemies.Length; i++)
            {
                if (!enemies[i].activeSelf)
                {
                    Debug.Log("Spawning enemy : " + enemies[i]);
                    enemies[i] = SpawnEnemy(enemies[i], enemyIndex);
                    enemies[i].transform.position = spawnPos.transform.position;
                    enemies[i].SetActive(true);
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }
	



    GameObject SpawnEnemy(GameObject _enemy, int enemyIndex)
    {
       return EnemyManager.instance.SpawnEnemy(_enemy, enemyIndex);
    }
}
