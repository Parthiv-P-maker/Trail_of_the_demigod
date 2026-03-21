using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Goal tile — fires OnGoalReached when player touches it.
/// Spawned automatically by MazeGenerator.
/// Requires: BoxCollider2D with Is Trigger = true (added by MazeGenerator).
/// </summary>
public class MazeGoal : MonoBehaviour
{
    public UnityEvent OnGoalReached;

    private bool reached = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (reached) return;
        if (!other.CompareTag("Player")) return;

        reached = true;
        Debug.Log("[MazeGoal] Player reached the goal!");
        OnGoalReached?.Invoke();
    }
}