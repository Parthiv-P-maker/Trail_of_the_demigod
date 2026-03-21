using UnityEngine;

/// <summary>
/// Reads a MazeData array and builds the entire maze as GameObjects at runtime.
/// No Tilemap, no hand-painting. Just attach this to an empty GO and hit Play.
/// Walls get solid BoxCollider2D on the "Wall" layer.
/// Goal gets a trigger + MazeGoal script.
/// Traps get a trigger + MazeTrap script.
/// </summary>
public class MazeGenerator : MonoBehaviour
{
    [Header("Cell Size (match this to MazePlayerController.cellSize)")]
    public float cellSize = 1f;

    [Header("Maze Selection (-1 = random each run)")]
    public int mazeIndex = -1;

    [Header("Colors (no sprites needed — colored squares are auto-created)")]
    public Color wallColor = new Color(0.15f, 0.10f, 0.25f);
    public Color floorColor = new Color(0.88f, 0.83f, 0.68f);
    public Color goalColor = new Color(0.10f, 0.90f, 0.40f);
    public Color trapColor = new Color(0.90f, 0.20f, 0.20f);
    public Color startColor = new Color(0.30f, 0.60f, 1.00f);

    // Read by Trial2Manager after Awake
    [HideInInspector] public Vector3 playerStartPosition;
    [HideInInspector] public Vector3 goalWorldPosition;

    private Transform mazeRoot;

    // ── Unity lifecycle ────────────────────────────────────────────────────
    private void Awake()
    {
        BuildMaze();
    }

    // ── Main build ─────────────────────────────────────────────────────────
    public void BuildMaze()
    {
        // Pick which maze to use
        if (mazeIndex < 0)
            mazeIndex = Random.Range(0, MazeData.AllMazes.Length);
        else
            mazeIndex = Mathf.Clamp(mazeIndex, 0, MazeData.AllMazes.Length - 1);

        int[,] maze = MazeData.AllMazes[mazeIndex];
        int rows = maze.GetLength(0);   // 11
        int cols = maze.GetLength(1);   // 13

        // Centre the whole maze on world origin
        float startX = -(cols * cellSize) / 2f + cellSize / 2f;
        float startY = (rows * cellSize) / 2f - cellSize / 2f;

        // Parent GO keeps hierarchy clean
        mazeRoot = new GameObject("_MazeTiles").transform;
        mazeRoot.SetParent(transform);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                Vector3 pos = new Vector3(startX + c * cellSize, startY - r * cellSize, 0f);
                int cell = maze[r, c];

                switch (cell)
                {
                    case 0: MakeTile("Floor", pos, floorColor); break;
                    case 1: MakeWall(pos); break;
                    case 2: MakeTile("Start", pos, startColor); playerStartPosition = pos; break;
                    case 3: MakeGoal(pos); break;
                    case 4: MakeTrap(pos); break;
                }
            }
        }

        Debug.Log($"[MazeGenerator] Maze {mazeIndex} built. Start={playerStartPosition} Goal={goalWorldPosition}");
    }

    // ── Tile builders ──────────────────────────────────────────────────────
    private void MakeWall(Vector3 pos)
    {
        GameObject go = MakeTile("Wall", pos, wallColor);
        go.layer = LayerMask.NameToLayer("Wall");

        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.size = Vector2.one * cellSize;
    }

    private void MakeGoal(Vector3 pos)
    {
        goalWorldPosition = pos;
        MakeTile("Floor", pos, floorColor);          // floor underneath

        GameObject go = MakeTile("Goal", pos, goalColor);
        go.transform.localScale = Vector3.one * 0.85f;

        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.size = Vector2.one * 0.85f;
        col.isTrigger = true;

        go.AddComponent<MazeGoal>();
    }

    private void MakeTrap(Vector3 pos)
    {
        MakeTile("Floor", pos, floorColor);          // floor underneath

        GameObject go = MakeTile("Trap", pos, trapColor);
        go.transform.localScale = Vector3.one * 0.65f;

        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.size = Vector2.one * 0.65f;
        col.isTrigger = true;

        go.AddComponent<MazeTrap>();
        // Reset point is wired by Trial2Manager after build
    }

    private GameObject MakeTile(string tileName, Vector3 pos, Color color)
    {
        GameObject go = new GameObject(tileName);
        go.transform.SetParent(mazeRoot);
        go.transform.position = pos;
        go.transform.localScale = Vector3.one * cellSize;

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = GetDefaultSprite();
        sr.color = color;

        return go;
    }

    // Generates a 1x1 white sprite at runtime — no asset needed
    private Sprite GetDefaultSprite()
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }

    // ── Public helpers ─────────────────────────────────────────────────────
    public MazeTrap[] GetAllTraps()
    {
        if (mazeRoot == null)
        {
            Debug.LogWarning("[MazeGenerator] mazeRoot is null — BuildMaze may not have run.");
            return new MazeTrap[0];
        }
        var traps = mazeRoot.GetComponentsInChildren<MazeTrap>();
        return traps ?? new MazeTrap[0];
    }

    public void DestroyMaze()
    {
        if (mazeRoot != null)
            Destroy(mazeRoot.gameObject);
    }
}