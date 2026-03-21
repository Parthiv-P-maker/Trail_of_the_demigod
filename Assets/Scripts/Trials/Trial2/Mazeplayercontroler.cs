using UnityEngine;

/// <summary>
/// Grid-locked top-down player movement for Trial 2.
/// Player slides from cell to cell — no free movement.
/// Reads horizontal from InputManager; reads vertical directly (extend InputManager later).
/// Requires: Rigidbody2D (Kinematic), BoxCollider2D on same GameObject.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class MazePlayerController : MonoBehaviour
{
    [Header("Must match MazeGenerator.cellSize")]
    public float cellSize = 1f;
    public float moveSpeed = 10f;

    [Header("Wall detection layer")]
    public LayerMask wallLayer;

    // ── State ──────────────────────────────────────────────────────────────
    private Rigidbody2D rb;
    private Vector2 targetPos;
    private bool isMoving;
    private Vector2 inputDir;
    private float inputTimer;
    private const float INPUT_COOLDOWN = 0.12f;

    // ── Unity lifecycle ────────────────────────────────────────────────────
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        targetPos = SnapToGrid(transform.position);
        rb.position = targetPos;
    }

    private void Update()
    {
        inputTimer -= Time.deltaTime;
        ReadInput();
    }

    private void FixedUpdate()
    {
        if (isMoving)
            SlideToTarget();
        else
            TryMove(inputDir);
    }

    // ── Input ──────────────────────────────────────────────────────────────
    private void ReadInput()
    {
        if (inputTimer > 0f) return;

        float h = InputManager.Instance != null ? InputManager.Instance.Horizontal : Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector2 dir = Vector2.zero;
        if (Mathf.Abs(h) > 0.5f) dir = new Vector2(Mathf.Sign(h), 0f);
        else if (Mathf.Abs(v) > 0.5f) dir = new Vector2(0f, Mathf.Sign(v));

        if (dir != Vector2.zero)
        {
            inputDir = dir;
            inputTimer = INPUT_COOLDOWN;
        }
    }

    // ── Movement ───────────────────────────────────────────────────────────
    private void TryMove(Vector2 dir)
    {
        if (dir == Vector2.zero) return;

        Vector2 next = targetPos + dir * cellSize;

        if (IsWall(next)) return;

        targetPos = next;
        isMoving = true;
        inputDir = Vector2.zero;
    }

    private void SlideToTarget()
    {
        Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        if (Vector2.Distance(newPos, targetPos) < 0.01f)
        {
            rb.MovePosition(targetPos);
            isMoving = false;
        }
    }

    private bool IsWall(Vector2 pos)
    {
        return Physics2D.OverlapCircle(pos, cellSize * 0.35f, wallLayer) != null;
    }

    private Vector2 SnapToGrid(Vector3 pos)
    {
        return new Vector2(
            Mathf.Round(pos.x / cellSize) * cellSize,
            Mathf.Round(pos.y / cellSize) * cellSize
        );
    }

    // ── Public — called by MazeTrap ────────────────────────────────────────
    /// <summary>Hard-snaps player to a position and clears movement state.</summary>
    public void SnapTo(Vector3 pos)
    {
        targetPos = SnapToGrid(pos);
        transform.position = targetPos;
        rb.position = targetPos;
        isMoving = false;
        inputDir = Vector2.zero;
    }
}