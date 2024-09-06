using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : BasicEnemy
{
    void Start()
    {
        // Set Enemy1 specific properties
        enemyHealth = 50f;         // Less health but faster
        movementSpeed = 3f;        // Faster movement speed
        damage = 10f;               // Less damage
        attackRange = 8f;        // Override attack range

        // Find the nearest path based on the enemy's spawn position
        int pathIndex = GetNearestPathIndex();

        // Initialize the path to follow
        InitializePath(pathIndex);
    }

    public override void Update()
    {
        base.Update();  // Use the base movement and attack logic
    }

    // Function to find the nearest path start point based on spawn position
    int GetNearestPathIndex()
    {
        if (meshGenerator == null || meshGenerator.pathStartPoints.Count == 0)
        {
            Debug.LogError("MeshGenerator or pathStartPoints not initialized.");
            return 0;  // Default to path 0 if something is wrong
        }

        // Default to the first path
        int nearestPathIndex = 0;
        float shortestDistance = Vector3.Distance(transform.position, meshGenerator.pathStartPoints[0]);

        // Loop through all path start points and find the nearest one
        for (int i = 1; i < meshGenerator.pathStartPoints.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, meshGenerator.pathStartPoints[i]);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestPathIndex = i;
            }
        }

        return nearestPathIndex;  // Return the index of the nearest path
    }
}