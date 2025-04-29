using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Panning")]
    public float panSpeed = 10f;
    
    [Header("Zooming")]
    public Camera mainCamera;
    public float zoomSpeed = 2f;
    public float minZoom = 3f;
    public float maxZoom = 10f;

    [Header("Rotation")]
    public float rotationStep = 90f;
    private float currentRotation = 0f;

    private Vector3 lastPosition;
    private Quaternion lastRotation;

    [Header("Occlusion Refresh")]
    public float movementThreshold = 0.5f;
    public float rotationThreshold = 2f; // degrees

    void Start()
    {
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    void Update()
    {
        HandlePanning();
        HandleZooming();
        HandleRotation();

        float moveDistance = Vector3.Distance(transform.position, lastPosition);
        float rotDifference = Quaternion.Angle(transform.rotation, lastRotation);

        if (moveDistance > movementThreshold || rotDifference > rotationThreshold)
        {
            RefreshAllWallOcclusion();
            lastPosition = transform.position;
            lastRotation = transform.rotation;
        }
    }

    void RefreshAllWallOcclusion()
    {
        foreach (WallController wall in FindObjectsOfType<WallController>())
        {
            wall.CheckOcclusion();
        }
    }

void HandlePanning()
{
    float moveX = Input.GetAxis("Horizontal");
    float moveZ = Input.GetAxis("Vertical");

    Vector3 input = new Vector3(moveX, 0f, moveZ).normalized;

    // Calculate world directions relative to camera rotation, adjusted for 45° isometric tilt
    Quaternion cameraRotation = Quaternion.Euler(0, currentRotation + 45f, 0);
    Vector3 right = cameraRotation * Vector3.right;
    Vector3 forward = cameraRotation * Vector3.forward;

    Vector3 move = (right * moveX + forward * moveZ) * panSpeed * Time.unscaledDeltaTime;
    move.y = 0f;

    transform.position += move;
}

    void HandleZooming()
    {
        if (mainCamera != null)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                float newSize = mainCamera.orthographicSize - scroll * zoomSpeed;
                mainCamera.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
            }
        }
    }

    void HandleRotation()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentRotation -= rotationStep;
            transform.rotation = Quaternion.Euler(0, currentRotation, 0);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            currentRotation += rotationStep;
            transform.rotation = Quaternion.Euler(0, currentRotation, 0);
        }
    }

}