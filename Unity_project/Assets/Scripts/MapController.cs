using com.cyborgAssets.inspectorButtonPro;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    public static MapController instance;

    [Header("Prefabs")]
    public SpriteRenderer wallSprite;
    public SpriteRenderer fruitSprite;
    public SpriteRenderer emptySprite;

    private float step = 1.0f;

    [Header("Map Settings")]
    [SerializeField] private int width = 28;
    [SerializeField] private int height = 31;
    [SerializeField] private bool isRandom = true;
    [SerializeField] private bool shouldGenerate = false;

    // Matrice représentant la carte
    public int[,] map = new int[10, 10];

    private float tileSize;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(gameObject);
            Debug.LogWarning("Il y a déjà une instance de MapController");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tileSize = TilesToMatrixConverter.instance.GetTileSize();

        if (!shouldGenerate) return;

        map = new int[10, 10]
        {
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 1},
            {1, 0, 1, 1, 0, 1, 1, 1, 0, 1},
            {1, 2, 0, 0, 0, 1, 0, 1, 0, 1},
            {1, 0, 1, 0, 0, 1, 0, 0, 0, 1},
            {1, 0, 1, 1, 0, 0, 0, 1, 0, 1},
            {1, 0, 0, 0, 1, 0, 1, 1, 0, 1},
            {1, 0, 1, 1, 1, 0, 2, 0, 0, 1},
            {1, 0, 0, 0, 0, 0, 0, 1, 0, 1},
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
        };

        if (isRandom)
        {
            GenerateRandomMap(width, height);
        }
        else
        {
            GenerateMap();
        }
    }

    [Obsolete("Ne permet pas de réaliser de bonnes maps random, juste pour tester")]
    /// <summary>
    /// Méthode temporaire pour générer une map aléatoire
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void GenerateRandomMap(int width, int height)
    {
        map = new int[width, height];
        Debug.Log("Map size: " + width + "x" + height);

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                // Si on est sur le bord de la map, on met un mur
                if (x == 0 || y == 0 || x == map.GetLength(0) - 1 || y == map.GetLength(1) - 1)
                {
                    map[x, y] = 1;
                }
                else
                {
                    // On ne peut mettre des murs que sur les colonnes paires
                    if (x % 2 == 0)
                    {
                        map[x, y] = UnityEngine.Random.Range(0, 2);
                    }
                    else
                    {
                        map[x, y] = UnityEngine.Random.Range(0, 1) * 2;
                    }
                }
            }
        }

        GenerateMap();
    }

    [ProButton]
    public void LoadFromFile(string name)
    {
        // On véfifie que le fichier existe
        string path = Application.dataPath + "/Matrix/" + name + ".txt";
        if (!System.IO.File.Exists(path))
        {
            Debug.LogError("Le fichier n'existe pas");
            return;
        }

        map = TilesToMatrixConverter.instance.ReadMatrixFile(name);
        GenerateMap();
    }

    /// <summary>
    /// Méthode pour s'assurer que la map reste dans les limites de l'écran
    /// </summary>
    public float GetBestStep()
    {
        float stepX = SizeHelper.instance.GetWidth() / map.GetLength(0);
        float stepY = SizeHelper.instance.GetHeight() / map.GetLength(1);

        return Mathf.Min(stepX, stepY);
    }

    [ProButton]
    public void GenerateMap()
    {
        step = GetBestStep();
        Debug.Log("Step: " + step);

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (IsWall(x, y))
                {
                    SpriteRenderer clone = Instantiate(wallSprite, new Vector3(x * step, y * step, 0), Quaternion.identity, this.transform);
                    clone.name = "Wall_" + x + "_" + y;
                    clone.transform.localScale = new Vector3(step, step, 1);
                }
                else if (IsFruit(x, y))
                {
                    SpriteRenderer clone = Instantiate(fruitSprite, new Vector3(x * step, y * step, 0), Quaternion.identity, this.transform);
                    clone.name = "Fruit_" + x + "_" + y;
                    clone.transform.localScale = new Vector3(step, step, 1);
                }
                else
                {
                    SpriteRenderer clone = Instantiate(emptySprite, new Vector3(x * step, y * step, 0), Quaternion.identity, this.transform);
                    clone.name = "Empty_" + x + "_" + y;
                    clone.transform.localScale = new Vector3(step, step, 1);
                }
            }
        }

        // On place le centre en bas à gauche
        float height = SizeHelper.instance.GetHeight();
        float width = SizeHelper.instance.GetWidth();

        transform.position = new Vector3(-width/4, -height/2 + step/2, 0);
    }

    [ProButton]
    public void ClearMap()
    {
        // On détruit tous les enfants de la map
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        transform.position = Vector3.zero;
    }

    public bool IsWall(int x, int y)
    {
        if ( x >= map.GetLength(0) - 1 || x < 0 || y >= map.GetLength(1) || y < 0)
        {
            return true;
        }
        return map[x, y] == 1;
    }

    public bool IsFruit(int x, int y)
    {
        return map[x, y] == 2;
    }

    public List<Vector2> GetPossiblePaths(int x, int y)
    {
        List<Vector2> possiblePaths = new List<Vector2>();

        if (!IsWall(x + 1, y))
        {
            possiblePaths.Add(new Vector2(x + 1, y));
        }
        if (!IsWall(x - 1, y))
        {
            possiblePaths.Add(new Vector2(x - 1, y));
        }
        if (!IsWall(x, y + 1))
        {
            possiblePaths.Add(new Vector2(x, y + 1));
        }
        if (!IsWall(x, y - 1))
        {
            possiblePaths.Add(new Vector2(x, y - 1));
        }
        return possiblePaths;
    }

    /// <summary>
    /// Obtenir la position d'une tuile en fonction de ses coordonnées
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public Vector3 GetRealTilePos(int x, int y)
    {
        Vector3 offset = new Vector3(height * step / 2, width * step / 2 + 3*step/4, 0);
        return new Vector3(x * step, y * step, 0) - offset;
    }

    /// <summary>
    /// Retourne une liste de toutes les positions des tuiles étant des chemins
    /// </summary>
    /// <returns></returns>
    public List<Vector3> GetAllPathPosition()
    {
        List<Vector3> pathPos = new List<Vector3>();

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (!IsWall(x, y))
                {
                    pathPos.Add(GetRealTilePos(x, y));
                }
            }
        }

        return pathPos;
    }

    public void SetMap(int[,] newMap)
    {
        map = newMap;
    }


}
