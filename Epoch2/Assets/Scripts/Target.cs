using UnityEngine; using UnityEngine.Rendering;

public class Target : MonoBehaviour
{
    void Awake() {
        GetComponent<FlatRenderer>().AfterRenderPosition += AfterRenderPosition;
    }

    void AfterRenderPosition(ScriptableRenderContext context, Camera camera, Vector3 position) { // lockon
        if (this.enabled==false) return; // if disabled dont run
        if (!camera.GetComponent<PlayerMovement>()) return;
        camera.GetComponent<PlayerMovement>().LockOn(gameObject, position);
    }
}
