using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Results screen shown after all three trials.
/// Reads scores from ScoreManager and displays the combined total.
/// Attach to a GameObject in the Results scene.
/// </summary>
public class ResultsScreen : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text coinScoreText;
    public TMP_Text mazeScoreText;
    public TMP_Text combatScoreText;
    public TMP_Text totalScoreText;
    public TMP_Text gradeText;

    [Header("Navigation")]
    public string mainMenuScene = "MainMenu";

    // ── Unity lifecycle ────────────────────────────────────────────────────
    private void Start()
    {
        DisplayScores();
    }

    // ── Display ────────────────────────────────────────────────────────────
    private void DisplayScores()
    {
        if (ScoreManager.Instance == null) return;

        int coinPts   = ScoreManager.Instance.CoinsCollected * 10;
        int mazePts   = Mathf.RoundToInt(ScoreManager.Instance.MazeTimeRemaining * 20);
        int combatPts = ScoreManager.Instance.EnemiesDefeated * 50;
        int total     = ScoreManager.Instance.TotalScore;

        if (coinScoreText   != null) coinScoreText.text   = $"Coin Rush:  {coinPts} pts";
        if (mazeScoreText   != null) mazeScoreText.text   = $"Labyrinth:  {mazePts} pts";
        if (combatScoreText != null) combatScoreText.text = $"Olympus:    {combatPts} pts";
        if (totalScoreText  != null) totalScoreText.text  = $"TOTAL: {total}";
        if (gradeText       != null) gradeText.text       = GetGrade(total);
    }

    private string GetGrade(int score)
    {
        if (score >= 2000) return "S — Demigod!";
        if (score >= 1500) return "A — Champion";
        if (score >= 1000) return "B — Warrior";
        if (score >= 500)  return "C — Initiate";
        return "D — Try again";
    }

    // ── Buttons ────────────────────────────────────────────────────────────
    public void OnPlayAgain()
    {
        ScoreManager.Instance?.ResetAll();
        SceneManager.LoadScene("Trial1_CoinRush");
    }

    public void OnMainMenu()
    {
        ScoreManager.Instance?.ResetAll();
        SceneManager.LoadScene(mainMenuScene);
    }
}
