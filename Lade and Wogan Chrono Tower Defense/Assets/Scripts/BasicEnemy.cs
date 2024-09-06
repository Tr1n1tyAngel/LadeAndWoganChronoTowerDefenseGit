using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    public float enemyHealth = 50;    // Health of the enemy
    public float movementSpeed = 3f;  // Speed of the enemy
    public float damage = 10f;        // Damage dealt to the defender
    public bool canAttack = true;     // Whether this enemy can attack
    public float attackRange = 1f;    // Attack range for detecting defenders
    public MeshGenerator meshGenerator; // Reference to MeshGenerator

    private List<Vector3> waypoints = new List<Vector3>(); // List of waypoints for this enemy
    private int currentWaypointIndex = 0; // The current waypoint the enemy is moving towards
    protected DefenderBase currentTarget; // Current target in range
    protected Hourglass currentTargetH;

    public void InitializePath(int pathIndex)
    {
        // Initialize the waypoints based on the chosen path index
        if (meshGenerator != null && meshGenerator.pathWaypoints.ContainsKey(pathIndex))
        {
            waypoints = meshGenerator.pathWaypoints[pathIndex];
        }
    }

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

    // Enemy movement logic
    public virtual void MoveAlongPath()
    {
        if (waypoints.Count == 0 || currentWaypointIndex >= waypoints.Count)
            return;

        Vector3 targetPosition = waypoints[currentWaypointIndex];
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.Translate(direction * movementSpeed * Time.deltaTime, Space.World);

        // If we reach the current waypoint, move to the next one
        if (Vector3.Distance(transform.position, targetPosition) < 0.2f)
        {
            currentWaypointIndex++;
        }
    }

    // Method to find defenders in attack range
    public virtual void DetectDefendersInRange()
    {
        if (currentTarget == null || currentTarget.defenderHealth <= 0)
        {
            FindNewTarget();  // Find a new defender to attack
        }
        else
        {
            AttackTarget();  // Continue attacking the current defender
        }
        if (currentTargetH == null || currentTargetH.hourglassHealth <= 0)
        {
            FindNewTarget();  // Find a new defender to attack
        }
        else
        {
            AttackTarget();  // Continue attacking the current defender
        }
    }

    // Method to attack the current target
    protected virtual void AttackTarget()
    {
        if (currentTarget != null)
        {
            if (Vector3.Distance(transform.position, currentTarget.transform.position) <= attackRange)
            {
                if (canAttack)
                {
                    currentTarget.TakeDamage(damage);
                    canAttack = false;
                    Invoke(nameof(ResetAttack), 1f); // Adjust attack cooldown
                }
            }
        }
        if (currentTargetH != null)
        {
            if (Vector3.Distance(transform.position, currentTargetH.transform.position) <= attackRange)
            {
                if (canAttack)
                {
                    currentTargetH.TakeDamage(damage * 0.5f);
                    canAttack = false;
                    Invoke(nameof(ResetAttack), 1f); // Adjust attack cooldown
                }
            }
        }
    }

    // Reset the attack cooldown
    protected void ResetAttack()
    {
        canAttack = true;
    }

    // Find a new target to attack
    protected void FindNewTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);  // Adjust detection range
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Defender"))
            {
                currentTarget = hitCollider.GetComponent<DefenderBase>();
                if (currentTarget != null && currentTarget.defenderHealth > 0)
                {
                    break;  // Focus on one defender at a time
                }
            }
            if (hitCollider.CompareTag("HourGlass"))
            {
                currentTargetH = hitCollider.GetComponent<Hourglass>();
                if (currentTargetH != null && currentTargetH.hourglassHealth > 0)
                {
                    break;  // Focus on one defender at a time
                }
            }
        }
    }

    public virtual void Update()
    {
        // Move along the path
        MoveAlongPath();

        // Check for defenders in range
        DetectDefendersInRange();
    }
}
