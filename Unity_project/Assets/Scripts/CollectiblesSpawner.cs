using System;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblesSpawner : MonoBehaviour
{
    public GameObject yellowBubblePrefab;

    public static CollectiblesSpawner instance;

    public int currentSpawnedNumber;

    private void Awake()
    {
        instance = this;
        currentSpawnedNumber = 0;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnYellowBubbles();
    }

    public void SpawnYellowBubbles()
    {
        List<Vector3> yellowBubblePositions = MapController.instance.GetAllPathPosition();

        currentSpawnedNumber = yellowBubblePositions.Count;

        foreach (Vector3 position in yellowBubblePositions)
        {
            GameObject clone = Instantiate(yellowBubblePrefab, position, Quaternion.identity, this.transform);
            clone.name = "YellowBubble";
        }
    }

    public void OnCollectibleCollected()
    {
        currentSpawnedNumber--;
        if (currentSpawnedNumber <= 0)
        {
            SpawnYellowBubbles();
        }
    }
}
