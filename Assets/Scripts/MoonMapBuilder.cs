using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Grid))]
public class MoonMapBuilder : MonoBehaviour
{
    [SerializeField] private TextAsset layout;
    [SerializeField] private Transform player;
    [SerializeField] private int seed = 2026;
    [SerializeField] private string tileFolder = "Assets/MoonTiles";

    private class CellStyle
    {
        public string[] ground = { };
        public string[] decoration = { };
        public string[] prop = { };
        public string[] raised = { };
        public string[] high = { };
        public bool blocksMovement;
    }

    private static readonly string[] Floor = Weighted("Slice 10", 24, "Slice 3", 1, "Slice 4", 1);
    private static readonly string[] DarkFloor = { "Slice 36" };

    private static readonly Dictionary<char, CellStyle> Styles = new Dictionary<char, CellStyle>
    {
        ['.'] = new CellStyle { ground = Floor },
        [':'] = new CellStyle { ground = DarkFloor },
        ['P'] = new CellStyle { ground = DarkFloor },
        ['X'] = new CellStyle { ground = DarkFloor },
        ['#'] = new CellStyle { ground = Floor, raised = new[] { "Slice 10", "Slice 10", "Slice 9" }, blocksMovement = true },
        ['^'] = new CellStyle { ground = Floor, raised = new[] { "Slice 3", "Slice 4", "Slice 10" }, blocksMovement = true },
        ['%'] = new CellStyle { ground = Floor, raised = new[] { "Slice 9", "Slice 10" }, high = new[] { "Slice 3", "Slice 4", "Slice 10", "Slice 10" }, blocksMovement = true },
        ['o'] = new CellStyle { ground = Floor, decoration = new[] { "Big_Crater_hit_1", "Big_asteroid_impact_1", "Big_asteroid_impact_2" } },
        ['c'] = new CellStyle { ground = Floor, decoration = new[] { "Small_Crater_hit_1", "Small_Crater_hit_2", "small_asteroid_impact_1", "small_asteroid_impact_2", "medium_asteroid_impact_1" } },
        ['r'] = new CellStyle { ground = Floor, decoration = new[] { "Small_rocks_1", "Small_rocks_2", "Small_rocks_3", "Medium_rocks_1", "Medium_rocks_2", "Medium_rocks_3", "Medium_rocks_4", "Medium_rocks_5", "Medium_rocks_6" } },
        ['R'] = new CellStyle { ground = Floor, prop = new[] { "Big_rocks_1" }, blocksMovement = true },
        ['s'] = new CellStyle { ground = Floor, prop = new[] { "Spike_rock_1", "Spike_rock_2" } },
        ['1'] = new CellStyle { ground = DarkFloor, decoration = new[] { "Big_asteroid_impact_2" } },
    };

    private static string[] Weighted(params object[] namesAndWeights)
    {
        List<string> names = new List<string>();
        for (int i = 0; i < namesAndWeights.Length; i += 2)
        {
            for (int count = 0; count < (int)namesAndWeights[i + 1]; count++)
            {
                names.Add((string)namesAndWeights[i]);
            }
        }
        return names.ToArray();
    }

#if UNITY_EDITOR
    private Tilemap ground;
    private Tilemap decorations;
    private Tilemap props;
    private Tilemap raised;
    private Tilemap high;
    private Tilemap collision;
    private Dictionary<string, Sprite> spritesByName;
    private readonly Dictionary<string, Tile> tilesByName = new Dictionary<string, Tile>();
    private System.Random random;

    [ContextMenu("Build Map")]
    private void BuildMap()
    {
        ResetCaches();
        if (layout == null)
        {
            Debug.LogError("MoonMapBuilder: drag MoonMap.txt into the Layout slot first.");
            return;
        }

        Sprite floorSprite = FindSprite("Slice 10");
        if (floorSprite == null)
        {
            return;
        }

        float pixelsPerUnit = floorSprite.pixelsPerUnit;
        SetUpGrid(pixelsPerUnit);
        SetUpTilemaps(pixelsPerUnit);
        ClearTilemaps();

        random = new System.Random(seed);
        string[] rows = layout.text.Replace("\r", "").TrimEnd('\n').Split('\n');
        Vector2Int spawn = FindSpawn(rows);
        HashSet<Vector3Int> filledCells = new HashSet<Vector3Int>();
        List<(char digit, Vector3Int cell)> missionSites = new List<(char, Vector3Int)>();

        for (int row = 0; row < rows.Length; row++)
        {
            for (int column = 0; column < rows[row].Length; column++)
            {
                char symbol = rows[row][column];
                if (symbol == ' ')
                {
                    continue;
                }

                Vector3Int cell = new Vector3Int(column - spawn.x, (rows.Length - 1 - row) - spawn.y, 0);
                PlaceCell(symbol, cell);
                filledCells.Add(cell);

                if (char.IsDigit(symbol))
                {
                    missionSites.Add((symbol, cell));
                }
            }
        }

        BlockMapEdges(filledCells);
        CreateMissionSiteMarkers(missionSites, pixelsPerUnit);
        MovePlayerToSpawn(pixelsPerUnit);
        ReportOtherColliders();
        AssetDatabase.SaveAssets();
        Debug.Log($"MoonMapBuilder: built {filledCells.Count} cells and {missionSites.Count} mission sites.");
    }

