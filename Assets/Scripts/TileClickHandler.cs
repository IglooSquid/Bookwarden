using UnityEngine;
using UnityEngine.InputSystem;

public class TileClickHandler : MonoBehaviour
{
    private LayerMask clickableMask;
    private GameControls controls;

    void Start()
    {
        clickableMask = ~LayerMask.GetMask("Wall");
    }

    void Update()
    {
        if (controls.Gameplay.Select.triggered)
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
                if (props != null && props.isBuilt && props.placedStructure == null)
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

                    PlaceableStructure structure = placed.GetComponentInChildren<PlaceableStructure>();
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

        if (controls.Gameplay.ClearSelection.triggered)
        {
            BuildManager.Instance.ClearSelection();
            Debug.Log("Selection cleared.");
        }
    }

    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new GameControls();
        }
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }
}