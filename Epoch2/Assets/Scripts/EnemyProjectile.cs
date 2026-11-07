using UnityEngine;
using System.Linq;

public class EnemyProjectile : Entity
{
    public float damage = 0;
    public Vector3 velocity;

    void FixedUpdate() {
        transform.position += velocity*Time.fixedDeltaTime;
    }
    public virtual void OnTriggerEnter(Collider other) {
        Entity entity = other.GetComponent<Entity>();
        if (!entity) return; //If it didnt collide with an entity.. return!
        if (other.GetComponent<Enemy>()) return; //Collided with an enemy, return!
        if (other.GetComponent<EnemyProjectile>()) return; //Collided with a enemy-made projectile, return!
        if (other.GetComponent<PlayerProjectile>()) return; //Collided with a player-made projectile, return because the PlayerProjectile handles this!

        //If the entity is not an enemy or a enemy-made projectile...
        entity.DealDamage(damage);
        Destroy(gameObject);
    }
}
