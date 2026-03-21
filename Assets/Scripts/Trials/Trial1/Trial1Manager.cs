using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Scene controller for Trial 1: Hermes Coin Rush.
/// Attach to an empty "TrialManager" GameObject in Trial1_CoinRush scene.
/// Wires together the timer, spawner, and UI.
/// </summary>
public class Trial1Manager : MonoBehaviour
{
    [Header("References")]
    public TrialTimer    timer;
    public CoinSpawner   spawner;

    [Header("UI (TextMeshPro)")]
    [Tooltip("Large centre-screen score label")]
    public TMP_Text scoreText;
    [Tooltip("Timer label — top of screen")]
    public TMP_Text timerText;
    [Tooltip("Brief end-of-trial message")]
    public TMP_Text endMessage;

    [Header("Scene Transition")]
    [Tooltip("Name of the next scene to load when trial ends")]
    public string nextSceneName = "Trial2_Labyrinth";
    [Tooltip("Delay (seconds) before loading next scene")]
    public float transitionDelay = 2.5f;

    // ── Unity lifecycle ────────────────────────────────────────────────────
    private void Start()
    {
        if (endMessage != null)
            endMessage.gameObject.SetActive(false);

        // Hook the timer's end event
        timer.OnTimerEnd.AddListener(OnTrialEnd);

        UpdateScoreUI();
    }

    private void Update()
    {
        // Refresh timer display every frame
        if (timerText != null && timer != null)
            timerText.text = timer.GetFormattedTime();

        UpdateScoreUI();
    }

    // ── Events ─────────────────────────────────────────────────────────────
    private void OnTrialEnd()
    {
        // Stop spawning new coins
        if (spawner != null)
            spawner.StopSpawning();

        // Show end message
        if (endMessage != null)
        {
            endMessage.gameObject.SetActive(true);
            endMessage.text = $"Trial Complete!\nCoins: {ScoreManager.Instance?.CoinsCollected}";
        }

        // Load next trial after a short delay
        Invoke(nameof(LoadNextScene), transitionDelay);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    // ── Helpers ────────────────────────────────────────────────────────────
    private void UpdateScoreUI()
    {
        if (scoreText != null && ScoreManager.Instance != null)
            scoreText.text = $"Coins: {ScoreManager.Instance.CoinsCollected}";
    }
}
