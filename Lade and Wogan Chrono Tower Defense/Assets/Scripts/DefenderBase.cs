using System.Collections;
using UnityEngine;


public class DefenderBase : MonoBehaviour
{
    public float defenderHealth = 100f;
    public float defenderMaxHealth = 100f;// Health of the defender
    public float attackDamage = 10f;       // Damage dealt to enemies
    public float attackRange = 5f;        // Range within which it can attack
    public float attackCooldown = 2f;     // Time between attacks

    public float lastAttackTime = 0f;    // Track the last time the defender attacked
    public WorldSpaceHealthBar worldSpaceHealthBar;
    public void Start()
    {
        worldSpaceHealthBar = GetComponentInChildren<WorldSpaceHealthBar>();
    }

    // Take damage function (called when the defender is attacked)
    public void TakeDamage(float damage)
    {
        defenderHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage, health left: " + defenderHealth);
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
            Debug.Log(gameObject.name + " is attacking " + enemy.gameObject.name);
            enemy.TakeDamage(attackDamage);
            lastAttackTime = Time.time; // Reset the cooldown timer
        }
    }

    // If the defender's health is 0, destroy the defender
    void Die()
    {
        Debug.Log(gameObject.name + " has died!");
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
                break; // Attack one enemy at a time
            }
        }
    }
}