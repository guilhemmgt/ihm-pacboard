using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using com.cyborgAssets.inspectorButtonPro;

public class TilesToMatrixConverter : MonoBehaviour
{
    public static TilesToMatrixConverter instance;
    public Tilemap tilemap;
    public Tile pathTile;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            Debug.LogWarning("Il y a déjà une instance de TilesToMatrixConverter");
        }
    }

    public float GetTileSize()
    {
        return tilemap.cellSize.x;
    }

    private void Start()
    {
        int[,] map = Convert(tilemap);
        ShowMatrix(map);
        MapController.instance.SetMap(map);
    }

    [ProButton]
    public void SaveMatrix(string fileName)
    {
        int[,] map = Convert(tilemap);
        SaveMatrix(map, fileName);
    }

    /// <summary>
    /// Observe les tuiles du Tilemap et affecte 0 si c'est une pathTile, 1 sinon
    /// </summary>
    /// <param name="tilemap"></param>
    /// <returns></returns>
    public int[,] Convert(Tilemap tilemap)
    {
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
        int[,] map = new int[bounds.size.x, bounds.size.y];
        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                map[x, y] = tile == pathTile ? 0 : 1;
            }
        }
        return map;
    }

    public void ShowMatrix(int[,] map)
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            string line = "";
            for (int y = 0; y < map.GetLength(1); y++)
            {
                line += map[x, y] + "|";
            }
            Debug.Log(line);
        }
    }

    /// <summary>
    /// Sauvegarde la matrice dans un fichier texte vers Assets/Matrix
    /// </summary>
    /// <param name="map"></param>
    public void SaveMatrix(int[,] map, string fileName)
    {
        if (!Directory.Exists(Application.dataPath + "/Matrix"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Matrix");
        }

        string path = Application.dataPath + "/Matrix/" + fileName + ".txt";
        string content = "";

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                content += map[x, y] + "|";
            }
            content += "\n";
        }
        File.WriteAllText(path, content);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    public int[,] ReadMatrixFile(string fileName)
    {
        string path = Application.dataPath + "/Matrix/" + fileName + ".txt";
        string content = File.ReadAllText(path);
        string[] lines = content.Split('\n');

        int[,] map = new int[lines.Length - 1, lines[0].Split('|').Length - 1];
        for (int x = 0; x < map.GetLength(0); x++)
        {
            string[] values = lines[x].Split('|');
            for (int y = 0; y < map.GetLength(1); y++)
            {
                map[x, y] = int.Parse(values[y]);
            }
        }
        return map;
    }
}
