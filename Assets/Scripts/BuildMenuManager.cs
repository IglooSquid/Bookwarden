using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class BuildMenuManager : MonoBehaviour
{
    public GameObject buildMenuPanel;
    public Button buildButton;
    public Transform itemButtonContainer;
    public GameObject buildItemButtonPrefab;
    public TextMeshProUGUI selectedPrefabText;

    private bool menuOpen = false;

private void OnEnable()
    {
        UpdateVisibilityBasedOnMode();
    }

    void Start()
    {
        buildButton.onClick.AddListener(ToggleBuildMenu);
        PopulateBuildMenu();
        buildMenuPanel.SetActive(false);
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            BuildManager.Instance.ClearSelection();
            selectedPrefabText.text = "Selected Prefab: None";
            buildMenuPanel.SetActive(false);
            menuOpen = false;
        }

        UpdateVisibilityBasedOnMode();
    }

    private void UpdateVisibilityBasedOnMode()
    {
        if (GameModeManager.Instance != null)
        {
            bool isBuildMode = GameModeManager.Instance.IsInBuildMode();
            buildButton.gameObject.SetActive(isBuildMode);
            if (!isBuildMode)
            {
                buildMenuPanel.SetActive(false);
                menuOpen = false;
            }
        }
    }

    private void ToggleBuildMenu()
    {
        menuOpen = !menuOpen;
        buildMenuPanel.SetActive(menuOpen);
    }

    private void PopulateBuildMenu()
    {
        foreach (var item in BuildManager.Instance.buildableItems)
        {
            if (item.isUnlocked)
            {
                GameObject buttonObj = Instantiate(buildItemButtonPrefab, itemButtonContainer);
                Button button = buttonObj.GetComponent<Button>();
                TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = item.itemName;

                button.onClick.AddListener(() => {
                    BuildManager.Instance.SelectItem(item);
                    selectedPrefabText.text = $"Selected Prefab: {item.itemName}";
                });
            }
        }
    }
}
