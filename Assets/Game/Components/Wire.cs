using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class Wire : MonoBehaviour
{
    [FormerlySerializedAs("signalReceiver")]
    [SerializeField] SignalReceiverBase signalReceiverBase;
    
    [SerializeField] Vector3Int[] wireChangePositions;
    
    
    bool isSignalOpened = false;
    public bool IsSignalOpened => isSignalOpened;
    
    void Start()
    {
        Visualize();
    }
    
    public void OpenSignal()
    {
        isSignalOpened = true;
        signalReceiverBase.OnSignalOpened();
        Visualize();
    }

    public void CloseSignal()
    {
        isSignalOpened = false;
        signalReceiverBase.OnSignalClosed();
        Visualize();
    }

    void Visualize()
    {
        foreach (var pos in wireChangePositions)
        {
            Initializer.Instance.activatedTilemap.SetTileFlags(pos, TileFlags.None);
            Initializer.Instance.baseTilemap.SetTileFlags(pos, TileFlags.None);
            
            Initializer.Instance.activatedTilemap.SetColor(pos, isSignalOpened ? Color.white : Color.clear);
            Initializer.Instance.baseTilemap.SetColor(pos, isSignalOpened ? Color.clear : Color.white);
            Initializer.Instance.activatedTilemap.RefreshTile(pos);
            Initializer.Instance.baseTilemap.RefreshTile(pos);
        }
    }
}
