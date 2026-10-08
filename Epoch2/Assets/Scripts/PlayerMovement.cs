using UnityEngine; using UnityEngine.UI; using UnityEngine.InputSystem;
using System.Collections; using System.Threading.Tasks; using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    public RectTransform joystick;
    public Transform lockon;

    public Image shoot;
    public Image swap;
    public Image menu;

    public SpriteRenderer PlayerSprite;

    private float vFPS;

    private Vector2 inputDirection;
    private float plrSprAngVel;

    private GameObject currentTarget;
    private float currentTargetsqrDistance;
    private float timeSinceLastLockOn;
    private float minlockDistance = 15f;

    public void InputMove(InputAction.CallbackContext context) {
        inputDirection = context.ReadValue<Vector2>();
        if (inputDirection==Vector2.zero) {
            joystick.anchoredPosition = new Vector2(245f, 150f);
            return;
        }
        joystick.anchoredPosition = new Vector2(245+(50f*inputDirection.x), 150f+(50f*inputDirection.y));
    }

    public void InputShoot(InputAction.CallbackContext context) {
        shoot.color = (context.ReadValue<float>() > 0f) ? new Color(0f,1f,0f,1f) : new Color(1f,1f,1f,1f);
    }

    public void InputSwap(InputAction.CallbackContext context) {
        swap.color = (context.ReadValue<float>() > 0f) ? new Color(0f,1f,0f,1f) : new Color(1f,1f,1f,1f);
    }

    public void InputMenu(InputAction.CallbackContext context) {
        menu.color = (context.ReadValue<float>() > 0f) ? new Color(0f,1f,0f,1f) : new Color(1f,1f,1f,1f);
    }

    void FixedUpdate() {
        plrSprAngVel = Mathf.Lerp(plrSprAngVel,inputDirection.x,0.1f);
        PlayerSprite.transform.localRotation = Quaternion.Euler(0f,0f,-15f*plrSprAngVel);

        if (Time.time-timeSinceLastLockOn < 5f && currentTarget && currentTarget.GetComponent<Target>().enabled && Mathf.Abs((currentTarget.GetComponent<FlatRenderer>().position-transform.position).z) < minlockDistance) {} else {
            lockon.position = new Vector3(0,0,0);
            currentTargetsqrDistance = Mathf.Infinity;
            currentTarget = null;
        }
    }

    void Start() {
        Screen.orientat

        currentTargetsqrDistance = Mathf.Infinity;
        vFPS = 1/15f;
        StartCoroutine(VisualUpdate());
    }

    IEnumerator VisualUpdate() {
        if (inputDirection.x > 0f) { // right
            transform.rotation *= Quaternion.Euler(0,0,15f);
            
        } else if (inputDirection.x != 0f) { //left
            transform.rotation *= Quaternion.Euler(0,0,-15f);
        }

        yield return new WaitForSeconds(vFPS);
        StartCoroutine(VisualUpdate());
    }

    public void LockOn(GameObject target, Vector3 position) {
        if (Mathf.Abs((target.GetComponent<FlatRenderer>().position-transform.position).z) >= minlockDistance) return;
        if (target == currentTarget || (position-transform.position).sqrMagnitude < currentTargetsqrDistance) {
            currentTargetsqrDistance = (position-transform.position).sqrMagnitude;
            currentTarget = target;
            timeSinceLastLockOn = Time.time;
            lockon.position = target.transform.position-new Vector3(0,0,1f);
        }
    }
}
