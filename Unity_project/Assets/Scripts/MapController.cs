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
    private GridLayoutGroup gridLayoutGroup;

    [Header("Map Settings")]
    [SerializeField] private int width = 28;
    [SerializeField] private int height = 31;
    [SerializeField] private bool isRandom = true;

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

    // Matrice représentant la carte
    public int[,] map = new int[10, 10];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
                    // On ne peut mettre des murs que sur les lignes paires
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

            GenerateMap();
        }
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
    }

    public bool IsWall(int x, int y)
    {
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


}
