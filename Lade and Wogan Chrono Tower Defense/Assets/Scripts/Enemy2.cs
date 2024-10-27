using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2 : BasicEnemy
{
    void Start()
    {
        // Tank enemy stats
        enemyHealth = 150f;       // Higher health
        enemyMaxHealth = 150f;
        movementSpeed = 1.5f;     // Slower movement speed
        damage = 50f;             // High damage per hit
        attackRange = 4f;         // Similar range or adjust as needed

        // Enemy health bar
        worldSpaceHealthBar = GetComponentInChildren<WorldSpaceHealthBar>();

        // Initialize the path
        int pathIndex = GetNearestPathIndex();
        InitializePath(pathIndex);

        // Setting the hourglass as the main target
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

    protected override void AttackTarget()
    {
        // Only attack the hourglass (main tower)
        if (currentTargetH != null)
        {
            if (Vector3.Distance(transform.position, currentTargetH.transform.position) <= attackRange)
            {
                if (canAttack)
                {
                    currentTargetH.TakeDamage(damage); // High damage
                    canAttack = false;
                    Invoke(nameof(ResetAttack), 3f); // Slower attack rate
                }
            }
        }
    }

    protected override void Die()
    {
        hourglass.RestoreHealthForEnemyKill(30f); // More health restored on death
        base.Die();
    }

    int GetNearestPathIndex()
    {
        int nearestPathIndex = 0;
        float shortestDistance = Vector3.Distance(transform.position, meshGenerator.pathStartPoints[0]);

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
