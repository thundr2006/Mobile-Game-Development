using UnityEngine;
using System.Collections.Generic;

public enum GameState {
    Running,
    Paused,
    Loading,
    Saving
}

public class GameManager : MonoBehaviour
{
    public static GameManager Singleton;
    [HideInInspector] public GameState gameState; //default value
    
    public SaveData currentSave;

    void Awake() {
        if (Singleton) {Destroy(gameObject); return;}
        Singleton = this;
        DontDestroyOnLoad(gameObject); //persist
    }

    void Start() {
        gameState = GameState.Loading;
    }

    public bool SaveGame() {
        
    }

    public SaveData LoadGame() {

    }
}

public struct SaveData {
    public string version;
}




//for saving json lists

//[System.Serializable]
//public class SaveDataListItem {} 

//used like List<SaveDataListItem>()
//and JSONList<SaveDataListItem>(List)

[System.Serializable]
public class JSONList<typeSet>
{
    public List<typeSet> list;
    public JSONList(List<typeSet> list) {
        this.list = list;
    }
}