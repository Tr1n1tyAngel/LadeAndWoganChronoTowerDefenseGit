using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender1 : DefenderBase
{
    private BasicEnemy currentTarget;  // Store the current enemy being targeted

    void Start()
    {
        defenderHealth = 100f;    // Override health for Defender1
        attackDamage = 15f;       // Override damage for Defender1
        attackRange = 5f;        // Override attack range for Defender1
        attackCooldown = 1.5f;   // Override cooldown for Defender1
        
    }

    // Override the base class update logic for unique behavior
    public override void Update()
    {
        // If we have a current target, check if it's still in range and alive
        if (currentTarget != null)
        {
            // If the target is out of range or dead, clear the current target
            if (Vector3.Distance(transform.position, currentTarget.transform.position) > attackRange || currentTarget.enemyHealth <= 0)
            {
                currentTarget = null;  // Clear the target and search for a new one
            }
        }

        // If we don't have a target, find a new one
        if (currentTarget == null)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
            foreach (var hitCollider in hitColliders)
            {
                // Find the first valid enemy
                if (hitCollider.CompareTag("Enemy"))
                {
                    BasicEnemy enemy = hitCollider.GetComponent<BasicEnemy>();
                    if (enemy != null)
                    {
                        currentTarget = enemy;  // Set the first enemy in range as the target
                        break;
                    }
                }
            }
        }

        // If we have a target, attack it
        if (currentTarget != null)
        {
            AttackEnemy(currentTarget);  // Attack the current target
        }
    }

    // Override the attack logic to focus on the current target
    public override void AttackEnemy(BasicEnemy enemy)
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Debug.Log(gameObject.name + " is attacking " + enemy.gameObject.name);
            enemy.TakeDamage(attackDamage);
            lastAttackTime = Time.time; // Reset the cooldown timer
        }
    }
}
