using UnityEngine;

public class GameHandler : MonoBehaviour
{
    //settings

    [HideInInspector] public static float InverseFOV;
    [System.Serializable] public struct GameSettings {
        public bool Paused;

        public float FOV;
        public float Scale;

        public float MinLockDistance;

        public GameSettings(int ignore) {
            Paused = false; FOV = 70f; Scale = 5f; MinLockDistance = 15f;
        }
    }
    public GameSettings Settings = new GameSettings(1);

    void Start() {
        InverseFOV = Settings.Scale / Mathf.Tan(Mathf.Deg2Rad * Settings.FOV/2);
    }
    private void OnValidate() { //on inspector update
        InverseFOV = Settings.Scale / Mathf.Tan(Mathf.Deg2Rad * Settings.FOV/2);
    }

    //gui references

    [System.Serializable] public struct GUIReferences {
        public GameObject GUIGameplay;
        public GameObject GUIPause;
        public SpriteRenderer LockOn;
    }
    public GUIReferences GUI;

    //gameplay functions
    
    public void Pause() {
        Settings.Paused = true;
    }

    private GameObject currentTarget; private float currentTargetsqrDistance = Mathf.Infinity; private float timeSinceLastLockOn;
    public void LockOnTarget(GameObject target, Vector3 realPosition, Vector3 position) {
        if (position.z-transform.position.z >= Settings.MinLockDistance) return; // out of range
        float sqrDistance = (realPosition-transform.position).sqrMagnitude;
        if (target == currentTarget || sqrDistance < currentTargetsqrDistance) { //is the existing target or is closer
            currentTarget = target;
            currentTargetsqrDistance = sqrDistance; timeSinceLastLockOn = Time.time;
            GUI.LockOn.transform.position = position -new Vector3(0,0,1f);
        }
    }

    //singleton logic

    [HideInInspector] public static GameHandler Singleton;
    void Awake() {
        if (Singleton){ Destroy(gameObject); return;}
        Singleton = this;
        DontDestroyOnLoad(gameObject); //persistent
    }
}