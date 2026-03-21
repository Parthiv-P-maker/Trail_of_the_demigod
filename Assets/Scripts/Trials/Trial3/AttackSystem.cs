using UnityEngine;

/// <summary>
/// Handles player attacks in Trial 3.
/// Uses an instant radial hit (no projectile) for simplicity and projector clarity.
/// Reads attack input from InputManager and applies the active power from PowerSystem.
/// Attach to the Player GameObject alongside PowerSystem.
/// </summary>
[RequireComponent(typeof(PowerSystem))]
public class AttackSystem : MonoBehaviour
{
    [Header("Attack Settings")]
    [Tooltip("Radius of the attack hit zone around the player")]
    public float attackRadius = 2.5f;

    [Tooltip("Layer mask for enemies")]
    public LayerMask enemyLayer;

    [Tooltip("Cooldown between attacks in seconds")]
    public float attackCooldown = 0.4f;

    [Header("Visual feedback")]
    [Tooltip("Optional ring effect GameObject — enable briefly on attack")]
    public GameObject attackRingEffect;

    // ── Private state ──────────────────────────────────────────────────────
    private PowerSystem powerSystem;
    private float cooldownTimer = 0f;

    // ── Unity lifecycle ────────────────────────────────────────────────────
    private void Awake()
    {
        powerSystem = GetComponent<PowerSystem>();

        if (attackRingEffect != null)
            attackRingEffect.SetActive(false);
    }

    private void Update()
    {
        cooldownTimer -= Time.deltaTime;

        bool attackPressed = InputManager.Instance != null
            ? InputManager.Instance.AttackPressed
            : Input.GetKeyDown(KeyCode.Space);

        if (attackPressed && cooldownTimer <= 0f)
        {
            PerformAttack();
            cooldownTimer = attackCooldown;
        }
    }

    // ── Attack ─────────────────────────────────────────────────────────────
    private void PerformAttack()
    {
        // Find all enemies in radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            attackRadius,
            enemyLayer
        );

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            enemy?.TakeHit(powerSystem.CurrentPower);
        }

        // Show ring effect briefly
        if (attackRingEffect != null)
            StartCoroutine(ShowRing());

        Debug.Log($"[AttackSystem] Attacked with {powerSystem.CurrentPower}. Hit {hits.Length} enemies.");
    }

    private System.Collections.IEnumerator ShowRing()
    {
        attackRingEffect.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        attackRingEffect.SetActive(false);
    }

    // ── Gizmo (visible in Scene view for radius tuning) ───────────────────
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
