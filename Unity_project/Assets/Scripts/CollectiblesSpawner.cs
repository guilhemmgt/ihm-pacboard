using System;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblesSpawner : MonoBehaviour
{
    public GameObject yellowBubblePrefab;

    public static CollectiblesSpawner instance;

    public int nbSpawned;

    private void Awake()
    {
        instance = this;
        nbSpawned = 0;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnYellowBubbles();
    }

    public void SpawnYellowBubbles()
    {
        List<Vector3> yellowBubblePositions = MapController.instance.GetAllPathPosition();

        nbSpawned = yellowBubblePositions.Count;

        foreach (Vector3 position in yellowBubblePositions)
        {
            GameObject clone = Instantiate(yellowBubblePrefab, position, Quaternion.identity, this.transform);
            clone.name = "YellowBubble";
        }
    }

    public void OnCollectibleCollected()
    {
        nbSpawned--;
        if (nbSpawned <= 0)
        {
            SpawnYellowBubbles();
        }
    }
}
