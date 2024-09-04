using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hourglass : MonoBehaviour
{
    public int hourglassHealth = 100; // Object's health
    public float radius = 5f; // Radius to detect enemies
    public int damage = 10; // Damage dealt to enemies
    public float damageInterval = 2f; // Time interval between damaging enemies
    public Image healthbar;

    private void Start()
    {
        // Start the damage dealing coroutine
        StartCoroutine(DamageEnemiesOverTime());
    }

    // Coroutine that damages enemies within range every 2 seconds
    IEnumerator DamageEnemiesOverTime()
    {
        while (hourglassHealth > 0) // As long as the object has health
        {
            // Find all colliders within the specified radius
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Enemy"))
                {
                    // Get the Enemy component (assuming enemies have a health script)
                    BasicEnemy enemy = hitCollider.GetComponent<BasicEnemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(damage); // Apply damage to the enemy
                    }
                }
            }

            // Wait for the specified interval before damaging again
            yield return new WaitForSeconds(damageInterval);
        }
    }
    

    public void TakeDamage(int damage)
    {
        hourglassHealth -= damage;
        healthbar.fillAmount = hourglassHealth / 100f;
        Debug.Log(gameObject.name + " took " + damage + " damage, health left: " + hourglassHealth);

        if (hourglassHealth <= 0)
        {
            Die();
        }
    }

   
    void Die()
    {
        Debug.Log(gameObject.name + " has died!");
        Destroy(gameObject);
    }
    // Optional: Visualize the radius in the Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
