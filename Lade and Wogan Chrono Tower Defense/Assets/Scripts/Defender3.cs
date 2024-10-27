using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender3 : DefenderBase
{
    private BasicEnemy currentTarget;

    void Start()
    {
        // Sniper defender stats
        defenderHealth = 150f;        // High health
        defenderMaxHealth = 150f;
        attackDamage = 100f;           // High damage per attack
        attackRange = 15f;            // Large attack range
        attackCooldown = 5f;          // Slower attack rate

        worldSpaceHealthBar = GetComponentInChildren<WorldSpaceHealthBar>();
    }

    public override void Update()
    {
        // Similar to Defender1, focusing on a single target
        if (currentTarget != null)
        {
            if (Vector3.Distance(transform.position, currentTarget.transform.position) > attackRange || currentTarget.enemyHealth <= 0)
            {
                currentTarget = null;
            }
        }

        if (currentTarget == null)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Enemy"))
                {
                    BasicEnemy enemy = hitCollider.GetComponent<BasicEnemy>();
                    if (enemy != null)
                    {
                        currentTarget = enemy;
                        break;
                    }
                }
            }
        }

        if (currentTarget != null)
        {
            AttackEnemy(currentTarget);
        }
    }

    public override void AttackEnemy(BasicEnemy enemy)
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            enemy.TakeDamage(attackDamage);
            lastAttackTime = Time.time;
        }
    }
}