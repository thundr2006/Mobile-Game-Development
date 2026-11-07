using UnityEngine;
using System.Collections; using System.Collections.Generic;

public class Player : Entity
{
    public Attack[] attacks;
    int currentAttackIndex=0;
    float offCooldown=0;

    public void Shoot() {
        if (Time.time < offCooldown) return; // not yet off cooldown!!
        offCooldown = Time.time+attacks[currentAttackIndex].cooldown;

        GameObject projectile = Instantiate(
            attacks[currentAttackIndex].prefab, 
            transform.position, 
            Quaternion.identity);

    }
}

[System.Serializable] public struct Attack {
    public GameObject prefab;
    public float cooldown;
}