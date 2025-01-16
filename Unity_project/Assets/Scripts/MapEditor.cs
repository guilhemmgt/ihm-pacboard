using UnityEngine;

/// <summary>
/// Classe permettant de gérer l'édition de la carte, n'est pas encore utilisée
/// </summary>
public class MapEditor : MonoBehaviour
{
    public static MapEditor instance;
    public int[,] map;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            Debug.LogWarning("Il y a déjà une instance de MapEditor");
        }
    }

    public void SetSize(int width, int height)
    {
        map = new int[width, height];
    }

    public void SetTile(int x, int y, int value)
    {
        if (map == null)
        {
            Debug.LogError("La carte n'a pas été initialisée");
            return;
        }

        if (x < 0 || x >= map.GetLength(0) || y < 0 || y >= map.GetLength(1))
        {
            Debug.LogError("Coordonnées invalides");
            return;
        }

        map[x, y] = value;
    }
}
