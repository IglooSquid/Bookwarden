using UnityEngine;

public class TileClickHandler : MonoBehaviour
{
    private LayerMask clickableMask;

    void Start()
    {
        // Invert the layer mask to exclude the Wall layer
        clickableMask = ~LayerMask.GetMask("Wall");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, clickableMask))
            {
                GridTileClick tile = hit.collider.GetComponentInParent<GridTileClick>();
                if (tile != null)
                {
                    tile.OnTileClicked();
                }
            }
        }
    }
}