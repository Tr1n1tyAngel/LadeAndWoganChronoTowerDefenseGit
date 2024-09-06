using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender1 : DefenderBase
{
    // Inherits all functionality from the Defender base class
    void Start()
    {
        defenderHealth = 30f;    // Override health for Defender1
        attackDamage = 15f;       // Override damage for Defender1
        attackRange = 5f;        // Override attack range for Defender1
        attackCooldown = 1.5f;   // Override cooldown for Defender1
        base.Start();
    }

    // If Defender1 has unique behavior, we can override the Update method here
    public override void Update()
    {
        base.Update(); // Use the base class logic for attacking
    }
}
