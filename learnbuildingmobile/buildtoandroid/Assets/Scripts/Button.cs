using UnityEngine;
using UnityEngine.UI;

public class Button : MonoBehaviour
{
    public Image buttonImage;

    private bool active = false;

    public void OnMouseDown() {
        active = !active;
        if (active) {
            buttonImage.color = new Color(1f, 0f, 0f, 1f);
        } else {
            buttonImage.color = new Color(1f, 1f, 1f, 1f);
        }
    }
}
