using UnityEngine;
using System.Collections.Generic;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    [Header("List of Buildable Items")]
    public List<BuildableItem> buildableItems = new List<BuildableItem>();

    [Header("Currently selected buildable item")]
    public BuildableItem selectedItem;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SelectItem(BuildableItem item)
    {
        selectedItem = item;
    }

    public void ClearSelection()
    {
        selectedItem = null;
    }  

    public bool HasSelectedItem()
    {
        return selectedItem != null;
    }

    public void RefreshAllIndicators()
    {
        foreach (var indicator in GameObject.FindObjectsOfType<IndicatorController>(true))
        {
            indicator.UpdateIndicators(GameModeManager.Instance.IsInBuildMode());
        }
    }
}
