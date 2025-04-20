using UnityEngine;

public class WallController : MonoBehaviour
{
    [Header("Occlusion Settings")]
    public Material originalMaterial;
    public Material occlusionMaterial;
    public bool disableObstructionInstead = false;
    public float occlusionCheckInterval = 2f;

    [Header("Direction Settings")]
    public Vector2Int direction; // Set at placement time (e.g., (1,0), (0,-1), etc.)
    public Vector2Int sourceTileCoord; // New: actual tile this wall was placed from

    private Renderer wallRenderer;
    private Transform mainCamera;
    private Coroutine occlusionRoutine;

    void Start()
    {
        mainCamera = Camera.main.transform;
        wallRenderer = GetComponentInChildren<Renderer>();

        if (occlusionRoutine == null)
            occlusionRoutine = StartCoroutine(CheckOcclusionRoutine());
    }

    System.Collections.IEnumerator CheckOcclusionRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(occlusionCheckInterval);

        while (true)
        {
            CheckOcclusion();
            yield return wait;
        }
    }

    public void CheckOcclusion()
    {
        if (mainCamera == null || wallRenderer == null)
            return;

        Vector3 wallCheckPoint = transform.position + Vector3.up;
        Vector3 directionToWall = (wallCheckPoint - mainCamera.position).normalized;
        float distanceToWall = Vector3.Distance(mainCamera.position, wallCheckPoint);

        Ray ray = new Ray(mainCamera.position, directionToWall);

        if (Physics.Raycast(ray, out RaycastHit firstHit, Mathf.Infinity))
        {
            if (firstHit.collider.gameObject == gameObject || firstHit.collider.transform.IsChildOf(transform))
            {
                // Continue ray past this wall
                Vector3 newOrigin = firstHit.point + directionToWall * 0.01f;
                Ray continuationRay = new Ray(newOrigin, directionToWall);

                if (Physics.Raycast(continuationRay, out RaycastHit behindHit, Mathf.Infinity))
                {
                    TileProperties tile = behindHit.collider.GetComponentInParent<TileProperties>();
                    if (tile != null && !tile.isBuilt)
                    {
                        RestoreWallVisual();
                        return;
                    }

                    ApplyOcclusion();
                    return;
                }
            }
        }

        RestoreWallVisual();
    }

    void ApplyOcclusion()
    {
        if (disableObstructionInstead)
        {
            if (wallRenderer != null)
                wallRenderer.enabled = false;
        }
        else
        {
            if (wallRenderer != null && occlusionMaterial != null)
                wallRenderer.material = occlusionMaterial;
        }
    }

    void RestoreWallVisual()
    {
        if (disableObstructionInstead)
        {
            if (wallRenderer != null)
                wallRenderer.enabled = true;
        }
        else
        {
            if (wallRenderer != null && originalMaterial != null)
                wallRenderer.material = originalMaterial;
        }
    }
}
