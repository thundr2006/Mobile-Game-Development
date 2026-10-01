using UnityEngine; using UnityEngine.UI; using UnityEngine.InputSystem;
using System.Collections; using System.Threading.Tasks; using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    public RectTransform joystick;
    public Transform lockon;
    public Image up;
    public Image down;
    public Image left;
    public Image right;

    public Image shoot;
    public Image swap;
    public Image menu;

    public SpriteRenderer PlayerSprite;

    private float vFPS;

    private Vector2 inputDirection;
    private float plrSprAngVel;

    private GameObject currentTarget;
    private float currentTargetsqrDistance;

    public void InputMove(InputAction.CallbackContext context) {
        inputDirection = context.ReadValue<Vector2>();

        up.color = (inputDirection.y > 0f) ? new Color(0f,1f,0f,1f) : new Color(1f,1f,1f,1f);
        down.color = (inputDirection.y < 0f) ? new Color(0f,1f,0f,1f) : new Color(1f,1f,1f,1f);
        left.color = (inputDirection.x < 0f) ? new Color(0f,1f,0f,1f) : new Color(1f,1f,1f,1f);
        right.color = (inputDirection.x > 0f) ? new Color(0f,1f,0f,1f) : new Color(1f,1f,1f,1f);

        if (inputDirection==Vector2.zero) {
            joystick.anchoredPosition = new Vector2(-400f, -150f);
            return;
        }
        joystick.anchoredPosition = new Vector2(-400f+(50f*inputDirection.x), -150f+(50f*inputDirection.y));
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

    void Update() {
        plrSprAngVel = Mathf.Lerp(plrSprAngVel,inputDirection.x,0.1f);
        PlayerSprite.transform.localRotation = Quaternion.Euler(0f,0f,-15f*plrSprAngVel);
    }

    void Start() {
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
        if (target == currentTarget || (position-transform.position).sqrMagnitude < currentTargetsqrDistance) {
            currentTargetsqrDistance = (position-transform.position).sqrMagnitude;
            currentTarget = target;
            lockon.position = target.transform.position-new Vector3(0,0,1f);
        }
    }
}
