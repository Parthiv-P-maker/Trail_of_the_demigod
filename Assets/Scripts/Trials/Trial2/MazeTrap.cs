using UnityEngine;

/// <summary>
/// Trap tile — teleports the player back to start and deducts timer time.
/// Spawned automatically by MazeGenerator. Reset point is wired by Trial2Manager.
/// Requires: BoxCollider2D with Is Trigger = true (added by MazeGenerator).
/// </summary>
public class MazeTrap : MonoBehaviour
{
    [HideInInspector] public Transform resetPoint;

    [Header("Penalty")]
    public float timePenalty = 5f;

    private bool onCooldown = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (onCooldown) return;
        if (!other.CompareTag("Player")) return;

        // Snap player back to start
        if (resetPoint != null)
            other.transform.position = resetPoint.position;

        // Also reset the MazePlayerController target position
        MazePlayerController mpc = other.GetComponent<MazePlayerController>();
        if (mpc != null)
            mpc.SnapTo(resetPoint.position);

        // Deduct time
        TrialTimer timer = FindFirstObjectByType<TrialTimer>();
        if (timer != null)
            timer.DeductTime(timePenalty);

        Debug.Log($"[MazeTrap] Hit! -{timePenalty}s penalty.");

        // Brief cooldown so it doesn't fire twice on same contact
        onCooldown = true;
        Invoke(nameof(ResetCooldown), 0.5f);
    }

    private void ResetCooldown() => onCooldown = false;
}