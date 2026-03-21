using UnityEngine;

/// <summary>
/// Attached to each spawned coin prefab.
/// Falls downward at a given speed and destroys itself when off-screen.
/// Notifies ScoreManager on collection.
/// Requires: Collider2D (set to Is Trigger = true) on the same GameObject.
/// </summary>
public class Coin : MonoBehaviour
{
    [Header("Fall Settings")]
    [Tooltip("Starting fall speed — CoinSpawner can override this at spawn time")]
    public float fallSpeed = 3f;

    [Tooltip("Y position below which the coin is considered off-screen")]
    public float destroyBelowY = -7f;

    [Header("Score")]
    public int scoreValue = 10;

    // ── Unity lifecycle ────────────────────────────────────────────────────
    private void Update()
    {
        // Move straight down every frame
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

        // Self-destruct once below the visible screen
        if (transform.position.y < destroyBelowY)
            Destroy(gameObject);
    }

    // ── Trigger collision ──────────────────────────────────────────────────
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only the player collects coins (tag the player GameObject as "Player")
        if (!other.CompareTag("Player")) return;

        // Add score
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddCoins(scoreValue);

        // Destroy coin on collection
        Destroy(gameObject);
    }
}
