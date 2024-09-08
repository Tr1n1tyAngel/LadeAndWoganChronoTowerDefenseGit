using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Hourglass : MonoBehaviour
{
    public float hourglassHealth = 100; // Object's health
    public float radius = 5f; // Radius to detect enemies
    public float damage = 10; // Damage dealt to enemies
    public float damageInterval = 5f; // Time interval between damaging enemies
    public Image healthbar;
    public TextMeshProUGUI healthText; // Reference to the UI Text component

    private void Start()
    {
        // Start the damage dealing coroutine
        StartCoroutine(DamageEnemiesOverTime());
        UpdateHealthUI(); // Initialize the health UI text
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(5);
        }
    }

    // Coroutine that damages enemies within range every X seconds
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

    public void TakeDamage(float damage)
    {
        hourglassHealth -= damage;
        healthbar.fillAmount = hourglassHealth / 500f;
        Debug.Log(gameObject.name + " took " + damage + " damage, health left: " + hourglassHealth);

        UpdateHealthUI(); // Update the health UI text when damage is taken

        if (hourglassHealth <= 0)
        {
            Die();
        }
    }

    public void ReduceHealthForDefenderPlacement(float healthCost)
    {
        hourglassHealth -= healthCost;
        healthbar.fillAmount = hourglassHealth / 500f;
        Debug.Log("Hourglass health reduced by " + healthCost + " due to defender placement.");

        UpdateHealthUI(); // Update the health UI text when health is reduced
    }

    // Method to restore health when an enemy is killed
    public void RestoreHealthForEnemyKill(float healthRestored)
    {
        hourglassHealth += healthRestored;
        healthbar.fillAmount = hourglassHealth / 500f;
        Debug.Log("Hourglass restored by " + healthRestored + " from enemy death.");

        UpdateHealthUI(); // Update the health UI text when health is restored
    }

    // Update the UI Text with the current health
    private void UpdateHealthUI()
    {
        healthText.text = "" +hourglassHealth;
    }

    void Die()
    {
        Debug.Log(gameObject.name + " has died!");
        Destroy(gameObject);
        SceneManager.LoadScene("GameOver");
    }

    // Optional: Visualize the radius in the Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
