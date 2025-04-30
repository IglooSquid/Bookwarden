using UnityEngine;

public class IndicatorController : MonoBehaviour
{
    void Start()
    {
        UpdateIndicators(GameModeManager.Instance.IsInBuildMode());
    }

     private void OnEnable()
    {
        GameModeManager.Instance.OnModeChanged += UpdateIndicators;
        UpdateIndicators(GameModeManager.Instance.IsInBuildMode());
    }

    private void OnDisable()
    {
        if (GameModeManager.Instance != null)
            GameModeManager.Instance.OnModeChanged -= UpdateIndicators;
    }

    public void UpdateIndicators(bool isBuildMode)
    {
        gameObject.SetActive(isBuildMode);
    }
}
