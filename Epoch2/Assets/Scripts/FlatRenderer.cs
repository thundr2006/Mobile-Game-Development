using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(SpriteRenderer))]
public class FlatRenderer : MonoBehaviour
{
    [HideInInspector] public SpriteRenderer spriteRenderer; 
    public Vector3 position;
    public delegate void AfterRenderPositionDelegate(ScriptableRenderContext context, Camera camera, Vector3 position);
    public AfterRenderPositionDelegate AfterRenderPosition;

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

        transform.rotation = camera.transform.rotation*Quaternion.Euler(0f,0f,15f*camera.GetComponent<PlayerController>().plrSprAngVel);

        if (AfterRenderPosition!=null) AfterRenderPosition(context, camera, position);
    }

    void OnDestroy()
    {
        RenderPipelineManager.beginCameraRendering -= onPreRenderCallback;
    }
}