using UnityEngine;
using TMPro;

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance;

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
    }

    void Start()
    {
        UpdateModeUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMode();
        }
    }

    public void ToggleMode()
    {
        currentMode = (currentMode == GameMode.Play) ? GameMode.Build : GameMode.Play;
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
}
