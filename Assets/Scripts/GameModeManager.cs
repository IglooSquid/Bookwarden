using UnityEngine;
using TMPro;

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance;

    public System.Action<bool> OnModeChanged;

    private GameControls controls;

    public enum GameMode
    {
        Play,
        Build

    }

    public GameMode currentMode = GameMode.Play;
    public TextMeshProUGUI modeText;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        controls = new GameControls();
        controls.Gameplay.ToggleBuildMode.performed += ctx => ToggleMode();
        controls.Enable();
    }

    void Start()
    {
        UpdateModeUI();
    }

    void Update()
    {
    }

    public void ToggleMode()
    {
        currentMode = (currentMode == GameMode.Play) ? GameMode.Build : GameMode.Play;
        OnModeChanged?.Invoke(IsInBuildMode());
        BuildManager.Instance?.RefreshAllIndicators();
        UpdateModeUI();
    }

    private void UpdateModeUI()
    {
        if (modeText != null)
        {
            modeText.text = $"Mode: {currentMode}";
        }
    }

    public bool IsInBuildMode()
    {
        return currentMode == GameMode.Build;
    }

    public bool IsInPlayMode()
    {
        return currentMode == GameMode.Play;
    }

    void OnDestroy()
    {
        if (controls != null)
            controls.Disable();
    }
}
