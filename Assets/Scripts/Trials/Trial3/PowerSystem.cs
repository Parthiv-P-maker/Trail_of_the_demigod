using UnityEngine;

/// <summary>
/// Tracks the player's currently selected power.
/// Powers map directly to InputManager.SelectedPower (1–5).
/// Attach to the Player GameObject in Trial3_Olympus scene.
/// </summary>
public class PowerSystem : MonoBehaviour
{
    // ── Power definitions ──────────────────────────────────────────────────
    public enum PowerType
    {
        None      = 0,
        Lightning = 1,
        Water     = 2,
        Fire      = 3,
        Shadow    = 4,
        Shield    = 5
    }

    public PowerType CurrentPower { get; private set; } = PowerType.Lightning;

    [Header("UI Colors (optional — tint the player sprite)")]
    public SpriteRenderer playerSprite;
    public Color lightningColor = new Color(1f, 1f, 0f);    // yellow
    public Color waterColor     = new Color(0.2f, 0.6f, 1f); // blue
    public Color fireColor      = new Color(1f, 0.3f, 0f);   // orange
    public Color shadowColor    = new Color(0.4f, 0f, 0.8f); // purple
    public Color shieldColor    = new Color(0f, 1f, 0.5f);   // teal

    // ── Unity lifecycle ────────────────────────────────────────────────────
    private void Update()
    {
        int selected = InputManager.Instance != null
            ? InputManager.Instance.SelectedPower
            : (int)CurrentPower;

        PowerType next = (PowerType)Mathf.Clamp(selected, 1, 5);

        if (next != CurrentPower)
        {
            CurrentPower = next;
            OnPowerChanged();
        }
    }

    // ── Power change ───────────────────────────────────────────────────────
    private void OnPowerChanged()
    {
        Debug.Log($"[PowerSystem] Power changed to: {CurrentPower}");

        if (playerSprite == null) return;

        playerSprite.color = CurrentPower switch
        {
            PowerType.Lightning => lightningColor,
            PowerType.Water     => waterColor,
            PowerType.Fire      => fireColor,
            PowerType.Shadow    => shadowColor,
            PowerType.Shield    => shieldColor,
            _                   => Color.white
        };
    }

    /// <summary>Returns true if this power is effective against the given enemy weakness</summary>
    public bool IsEffectiveAgainst(PowerType enemyWeakness)
    {
        return CurrentPower == enemyWeakness;
    }
}
