using UnityEngine;

public class SizeHelper : MonoBehaviour
{
    public static SizeHelper instance;

    private float width;
    private float height;

    private void Awake()
    {
        instance = this;

        // Obtenir la taille réelle de la fenêtre à partir des coins du panel
        RectTransform rectTransform = GetComponent<RectTransform>();

        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        width = corners[2].x - corners[0].x;
        Debug.Log("Width: " + width);

        height = corners[2].y - corners[0].y;
        Debug.Log("Height: " + height);

    }

    public float GetWidth()
    {
        return width;
    }

    public float GetHeight()
    {
        return height;
    }
}
