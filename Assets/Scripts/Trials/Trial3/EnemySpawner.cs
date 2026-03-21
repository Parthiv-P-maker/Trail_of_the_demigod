using UnityEngine;
using System.Collections;

/// <summary>
/// Spawns enemies in waves for Trial 3: Wrath of Olympus.
/// Each wave adds more enemies with randomised weakness types.
/// Attach to an empty GameObject in Trial3_Olympus scene.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [Tooltip("One prefab per weakness type — Lightning, Water, Fire, Shadow, Shield")]
    public GameObject[] enemyPrefabs;   // Index 0=Lightning 1=Water 2=Fire 3=Shadow 4=Shield

    [Header("Spawn Settings")]
    public float spawnRadius     = 9f;   // How far from centre enemies appear
    public float timeBetweenWaves = 6f;
    public int   enemiesPerWave   = 4;
    public int   maxWaves         = 5;

    [Header("Difficulty scaling")]
    [Tooltip("Additional enemies added per wave")]
    public int enemiesPerWaveIncrease = 1;
    [Tooltip("Extra move speed added per wave")]
    public float speedIncreasePerWave = 0.3f;

    // ── Private state ──────────────────────────────────────────────────────
    private int currentWave = 0;

    // ── Unity lifecycle ────────────────────────────────────────────────────
    private void OnEnable()
    {
        StartCoroutine(SpawnWaves());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    // ── Wave spawning ──────────────────────────────────────────────────────
    private IEnumerator SpawnWaves()
    {
        while (currentWave < maxWaves)
        {
            yield return new WaitForSeconds(currentWave == 0 ? 1f : timeBetweenWaves);

            SpawnWave(currentWave);
            currentWave++;
        }
    }

    private void SpawnWave(int waveIndex)
    {
        int count = enemiesPerWave + waveIndex * enemiesPerWaveIncrease;
        float speedBonus = waveIndex * speedIncreasePerWave;

        Debug.Log($"[EnemySpawner] Wave {waveIndex + 1} — spawning {count} enemies");

        for (int i = 0; i < count; i++)
        {
            SpawnEnemy(speedBonus);
        }
    }

    private void SpawnEnemy(float speedBonus)
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;

        // Random position around the arena edge
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector3 spawnPos = new Vector3(
            Mathf.Cos(angle) * spawnRadius,
            Mathf.Sin(angle) * spawnRadius,
            0f
        );

        // Pick a random enemy type
        int typeIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject prefab = enemyPrefabs[typeIndex];
        if (prefab == null) return;

        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);

        // Apply speed scaling
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
            enemyScript.moveSpeed += speedBonus;
    }

    // ── Public control ─────────────────────────────────────────────────────
    public void StopSpawning()
    {
        StopAllCoroutines();
        enabled = false;
    }
}
