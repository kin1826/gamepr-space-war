using UnityEngine;

public class TileGridSpawner : MonoBehaviour
{
    public GameObject tilePrefab;

    [Header("Grid Size")]
    public int width = 10;
    public int height = 10;

    [Header("Tile Size")]
    public float tileSize = 1f;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 pos = new Vector3(
                    x * tileSize,
                    0,
                    z * tileSize
                );

                Instantiate(tilePrefab, pos, Quaternion.identity, transform);
            }
        }
    }
}