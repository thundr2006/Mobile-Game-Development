using UnityEngine;

public class Entity : MonoBehaviour
{
    public float health;

    public virtual void Damage(float amount)
    {
        health-=amount;
        health= health < 0 ? 0 : health; // forces health to be greater than 0 at all times.
    }
}
