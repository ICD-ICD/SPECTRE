using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SmartShadowCaster : MonoBehaviour
{
    [SerializeField] ShadowCaster2D upperPlayerCast;
    [SerializeField] ShadowCaster2D lowerPlayerCast;
    [SerializeField] Transform wallBottom;
    
    [SerializeField] Transform playerVisualsTransform;

    void Update()
    {
        if (playerVisualsTransform.position.y < wallBottom.position.y)
        {
            lowerPlayerCast.enabled = true;
            upperPlayerCast.enabled = false;
        }
        else
        {
            lowerPlayerCast.enabled = false;
            upperPlayerCast.enabled = true;
        }
    }
}
