using UnityEngine;

/// <summary>
/// Free top-down movement for Trial 3: Wrath of Olympus.
/// Reads from InputManager. Clamped within the arena boundary.
/// Requires: Rigidbody2D on the same GameObject.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class ArenaPlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Arena Bounds")]
    public float boundaryRadius = 8.5f;

    // ── Private refs ───────────────────────────────────────────────────────
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    // ── Unity lifecycle ────────────────────────────────────────────────────
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        rb.gravityScale = 0f;
        rb.constraints  = RigidbodyConstraints2D.FreezeRotation;
    }

    private void FixedUpdate()
    {
        Move();
        ClampToBoundary();
    }

    // ── Movement ───────────────────────────────────────────────────────────
    private void Move()
    {
        float h = InputManager.Instance != null ? InputManager.Instance.Horizontal : 0f;
        float v = Input.GetAxisRaw("Vertical"); // vertical read directly — can extend InputManager later

        Vector2 dir = new Vector2(h, v).normalized;
        rb.linearVelocity = dir * moveSpeed;

        // Flip sprite based on horizontal direction
        if (sr != null && h != 0f)
            sr.flipX = h < 0f;
    }

    private void ClampToBoundary()
    {
        if (rb.position.magnitude > boundaryRadius)
        {
            rb.position = rb.position.normalized * boundaryRadius;
            rb.linearVelocity = Vector2.zero;
        }
    }
}
