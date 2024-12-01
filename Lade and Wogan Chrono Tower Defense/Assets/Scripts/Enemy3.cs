using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy3 : BasicEnemy
{
    
    void Start()
    {
        // Fast enemy stats
        enemyHealth = 30f;        // Lower health
        enemyMaxHealth = 30f;
        movementSpeed = 5f;       // Faster movement speed
        damage = 5f;              // Lower damage per hit
        attackRange = 8f;         // Similar range or adjust as needed

        // Enemy health bar
        worldSpaceHealthBar = GetComponentInChildren<WorldSpaceHealthBar>();
        proceduralSoundtrack = FindObjectOfType<ProceduralSoundtrack>();

        // Initialize the path
        int pathIndex = GetNearestPathIndex();
        InitializePath(pathIndex);

        // Setting the hourglass as the main target but can attack defenders as well
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
        // Attack defenders and the hourglass
        if (currentTarget != null)
        {
            if (Vector3.Distance(transform.position, currentTarget.transform.position) <= attackRange)
            {
                if (canAttack)
                {
                    currentTarget.TakeDamage(damage); // Low damage per hit
                    canAttack = false;
                    Invoke(nameof(ResetAttack), 0.2f); // Faster attack rate
                }
            }
        }

        if (currentTargetH != null)
        {
            if (Vector3.Distance(transform.position, currentTargetH.transform.position) <= attackRange)
            {
                if (canAttack)
                {
                    currentTargetH.TakeDamage(damage); // Low damage per hit
                    canAttack = false;
                    Invoke(nameof(ResetAttack), 0.2f); // Faster attack rate
                }
            }
        }
    }

    protected override void Die()
    {
        hourglass.RestoreHealthForEnemyKill(5f); // Less health restored on death
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
