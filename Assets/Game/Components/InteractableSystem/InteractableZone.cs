using UnityEngine;
public class InteractableZone : MonoBehaviour
{
    public IInteractable Interactable { get => _interactable; }
    private IInteractable _interactable;
    private InteractionController _interactionController;

    public void InteractableZoneActive()
    {
        UIPointerVisibility pointer = transform.parent.GetComponentInChildren<UIPointerVisibility>();
        if (pointer != null)
        {
            pointer.ShowPointer();
        }
    }

    public void InteractableZoneInactive()
    {
        UIPointerVisibility pointer = transform.parent.GetComponentInChildren<UIPointerVisibility>();
        if (pointer != null)
        {
            pointer.HidePointer();
        }
    }

    private void Awake()
    {
        _interactable = GetComponentInParent<IInteractable>();
        _interactionController = FindObjectOfType<InteractionController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _interactionController.OnPlayerEnteredInteractableZone(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _interactionController.OnPlayerExitedInteractableZone(this);
        }
    }
}
