using UnityEngine;

public interface IRecyclableGameObject
{
    void ResetState();
    void StartNewCycle();
    bool IsActive { get; }
    GameObject gameObject { get; }
}
