using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private GameControls controls;
    private Camera mainCam;
    private LayerMask tileLayerMask;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        controls = new GameControls();
        tileLayerMask = LayerMask.GetMask("Tile");
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Gameplay.Select.performed += OnSelectPerformed;
    }

    private void OnDisable()
    {
        controls.Gameplay.Select.performed -= OnSelectPerformed;
        controls.Disable();
    }

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void OnSelectPerformed(InputAction.CallbackContext context)
    {
        if (GameModeManager.Instance == null || !GameModeManager.Instance.IsInPlayMode())
            return;

        Ray ray = mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, tileLayerMask))
        {
            TileProperties tile = hit.collider.GetComponentInParent<TileProperties>();
            if (tile != null && tile.isBuilt)
            {
                agent.SetDestination(tile.transform.position);
            }
        }
    }
}