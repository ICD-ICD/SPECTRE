using System;
using System.Collections.Generic;
using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SignalEmitter : MonoBehaviour, IParticleReceiver
{
    public const float signalActiveTime = 2f;
    
    [Header("Visuals")]
    [SerializeField] ConnectionData[] connections;
    [SerializeField] TimerVisualizer timerVisualizer;
    [SerializeField] Sprite lightSprite;
    [SerializeField] Sprite radioSprite;
    [SerializeField] SpriteRenderer sprRenderer;
    [SerializeField] Color radioLightColor;
    [SerializeField] Color lightLightColor;
    [SerializeField] Light2D lightEmitter;
    [SerializeField] Color radioLightColorSupport;
    [SerializeField] Color lightLightColorSupport;
    [SerializeField] Light2D lightEmitterSupport;
    
    [SerializeField] bool isPermanent;
    [SerializeField] float overrideActiveTime;
    
    [NonSerialized] public Timer signalStayTimer = new Timer { Duration = signalActiveTime };
    IDisposable signalStayTimerSub;
    
    public List<int> lastParticleIds { get; } = new List<int>();
    
    [Space(20), Header("Shake")]
    [SerializeField] float maxAmplitude;
    [SerializeField] float frequency;
    
    Tweener shakeTween;
    Vector3 origPos;
    
    [Serializable] struct ConnectionData
    {
        public Wire connectedWire;
        public WaveType receiveWaveType;
    }
    
    float currentActiveTime;
    
    void Awake()
    {
        origPos = transform.position;
        Visualize();
    }
    
    void Start()
    {
        currentActiveTime = overrideActiveTime == 0f ? signalActiveTime : overrideActiveTime;
        
        signalStayTimer = new Timer { Duration = currentActiveTime };
        timerVisualizer.SetVisualizableTimer(signalStayTimer);
    }

    [ContextMenu("Visualize")]
    void Visualize()
    {
        if (connections[0].receiveWaveType == WaveType.Light)
        {
            sprRenderer.sprite = lightSprite;
            lightEmitter.color = lightLightColor;
            lightEmitterSupport.color = lightLightColorSupport;
        }
        else
        {
            sprRenderer.sprite = radioSprite;
            lightEmitter.color = radioLightColor;
            lightEmitterSupport.color = radioLightColorSupport;
        }
    }
    
    public void ReceiveParticle(LightContactData contactData)
    {
        lastParticleIds.Insert(0, contactData.particleId);
        if (lastParticleIds.Count > 100)
            lastParticleIds.RemoveAt(lastParticleIds.Count - 1);
        
        foreach (var connection in connections)
        {
            if (connection.receiveWaveType == contactData.WaveType)
            {
                if (connection.connectedWire.IsSignalOpened == false)
                {
                    connection.connectedWire.OpenSignal();
                    Shake();
                }
                
                if (isPermanent == false)
                {
                    if (signalStayTimer.IsRunning())
                        signalStayTimer.Reset();

                    signalStayTimerSub?.Dispose();
                    signalStayTimerSub = signalStayTimer
                        .TimerFinished
                        .Where(s => s)
                        .Take(1)
                        .Subscribe(_ => {
                            CloseAllSignals();
                            Shake();
                        });
                    signalStayTimer.Start();
                }
            }
        }
    }


    void Shake()
    {
        shakeTween?.Kill();
        
        transform.position = origPos;
        shakeTween = transform.DOShakePosition(frequency, strength: maxAmplitude, vibrato: 20, randomness: 0f);
    }
    
    void CloseAllSignals()
    {
        foreach (var connection in connections)
            connection.connectedWire.CloseSignal();
    }
}
