using UnityEngine;

/// <summary>
/// Base enemy class for Trial 3: Wrath of Olympus.
/// Moves toward the player, has a weakness to one power type.
/// Takes double damage when hit with its weakness power.
/// Requires: Rigidbody2D, Collider2D (Is Trigger = false) on this GameObject.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 3;
    public float moveSpeed = 2f;

    [Header("Weakness")]
    [Tooltip("Which power kills this enemy in one hit")]
    public PowerSystem.PowerType weakness = PowerSystem.PowerType.Lightning;

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    [Tooltip("Color tint to show this enemy's weakness type")]
    public Color weaknessColor = Color.yellow;

    // ── Private state ──────────────────────────────────────────────────────
    private int currentHealth;
    private Rigidbody2D rb;
    private Transform playerTarget;

    // ── Unity lifecycle ────────────────────────────────────────────────────
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        currentHealth = maxHealth;

        if (spriteRenderer != null)
            spriteRenderer.color = weaknessColor;
    }

    private void Start()
    {
        // Find the player by tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTarget = player.transform;
    }

    private void FixedUpdate()
    {
        ChasePlayer();
    }

    // ── AI ─────────────────────────────────────────────────────────────────
    private void ChasePlayer()
    {
        if (playerTarget == null) return;

        Vector2 direction = ((Vector2)playerTarget.position - rb.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }

    // ── Combat ─────────────────────────────────────────────────────────────
    /// <summary>Called by the attack system when the player hits this enemy</summary>
    public void TakeHit(PowerSystem.PowerType attackPower)
    {
        int damage = (attackPower == weakness) ? currentHealth : 1; // weakness = instant kill
        currentHealth -= damage;

        // Flash red on hit
        if (spriteRenderer != null)
            StartCoroutine(FlashRed());

        Debug.Log($"[Enemy] Hit with {attackPower} (weakness: {weakness}). HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        ScoreManager.Instance?.AddEnemyKill();
        Destroy(gameObject);
    }

    private System.Collections.IEnumerator FlashRed()
    {
        Color original = spriteRenderer.color;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.12f);
        spriteRenderer.color = original;
    }

    // ── Player collision ───────────────────────────────────────────────────
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Enemy touching the player deals damage to trial manager
        if (other.CompareTag("Player"))
        {
            FindFirstObjectByType<Trial3Manager>()?.OnPlayerHit();
        }
    }
}