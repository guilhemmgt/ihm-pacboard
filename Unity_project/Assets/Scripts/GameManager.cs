using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        PLAYING,
        PAUSED,
        GAME_OVER
    }

    public GameState gameState = GameState.PAUSED;

    public static GameManager instance;

    private IEnumerator coroutine;

    [SerializeField] private float timeBetweenPlays = 0.5f;

    public UnityEvent OnTurnStart = new UnityEvent();

    [SerializeField] private ChargingBar chargingBar;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            Debug.LogWarning("Il y a déjà une instance de GameManager");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            coroutine = null;
        }
    }

    public void StartGame()
    {
        gameState = GameState.PLAYING;
        coroutine = StartGameCoroutine();
        StartCoroutine(coroutine);
    }

    private IEnumerator StartGameCoroutine()
    {
        while (true)
        {
            chargingBar.DoChargeBar(timeBetweenPlays);
            yield return new WaitForSeconds(timeBetweenPlays);
            OnTurnStart.Invoke();
            Debug.Log("Turn Starts");
        }
    }

    public void GameOver()
    {
        gameState = GameState.GAME_OVER;
        StopCoroutine(coroutine);
        Debug.Log("Game Over");
    }

    public void PauseGame()
    {
        switch (gameState)
        {
            case GameState.PLAYING:
                gameState = GameState.PAUSED;
                StopCoroutine(coroutine);
                Debug.Log("Game Paused");
                break;
            case GameState.PAUSED:
                gameState = GameState.PLAYING;
                StartCoroutine(coroutine);
                Debug.Log("Game Resumed");
                break;
            case GameState.GAME_OVER:
                Debug.Log("Nothing to pause");
                break;
        }
    }

    public float GetTimeBetweenPlays()
    {
        return timeBetweenPlays;
    }
}
