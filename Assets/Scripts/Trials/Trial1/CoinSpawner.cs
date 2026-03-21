using UnityEngine;

/// <summary>
/// Spawns coins at random horizontal positions above the screen.
/// Difficulty ramps up over time: spawn rate increases, coins fall faster.
/// Attach to an empty GameObject in the Trial1_CoinRush scene.
/// </summary>
public class CoinSpawner : MonoBehaviour
{
    [Header("Coin Prefab")]
    [Tooltip("Drag the Coin prefab here in the Inspector")]
    public GameObject coinPrefab;

    [Header("Spawn Area")]
    [Tooltip("Horizontal range for spawn positions")]
    public float spawnMinX = -8f;
    public float spawnMaxX =  8f;
    [Tooltip("Y position where coins appear (just above the camera top)")]
    public float spawnY = 6f;

    [Header("Timing (seconds)")]
    [Tooltip("Interval between spawns at start")]
    public float initialSpawnInterval = 1.2f;
    [Tooltip("Minimum spawn interval (fastest difficulty)")]
    public float minSpawnInterval = 0.35f;
    [Tooltip("How quickly the interval shrinks over time")]
    public float difficultyRampRate = 0.05f;   // seconds removed per second elapsed

    [Header("Speed")]
    public float initialFallSpeed = 3f;
    public float maxFallSpeed = 7f;

    // ── Private state ──────────────────────────────────────────────────────
    private float currentInterval;
    private float timer;
    private float elapsed;

    // ── Unity lifecycle ────────────────────────────────────────────────────
    private void OnEnable()
    {
        currentInterval = initialSpawnInterval;
        timer = 0f;
        elapsed = 0f;
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        timer   += Time.deltaTime;

        // Ramp difficulty over time
        currentInterval = Mathf.Max(
            minSpawnInterval,
            initialSpawnInterval - elapsed * difficultyRampRate
        );

        if (timer >= currentInterval)
        {
            SpawnCoin();
            timer = 0f;
        }
    }

    // ── Spawning ───────────────────────────────────────────────────────────
    private void SpawnCoin()
    {
        if (coinPrefab == null) return;

        float x = Random.Range(spawnMinX, spawnMaxX);
        Vector3 spawnPos = new Vector3(x, spawnY, 0f);

        GameObject coin = Instantiate(coinPrefab, spawnPos, Quaternion.identity);

        // Scale fall speed with difficulty
        Coin coinScript = coin.GetComponent<Coin>();
        if (coinScript != null)
        {
            float t = Mathf.InverseLerp(initialSpawnInterval, minSpawnInterval, currentInterval);
            coinScript.fallSpeed = Mathf.Lerp(initialFallSpeed, maxFallSpeed, t);
        }
    }

    // ── Public control ─────────────────────────────────────────────────────
    /// <summary>Call this to pause spawning (e.g. when trial ends)</summary>
    public void StopSpawning() => enabled = false;
    public void StartSpawning() => enabled = true;
}
