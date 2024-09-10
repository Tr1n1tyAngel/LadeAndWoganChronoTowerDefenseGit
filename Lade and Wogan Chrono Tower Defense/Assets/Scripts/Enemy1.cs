using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : BasicEnemy
{
    void Start()
    {
        //Enemy stats
        enemyHealth = 50f;         
        enemyMaxHealth = 50f;
        movementSpeed = 3f;        
        damage = 10f;              
        attackRange = 8f;  
        
        //Enemy health bar
        worldSpaceHealthBar = GetComponentInChildren<WorldSpaceHealthBar>();

        //initializes the path the enemy must walk on
        int pathIndex = GetNearestPathIndex();
        InitializePath(pathIndex);

        //setting the hourglass variable
        if (hourglass == null)
        {
            hourglass = GameObject.FindWithTag("HourGlass")?.GetComponent<Hourglass>();
            enemyTarget = hourglass.transform;
        }
    }

    public override void Update()
    {
        base.Update();  
    }

    // if an enemy dies give the hourglass 10 health
    protected override void Die()
    {
        hourglass.RestoreHealthForEnemyKill(10f);  
        base.Die();  

    }

    // Finds the nearest path to where it spawns
    int GetNearestPathIndex()
    {
        //Defaults to the first path
        int nearestPathIndex = 0;
        float shortestDistance = Vector3.Distance(transform.position, meshGenerator.pathStartPoints[0]);

        //loops through every start point to find the nearest path
        for (int i = 1; i < meshGenerator.pathStartPoints.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, meshGenerator.pathStartPoints[i]);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestPathIndex = i;
            }
        }

        return nearestPathIndex;  
    }
}