using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Main menu controller.
/// Handles Start Game and input mode selection (Keyboard / CV placeholder).
/// Attach to a GameObject in the MainMenu scene.
/// </summary>
public class MainMenu : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text modeDisplayText;

    private InputManager.InputMode selectedMode = InputManager.InputMode.Keyboard;

    // ── Unity lifecycle ────────────────────────────────────────────────────
    private void Start()
    {
        UpdateModeDisplay();

        // Reset any leftover score from a previous run
        ScoreManager.Instance?.ResetAll();
    }

    // ── Button callbacks (wire to Unity UI Buttons) ────────────────────────

    public void OnStartGame()
    {
        // Apply the selected mode before the game begins
        if (InputManager.Instance != null)
            InputManager.Instance.SetMode(selectedMode);

        SceneManager.LoadScene("Trail1_CoinRush");
    }

    public void OnSelectKeyboard()
    {
        selectedMode = InputManager.InputMode.Keyboard;
        UpdateModeDisplay();
    }

    public void OnSelectCV()
    {
        // CV mode — placeholder until MediaPipe integration is complete
        selectedMode = InputManager.InputMode.CV;
        UpdateModeDisplay();
        Debug.Log("[MainMenu] CV mode selected — ensure Python sidecar is running.");
    }

    // ── Helpers ────────────────────────────────────────────────────────────
    private void UpdateModeDisplay()
    {
        if (modeDisplayText != null)
            modeDisplayText.text = $"Mode: {selectedMode}";
    }
}
