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
    private float maxLockOnDistance = 20f;
    [HideInInspector] public GameObject hitscanObject;
    [HideInInspector] public bool shootHeldDown;

    public GameObject PauseScreen;
    public Image Healthbar;
    public RectTransform SwapButton;

    public IEnumerator Shoot() {
        if (!shootHeldDown) {
            if (hitscanObject) Destroy(hitscanObject);
            yield break;
        };
        if (GameManager.Singleton.gameState != GameState.Running) yield break; 
        if (Time.time < offCooldown) yield break; // not yet off cooldown!!
        Attack currentAttack = attacks[currentAttackIndex];
        if (currentAttack.attackType==AttackType.Projectile) {
            offCooldown = Time.time+currentAttack.projectileCooldown;

            GameObject projectile = Instantiate(
            currentAttack.prefab, 
            shipTransform.position, 
            currentAttack.prefab.transform.rotation);

            if (lockOnTarget) {
                PlayerProjectile projectileScript = projectile.GetComponent<PlayerProjectile>();
                projectileScript.velocity = (lockOnTarget.transform.position-shipTransform.position).normalized*projectileScript.velocity.magnitude;
                //no y
                Vector3 rotationTowards = (projectileScript.velocity - Vector3.up*projectileScript.velocity.y).normalized;
                projectile.transform.rotation = Quaternion.LookRotation(rotationTowards, Vector3.up) * projectile.transform.rotation;
            }
            yield return new WaitForSeconds(currentAttack.projectileCooldown); // wait the attack cooldown
        } else { //assume hitscan
            if (currentAttack.hitscanCharge > 0) {
                if (!hitscanObject) { // if there isnt an existing hitscan object.. create one
                    hitscanObject = Instantiate(
                        currentAttack.prefab, 
                        shipTransform.position + transform.forward*0.35f, 
                        currentAttack.prefab.transform.rotation);

                    hitscanObject.GetComponent<PlayerHitscan>().spriteSize = hitscanObject.GetComponent<PlayerHitscan>().spriteRenderer.size;
                }
                
                PlayerHitscan hitscanScript = hitscanObject.GetComponent<PlayerHitscan>();
                hitscanObject.transform.position = shipTransform.position+Vector3.forward*0.35f;
                hitscanScript.SetTarget(lockOnTarget ? lockOnTarget.transform.position-hitscanObject.transform.position: transform.forward*maxLockOnDistance);
                
                if (lockOnTarget) {
                    lockOnTarget.DealDamage(hitscanScript.dps*visualSPF);
                }
                attacks[currentAttackIndex].hitscanCharge-=visualSPF;
                yield return new WaitForSeconds(visualSPF); // wait visual seconds per frame
            } else {
                shootHeldDown = false; // looks like there's no charge!
            }
        }

        StartCoroutine(Shoot());
    }

    public void ShootTriggerDown() {
        shootHeldDown = true;
        StartCoroutine(Shoot());
        
    }

    public void ShootTriggerUp() {
        shootHeldDown = false;
        if (hitscanObject) Destroy(hitscanObject);
    }

    public void Swap() {
        currentAttackIndex++;
        currentAttackIndex%=attacks.Length;
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

        SwapButton.rotation = Quaternion.Slerp(SwapButton.rotation, Quaternion.Euler(0,0,180*currentAttackIndex),0.3f);

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

    public void FixedUpdate() {
        for (int i = 0; i < attacks.Length; i++) {
            if (attacks[i].hitscanCharge!=attacks[i].hitscanMaxCharge && attacks[i].attackType==AttackType.Hitscan) { // regenerate charge for hitscan attacks
                if (i==currentAttackIndex && shootHeldDown) continue; // if in use, dont regenerate..
                attacks[i].hitscanCharge+=attacks[i].hitscanChargeRegen*Time.fixedDeltaTime;
                if (attacks[i].hitscanCharge > attacks[i].hitscanMaxCharge) { // clamp
                    attacks[i].hitscanCharge = attacks[i].hitscanMaxCharge;
                }
            }
        }
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
        if (SQRDistance > maxLockOnDistance) return false;
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

        for (int i = 0; i < attacks.Length; i++) {
            if (attacks[i].attackType==AttackType.Hitscan) {
                attacks[i].hitscanCharge=attacks[i].hitscanMaxCharge; // set to max charge for hitscan attacks
            }
        }

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

public enum AttackType {
    Projectile,
    Hitscan
}

[System.Serializable] public struct Attack {
    public GameObject prefab;
    public AttackType attackType;
    public float projectileCooldown;

    public float hitscanCharge; //cooldown equiv but for hitscan continuous attacks!
    public float hitscanMaxCharge;
    public float hitscanChargeRegen; // charge regen per second
}