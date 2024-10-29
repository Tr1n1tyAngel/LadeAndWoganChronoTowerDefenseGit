using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BasicEnemy : MonoBehaviour
{
    //base enemy stats
    public float enemyHealth = 50;
    public float enemyMaxHealth = 50;
    public float movementSpeed = 3f;
    public float damage = 10f;
    public bool canAttack = true;
    public float attackRange = 1f;
    public Transform enemyTarget; // The target object to rotate towards
    private float rotationSpeed = 5f;
    
    //references to other scripts
    public MeshGenerator meshGenerator;
    public Hourglass hourglass;
    protected DefenderBase currentTarget;
    protected Hourglass currentTargetH;
    public WorldSpaceHealthBar worldSpaceHealthBar;

    //enemy waypoints for them to move along the path
    private List<Vector3> waypoints = new List<Vector3>();
    private int currentWaypointIndex = 0;


    private void Start()
    {
        worldSpaceHealthBar = GetComponentInChildren<WorldSpaceHealthBar>();
    }
    //initializes the path depending on which path is chosen upon enemy spawn
    public void InitializePath(int pathIndex)
    {
        if (meshGenerator != null && meshGenerator.pathWaypoints.ContainsKey(pathIndex))
        {
            waypoints = meshGenerator.pathWaypoints[pathIndex];
        }
    }


    public virtual void TakeDamage(float damage)
    {
        enemyHealth -= damage;
        worldSpaceHealthBar.UpdateHealthBar(enemyHealth, enemyMaxHealth);

        if (enemyHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    // This is the logic for enemy movement
    public virtual void MoveAlongPath()
    {
        if (waypoints.Count == 0 || currentWaypointIndex >= waypoints.Count)
            return;

        Vector3 targetPosition = waypoints[currentWaypointIndex];
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.Translate(direction * movementSpeed * Time.deltaTime, Space.World);

        // If the current waypoint is reached move to the next one
        if (Vector3.Distance(transform.position, targetPosition) < 0.2f)
        {
            currentWaypointIndex++;
        }
    }

    // This finds if a defender tower is in range and then focuses on the closest one until it is either out of range or dead before switching to the next one
    public virtual void DetectDefendersInRange()
    {
        if (currentTarget == null || currentTarget.defenderHealth <= 0)
        {
            FindNewTarget();
        }
        else
        {
            AttackTarget();
        }
        if (currentTargetH == null || currentTargetH.hourglassHealth <= 0)
        {
            FindNewTarget();
        }
        else
        {
            AttackTarget();
        }
    }

    // Attacks the current target
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
                    Invoke(nameof(ResetAttack), 1f);
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
                    Invoke(nameof(ResetAttack), 1f);
                }
            }
        }
    }

    // Resets the attack cooldown
    protected void ResetAttack()
    {
        canAttack = true;
    }

    // Once a target is dead or out of range the enemy must look for a new one
    protected void FindNewTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Defender"))
            {
                currentTarget = hitCollider.GetComponent<DefenderBase>();
                if (currentTarget != null && currentTarget.defenderHealth > 0)
                {
                    break;
                }
            }
            if (hitCollider.CompareTag("HourGlass"))
            {
                currentTargetH = hitCollider.GetComponent<Hourglass>();
                if (currentTargetH != null && currentTargetH.hourglassHealth > 0)
                {
                    break;
                }
            }
        }
    }

    public virtual void Update()
    {
        MoveAlongPath();
        DetectDefendersInRange();
        RotateTowardsTarget();
    }

    //Makes the enemy face its target
    public void RotateTowardsTarget()
    {
        if (enemyTarget != null)
        {
            Vector3 direction = enemyTarget.position - transform.position;
            direction.y = 0;

            if (direction != Vector3.zero)
            {

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }
}
