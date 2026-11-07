using UnityEngine;
using System.Linq;

public class PlayerProjectile : Entity
{
    public float damage = 0;
    public Vector3 velocity;
    private Entity[] collidedEntities;
    private int collisionsRemaining=1;

    void FixedUpdate() {
        transform.position += velocity*Time.fixedDeltaTime;
    }
    public virtual void OnTriggerEnter(Collider other) {
        print("Collided!");
        Entity entity = other.GetComponent<Entity>();
        if (!entity) return; //If it didnt collide with an entity.. return!
        if (other.GetComponent<Player>()) return; //Collided with the player, return!
        if (other.GetComponent<PlayerProjectile>()) return; //Collided with a player-made projectile, return!

        if (collidedEntities.Contains(entity)) return; //Already collided with!

        //If the entity is not a player or a player-made projectile...
        entity.DealDamage(damage);
        collidedEntities.Append(entity);
        collisionsRemaining--;
        
        if (collisionsRemaining == 0) Destroy(gameObject); //If the projectile has used up all its collisions.. destroy! (negative collisions means its just not destroyed..)
    }
}
