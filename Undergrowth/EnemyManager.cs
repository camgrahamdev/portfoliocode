using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour {

    public static EnemyManager instance;



    public int pooledEnemies;
    List<GameObject>[] enemies;


    public GameObject[] enemy;


    //Each map contains multiple areas where enemies can spawn and move
    //Each area needs to be able to dynamically adjust enemies spawning
    //Each area needs to be able to set different amounts of maximum enemies



    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }



	// Use this for initialization
	void OnEnable () {
        enemies = new List<GameObject>[enemy.Length];
        for (int j = 0; j < enemy.Length; j++)
        {
            enemies[j] = new List<GameObject>();
            for (int i = 0; i < pooledEnemies; i++)
            {
                GameObject obj = (GameObject)Instantiate(enemy[j]);
                obj.SetActive(false);
                enemies[j].Add(obj);
            }
        }
	}
	
    public GameObject SpawnEnemy(GameObject enemy, int enemyIndex)
    {
        for (int i = 0; i < enemies[enemyIndex].Count; i++)
        { 
            if (!enemies[enemyIndex][i].activeInHierarchy)
            {
                return enemies[enemyIndex][i];
            }
        }
        return null;
    }
}
