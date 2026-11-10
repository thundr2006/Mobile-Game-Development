using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton;
    
    void Awake() {
        if (Singleton) {Destroy(gameObject); return;}
        Singleton = this;
        DontDestroyOnLoad(gameObject); //persist
    }
}
