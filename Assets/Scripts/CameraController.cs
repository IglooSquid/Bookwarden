using UnityEngine;

public class CameraController : MonoBehaviour
{
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
}