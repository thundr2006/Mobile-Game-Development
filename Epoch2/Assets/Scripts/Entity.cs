using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Entity : MonoBehaviour
{
    public bool canLockOn; //Can it be locked onto?
    public float health;

    public virtual void DealDamage(float amount)
    {
        health-=amount;
        health= health < 0 ? 0 : health; //Forces health to be greater than 0.
    }

    void FixedUpdate() {
        if (Player.Singleton && canLockOn) {
            Player.Singleton.TryLockOn(this);
        }
    }
}
