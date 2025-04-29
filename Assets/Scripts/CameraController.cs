using UnityEngine;
using UnityEngine.InputSystem;

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

    private GameControls inputActions;

    private Vector3 currentVelocity;
    private Vector3 targetPosition;

    private float targetZoom;
    private float zoomVelocity;
    public float zoomSmoothTime = 0.1f;

    private bool rotateRight = false;
    private bool rotateLeft = false;

    void Start()
    {
        lastPosition = transform.position;
        lastRotation = transform.rotation;

        inputActions = new GameControls();
        inputActions.Enable();

        targetPosition = transform.position;
        if (mainCamera != null)
        {
            targetZoom = mainCamera.orthographicSize;
        }

        inputActions.Gameplay.RotateCamera.performed += ctx =>
        {
            float value = ctx.ReadValue<float>();
            if (value > 0.1f) rotateRight = true;
            else if (value < -0.1f) rotateLeft = true;
        };
    }

    void Update()
    {
        HandlePanning();
        HandleZooming();
        HandleRotation(); // New behavior: single press trigger

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
        Vector2 inputVector = inputActions.Gameplay.PanCamera.ReadValue<Vector2>();

        Quaternion cameraRotation = Quaternion.Euler(0, currentRotation + 45f, 0);
        Vector3 right = cameraRotation * Vector3.right;
        Vector3 forward = cameraRotation * Vector3.forward;

        Vector3 desiredMove = (right * inputVector.x + forward * inputVector.y) * panSpeed;
        desiredMove.y = 0f;

        targetPosition += desiredMove * Time.unscaledDeltaTime;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, 0.15f, Mathf.Infinity, Time.unscaledDeltaTime);
    }

    void HandleZooming()
    {
        if (mainCamera != null)
        {
            float scroll = inputActions.Gameplay.ZoomCamera.ReadValue<float>();
            if (scroll != 0f)
            {
                targetZoom -= scroll * zoomSpeed;
                targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            }

            mainCamera.orthographicSize = Mathf.SmoothDamp(mainCamera.orthographicSize, targetZoom, ref zoomVelocity, zoomSmoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
        }
    }

    void HandleRotation()
    {
        if (rotateRight)
        {
            currentRotation += rotationStep;
            transform.rotation = Quaternion.Euler(0, currentRotation, 0);
            rotateRight = false;
        }
        else if (rotateLeft)
        {
            currentRotation -= rotationStep;
            transform.rotation = Quaternion.Euler(0, currentRotation, 0);
            rotateLeft = false;
        }
    }

    void OnDisable()
    {
        inputActions.Disable();
    }
}