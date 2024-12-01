using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender1 : DefenderBase
{
    private BasicEnemy currentTarget;  

    void Start()
    {
        //defender stats
        defenderHealth = 100f;    
        defenderMaxHealth = 100f;
        attackDamage = 15f;       
        attackRange = 5f;        
        attackCooldown = 1.5f;   
        //defender health bar
        worldSpaceHealthBar = GetComponentInChildren<WorldSpaceHealthBar>();
        proceduralSoundtrack = FindObjectOfType<ProceduralSoundtrack>();

    }

    public override void Update()
    {
        // Looks for a target, if that target is dead or out of range looks for another one 
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

    // Attack logic for the defender
    public override void AttackEnemy(BasicEnemy enemy)
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            enemy.TakeDamage(attackDamage);
            lastAttackTime = Time.time;
        }
    }
    public void UpgradeDamage(float amount)
    {
        attackDamage += amount;
    }
}
