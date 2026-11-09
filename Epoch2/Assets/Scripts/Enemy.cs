using UnityEngine;

public class Enemy : Entity {
    public override void DealDamage(float amount)
    {
        health-=amount;
        health= health < 0 ? 0 : health; //Forces health to be greater than 0.
        if (health == 0) { //If no health...
            Destroy(gameObject); // destroy self!
        }
    }
}
