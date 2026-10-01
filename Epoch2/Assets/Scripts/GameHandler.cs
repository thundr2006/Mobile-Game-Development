using UnityEngine;

public class GameHandler : MonoBehaviour
{
    [HideInInspector] public static GameHandler main;
    [HideInInspector] public float inverseFieldOfView;
    public float FieldOfView = 70f;
    public float GlobalScaleFactor = 5f;

    void Awake() {
        if (main){ Destroy(gameObject); return;}
        main = this;

        DontDestroyOnLoad(gameObject); //persistent
    }

    void Start() {
        inverseFieldOfView = GlobalScaleFactor / Mathf.Tan(Mathf.Deg2Rad * FieldOfView/2);
    }

    private void OnValidate() { //on inspector update
        inverseFieldOfView = GlobalScaleFactor / Mathf.Tan(Mathf.Deg2Rad * FieldOfView/2);
    }
}
