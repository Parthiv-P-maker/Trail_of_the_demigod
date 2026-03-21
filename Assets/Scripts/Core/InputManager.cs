using UnityEngine;

/// <summary>
/// Central input abstraction layer.
/// All gameplay scripts read input from here — never from Input.GetKey directly.
/// To add CV mode later: add a new InputMode enum value and populate the fields
/// in Update() using your CV data source (e.g. a MediaPipe socket listener).
/// </summary>
public class InputManager : MonoBehaviour
{
    // ── Singleton ──────────────────────────────────────────────────────────
    public static InputManager Instance { get; private set; }

    // ── Input Mode ─────────────────────────────────────────────────────────
    public enum InputMode
    {
        Keyboard,
        CV          // Computer Vision — wire up in the CV block below
    }

    [Header("Input Mode")]
    public InputMode currentMode = InputMode.Keyboard;

    // ── Outputs (read these from gameplay scripts) ─────────────────────────

    /// <summary>Horizontal movement: -1 = left, 0 = idle, +1 = right</summary>
    public float Horizontal { get; private set; }

    /// <summary>True on the frame the player triggers an attack</summary>
    public bool AttackPressed { get; private set; }

    /// <summary>Active power slot: 1–5 (0 = none selected yet)</summary>
    public int SelectedPower { get; private set; } = 1;

    // ── Unity lifecycle ────────────────────────────────────────────────────
    private void Awake()
    {
        // Simple singleton — one InputManager lives across scenes
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        switch (currentMode)
        {
            case InputMode.Keyboard:
                ReadKeyboard();
                break;

            case InputMode.CV:
                ReadCV();
                break;
        }
    }

    // ── Keyboard implementation ────────────────────────────────────────────
    private void ReadKeyboard()
    {
        // Horizontal: Arrow keys or A/D
        Horizontal = Input.GetAxisRaw("Horizontal");   // returns -1, 0, or +1

        // Attack: Spacebar
        AttackPressed = Input.GetKeyDown(KeyCode.Space);

        // Power selection: number keys 1–5
        for (int i = 1; i <= 5; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                SelectedPower = i;
            }
        }
    }

    // ── CV stub (implement when ready) ────────────────────────────────────
    /// <summary>
    /// Replace the body of this method with your CV integration.
    /// Suggested approach:
    ///   1. Run MediaPipe hand tracking in a Python sidecar process.
    ///   2. Send gesture data over a localhost UDP socket.
    ///   3. Read the socket here and map gestures to Horizontal / AttackPressed / SelectedPower.
    ///
    /// Example gesture map:
    ///   Open hand left  → Horizontal = -1
    ///   Open hand right → Horizontal = +1
    ///   Fist            → AttackPressed = true
    ///   Fingers 1–5 up  → SelectedPower = finger count
    /// </summary>
    private void ReadCV()
    {
        // TODO: Replace with real CV data
        Horizontal = 0f;
        AttackPressed = false;
        // SelectedPower stays at last value
    }

    // ── Public helpers (optional convenience) ─────────────────────────────

    /// <summary>Switch input mode at runtime (e.g. from a settings menu)</summary>
    public void SetMode(InputMode mode)
    {
        currentMode = mode;
        Debug.Log($"[InputManager] Mode switched to: {mode}");
    }
}