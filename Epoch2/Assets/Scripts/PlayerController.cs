using UnityEngine; using UnityEngine.UI; using UnityEngine.InputSystem;
using System.Collections; using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{   
    [System.Serializable] public struct GameSettings {
        public bool Paused; // for debug purposes

        public float FOV;
        public float Scale;

        public float MinLockDistance;

        public GameSettings(int ignore) {
            Paused = false; FOV = 70f; Scale = 5f; MinLockDistance = 15f;
        }
    }
    [System.Serializable] public struct GUIReferences {
        public GameObject GUIGameplay;
        public GameObject GUIPause;
        public SpriteRenderer LockOn;
    }

    public static PlayerController Singleton;
    public static float InverseFOV;
    public static bool Paused; // real paused

    public GameSettings Settings = new GameSettings(1);
    public GUIReferences GUI;

    private GameObject lockOnTarget; 
    private float lockOnDistance = Mathf.Infinity; 
    private float lockOnTime;

    private float visualFPS;
    private Vector2 inputDirection;

    public void InputMove(InputAction.CallbackContext context) {
        if (PlayerController.Paused) return;
        inputDirection = context.ReadValue<Vector2>();
    }

    public void InputShoot(InputAction.CallbackContext context) {
        if (PlayerController.Paused) return;
        //(context.ReadValue<float>() > 0f)
    }

    public void InputSwap(InputAction.CallbackContext context) {
        if (PlayerController.Paused) return;
    }

    public void InputMenu(InputAction.CallbackContext context) {
        if (PlayerController.Paused) return;
        PlayerController.Singleton.Pause();
    }
    
    public void Pause() {
        Settings.Paused = true;
        Paused = true;
    }

    public void LockOnTarget(GameObject target, Vector3 realPosition, Vector3 position) {
        if (position.z-transform.position.z >= Settings.MinLockDistance) return; // out of range
        float sqrDistance = (realPosition-transform.position).sqrMagnitude;
        if (target == lockOnTarget || sqrDistance < lockOnDistance) { //is the existing target or is closer
            lockOnTarget = target;
            lockOnDistance = sqrDistance; 
            lockOnTime = Time.time;
            GUI.LockOn.transform.position = position - new Vector3(0,0,1f);
        }
    }

    IEnumerator VisualUpdate() { // visual update loop
        if (PlayerController.Paused) yield return new WaitUntil(()=> !PlayerController.Paused);

        if (inputDirection.x > 0f) { // right
            transform.rotation *= Quaternion.Euler(0,0,10f * visualFPS);
            
        } else if (inputDirection.x != 0f) { //left
            transform.rotation *= Quaternion.Euler(0,0,-10f * visualFPS);
        }

        if ( Time.time-lockOnTime > 5f || !lockOnTarget || !lockOnTarget.GetComponent<Target>().enabled ) {} else {
            GUI.LockOn.transform.position = new Vector3(0,0,0);
            lockOnDistance = Mathf.Infinity;
            lockOnTarget = null;
        }
        
        yield return new WaitForSeconds(visualFPS);
        StartCoroutine(VisualUpdate());
    }

    void Awake() { // singleton logic
        if (Singleton){ Destroy(gameObject); return;}
        Singleton = this;
        DontDestroyOnLoad(gameObject); //persistent
    }

    void Start() {
        OnValidate(); // validate settings

        visualFPS = 1/30f;
        StartCoroutine(VisualUpdate());
    }
    private void OnValidate() { // on inspector update
        InverseFOV = Settings.Scale / Mathf.Tan(Mathf.Deg2Rad * Settings.FOV/2);
        Paused = Settings.Paused;
    }
}