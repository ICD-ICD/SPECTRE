using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;
public class Initializer : MonoBehaviour
{
    public static Initializer Instance { get; private set; }
    public Tilemap baseTilemap;
    public Tilemap activatedTilemap;
    
    void Awake()
    {
        Instance = this;
        DOTween.SetTweensCapacity(1250, 50);
    }

    void OnDestroy()
    {
        Instance = null;
    }
}
