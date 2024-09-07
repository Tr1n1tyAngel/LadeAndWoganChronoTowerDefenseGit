using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public MeshGenerator meshGenerator;    // Reference to the MeshGenerator in the scene
    public float spawnInterval = 3f;

 
    private void Start()
    {
        // Start spawning enemies after ensuring that pathStartPoints have been initialized
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        // Wait until the meshGenerator has path start points available
        while (meshGenerator.pathStartPoints == null || meshGenerator.pathStartPoints.Count == 0)
        {
            Debug.LogWarning("Waiting for pathStartPoints to be initialized.");
            yield return null;  // Wait for the next frame
        }

        // Now we can spawn enemies
        while (true)
        {
            // Randomly pick one of the path start points from the MeshGenerator
            int randomIndex = Random.Range(0, meshGenerator.pathStartPoints.Count);

            // Instantiate the enemy prefab at the randomly selected start point
            GameObject enemy = Instantiate(enemyPrefab, meshGenerator.pathStartPoints[randomIndex], Quaternion.identity);

            // Get the enemy script and assign the MeshGenerator reference
            BasicEnemy enemyScript = enemy.GetComponent<BasicEnemy>();
            if (enemyScript != null)
            {
                enemyScript.meshGenerator = meshGenerator; // Assign the MeshGenerator to the enemy
            }
            
            // Wait for the specified spawn interval before spawning another enemy
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
