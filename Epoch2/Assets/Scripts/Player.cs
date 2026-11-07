using UnityEngine; using UnityEngine.InputSystem; using UnityEngine.UI;
using System.Collections; using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public static Player Singleton;

    public float health;

    public Attack[] attacks;
    private int currentAttackIndex=0;
    private float offCooldown=0;

    private float visualSPF; // Seconds Per Frame... FPS reversed

    private Vector2 inputDirection;
    public Transform ship; // Transform of the visual ship.
    private Vector3 shipOffset;

    public void Shoot() {
        if (Time.time < offCooldown) return; // not yet off cooldown!!
        offCooldown = Time.time+attacks[currentAttackIndex].cooldown;

        GameObject projectile = Instantiate(
            attacks[currentAttackIndex].prefab, 
            ship.position, 
            Quaternion.identity);
    }


    public void Move(InputAction.CallbackContext context) {
        inputDirection = context.ReadValue<Vector2>();
    }

    public IEnumerator VisualUpdate() {
        ship.localRotation = Quaternion.Slerp(ship.localRotation, Quaternion.Euler(0,0,inputDirection.x*20),0.185f);
        ship.localPosition = Vector3.Slerp(ship.localPosition, Quaternion.Euler(0,0,inputDirection.x*20)*shipOffset,0.185f);

        yield return new WaitForSeconds(visualSPF);
        StartCoroutine(VisualUpdate());
    }

    public void DealDamage(float damage) {
        health-=damage;
        health= health < 0 ? 0 : health; //Forces health to be greater than 0.
    }

    void Awake() {
        if (Singleton) { Destroy(gameObject); return; } //If a player already exists, don't create a new one!!
        Singleton = this;
    }

    void Start() {
        shipOffset = ship.localPosition;
        visualSPF = 1/30f;
        StartCoroutine(VisualUpdate());
    }
}

[System.Serializable] public struct Attack {
    public GameObject prefab;
    public float cooldown;
}