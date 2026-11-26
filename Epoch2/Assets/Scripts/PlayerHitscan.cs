using UnityEngine;

public class PlayerHitscan : MonoBehaviour
{
    public float dps;
    [HideInInspector] public Vector2 spriteSize;
    public SpriteRenderer spriteRenderer;

    public void SetTarget(Vector3 deltaPosition) {
        spriteRenderer.transform.rotation = Quaternion.LookRotation(deltaPosition, transform.position-Player.Singleton.transform.position) * Quaternion.Euler(90,0,0);

        spriteRenderer.transform.localPosition = spriteRenderer.transform.up*deltaPosition.magnitude/2;
        spriteRenderer.size = new Vector2(spriteSize.x,deltaPosition.magnitude);
    }
}
