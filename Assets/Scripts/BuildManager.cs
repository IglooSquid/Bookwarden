using UnityEngine;
using System.Collections.Generic;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    [Header("List of Buildable Items")]
    public List<BuildableItem> buildableItems = new List<BuildableItem>();

    [Header("Currently selected buildable item")]
    public BuildableItem selectedItem;

    [Header("Build Preview Ghost Settings")]
    public Material ghostMaterial;
    private GameObject ghostObject;
    private float currentGhostRotationY = 0f;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SelectItem(BuildableItem item)
    {
        HideGhost();
        selectedItem = item;
    }

    public void ClearSelection()
    {
        HideGhost();
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

    public void ShowGhost(Vector3 position)
    {
        if (selectedItem == null || selectedItem.prefab == null || ghostMaterial == null) return;

        if (ghostObject == null)
        {
            ghostObject = Instantiate(selectedItem.prefab);
            SetGhostMaterial(ghostObject);
        }

        ghostObject.transform.position = position;
        ghostObject.transform.rotation = Quaternion.Euler(0f, currentGhostRotationY, 0f);
    }

    private void SetGhostMaterial(GameObject obj)
    {
        foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
        {
            renderer.sharedMaterial = ghostMaterial;
        }
    }

    public void RotateGhost()
    {
        currentGhostRotationY = Mathf.Repeat(currentGhostRotationY + 90f, 360f);
        Debug.Log("Rotating ghost to Y angle: " + currentGhostRotationY);

        if (ghostObject != null)
        {
            ghostObject.transform.rotation = Quaternion.Euler(0f, currentGhostRotationY, 0f);
        }
    }

    public Quaternion GetCurrentGhostRotation()
    {
        return Quaternion.Euler(0f, currentGhostRotationY, 0f);
    }

    public void HideGhost()
    {
        if (ghostObject != null)
            Destroy(ghostObject);
    }
}
