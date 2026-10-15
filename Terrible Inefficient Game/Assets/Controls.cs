using UnityEngine;
using UnityEngine.InputSystem;

public class Controls : MonoBehaviour
{
    public bool directionBallDefault = false; // move left;
    public bool pressed = false;
    public float timeLastSince = 0f;
    void Update()
    {
        float indexPaddles1 = 0;
        while (indexPaddles1 < GameObject.Find("Board/Paddle1s").transform.childCount) {
            GameObject.Find("Board/Paddle1s").transform.GetChild((int)indexPaddles1).localRotation = Quaternion.Euler(0f,0f,
                Mathf.Atan2(
                    InputSystem.actions["Move"].ReadValue<Vector2>().y,
                    Mathf.Clamp(InputSystem.actions["Move"].ReadValue<Vector2>().x, -1f, 0f)
                )*Mathf.Rad2Deg+90f
            );
            indexPaddles1++;
        }

        float indexPaddles2 = 0;
        while (indexPaddles2 < GameObject.Find("Board/Paddle2s").transform.childCount) {
            GameObject.Find("Board/Paddle2s").transform.GetChild((int)indexPaddles2).localRotation = Quaternion.Euler(0f,0f,
                Mathf.Atan2(
                    InputSystem.actions["Move"].ReadValue<Vector2>().y,
                    Mathf.Clamp(InputSystem.actions["Move"].ReadValue<Vector2>().x, 0f, 1f)
                )*Mathf.Rad2Deg+90f
            );
            indexPaddles2++;
        }

        if (InputSystem.actions["Tap"].ReadValue<float>() > 0.5f && pressed == false) { // tap pressed
            //pressed = true;
            timeLastSince = Time.time;
            GameObject clone = Instantiate(GameObject.Find("BallDefault"));
            clone.GetComponent<CircleCollider2D>().enabled = true;
            clone.GetComponent<Rigidbody2D>().simulated = true;
            clone.GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,1f);
        }
        if (InputSystem.actions["Tap"].ReadValue<float>() < 0.5f) {
            pressed = false;
        }
        if (Time.time - timeLastSince > 0.15f) {
            pressed = false;
        }

        if (directionBallDefault) {
            GameObject.Find("BallDefault").transform.position+=new Vector3(5*Time.deltaTime,0,0);
        }
        if (!directionBallDefault) {
            GameObject.Find("BallDefault").transform.position-=new Vector3(5*Time.deltaTime,0,0);
        }
        if (GameObject.Find("BallDefault").transform.position.x >= 1.63) {
            directionBallDefault = false;
        }
        if (GameObject.Find("BallDefault").transform.position.x <= -1.63) {
            directionBallDefault = true;
        }
    }
}
