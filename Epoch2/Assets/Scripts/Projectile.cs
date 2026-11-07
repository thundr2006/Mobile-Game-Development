using UnityEngine;

public class Projectile : Entity
{
    public float damage = 0;
    public bool fromPlayer = false; //Is projectile from the player?
    public Vector3 velocity;
    void FixedUpdate() {
        transform.position += velocity*Time.fixedDeltaTime;
    }
}
