using System;
using System.Collections.Generic;
using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WaveGenerator : MonoBehaviour, IWaveReceiver
{
    public List<int> lastWaveIds { get; } = new List<int>();

    [SerializeField] LightSpawner spawner;
    [SerializeField] Light2D light2D;
    [SerializeField] SpriteRenderer sprRenderer;
    [SerializeField] Transform shootToPosition;
    [SerializeField] TimerVisualizer timerVisualizer;
    [SerializeField] AudioSource lightEmitSound;
    
    [SerializeField] Vector3 spawnOffset;

    [SerializeField] float waveSpawnDuration;
    [SerializeField] int waveCount;

    [SerializeField] Color lightColor;
    [SerializeField] Color radioColor;
    [SerializeField] Sprite lightSprite;
    [SerializeField] Sprite radioSprite;
    [SerializeField] Sprite defaultSprite;
    
    [SerializeField] float lightFlickDuration;
    [SerializeField] float lightFadeOutDuration;
    
    [Space(20), Header("Shake")]
    [SerializeField] float maxAmplitude;
    [SerializeField] float frequency;
    Tweener shakeTween;
    Vector3 origPos;
    
    float maxIntensity;
    IDisposable spawnWaveIntervalSub;
    IDisposable workTimerSub;
    
    Sequence flickSequence;
    
    Timer workTimer;
    
    int currentWaveCount;
    
    RefWaveType currentWaveType = new();
    class RefWaveType
    {
        public WaveType type;
    }
    
    
    void Awake()
    {
        workTimer = new Timer { Duration = waveSpawnDuration };
        
        maxIntensity = light2D.intensity;
        light2D.intensity = 0f;
        origPos = transform.position;
    }
    
    void Start()
    {
        timerVisualizer.SetVisualizableTimer(workTimer);
    }
    
    public void ReceiveWave(LightContactData contactData)
    {
        InsertNewWaveId(contactData.waveId);
        currentWaveType.type = contactData.WaveType;
        timerVisualizer.ChangeFillColor(contactData.WaveType);
        
        if (workTimer.IsRunning() == false)
        {
            currentWaveCount = 0;
            
            spawnWaveIntervalSub?.Dispose();
            spawnWaveIntervalSub = Observable
                .Interval(TimeSpan.FromSeconds(waveSpawnDuration / (float)waveCount))
                .Subscribe(_ => {
                    SpawnWave(currentWaveType, LightSpawner.GenerateId());
                    currentWaveCount += 1;

                    if (currentWaveCount >= waveCount)
                        spawnWaveIntervalSub?.Dispose();
                });
            workTimer.Start();
            Shake();
        }
        else
        {
            currentWaveCount = 0;
            workTimer.Start();
        }
    }
    
    void SpawnWave(RefWaveType waveType, int id)
    {
        lightEmitSound.PlayOneShot(lightEmitSound.clip);
        
        InsertNewWaveId(id);
        spawner.SpawnWaveInArc(waveType.type, shootToPosition.position, id, transform.position + spawnOffset);

        if (waveType.type == WaveType.Light)
        {
            light2D.color = lightColor;
        }
        else
        {
            light2D.color = radioColor;
        }
        
        flickSequence?.Kill();
        flickSequence = DOTween.Sequence();

        flickSequence.Append(
            DOTween.To(
            () => light2D.intensity,
            x => light2D.intensity = x,
            maxIntensity,
            lightFlickDuration
            )
            .SetEase(Ease.Flash)
            .OnStart(() => {
                if (waveType.type == WaveType.Light)
                {
                    sprRenderer.sprite = lightSprite;
                }
                else
                {
                    sprRenderer.sprite = radioSprite;
                }
            })
        );
        
        flickSequence.Append(
            DOTween.To(
                () => light2D.intensity,
                x => light2D.intensity = x,
                0f,
                lightFadeOutDuration
            )
            .SetEase(Ease.Flash)
            .OnComplete(() => {
                sprRenderer.sprite = defaultSprite;
            })
        );

        flickSequence.Play();
    }
    
    
    void Shake()
    {
        shakeTween?.Kill();
        
        transform.position = origPos;
        shakeTween = transform.DOShakePosition(frequency, strength: maxAmplitude, vibrato: 20, randomness: 0f);
    }
    
    void InsertNewWaveId(int id)
    {
        lastWaveIds.Insert(0, id);
        if (lastWaveIds.Count > 16)
            lastWaveIds.RemoveAt(lastWaveIds.Count - 1);
    }
}
