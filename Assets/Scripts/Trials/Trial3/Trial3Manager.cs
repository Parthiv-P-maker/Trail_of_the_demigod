using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Scene controller for Trial 3: Wrath of Olympus.
/// Manages player health, wave tracking, UI, and scene transition.
/// Attach to an empty "TrialManager" GameObject in Trial3_Olympus scene.
/// </summary>
public class Trial3Manager : MonoBehaviour
{
    [Header("References")]
    public TrialTimer    timer;
    public EnemySpawner  spawner;

    [Header("Player Health")]
    public int playerMaxHealth = 5;

    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text killCountText;
    public TMP_Text powerText;
    public TMP_Text healthText;
    public TMP_Text endMessage;

    [Header("Power Names")]
    private readonly string[] powerNames = { "", "Lightning", "Water", "Fire", "Shadow", "Shield" };

    [Header("Scene Transition")]
    public string nextSceneName   = "Results";
    public float  transitionDelay = 2.5f;

    // ── Private state ──────────────────────────────────────────────────────
    private int  playerHealth;
    private bool trialEnded = false;
    private PowerSystem powerSystem;

    // ── Unity lifecycle ────────────────────────────────────────────────────
    private void Start()
    {
        playerHealth = playerMaxHealth;

        if (endMessage != null)
            endMessage.gameObject.SetActive(false);

        timer.OnTimerEnd.AddListener(OnTrialEnd);

        // Find power system on player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            powerSystem = player.GetComponent<PowerSystem>();

        UpdateUI();
    }

    private void Update()
    {
        if (timerText != null)
            timerText.text = timer.GetFormattedTime();

        if (killCountText != null && ScoreManager.Instance != null)
            killCountText.text = $"Kills: {ScoreManager.Instance.EnemiesDefeated}";

        if (powerText != null && powerSystem != null)
        {
            int p = (int)powerSystem.CurrentPower;
            powerText.text = p >= 1 && p <= 5 ? powerNames[p] : "";
        }
    }

    // ── Public callbacks ───────────────────────────────────────────────────

    /// <summary>Called by Enemy when it touches the player</summary>
    public void OnPlayerHit()
    {
        if (trialEnded) return;

        playerHealth--;
        UpdateUI();
        Debug.Log($"[Trial3Manager] Player hit! Health: {playerHealth}/{playerMaxHealth}");

        if (playerHealth <= 0)
            OnTrialEnd();
    }

    // ── Trial end ──────────────────────────────────────────────────────────
    private void OnTrialEnd()
    {
        if (trialEnded) return;
        trialEnded = true;

        spawner?.StopSpawning();
        timer.StopTimer();

        int kills = ScoreManager.Instance?.EnemiesDefeated ?? 0;
        ShowEnd($"Trial Over!\n{kills} enemies defeated");

        Invoke(nameof(LoadNext), transitionDelay);
    }

    private void LoadNext() => SceneManager.LoadScene(nextSceneName);

    // ── UI ─────────────────────────────────────────────────────────────────
    private void UpdateUI()
    {
        if (healthText != null)
            healthText.text = $"HP: {playerHealth}/{playerMaxHealth}";
    }

    private void ShowEnd(string msg)
    {
        if (endMessage == null) return;
        endMessage.gameObject.SetActive(true);
        endMessage.text = msg;
    }
}
