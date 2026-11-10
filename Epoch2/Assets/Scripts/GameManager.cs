using UnityEngine;

public enum GameState {
    Running,
    Paused,
    Loading
}

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton;
    [HideInInspector] public GameState gameState; //default value
    
    void Awake() {
        if (Singleton) {Destroy(gameObject); return;}
        Singleton = this;
        DontDestroyOnLoad(gameObject); //persist
    }

    void Start() {
        gameState = = GameState.Loading;
    }
}
