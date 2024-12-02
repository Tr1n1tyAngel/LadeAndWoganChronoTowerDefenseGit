using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender2 : DefenderBase
{
    public float slowRadius = 10f;     // Radius in which enemies are slowed
    public float slowEffect = 0.3f;   // Slow factor (e.g., 0.5 means 50% of original speed)
    public DisplacementControl displacementControl;
    public bool inRange = false;
    private List<BasicEnemy> slowedEnemies = new List<BasicEnemy>(); // To keep track of slowed enemies
    public GameObject baseModel;
    public GameObject upgradeModel;

    void Start()
    {
        // Defender stats
        defenderHealth = 80f;         // Less health than Defender1
        defenderMaxHealth = 80f;
        attackDamage = 0f;            // No direct attack, just slow effect
        attackRange = 0f;             // Range is based on slowRadius instead
        attackCooldown = 0f;          // Constant slow effect

        // Other initializations
        worldSpaceHealthBar = GetComponentInChildren<WorldSpaceHealthBar>();
        displacementControl = GetComponentInChildren<DisplacementControl>();
        proceduralSoundtrack = FindObjectOfType<ProceduralSoundtrack>();
       
        // Add a trigger collider to define the slow radius
        SphereCollider slowCollider = gameObject.AddComponent<SphereCollider>();
        slowCollider.radius = slowRadius;
        slowCollider.isTrigger = true;
    }

    private void Update()
    {
        CheckForEnemiesInRange();
    }
    private void CheckForEnemiesInRange()
    {
        // Reset inRange state
        inRange = false;

        // Find all colliders within the slow radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, slowRadius);
        List<BasicEnemy> currentSlowedEnemies = new List<BasicEnemy>();

        foreach (var hitCollider in hitColliders)
        {
            BasicEnemy enemy = hitCollider.GetComponent<BasicEnemy>();
            if (enemy != null && !slowedEnemies.Contains(enemy))
            {
                // Apply slow effect to new enemies
                enemy.movementSpeed *= slowEffect;
                slowedEnemies.Add(enemy);
                inRange = true;
                Debug.Log($"Slowing enemy: {enemy.name}");
            }

            if (enemy != null)
            {
                currentSlowedEnemies.Add(enemy);
            }
        }

        // Restore speed for enemies that are no longer in range or have been destroyed
        for (int i = slowedEnemies.Count - 1; i >= 0; i--)
        {
            BasicEnemy enemy = slowedEnemies[i];
            if (enemy == null || !currentSlowedEnemies.Contains(enemy))
            {
                slowedEnemies.RemoveAt(i);
                if (enemy != null) // Only restore speed for existing enemies
                {
                    enemy.movementSpeed /= slowEffect;
                    Debug.Log($"Restoring speed for enemy: {enemy.name}");
                }
            }
        }

        // Play particle effects if there are enemies in range
        if (inRange)
        {
            if (upgradeModel.activeSelf)
            {
                displacementControl = GetComponentInChildren<DisplacementControl>();
                displacementControl.displacementAmount = Mathf.Lerp(displacementControl.displacementAmount, 0, Time.deltaTime);
                displacementControl.meshRenderer.material.SetFloat("_Amount", displacementControl.displacementAmount);

                if (!displacementControl.shootParticles.isPlaying)
                {
                    displacementControl.displacementAmount += 3f;
                    displacementControl.shootParticles.Play();
                }
            }
        }
        else
        {
            if (upgradeModel.activeSelf)
            {
                displacementControl = GetComponentInChildren<DisplacementControl>();
                if (displacementControl.shootParticles.isPlaying)
                {
                    displacementControl.shootParticles.Stop();
                }
            }
            
        }
    }


    public bool UpgradeSlowEffect(float cost, float slowEffectIncrease, Hourglass hourglass)
    {
        if (hourglass.hourglassHealth >= cost)
        {
            hourglass.ReduceHealthForDefenderPlacement(cost);
            slowEffect += slowEffectIncrease;
            return true;
        }
        return false;
    }

    public void UpgradeSlowEffect(float amount)
    {
        slowEffect += amount;
    }
    public void ChangeModel()
    {
        baseModel.SetActive(false);
        upgradeModel.SetActive(true);
    }
}
