using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    public float enemyHealth = 50;    // Health of the enemy
    public float movementSpeed = 3f;  // Speed of the enemy
    public float damage = 10f;        // Damage dealt to the defender
    public bool canAttack = true;     // Whether this enemy can attack

    // Take damage function (called when attacked by a defender)
    public virtual void TakeDamage(float damage)
    {
        enemyHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage, health left: " + enemyHealth);

        if (enemyHealth <= 0)
        {
            Die();
        }
    }

    // Enemy dies when health reaches 0
    protected virtual void Die()
    {
        Debug.Log(gameObject.name + " has died!");
        Destroy(gameObject);
    }

    // Enemy movement logic (can be customized in specific enemy types)
    public virtual void MoveTowardsTarget(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.Translate(direction * movementSpeed * Time.deltaTime);
    }

    // This function can be called to attack defenders
    public virtual void AttackDefender(DefenderBase defender)
    {
        if (canAttack)
        {
            defender.TakeDamage(damage);
            Debug.Log(gameObject.name + " attacked " + defender.gameObject.name + " for " + damage + " damage.");
        }
    }

    // Example Update method for moving and attacking
    public virtual void Update()
    {
        // Move towards a target (e.g., the player's base or a defender)
        // This target can be passed in or decided by the specific enemy logic
        // For simplicity, we'll move towards the origin (0,0,0)
        MoveTowardsTarget(Vector3.zero);
    }
}
