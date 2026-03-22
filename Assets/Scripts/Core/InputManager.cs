using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class InputManager : MonoBehaviour
{
    // ── Singleton ─────────────────────────────────────────
    public static InputManager Instance { get; private set; }

    // ── Input Mode ────────────────────────────────────────
    public enum InputMode
    {
        Keyboard,
        CV
    }

    [Header("Input Mode")]
    public InputMode currentMode = InputMode.Keyboard;

    // ── Outputs (used by gameplay) ───────────────────────
    public float Horizontal { get; private set; }
    public bool AttackPressed { get; private set; }
    public int SelectedPower { get; private set; } = 1;

    // ── UDP CV Receiver ──────────────────────────────────
    UdpClient client;
    IPEndPoint endPoint;

    // ── Unity Lifecycle ──────────────────────────────────
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (currentMode == InputMode.CV)
        {
            client = new UdpClient(5052);
            endPoint = new IPEndPoint(IPAddress.Any, 0);
            Debug.Log("CV Mode Active — Listening for UDP data...");
        }
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

    // ── Keyboard Controls ────────────────────────────────
    private void ReadKeyboard()
    {
        Horizontal = Input.GetAxisRaw("Horizontal");
        AttackPressed = Input.GetKeyDown(KeyCode.Space);

        for (int i = 1; i <= 5; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                SelectedPower = i;
        }
    }

    // ── CV Controls via Python OpenCV ────────────────────
    private void ReadCV()
    {
        if (client != null && client.Available > 0)
        {
            byte[] data = client.Receive(ref endPoint);
            string message = Encoding.UTF8.GetString(data);

            float value;
            if (float.TryParse(message, out value))
            {
                value = Mathf.Clamp(value, -1f, 1f);

                // Dead zone to prevent jitter
                if (Mathf.Abs(value) < 0.15f)
                    value = 0f;

                Horizontal = value;
            }
        }

        AttackPressed = false;
    }

    // ── Utility ──────────────────────────────────────────
    public void SetMode(InputMode mode)
    {
        currentMode = mode;

        if (mode == InputMode.CV && client == null)
        {
            client = new UdpClient(5052);
            endPoint = new IPEndPoint(IPAddress.Any, 0);
            Debug.Log("Switched to CV Mode — UDP Ready");
        }

        Debug.Log("Input Mode: " + mode);
    }
}