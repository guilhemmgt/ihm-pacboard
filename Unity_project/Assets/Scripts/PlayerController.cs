using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script contrôlant le joueur
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Tooltip("Position actuelle du joueur")]
    public Vector2Int pos;

    [Tooltip("Position de départ du joueur")]
    public Vector2Int startPosition = new Vector2Int(1, 1);

    [Tooltip("Dernière direction du joueur (0: haut, 1: droite, 2: bas, 3: gauche)")]
    public int lastDirection = 0;

    [Tooltip("Flèche indiquant la direction du joueur")]
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
        HandleInput();
    }

    /// <summary>
    /// Gère les entrées clavier, change la direction de la flèche en conséquence
    /// </summary>
    private void HandleInput()
    {
        lastDirection = Game.GetInstance().DirectionI;
        ChangeArrowRotation (Quaternion.Euler (0, 0,
            lastDirection == 0 ? 0 :
            lastDirection == 1 ? -90 :
            lastDirection == 2 ? 180 :
            90));

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
        } else if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.instance.PauseGame();
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
                Game.GetInstance ().SetCoin_CollectedO ();
            });
        }        
    }

    public void Die()
    {
        Debug.Log("Player died");
        Destroy(this.gameObject);

        GameManager.instance.GameOver();
    }
}
