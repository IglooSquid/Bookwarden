using UnityEngine;

public class TileClickHandler : MonoBehaviour
{
    private LayerMask clickableMask;

    void Start()
    {
        clickableMask = ~LayerMask.GetMask("Wall");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (GameModeManager.Instance == null || !GameModeManager.Instance.IsInBuildMode())
            {
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, clickableMask))
            {
                Debug.Log("Hit object: " + hit.collider.gameObject.name);

                GridTileClick tile = hit.collider.GetComponentInParent<GridTileClick>();
                if (tile != null)
                {
                    tile.OnTileClicked();
                }

                TileProperties props = hit.collider.GetComponentInParent<TileProperties>();
                if (props != null && props.isBuilt && !props.hasPlacedStructure)
                {
                    if (BuildManager.Instance == null || !BuildManager.Instance.HasSelectedItem())
                    {
                        Debug.Log("No item selected, skipping placement.");
                        return;
                    }

                    if (BuildManager.Instance.selectedItem.prefab == null)
                    {
                        Debug.LogWarning("Selected item has no prefab assigned!");
                        return;
                    }

                    Debug.Log("Placing object: " + BuildManager.Instance.selectedItem.itemName);
                    GameObject placed = Instantiate(BuildManager.Instance.selectedItem.prefab, props.transform.position, Quaternion.identity);
                    placed.transform.SetParent(props.transform);

                    PlaceableStructure structure = placed.GetComponent<PlaceableStructure>();
                    if (structure != null)
                    {
                        props.placedStructure = structure;
                    }
                }
            }
            else
            {
                Debug.Log("Nothing was hit by the raycast.");
            }
        }
    }
}