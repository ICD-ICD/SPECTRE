using UnityEngine;
public class BaseInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] bool destroyAfterInteract;
    [SerializeField] bool allowMultiInteract;
    
    InteractCases interactCases;

    private void Start()
    {
        interactCases = GetComponent<InteractCases>();
    }
    
    public void Interact()
    {
        if (CanBeInteracted)
        {
            bool additionalCheck = interactCases != null ? interactCases.AdditionalCheck() : true;
            if (additionalCheck)
            {
                if (!allowMultiInteract)
                    CanBeInteracted = false;
                
                interactCases?.Successful();
                
                if (destroyAfterInteract)
                {
                    Destroy(gameObject);
                }
                else
                {
                    GetComponentInChildren<InteractableZone>().InteractableZoneInactive();
                }
            }
            else
            {
                interactCases?.Failed();
            }
        }
    }

    [field: SerializeField] public bool CanBeInteracted
    {
        get;
        set;
    } = true;

    public Transform GetTransform() => transform;
}
