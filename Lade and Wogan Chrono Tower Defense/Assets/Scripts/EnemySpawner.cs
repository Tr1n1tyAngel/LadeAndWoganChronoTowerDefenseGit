using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs = new GameObject[3]; // Array of enemy prefabs by type
    public MeshGenerator meshGenerator; // Reference to the MeshGenerator script
    public DefenderPlacement placement; // Reference to the DefenderPlacement script
    public Hourglass hourglass; // Reference to the Hourglass script to access health

    public float baseSpawnInterval = 3f; // Base interval between spawns
    private float elapsedTime = 0f; // Tracks elapsed time for adaptive spawning

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        // Wait until the path start points are ready
        while (meshGenerator.pathStartPoints == null || meshGenerator.pathStartPoints.Count == 0)
        {
            yield return null;
        }

        // Continuously spawn enemies based on the adjusted strategy
        while (true)
        {
            // Base min and max enemies influenced by the number of placed towers
            int baseMinEnemies = 1 + (placement.placedDefenders / 2);
            int baseMaxEnemies = baseMinEnemies + 2; // Allow for some natural increase based on towers

            // Add elapsed time as a factor to increase the range over time
            float timeFactor = Mathf.Clamp01(elapsedTime / 300f); // Assume 300 seconds (5 min) as the point of max increase

            // Apply time factor to adjust base range
            int timeAdjustedMin = baseMinEnemies + Mathf.FloorToInt(timeFactor * 2); // Up to 2 extra enemies over time
            int timeAdjustedMax = baseMaxEnemies + Mathf.FloorToInt(timeFactor * 3); // Up to 3 extra enemies over time


            // Determine the random number of enemies to spawn within the adjusted range
            int numEnemiesToSpawn = Random.Range(baseMinEnemies, baseMaxEnemies);

            for (int i = 0; i < numEnemiesToSpawn; i++)
            {
                // Randomly select one of the three starting points
                int pathIndex = Random.Range(0, meshGenerator.pathStartPoints.Count);
                Vector3 spawnPoint = meshGenerator.pathStartPoints[pathIndex];

                // Choose an enemy type based on elapsed time and RNG
                int enemyTier = GetEnemyTierBasedOnTime();
                GameObject enemy = Instantiate(enemyPrefabs[enemyTier], spawnPoint, Quaternion.identity);

                var enemyScript = enemy.GetComponent<BasicEnemy>();
                if (enemyScript != null)
                {
                    enemyScript.meshGenerator = meshGenerator; // Assign the mesh generator for access to paths
                }

                yield return new WaitForSeconds(baseSpawnInterval / numEnemiesToSpawn); // Adjust spawn timing within interval
            }

            elapsedTime += baseSpawnInterval; // Increase elapsed time for difficulty scaling
            yield return new WaitForSeconds(baseSpawnInterval);
        }
    }

    private int GetEnemyTierBasedOnTime()
    {
        // Calculate enemy type probabilities based on elapsed time
        float timeFactor = elapsedTime / 60f; // Scale up over time (1 per minute)

        // Probability thresholds for basic (0), mid-tier (1), and high-tier (2) enemies
        float basicThreshold = Mathf.Max(0.6f - 0.1f * timeFactor, 0.2f);
        float midThreshold = Mathf.Max(0.3f - 0.05f * timeFactor, 0.1f);

        float randomValue = Random.value;

        if (randomValue < basicThreshold)
            return 0; // Basic enemy
        else if (randomValue < basicThreshold + midThreshold)
            return 1; // Mid-tier enemy
        else
            return 2; // High-tier enemy
    }
}

