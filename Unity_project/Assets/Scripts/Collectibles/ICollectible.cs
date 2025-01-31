using System;
using UnityEngine;

public interface ICollectible
{
    /// <summary>
    /// Collect the collectible and call the callback when done.
    /// </summary>
    /// <param name="collector"></param>
    public void Collect(GameObject collector, Action callback);
}
