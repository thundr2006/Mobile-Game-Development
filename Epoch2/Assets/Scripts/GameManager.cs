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
    /*[HideInInspector]*/ public GameState gameState; //default value
    
    public SaveData currentSave;

    void Awake() {
        if (Singleton) {Destroy(gameObject); return;}
        Singleton = this;
        DontDestroyOnLoad(gameObject); //persist
    }

    void Start() {
        LoadGame();

        gameState = GameState.Running;
    }

    public void SaveGame() {
        GameState previousState = gameState;
        gameState = GameState.Saving;

        string jsonData = JsonUtility.ToJson(currentSave);
        string filePath = Application.persistentDataPath + "/saveData.json";
        System.IO.File.WriteAllText(filePath, jsonData);

        gameState = previousState;
    }

    public SaveData LoadGame() {
        GameState previousState = gameState;
        gameState = GameState.Loading;

        string filePath = Application.persistentDataPath + "/saveData.json";
        bool fileLoaded = false;

        if (System.IO.File.Exists(filePath)) {
            string jsonData = System.IO.File.ReadAllText(filePath);
            if (jsonData!="") {
                currentSave = JsonUtility.FromJson<SaveData>(jsonData);
                fileLoaded = true;
            }
        }

        /*if (fileLoaded == false) {
            currentSave = new SaveData();
        }*/

        gameState = previousState;
        return currentSave;
    }
}

[System.Serializable]
public struct SaveData {
    public string version;

    public SaveData(string Version) {
        version = Version;
    } 
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