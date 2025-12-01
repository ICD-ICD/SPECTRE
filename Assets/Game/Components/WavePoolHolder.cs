using System;
using R3;
using UnityEngine;

public class WavePoolManager : MonoBehaviour
{
    public static WavePoolManager Instance { get; private set; }
    
    public readonly Pool lightParticlePool = new();
    
    void Awake()
    {
        if (Instance != null)
            Debug.LogError("More than one instance of WavePoolManager in the scene");
        
        Instance = this;
        
        // Observable.Interval(TimeSpan.FromSeconds(10f))
        //     .Subscribe(_ => CheckInvalidParticles())
        //     .AddTo(this);
    }

    // void CheckInvalidParticles()
    // {
    //     for (int i = 0; i < transform.childCount; i++)
    //     {
    //         LightParticle particle = transform.GetChild(i).gameObject.GetComponentAtIndex<LightParticle>(1);
    //         particle.CanBeRecycledCheck();
    //     }
    // }
    
    void OnDestroy()
    {
        Instance = null;
    }
}
