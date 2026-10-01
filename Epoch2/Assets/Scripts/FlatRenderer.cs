using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(SpriteRenderer))]
public class FlatRenderer : MonoBehaviour
{
    [HideInInspector] public SpriteRenderer spriteRenderer; 
    public Vector3 position;
    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        RenderPipelineManager.beginCameraRendering += onPreRenderCallback;
    }

    void onPreRenderCallback(ScriptableRenderContext context, Camera camera) //per camera rendering
    {
        if (!Application.IsPlaying(gameObject)) return;
        Vector3 positionDelta = position-camera.transform.position;
        Vector3 localDelta = camera.transform.InverseTransformVector(positionDelta);
        
        if (localDelta.z <= 0) { //behind screen, do nothing
            spriteRenderer.enabled = false;
            return;
        };
        spriteRenderer.enabled = true;

        float scaleFactor = GameHandler.main.inverseFieldOfView / (localDelta.z );
        transform.localScale = new Vector3(1,1,1) * scaleFactor;

        float scaledX = localDelta.x * scaleFactor;
        float scaledY = localDelta.y * scaleFactor;
        float magnitude = localDelta.magnitude;

        transform.position = camera.transform.position + camera.transform.TransformVector(new Vector3(scaledX, scaledY, magnitude));
    }

    void OnDestroy()
    {
        RenderPipelineManager.beginCameraRendering -= onPreRenderCallback;
    }
}