    [ContextMenu("Clear Map")]
    private void ClearMap()
    {
        ResetCaches();
        Sprite floorSprite = FindSprite("Slice 10");
        SetUpTilemaps(floorSprite != null ? floorSprite.pixelsPerUnit : 32f);
        ClearTilemaps();
    }

    private void ResetCaches()
    {
        spritesByName = null;
        tilesByName.Clear();
    }

    private void PlaceCell(char symbol, Vector3Int cell)
    {
        CellStyle style = char.IsDigit(symbol) ? Styles['1'] : null;
        if (style == null && !Styles.TryGetValue(symbol, out style))
        {
            Debug.LogWarning($"MoonMapBuilder: unknown symbol '{symbol}' at {cell}, placed floor instead.");
            style = Styles['.'];
        }

        SetRandomTile(ground, cell, style.ground);
        SetRandomTile(decorations, cell, style.decoration);
        bool hasProp = SetRandomTile(props, cell, style.prop);
        bool hasRaised = SetRandomTile(raised, cell, style.raised);
        SetRandomTile(high, cell, style.high);

        if (style.blocksMovement && (hasProp || hasRaised))
        {
            collision.SetTile(cell, GetCollisionTile());
        }
    }

    private bool SetRandomTile(Tilemap tilemap, Vector3Int cell, string[] spriteNames)
    {
        if (spriteNames.Length == 0)
        {
            return false;
        }

        string spriteName = spriteNames[random.Next(spriteNames.Length)];
        TileBase tile = GetTile(spriteName);
        tilemap.SetTile(cell, tile);
        return tile != null;
    }

