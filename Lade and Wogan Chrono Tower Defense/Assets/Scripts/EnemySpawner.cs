using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs = new GameObject[3];
    public MeshGenerator meshGenerator;

    public float spawnInterval = 3f;

 
    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }
    // The enemy spawning method which spawns an enemy every few seconds
    IEnumerator SpawnEnemies()
    {
        // Makes sure that the mesh has been generated so enemies can spawn
        while (meshGenerator.pathStartPoints == null || meshGenerator.pathStartPoints.Count == 0)
        {
            yield return null;
        }

        //randomly picks one of the paths, then spawns an enemy at that starting point, then assigns any information needed to the enemy
        while (true)
        {
           
            int randomIndex = Random.Range(0, meshGenerator.pathStartPoints.Count);
            int randomEnemy = Random.Range(0, 3);

            GameObject enemy = Instantiate(enemyPrefabs[randomEnemy], meshGenerator.pathStartPoints[randomIndex], Quaternion.identity);

            BasicEnemy enemyScript = enemy.GetComponent<BasicEnemy>();
            if (enemyScript != null)
            {
                enemyScript.meshGenerator = meshGenerator; 
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
