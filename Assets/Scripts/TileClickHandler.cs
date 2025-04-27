using UnityEngine;

public class TileClickHandler : MonoBehaviour
{
    private LayerMask clickableMask;
    public GameObject buildablePrefab;

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
                    Debug.Log("Placing object");
                    GameObject placed = Instantiate(buildablePrefab, props.transform.position, Quaternion.identity);
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