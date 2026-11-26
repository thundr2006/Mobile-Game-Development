using UnityEngine; using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{   
    public static MenuScript Singleton;

    void Awake() {
        if (Singleton) {Destroy(Singleton); return;}
        Singleton = this;
    }
    
    //Scene settingsScene;
    Scene currentScene;

    void Start() {
        currentScene = SceneManager.GetActiveScene();
    }

    public void Play() {
        SceneManager.LoadScene("scenePlayer", LoadSceneMode.Single);
    }
    public void Settings() {
        /*
        Settings = SceneManager.LoadScene("sceneSettings", LoadSceneMode.Additive);
        SceneManager.SetActiveScene(settingsScene);
        */  
    }
    public void CloseSettings() {
        /*
        SceneManager.SetActiveScene(currentScene);
        SceneManager.UnloadScene(settingsScene);
        */
    }
}
