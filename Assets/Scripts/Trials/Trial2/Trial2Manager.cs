using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Scene controller for Trial 2: Athena's Labyrinth.
/// Fully null-safe — logs exactly what is missing.
/// </summary>
public class Trial2Manager : MonoBehaviour
{
    [Header("References — drag these in the Inspector")]
    public MazeGenerator mazeGenerator;
    public TrialTimer timer;
    public GameObject playerObject;

    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text statusText;
    public TMP_Text mazeNameText;

    [Header("Scene Flow")]
    public string nextSceneName = "Trial3_Olympus";
    public float transitionDelay = 2.5f;

    private bool trialEnded = false;

    private void Start()
    {
        // ── Null checks first — stop and log exactly what's missing ────────
        if (mazeGenerator == null)
        {
            Debug.LogError("[Trial2Manager] mazeGenerator is NULL. Drag MazeGenerator GO into the slot.");
            return;
        }
        if (timer == null)
        {
            Debug.LogError("[Trial2Manager] timer is NULL. Drag TrialTimer GO into the slot.");
            return;
        }
        if (playerObject == null)
        {
            Debug.LogError("[Trial2Manager] playerObject is NULL. Drag Player GO into the slot.");
            return;
        }

        // ── Hide status UI ─────────────────────────────────────────────────
        if (statusText != null)
            statusText.gameObject.SetActive(false);

        // ── 1. Move player to maze start ───────────────────────────────────
        playerObject.transform.position = mazeGenerator.playerStartPosition;

        MazePlayerController mpc = playerObject.GetComponent<MazePlayerController>();
        if (mpc != null)
            mpc.SnapTo(mazeGenerator.playerStartPosition);
        else
            Debug.LogWarning("[Trial2Manager] MazePlayerController not found on Player GO.");

        // ── 2. Wire trap reset points ──────────────────────────────────────
        MazeTrap[] traps = mazeGenerator.GetAllTraps();
        if (traps != null && traps.Length > 0)
        {
            GameObject resetGO = new GameObject("_TrapResetPoint");
            resetGO.transform.position = mazeGenerator.playerStartPosition;

            foreach (MazeTrap trap in traps)
            {
                if (trap != null)
                    trap.resetPoint = resetGO.transform;
            }
        }
        else
        {
            Debug.Log("[Trial2Manager] No traps found in this maze (that's OK).");
        }

        // ── 3. Find MazeGoal and subscribe ────────────────────────────────
        MazeGoal goal = FindFirstObjectByType<MazeGoal>();
        if (goal != null)
            goal.OnGoalReached.AddListener(OnGoalReached);
        else
            Debug.LogWarning("[Trial2Manager] MazeGoal not found. Did MazeGenerator run in Awake?");

        // ── 4. Subscribe to timer ──────────────────────────────────────────
        timer.OnTimerEnd.AddListener(OnTimeUp);

        // ── 5. Maze label ──────────────────────────────────────────────────
        if (mazeNameText != null)
            mazeNameText.text = $"Maze {mazeGenerator.mazeIndex + 1} / {MazeData.AllMazes.Length}";
    }

    private void Update()
    {
        if (timerText != null && timer != null)
            timerText.text = timer.GetFormattedTime();
    }

    // ── Events ─────────────────────────────────────────────────────────────
    private void OnGoalReached()
    {
        if (trialEnded) return;
        trialEnded = true;

        timer.StopTimer();
        ScoreManager.Instance?.SetMazeTime(timer.TimeRemaining);

        int bonus = Mathf.RoundToInt(timer.TimeRemaining * 20);
        ShowStatus($"Escaped!\n+{bonus} pts");
        Invoke(nameof(LoadNext), transitionDelay);
    }

    private void OnTimeUp()
    {
        if (trialEnded) return;
        trialEnded = true;

        ScoreManager.Instance?.SetMazeTime(0f);
        ShowStatus("Time's up!");
        Invoke(nameof(LoadNext), transitionDelay);
    }

    private void LoadNext() => SceneManager.LoadScene(nextSceneName);

    private void ShowStatus(string msg)
    {
        if (statusText == null) return;
        statusText.gameObject.SetActive(true);
        statusText.text = msg;
    }
}