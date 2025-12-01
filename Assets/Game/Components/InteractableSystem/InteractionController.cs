using System.Collections.Generic;
using UnityEngine;

public class InteractionController : MonoBehaviour
{
    private List<InteractableZone> _zones = new();
    
    public void OnPlayerEnteredInteractableZone(InteractableZone zone)
    {
        if (zone == null || _zones.Contains(zone) || zone.Interactable.CanBeInteracted == false) return;

        if (_zones.Count > 0)
        {
            _zones[_zones.Count - 1].InteractableZoneInactive();
        }

        _zones.Add(zone);
        zone.InteractableZoneActive();
    }

    public void OnPlayerExitedInteractableZone(InteractableZone zone)
    {
        if (zone == null || !_zones.Contains(zone) || zone.Interactable.CanBeInteracted == false) return;

        _zones.Remove(zone);
        zone.InteractableZoneInactive();
        
        if (_zones.Count > 0)
        {
            _zones[_zones.Count - 1].InteractableZoneActive();
        }
    }

    public void InteractWithObject()
    {
        if (_zones.Count > 0)
        {
            _zones[_zones.Count - 1].Interactable.Interact();
            if (_zones[_zones.Count - 1].Interactable.CanBeInteracted == false)
            {
                _zones.Remove(_zones[_zones.Count - 1]);
            }
        }
    }

    //private void ClearListFromInvalids()
    //{
    //    foreach (var el in _zones)
    //    {
    //        if (el == null || el.Interactable.CanBeInteracted == false) _zones.Remove(el);
    //    }
    //}
}
