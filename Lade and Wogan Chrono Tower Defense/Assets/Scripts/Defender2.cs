using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender2 : DefenderBase
{
    public float slowRadius = 5f;     // Radius in which enemies are slowed
    public float slowEffect = 0.3f;   // Slow factor (e.g., 0.5 means 50% of original speed)

    void Start()
    {
        // Defender stats
        defenderHealth = 80f;         // Less health than Defender1
        defenderMaxHealth = 80f;
        attackDamage = 0f;            // No direct attack, just slow effect
        attackRange = 0f;             // Range is based on slowRadius instead
        attackCooldown = 0f;          // Constant slow effect

        worldSpaceHealthBar = GetComponentInChildren<WorldSpaceHealthBar>();
        proceduralSoundtrack = FindObjectOfType<ProceduralSoundtrack>();

        // Add a trigger collider to define the slow radius
        SphereCollider slowCollider = gameObject.AddComponent<SphereCollider>();
        slowCollider.radius = slowRadius;
        slowCollider.isTrigger = true;
    }

    // Slows down the enemy when they enter the slow radius
    private void OnTriggerEnter(Collider other)
    {
        BasicEnemy enemy = other.GetComponent<BasicEnemy>();
        if (enemy != null)
        {
            enemy.movementSpeed *= slowEffect;
        }
    }

    // Restores the enemy speed when they exit the slow radius
    private void OnTriggerExit(Collider other)
    {
        BasicEnemy enemy = other.GetComponent<BasicEnemy>();
        if (enemy != null)
        {
            enemy.movementSpeed /= slowEffect;
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
}
