using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    public int enemyHealth = 50;

    public void TakeDamage(int damage)
    {
        enemyHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage, health left: " + enemyHealth);

        if (enemyHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " has died!");
        Destroy(gameObject);
    }
}
