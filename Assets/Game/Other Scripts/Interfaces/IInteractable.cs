using UnityEngine;
public interface IInteractable
{
    public void Interact();
    public bool CanBeInteracted { get; set; }
    
    public Transform GetTransform();
}
