using UnityEngine; using UnityEngine.UI; using TMPro;

public class Scoring : MonoBehaviour
{
    public int score = 0;
    void OnCollisionEnter2D(Collision2D collision) {
        collision.transform.position = new Vector3(1.7f,0.0f,0f);
        collision.rigidbody.linearVelocity = new Vector3(0,0,0);
        collision.transform.GetComponent<SpriteRenderer>().color = new Color(1f,1f,0f,0.5f);
        score++;
        GameObject.Find("Canvas/Text (TMP)").GetComponent<TMP_Text>().text = "Score: "+score.ToString();
    }
}
