using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public Vector2Int pos;

    public Vector2Int startPosition = new Vector2Int(1, 1);

    public int lastDirection = 0;

    [SerializeField] private Image arrowObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pos = startPosition;
        transform.position = MapController.instance.GetRealTilePos(startPosition.x, startPosition.y);
        GameManager.instance.OnTurnStart.AddListener(() =>
        {
            TryMove(lastDirection);
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            lastDirection = 0;
            ChangeArrowRotation(Quaternion.Euler(0, 0, 0));
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            lastDirection = 1;
            ChangeArrowRotation(Quaternion.Euler(0, 0, -90));
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            lastDirection = 2;
            ChangeArrowRotation(Quaternion.Euler(0, 0, 180));
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            lastDirection = 3;
            ChangeArrowRotation(Quaternion.Euler(0, 0, 90));
        }
    }

    public void ChangeArrowRotation(Quaternion rotation)
    {
        arrowObject.transform.rotation = rotation;
    }

    public void TryMove(int dir)
    {
        if (dir == 0) // Up
        {
            MoveDir(pos.x, pos.y + 1);
        }
        else if (dir == 1) // Right
        {
            MoveDir(pos.x + 1, pos.y);
        }
        else if (dir == 2) // Down
        {
            MoveDir(pos.x, pos.y - 1);
        }
        else if (dir == 3) // Left
        {
            MoveDir(pos.x - 1, pos.y);
        }
    }

    private void MoveDir(int x, int y)
    {
        if (!MapController.instance.IsWall(x,y))
        {
            transform.DOMove(MapController.instance.GetRealTilePos(x, y), GameManager.instance.GetTimeBetweenPlays()/2);
            // transform.position = MapController.instance.GetRealTilePos(x, y);

            pos = new Vector2Int(x, y);
            Debug.Log("Player moved to " + pos);
        }
        else
        {
            Debug.Log("Can't move to " + x + ", " + y);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out ICollectible component))
        {
            component.Collect(this.gameObject, () =>
            {
                Debug.Log("Player collected a collectible");
            });
        }        
    }
}
