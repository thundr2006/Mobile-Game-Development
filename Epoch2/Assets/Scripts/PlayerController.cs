using UnityEngine; using UnityEngine.UI; using UnityEngine.InputSystem;
using System.Collections; using System.Threading.Tasks; using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    private float vFPS;

    private Vector2 inputDirection;

    private GameObject currentTarget;
    private float currentTargetsqrDistance;
    private float timeSinceLastLockOn;
    private float minlockDistance = 15f;

    public void InputMove(InputAction.CallbackContext context) {
        if (GameHandler.Singleton.Settings.Paused) return;
        inputDirection = context.ReadValue<Vector2>();
    }

    public void InputShoot(InputAction.CallbackContext context) {
        if (GameHandler.Singleton.Settings.Paused) return;
        //(context.ReadValue<float>() > 0f)
    }

    public void InputSwap(InputAction.CallbackContext context) {
        if (GameHandler.Singleton.Settings.Paused) return;
    }

    public void InputMenu(InputAction.CallbackContext context) {
        if (GameHandler.Singleton.Settings.Paused) return;
        GameHandler.Singleton.Pause();
    }

    void Start() {
        currentTargetsqrDistance = Mathf.Infinity;
        vFPS = 1/30f;
        StartCoroutine(VisualUpdate());
    }

    IEnumerator VisualUpdate() {
        if (inputDirection.x > 0f) { // right
            transform.rotation *= Quaternion.Euler(0,0,10f * vFPS);
            
        } else if (inputDirection.x != 0f) { //left
            transform.rotation *= Quaternion.Euler(0,0,-10f * vFPS);
        }
        
        yield return new WaitForSeconds(vFPS);
        StartCoroutine(VisualUpdate());
    }
}
