using UnityEngine;

public class ExpansionTileController : MonoBehaviour
{
    private Renderer tileRenderer;

    private void Awake()
    {
        tileRenderer = GetComponent<Renderer>();
    }

    private void OnEnable()
    {
        GameModeManager.Instance.OnModeChanged += UpdateVisibility;
        UpdateVisibility(GameModeManager.Instance.IsInBuildMode());
    }

    private void OnDisable()
    {
        if (GameModeManager.Instance != null)
            GameModeManager.Instance.OnModeChanged -= UpdateVisibility;
    }

    private void UpdateVisibility(bool isBuildMode)
    {
        if (tileRenderer != null)
        {
            tileRenderer.enabled = isBuildMode;
        }
    }
}