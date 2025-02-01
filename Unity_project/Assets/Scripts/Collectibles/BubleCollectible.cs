using UnityEngine;

public class BubleCollectible : MonoBehaviour, ICollectible
{
    public void Collect(GameObject collector, System.Action callback)
    {
        // Add 1 to the player's score
        CollectiblesSpawner.instance.OnCollectibleCollected();
        // Destroy the collectible
        Destroy(this.gameObject);
        // Call the callback
        callback();
    }
}
