using UnityEngine; using UnityEngine.InputSystem; using UnityEngine.UI;
using System.Collections; using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public static Player Singleton;

    public float health;
    public float maxHealth;

    public Attack[] attacks;
    private int currentAttackIndex=0;
    private float offCooldown=0;

    private float visualSPF; // Seconds Per Frame... FPS reversed

    private Vector2 inputDirection;
    public Transform shipTransform; // Transform of the visual ship.
    private Vector3 shipOffset;

    private Entity lockOnTarget;
    private float lockOnSQRDistance;
    public Transform lockOnTransform;
    private float lockOnTime;
    private float minLockOnDistance = 20f;

    public GameObject PauseScreen;
    public Image Healthbar;

    public void Shoot() {
        if (GameManager.Singleton.gameState != GameState.Running) return; 
        if (Time.time < offCooldown) return; // not yet off cooldown!!
        offCooldown = Time.time+attacks[currentAttackIndex].cooldown;

        GameObject projectile = Instantiate(
            attacks[currentAttackIndex].prefab, 
            shipTransform.position, 
            Quaternion.identity);
        
        if (lockOnTarget) {
            PlayerProjectile projectileScript = projectile.GetComponent<PlayerProjectile>();
            projectileScript.velocity = (lockOnTarget.transform.position-shipTransform.position).normalized*projectileScript.velocity.magnitude;
        }
    }

    public void Pause() {
        if (GameManager.Singleton.gameState!=GameState.Running) return;
        GameManager.Singleton.gameState = GameState.Paused;
        PauseScreen.SetActive(true);

        GameManager.Singleton.SaveGame();
    }

    public void Resume() {
        GameManager.Singleton.gameState = GameState.Running;
        PauseScreen.SetActive(false);
    }

    public void Move(InputAction.CallbackContext context) {
        if (GameManager.Singleton.gameState != GameState.Running) return; 
        inputDirection = context.ReadValue<Vector2>();
    }

    public IEnumerator VisualUpdate() {
        if (GameManager.Singleton.gameState != GameState.Running) yield return new WaitUntil(() => GameManager.Singleton.gameState != GameState.Running);
        shipTransform.localRotation = Quaternion.Slerp(shipTransform.localRotation, Quaternion.Euler(0,0,inputDirection.x*20),0.185f);
        shipTransform.localPosition = Vector3.Lerp(shipTransform.localPosition, shipOffset+new Vector3(inputDirection.x*0.9f,0,0),0.1f);

        if ((lockOnTarget && !TryLockOn(lockOnTarget)) || Time.time - lockOnTime > 0.5f) {
            lockOnSQRDistance = Mathf.Infinity;
            lockOnTarget=null;
            lockOnTime = Mathf.Infinity;
            lockOnTransform.position = new Vector3(0,0,-100);
            lockOnTransform.gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(visualSPF);
        StartCoroutine(VisualUpdate());
    }

    public void DealDamage(float damage) {
        health-=damage;
        health= health < 0 ? 0 : health; //Forces health to be greater than 0
        UpdateHealth();
    }

    public void UpdateHealth() {
        Healthbar.fillAmount = health/maxHealth;
    }

    public bool TryLockOn(Entity target) {
        float SQRDistance = (target.transform.position-(transform.position+shipOffset+new Vector3(inputDirection.x*2f,0,0))).sqrMagnitude;
        if (!target.gameObject.activeSelf||!target.enabled||!target.canLockOn) return false; // if script is not active, or cant lock on then return false
        if (SQRDistance > minLockOnDistance) return false;
        if (SQRDistance < lockOnSQRDistance || target == lockOnTarget) {
            lockOnSQRDistance = SQRDistance;
            lockOnTarget = target;
            lockOnTime = Time.time;
            lockOnTransform.position = target.transform.position+new Vector3(0,0,-0.3f);
            lockOnTransform.gameObject.SetActive(true);
            return true;
        }
        return false;
    }

    void Awake() {
        if (Singleton) { Destroy(gameObject); return; } //If a player already exists, don't create a new one!!
        Singleton = this;
        DontDestroyOnLoad(gameObject); //persist
    }

    void Start() {
        shipOffset = shipTransform.localPosition;
        visualSPF = 1/30f;
        lockOnSQRDistance = Mathf.Infinity;

        StartCoroutine(VisualUpdate());
    }

    public void OnApplicationPause(bool pause) {
        if (pause) Pause();
    }

    public void OnApplicationFocus(bool focus) {
        if (!focus) Pause();
    }

    public void OnValidate() //on inspector changes
    {
        if (Application.isEditor) return;
        UpdateHealth();
    }
}

[System.Serializable] public struct Attack {
    public GameObject prefab;
    public float cooldown;
}