    private void BlockMapEdges(HashSet<Vector3Int> filledCells)
    {
        foreach (Vector3Int cell in filledCells)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    Vector3Int neighbor = cell + new Vector3Int(dx, dy, 0);
                    if (!filledCells.Contains(neighbor))
                    {
                        collision.SetTile(neighbor, GetCollisionTile());
                    }
                }
            }
        }
    }

    private void ReportOtherColliders()
    {
        foreach (Collider2D other in FindObjectsByType<Collider2D>(FindObjectsSortMode.None))
        {
            bool isMapCollision = other.gameObject == collision.gameObject;
            bool isPlayer = player != null && other.transform.IsChildOf(player);
            if (!isMapCollision && !isPlayer)
            {
                Debug.LogWarning($"MoonMapBuilder: '{other.name}' has a {other.GetType().Name} that can block the player. " +
                                 "Click this message to select it, and delete it if it isn't meant to be there.", other);
            }
        }
    }

    private Vector2Int FindSpawn(string[] rows)
    {
        for (int row = 0; row < rows.Length; row++)
        {
            int column = rows[row].IndexOf('X');
            if (column >= 0)
            {
                return new Vector2Int(column, rows.Length - 1 - row);
            }
        }

        Debug.LogWarning("MoonMapBuilder: no 'X' spawn in the layout, using the bottom-left corner.");
        return Vector2Int.zero;
    }

    private Vector3 TopOfBlock(Vector3Int cell, float pixelsPerUnit)
    {
        return ground.GetCellCenterWorld(cell) + Vector3.up * (8f / pixelsPerUnit);
    }

    private void CreateMissionSiteMarkers(List<(char digit, Vector3Int cell)> missionSites, float pixelsPerUnit)
    {
        Transform container = transform.Find("Mission Sites");
        if (container != null)
        {
            Undo.DestroyObjectImmediate(container.gameObject);
        }

        container = new GameObject("Mission Sites").transform;
        container.SetParent(transform, false);
        Undo.RegisterCreatedObjectUndo(container.gameObject, "Create Mission Sites");

        foreach ((char digit, Vector3Int cell) in missionSites)
        {
            GameObject marker = new GameObject($"Mission Site {digit}");
            marker.transform.SetParent(container, false);
            marker.transform.position = TopOfBlock(cell, pixelsPerUnit);
        }
    }

    private void MovePlayerToSpawn(float pixelsPerUnit)
    {
        if (player == null)
        {
            return;
        }

        Undo.RecordObject(player, "Move Player To Spawn");
        Vector3 spawnPosition = TopOfBlock(Vector3Int.zero, pixelsPerUnit);
        player.position = new Vector3(spawnPosition.x, spawnPosition.y, player.position.z);
    }

    private void SetUpGrid(float pixelsPerUnit)
    {
        Grid grid = GetComponent<Grid>();
        Undo.RecordObject(grid, "Set Up Moon Grid");
        grid.cellLayout = GridLayout.CellLayout.Isometric;
        grid.cellSize = new Vector3(32f / pixelsPerUnit, 16f / pixelsPerUnit, 1f);
    }

    private void SetUpTilemaps(float pixelsPerUnit)
    {
        ground = GetOrCreateTilemap("Ground", 0, 0f);
        decorations = GetOrCreateTilemap("Decorations", 1, 8f / pixelsPerUnit);
        props = GetOrCreateTilemap("Props", 2, 8f / pixelsPerUnit);
        raised = GetOrCreateTilemap("Raised", 2, 16f / pixelsPerUnit);
        high = GetOrCreateTilemap("High", 3, 32f / pixelsPerUnit);
        collision = GetOrCreateTilemap("Collision", 0, 8f / pixelsPerUnit);

        collision.GetComponent<TilemapRenderer>().enabled = false;
        if (collision.GetComponent<TilemapCollider2D>() == null)
        {
            Undo.AddComponent<TilemapCollider2D>(collision.gameObject);
        }
    }

    private Tilemap GetOrCreateTilemap(string tilemapName, int sortingOrder, float heightOffset)
    {
        Transform child = transform.Find(tilemapName);
        if (child == null)
        {
            child = new GameObject(tilemapName, typeof(Tilemap), typeof(TilemapRenderer)).transform;
            child.SetParent(transform, false);
            Undo.RegisterCreatedObjectUndo(child.gameObject, "Create " + tilemapName);
        }

        Tilemap tilemap = child.GetComponent<Tilemap>();
        TilemapRenderer tilemapRenderer = child.GetComponent<TilemapRenderer>();
        Undo.RecordObjects(new Object[] { child, tilemap, tilemapRenderer }, "Set Up " + tilemapName);

        child.localPosition = new Vector3(0f, heightOffset, 0f);
        tilemap.tileAnchor = new Vector3(0.5f, 0.5f, 0f);
        tilemapRenderer.mode = TilemapRenderer.Mode.Individual;
        tilemapRenderer.sortingOrder = sortingOrder;
        return tilemap;
    }

    private void ClearTilemaps()
    {
        Tilemap[] tilemaps = { ground, decorations, props, raised, high, collision };
        Undo.RegisterCompleteObjectUndo(tilemaps, "Clear Moon Map");
        foreach (Tilemap tilemap in tilemaps)
        {
            tilemap.ClearAllTiles();
        }
    }

    private Tile GetTile(string spriteName)
    {
        if (tilesByName.TryGetValue(spriteName, out Tile cachedTile))
        {
            return cachedTile;
        }

        string path = $"{tileFolder}/{spriteName}.asset";
        Tile tile = AssetDatabase.LoadAssetAtPath<Tile>(path);
        if (tile == null)
        {
            Sprite sprite = FindSprite(spriteName);
            if (sprite == null)
            {
                tilesByName[spriteName] = null;
                return null;
            }

            tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
            tile.colliderType = Tile.ColliderType.None;
            CreateTileFolder();
            AssetDatabase.CreateAsset(tile, path);
        }

        tilesByName[spriteName] = tile;
        return tile;
    }

    private Tile GetCollisionTile()
    {
        const string collisionName = "Collision";
        if (tilesByName.TryGetValue(collisionName, out Tile cachedTile))
        {
            return cachedTile;
        }

        string path = $"{tileFolder}/{collisionName}.asset";
        Tile tile = AssetDatabase.LoadAssetAtPath<Tile>(path);
        if (tile == null)
        {
            tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = FindSprite("Slice 10");
            tile.colliderType = Tile.ColliderType.Grid;
            CreateTileFolder();
            AssetDatabase.CreateAsset(tile, path);
        }

        tilesByName[collisionName] = tile;
        return tile;
    }

    private Sprite FindSprite(string spriteName)
    {
        if (spritesByName == null)
        {
            spritesByName = new Dictionary<string, Sprite>();
            foreach (string guid in AssetDatabase.FindAssets("t:Sprite"))
            {
                foreach (Object asset in AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GUIDToAssetPath(guid)))
                {
                    if (asset is Sprite sprite && !spritesByName.ContainsKey(SimplifyName(sprite.name)))
                    {
                        spritesByName.Add(SimplifyName(sprite.name), sprite);
                    }
                }
            }
        }

        if (spritesByName.TryGetValue(SimplifyName(spriteName), out Sprite found))
        {
            return found;
        }

        Debug.LogError($"MoonMapBuilder: no sprite named '{spriteName}' among the {spritesByName.Count} sprites in the project. " +
                       "Select the pack's PNGs and check Texture Type is 'Sprite (2D and UI)' and Sprite Mode is 'Single'.");
        return null;
    }

    private static string SimplifyName(string spriteName)
    {
        if (spriteName.EndsWith("_0"))
        {
            spriteName = spriteName.Substring(0, spriteName.Length - 2);
        }
        return spriteName.Replace(" ", "").Replace("_", "").ToLowerInvariant();
    }

    private void CreateTileFolder()
    {
        if (!AssetDatabase.IsValidFolder(tileFolder))
        {
            string parent = System.IO.Path.GetDirectoryName(tileFolder).Replace('\\', '/');
            string folderName = System.IO.Path.GetFileName(tileFolder);
            AssetDatabase.CreateFolder(parent, folderName);
        }
    }
#endif
}