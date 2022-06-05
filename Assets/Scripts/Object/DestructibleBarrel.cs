using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleBarrel : InteractionObject
{
    [Header("Destructible Barrel")]
    [SerializeField] GameObject destructibleBarrelPieces;

     bool isDestroyer = false;

    public override void TakeDamage(int damage)
    {
        currentHP -= damage;

        if(currentHP <= 0 && isDestroyer == false)
        {
            isDestroyer = true;

            Instantiate(destructibleBarrelPieces, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }
}
