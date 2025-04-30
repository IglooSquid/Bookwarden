using UnityEngine;
using UnityEngine.InputSystem;

public class TileClickHandler : MonoBehaviour
{
    private LayerMask clickableMask;
    private GameControls controls;
    private Vector3 lastHoveredTilePosition = Vector3.positiveInfinity;

    void Start()
    {
        clickableMask = ~LayerMask.GetMask("Wall");
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, clickableMask))
        {
            TileProperties props = hit.collider.GetComponentInParent<TileProperties>();
            if (GameModeManager.Instance != null && GameModeManager.Instance.IsInBuildMode() &&
                props != null && props.isBuilt && props.placedStructure == null &&
                BuildManager.Instance != null && BuildManager.Instance.HasSelectedItem())
            {
                Vector3 tilePos = props.transform.position;
                if (tilePos != lastHoveredTilePosition)
                {
                    if (BuildManager.Instance != null &&
                        BuildManager.Instance.HasSelectedItem() &&
                        BuildManager.Instance.selectedItem != null &&
                        BuildManager.Instance.selectedItem.prefab != null)
                    {
                        BuildManager.Instance.ShowGhost(tilePos);
                        lastHoveredTilePosition = tilePos;
                    }
                }
            }
            else
            {
                BuildManager.Instance.HideGhost();
                lastHoveredTilePosition = Vector3.positiveInfinity;
            }
        }
        else
        {
            BuildManager.Instance.HideGhost();
            lastHoveredTilePosition = Vector3.positiveInfinity;
        }

        if (controls.Gameplay.Select.triggered)
        {
            if (GameModeManager.Instance == null || !GameModeManager.Instance.IsInBuildMode())
            {
                return;
            }

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickableMask))
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
                    Quaternion rotation = BuildManager.Instance.GetCurrentGhostRotation();
                    GameObject placed = Instantiate(BuildManager.Instance.selectedItem.prefab, props.transform.position, rotation);
                    placed.transform.SetParent(props.transform, true);

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
            BuildManager.Instance.HideGhost();
            lastHoveredTilePosition = Vector3.positiveInfinity;
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
        controls.Gameplay.RotateItem.started += OnRotateItem;
    }

    private void OnDisable()
    {
        if (controls != null)
        {
            controls.Gameplay.RotateItem.started -= OnRotateItem;
            controls.Gameplay.Disable();
        }
    }

    private void OnRotateItem(InputAction.CallbackContext context)
    {
        if (GameModeManager.Instance != null && GameModeManager.Instance.IsInBuildMode() &&
            BuildManager.Instance != null && BuildManager.Instance.HasSelectedItem())
        {
            BuildManager.Instance.RotateGhost();
            Debug.Log("Rotated ghost preview.");
        }
    }
}