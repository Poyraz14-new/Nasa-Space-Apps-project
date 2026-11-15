using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MissionTracker : MonoBehaviour
{
    private const float ArrivalRadius = 1f;
    private const float MetersPerUnit = 10f;
    private const int ClearingRadius = 2;
    private const int SiteSortingOrder = 2;
    private static readonly string[] BlockingLayers = { "Collision", "Raised", "High", "Props" };

    private Mission mission;
    private Transform player;
    private Transform targetSite;
    private MissionUI.Compass compass;
    private SpriteRenderer beacon;

    private void Start()
    {
        MissionUI.RememberSceneFont();
        mission = MissionManager.CurrentMission;
        player = FindPlayer();
        List<Transform> sites = FindMissionSites();
        if (player == null || sites.Count == 0)
        {
            Debug.LogWarning("MissionTracker: needs an object named 'Player' and children under 'Mission Sites' in this scene.");
            enabled = false;
            return;
        }

        targetSite = sites[Random.Range(0, sites.Count)];
        Dictionary<string, Tilemap> tilemaps = FindTilemaps();
        Sprite siteProp = FindTileSprite(tilemaps.Values, mission.SitePropName);
        ClearAroundSite(targetSite.position, tilemaps);
        CreateSiteProp(siteProp, targetSite.position, mission.SitePropTint);
        beacon = CreateBeacon(targetSite.position);

        Canvas canvas = MissionUI.CreateCanvas("Mission Compass", 50);
        compass = MissionUI.CreateCompass(canvas.transform, 120);
    }

    private void Update()
    {
        if (MissionRun.IsFinished)
        {
            return;
        }

        Vector2 offset = targetSite.position - player.position;
        float distance = offset.magnitude;
        float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
        float snappedAngle = Mathf.Round(angle / 45f) * 45f;
        compass.Arrow.localEulerAngles = new Vector3(0f, 0f, snappedAngle - 90f);
        compass.Label.text = $"{distance * MetersPerUnit:0} m";

        Color beaconColor = beacon.color;
        beaconColor.a = 0.55f + 0.45f * Mathf.Sin(Time.time * 4f);
        beacon.color = beaconColor;

        if (distance <= ArrivalRadius)
        {
            CheckMissionResult();
        }
    }

    private void CheckMissionResult()
    {
        List<string> missingTools = mission.MissingTools();
        if (missingTools.Count == 0)
        {
            MissionRun.Win($"You reached the site with every tool the mission needed.\n\nObjective: {mission.Objective}");
        }
        else
        {
            MissionRun.Lose($"You reached the site, but you forgot: {string.Join(", ", missingTools)}.\nWithout them the samples cannot be collected.");
        }
    }

    private static Dictionary<string, Tilemap> FindTilemaps()
    {
        Dictionary<string, Tilemap> tilemaps = new Dictionary<string, Tilemap>();
        foreach (Tilemap tilemap in FindObjectsByType<Tilemap>(FindObjectsSortMode.None))
        {
            tilemaps[tilemap.gameObject.name] = tilemap;
        }
        return tilemaps;
    }

    private static Sprite FindTileSprite(IEnumerable<Tilemap> tilemaps, string tileName)
    {
        foreach (Tilemap tilemap in tilemaps)
        {
            TileBase[] usedTiles = new TileBase[tilemap.GetUsedTilesCount()];
            tilemap.GetUsedTilesNonAlloc(usedTiles);
            foreach (TileBase tile in usedTiles)
            {
                if (tile is Tile spriteTile && tile.name == tileName)
                {
                    return spriteTile.sprite;
                }
            }
        }
        Debug.LogWarning($"MissionTracker: no tile named '{tileName}' in this scene, so the site has no prop.");
        return null;
    }

    private static void ClearAroundSite(Vector3 sitePosition, Dictionary<string, Tilemap> tilemaps)
    {
        if (!tilemaps.TryGetValue("Collision", out Tilemap collision))
        {
            return;
        }

        Vector3Int siteCell = collision.WorldToCell(sitePosition);
        foreach (string layer in BlockingLayers)
        {
            if (!tilemaps.TryGetValue(layer, out Tilemap tilemap))
            {
                continue;
            }
            for (int x = -ClearingRadius; x <= ClearingRadius; x++)
            {
                for (int y = -ClearingRadius; y <= ClearingRadius; y++)
                {
                    tilemap.SetTile(siteCell + new Vector3Int(x, y, 0), null);
                }
            }
        }

        if (tilemaps.TryGetValue("Decorations", out Tilemap decorations))
        {
            decorations.SetTile(siteCell, null);
        }
    }

    private static void CreateSiteProp(Sprite sprite, Vector3 position, Color tint)
    {
        if (sprite == null)
        {
            return;
        }

        GameObject prop = new GameObject("Mission Site Prop");
        prop.transform.position = position;
        SpriteRenderer renderer = prop.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        renderer.color = tint;
        renderer.sortingOrder = SiteSortingOrder;
    }

    private static Transform FindPlayer()
    {
        GameObject playerObject = GameObject.Find("Player");
        if (playerObject == null)
        {
            playerObject = GameObject.FindWithTag("Player");
        }
        return playerObject != null ? playerObject.transform : null;
    }

    private static List<Transform> FindMissionSites()
    {
        List<Transform> sites = new List<Transform>();
        GameObject container = GameObject.Find("Mission Sites");
        if (container == null)
        {
            return sites;
        }
        foreach (Transform site in container.transform)
        {
            sites.Add(site);
        }
        return sites;
    }

    private static SpriteRenderer CreateBeacon(Vector3 position)
    {
        const int width = 34;
        const int height = 18;
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float diamond = Mathf.Abs(x - (width - 1) / 2f) / (width / 2f) + Mathf.Abs(y - (height - 1) / 2f) / (height / 2f);
                texture.SetPixel(x, y, diamond > 0.78f && diamond <= 1f ? MissionUI.Accent : Color.clear);
            }
        }
        texture.Apply();

        GameObject beaconObject = new GameObject("Mission Beacon");
        beaconObject.transform.position = position;
        SpriteRenderer renderer = beaconObject.AddComponent<SpriteRenderer>();
        renderer.sprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 32f);
        renderer.sortingOrder = SiteSortingOrder - 1;
        return renderer;
    }
}
