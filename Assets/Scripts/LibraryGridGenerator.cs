using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class LibraryGridGenerator : MonoBehaviour
{
    private static LibraryGridGenerator instance;

    public GameObject gridTilePrefab;
    public GameObject wallPrefab;
    public Material defaultUnbuiltMaterial;
    public Material defaultBuiltMaterial;
    public Material defaultExpansionMaterial;
    public bool showExpansionZones = true;
    
    private Transform wallsParent;
    private Dictionary<Vector2Int, GameObject> tileMap = new Dictionary<Vector2Int, GameObject>();
    private int chunkSize = 5;
    public static event System.Action OnGridReady;

    void Start()
    {
        instance = this;

        Vector2Int origin = Vector2Int.zero;

        // Place initial built chunk
        PlaceChunk(origin, true);

        if (showExpansionZones)
        {
            Vector2Int[] directions = new Vector2Int[]
            {
                new Vector2Int(0, chunkSize),   // North
                new Vector2Int(0, -chunkSize),  // South
                new Vector2Int(chunkSize, 0),   // East
                new Vector2Int(-chunkSize, 0)   // West
            };

            foreach (var dir in directions)
            {
                Vector2Int newChunkOrigin = origin + dir;
                PlaceChunk(newChunkOrigin, false);
            }
        }

        wallsParent = new GameObject("WallsRoot").transform;
        wallsParent.parent = transform;
        PlaceWallsForBuiltTiles();

        GetComponent<NavMeshSurface>().BuildNavMesh();

        OnGridReady?.Invoke();
    }

    void PlaceChunk(Vector2Int origin, bool isBuilt)
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                Vector2Int tileCoord = new Vector2Int(origin.x + x, origin.y + z);

                if (tileMap.ContainsKey(tileCoord))
                    continue;

                Vector3 spawnPos = new Vector3(tileCoord.x, 0, tileCoord.y);
                GameObject tile = Instantiate(gridTilePrefab, spawnPos, Quaternion.identity, transform);
                tile.name = $"Tile_{tileCoord.x}_{tileCoord.y}";
                tileMap.Add(tileCoord, tile);

                var tileProps = tile.GetComponent<TileProperties>();
                if (tileProps != null)
                {
                    tileProps.isBuilt = isBuilt;
                    tileProps.isExpansionZone = !isBuilt;
                    tileProps.occupancy = isBuilt ? TileProperties.TileOccupancy.RoomChunk : TileProperties.TileOccupancy.Empty;

                    var rend = tile.GetComponent<Renderer>();
                    if (rend != null)
                    {
                        if (isBuilt && defaultBuiltMaterial != null)
                            rend.material = defaultBuiltMaterial;
                        else if (!isBuilt && defaultExpansionMaterial != null)
                            rend.material = defaultExpansionMaterial;
                        else if (defaultUnbuiltMaterial != null)
                            rend.material = defaultUnbuiltMaterial;
                    }
                    if (!isBuilt)
                    {
                        tile.AddComponent<ExpansionTileController>();
                    }
                }
            }
        }
    }

    void PlaceWallsForBuiltTiles()
    {
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1),   // North
            new Vector2Int(1, 0),   // East
            new Vector2Int(0, -1),  // South
            new Vector2Int(-1, 0)   // West
        };

        foreach (var kvp in tileMap)
        {
            Vector2Int tileCoord = kvp.Key;
            TileProperties tileProps = kvp.Value.GetComponent<TileProperties>();

            if (tileProps == null || !tileProps.isBuilt)
                continue;

            foreach (var dir in directions)
            {
                Vector2Int neighborCoord = tileCoord + dir;

                // If there's no tile at the neighbor, or it's not built, we need a wall
                if (!tileMap.ContainsKey(neighborCoord) || 
                    !(tileMap[neighborCoord].GetComponent<TileProperties>()?.isBuilt ?? false))
                {
                    // Place wall at the midpoint between the tile and its neighbor
                    Vector3 wallPos = new Vector3(
                        tileCoord.x + dir.x * 0.5f,
                        1f, // since wall is 2 units tall
                        tileCoord.y + dir.y * 0.5f
                    );

                    Quaternion rotation = Quaternion.identity;
                    if (dir.x != 0) rotation = Quaternion.Euler(0, 90, 0); // rotate for east/west walls

                    GameObject wall = Instantiate(wallPrefab, wallPos, rotation, wallsParent);
                    
                    WallController controller = wall.GetComponent<WallController>();
                    if (controller != null)
                    {
                    }
                }
            }
        }
    }

    public static bool TryGetTile(Vector2Int coord, out GameObject tile)
    {
        if (instance != null && instance.tileMap.TryGetValue(coord, out tile))
            return true;

        tile = null;
        return false;
    }
}
