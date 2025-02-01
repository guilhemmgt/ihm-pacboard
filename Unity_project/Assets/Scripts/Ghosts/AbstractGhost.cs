using DG.Tweening;
using UnityEngine;

public abstract class AbstractGhost : MonoBehaviour
{
    [SerializeField] protected Vector2Int startPosition;
    protected Vector2Int pos;

    [Tooltip("Joueur que doit poursuivre le fantôme")]
    protected PlayerController player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        pos = startPosition;
        transform.position = MapController.instance.GetRealTilePos(startPosition.x, startPosition.y);
        GameManager.instance.OnTurnStart.AddListener(() =>
        {
            Move();
        });
    }

    protected void Move()
    {
        Vector2Int nextPos = ChooseDirection();
        if (nextPos == pos) return;

        MoveDir(nextPos.x, nextPos.y);
    }

    protected void MoveDir(int x, int y)
    {
        if (!MapController.instance.IsWall(x, y))
        {
            transform.DOMove(MapController.instance.GetRealTilePos(x, y), GameManager.instance.GetTimeBetweenPlays() / 2);
            pos = new Vector2Int(x, y);
        }
        else
        {
            Debug.LogError("Can't move to " + x + ", " + y);
        }
    }

    protected abstract Vector2Int ChooseDirection();

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("<color=red>Player touched by ghost</color>");
            collision.gameObject.GetComponent<PlayerController>().Die();
        }
    }
}
