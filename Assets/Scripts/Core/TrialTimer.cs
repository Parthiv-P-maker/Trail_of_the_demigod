using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Generic countdown timer used by all three trials.
/// Fires OnTimerEnd when it reaches zero.
/// Attach to a scene manager GameObject in each trial scene.
/// </summary>
public class TrialTimer : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Trial duration in seconds")]
    public float duration = 30f;

    [Header("Events")]
    [Tooltip("Fires when the timer reaches zero")]
    public UnityEvent OnTimerEnd;

    // ── State ──────────────────────────────────────────────────────────────
    public float TimeRemaining { get; private set; }
    public bool IsRunning { get; private set; }

    // ── Unity lifecycle ────────────────────────────────────────────────────
    private void OnEnable()
    {
        TimeRemaining = duration;
        IsRunning = true;
    }

    private void Update()
    {
        if (!IsRunning) return;

        TimeRemaining -= Time.deltaTime;

        if (TimeRemaining <= 0f)
        {
            TimeRemaining = 0f;
            IsRunning = false;
            OnTimerEnd?.Invoke();
        }
    }

    // ── Public API ─────────────────────────────────────────────────────────
    public void StopTimer() => IsRunning = false;
    public void ResumeTimer() => IsRunning = true;

    /// <summary>Deduct seconds from the timer (used by traps)</summary>
    public void DeductTime(float seconds)
    {
        TimeRemaining = Mathf.Max(0f, TimeRemaining - seconds);
    }

    /// <summary>Formatted string for UI display: "0:29"</summary>
    public string GetFormattedTime()
    {
        int seconds = Mathf.CeilToInt(TimeRemaining);
        int mins = seconds / 60;
        int secs = seconds % 60;
        return $"{mins}:{secs:00}";
    }
}