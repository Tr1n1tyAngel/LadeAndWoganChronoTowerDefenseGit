using System.Collections;
using UnityEngine;

public class DefenderBase : MonoBehaviour
{
    public float defenderHealth = 100;    // Health of the defender
    public float attackDamage = 10;       // Damage dealt to enemies
    public float attackRange = 5f;        // Range within which it can attack
    public float attackCooldown = 2f;     // Time between attacks

    private float lastAttackTime = 0f;    // Track the last time the defender attacked

    public GameObject rangeIndicatorPrefab;  // Reference to the range indicator prefab
    protected GameObject rangeIndicator;     // The actual instantiated range indicator

    public void Start()
    {
        // Instantiate the range indicator and scale it based on the attack range
        if (rangeIndicatorPrefab != null)
        {
            rangeIndicator = Instantiate(rangeIndicatorPrefab, transform.position, Quaternion.identity);
            rangeIndicator.transform.SetParent(transform); // Make it a child of the defender

            // Rotate the range indicator 90 degrees on the X-axis
            rangeIndicator.transform.localRotation = Quaternion.Euler(90f, 0f, 0f); // Rotate by 90 degrees on the X-axis

            // Scale the range indicator according to the attack range (X and Y axes now)
            float indicatorScale = attackRange * 2f; // Multiply by 2 to get the diameter

            // Apply scaling to both X and Y axes (since Y is now treated as width/height after the rotation)
            rangeIndicator.transform.localScale = new Vector3(indicatorScale, indicatorScale, 1f); // Scale in X and Y, keep Z thin

            // Position the indicator under the defender (e.g., slightly beneath)
            rangeIndicator.transform.localPosition = new Vector3(0, -0.5f, 0); // Slightly under the defender
        }
    }

    // Take damage function (called when the defender is attacked)
    public void TakeDamage(float damage)
    {
        defenderHealth -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage, health left: " + defenderHealth);

        if (defenderHealth <= 0)
        {
            Die();
        }
    }

    // Function to attack enemies
    public void AttackEnemy(BasicEnemy enemy)
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

    // This can be overridden in subclasses if defenders have unique attack behaviors
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
