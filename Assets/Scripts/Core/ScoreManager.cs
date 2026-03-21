using UnityEngine;

/// <summary>
/// Tracks scores across all three trials.
/// Persists across scene loads so the final score screen can read all values.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    // ── Singleton ──────────────────────────────────────────────────────────
    public static ScoreManager Instance { get; private set; }

    // ── Score data ─────────────────────────────────────────────────────────
    public int CoinsCollected { get; private set; }
    public float MazeTimeRemaining { get; private set; }
    public int EnemiesDefeated { get; private set; }

    /// <summary>Combined final score shown on results screen</summary>
    public int TotalScore =>
        (CoinsCollected * 10) +
        (Mathf.RoundToInt(MazeTimeRemaining) * 20) +
        (EnemiesDefeated * 50);

    // ── Unity lifecycle ────────────────────────────────────────────────────
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        ResetAll();
    }

    // ── Public API ─────────────────────────────────────────────────────────

    // Trial 1
    public void AddCoins(int amount)
    {
        CoinsCollected += amount;
        Debug.Log($"[Score] Coins: {CoinsCollected}");
    }

    // Trial 2
    public void SetMazeTime(float secondsRemaining)
    {
        MazeTimeRemaining = secondsRemaining;
        Debug.Log($"[Score] Maze time remaining: {MazeTimeRemaining:F1}s");
    }

    // Trial 3
    public void AddEnemyKill()
    {
        EnemiesDefeated++;
        Debug.Log($"[Score] Enemies defeated: {EnemiesDefeated}");
    }

    /// <summary>Call this when starting a fresh run</summary>
    public void ResetAll()
    {
        CoinsCollected = 0;
        MazeTimeRemaining = 0f;
        EnemiesDefeated = 0;
    }
}