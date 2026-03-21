using UnityEngine;

/// <summary>
/// Moves the player left and right based on InputManager.
/// Attach to the Player GameObject in Trial1_CoinRush scene.
/// Requires: Rigidbody2D component on the same GameObject.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("How fast the player moves horizontally (units/sec)")]
    public float moveSpeed = 6f;

    [Header("Screen Bounds")]
    [Tooltip("How far left/right the player can travel (world units from centre)")]
    public float horizontalBoundary = 8f;

    // ── Private refs ───────────────────────────────────────────────────────
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    // ── Unity lifecycle ────────────────────────────────────────────────────
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        // Lock rotation so the player never tips over
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // No gravity needed — player stays on a fixed row
        rb.gravityScale = 0f;
    }

    private void FixedUpdate()
    {
        Move();
        ClampPosition();
    }

    // ── Movement ───────────────────────────────────────────────────────────
    private void Move()
    {
        // Read from InputManager — never from Input.GetKey directly
        float h = InputManager.Instance != null ? InputManager.Instance.Horizontal : 0f;

        rb.linearVelocity = new Vector2(h * moveSpeed, 0f);

        // Flip sprite to face movement direction
        if (sr != null && h != 0f)
            sr.flipX = h < 0f;
    }

    private void ClampPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -horizontalBoundary, horizontalBoundary);
        transform.position = pos;
    }
}
