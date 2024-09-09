using System.Collections;
using UnityEngine;


public class DefenderBase : MonoBehaviour
{
    //base defender information
    public float defenderHealth = 100f;
    public float defenderMaxHealth = 100f;
    public float attackDamage = 10f;       
    public float attackRange = 5f;        
    public float attackCooldown = 2f;    
    public float lastAttackTime = 0f;
    //base defender health bar
    public WorldSpaceHealthBar worldSpaceHealthBar;
    public void Start()
    {
        worldSpaceHealthBar = GetComponentInChildren<WorldSpaceHealthBar>();
    }

    //For if the defender is attacked
    public void TakeDamage(float damage)
    {
        defenderHealth -= damage;
        worldSpaceHealthBar.UpdateHealthBar(defenderHealth, defenderMaxHealth);
        if (defenderHealth <= 0)
        {
            Die();
        }
    }

    // Function to attack enemies
    public virtual void AttackEnemy(BasicEnemy enemy)
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            enemy.TakeDamage(attackDamage);
            lastAttackTime = Time.time; 
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    public virtual void Update()
    {
        // Find enemies in range and attack them
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        foreach (var hitCollider in hitColliders)
        {
            BasicEnemy enemy = hitCollider.GetComponent<BasicEnemy>();
            if (enemy != null)
            {
                AttackEnemy(enemy);
                break;
            }
        }
    }
}