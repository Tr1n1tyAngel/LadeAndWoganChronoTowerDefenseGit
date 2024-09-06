using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastEnemy : BasicEnemy
{
    void Start()
    {
        enemyHealth = 30;         // Less health but faster
        movementSpeed = 6f;       // Faster movement speed
        damage = 5f;              // Less damage
    }

    public override void Update()
    {
        base.Update(); // Use the base movement logic or customize it here
    }
}