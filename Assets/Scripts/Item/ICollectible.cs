using UnityEngine;

public interface ICollectible
{
    void Collect(GameObject collector);
    bool CanBeCollected(GameObject collector);
} 