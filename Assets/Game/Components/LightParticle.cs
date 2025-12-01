using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class LightParticle : MonoBehaviour
{
    [Space(20), Header("Components")]
    [SerializeField] LightParticleRecyclable recyclable;
    [SerializeField] LightParticlesConnection particleConnection;
    [SerializeField] LineRenderer line;
    [SerializeField] Light2D light2d;
    
    [Space(20), Header("Other Settings")]
    [SerializeField] LayerMask obstaclesLayers;
    [SerializeField] LayerMask lightReceiverLayer;
    
    [SerializeField] WaveSettings waveSettings;
    public WaveSettings WaveSettings => waveSettings;
    
    
    public float MaxEnergy => waveSettings.maxEnergy;
    
    [SerializeField] float currentEnergy;
    public float CurrentEnergy => currentEnergy;
    
    [SerializeField] Ease energyLossFunction;
    
    Vector3 direction;

    public event Action<Vector2> reflected;
    public event Action ranOutOfEnergy;
    public event Action<WaveType> waveTypeChanged;

    float currentFluctuationAmplitudeRandomnessRange;
    float currentFluctuationPos;
    
    Tweener fluctuationTween;
    Tweener energyLossTween;
    
    [Space(20), Header("Debug Serialized")]
    public int reflectionCount = 0;
    public int waveId = 0;
    public int particleId = 0;
    [SerializeField] LightParticle pairParticle;
    public LightParticle PairParticle => pairParticle;
    public LightParticle hooker;
    
    public event Action isHookedByAnotherParticleChanged;
    
    bool isHookedByAnotherParticle = false;
    public bool IsHookedByAnotherParticle
    {
        get => isHookedByAnotherParticle;
        set
        {
            isHookedByAnotherParticle = value;
            isHookedByAnotherParticleChanged?.Invoke();
        }
    }
    
    bool isConnected = false;
    public bool IsConnected => isConnected;

    void Awake()
    {
        waveSettings = Instantiate(waveSettings);
        
        energyLossTween = DOTween.To(
            () => currentEnergy,
            x => currentEnergy = x,
            0f,
            1f
        )
        .OnComplete(() => {
            ranOutOfEnergy?.Invoke();
            PairEnergyCheck();
            CanBeRecycledCheck();
        })
        .SetAutoKill(false)
        .Pause();
    }

    public void OnEnable()
    {
        currentEnergy = waveSettings.maxEnergy;
        
        StartEnergyLoss();
        GenerateFluctuationAmplitudeRandomness();
        StartFluctuation();

        isHookedByAnotherParticleChanged += CanBeRecycledCheck;
    }
    
    public void CanBeRecycledCheck()
    {
        if (!isHookedByAnotherParticle && (pairParticle is null || pairParticle.IsRanOutOfEnergy()))
        {
            if (IsRanOutOfEnergy())
            {
                DisconnectFromAnotherParticle();
                Recycle();
            }
        }
    }

    void PairEnergyCheck()
    {
        if (IsRanOutOfEnergy())
        {
            if (pairParticle is null || pairParticle.IsRanOutOfEnergy())
            {
                DisconnectFromAnotherParticle();
            }
        }
    }
    
    public bool IsRanOutOfEnergy() => Functions.AreEqual(currentEnergy, 0f);
    
    public void SetDirection(Vector3 moveDirectionNormalized)
    {
        direction = moveDirectionNormalized;
    }
    
    public void ChangeEnergy(float newValue)
    {
        currentEnergy = Mathf.Clamp(newValue, 0f, float.MaxValue);
        StartEnergyLoss();
    }
    
    /// <summary>
    /// Remember to instantiate ScriptableObject!
    /// </summary>
    public void ChangeWaveType(WaveSettings settings)
    {
        HardChangeWaveType(settings);
        
        StartEnergyLoss();
        StartFluctuation();
        
        //waveTypeChanged?.Invoke(waveSettings.waveType);
    }
    
    public void HardChangeWaveType(WaveSettings settings)
    {
        ClearWaveSettings();
        waveSettings = Instantiate(settings);
        
        Gradient newGradient = new Gradient();
        
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0] = new GradientColorKey(waveSettings.waveColor, 0f);
        colorKeys[1] = new GradientColorKey(waveSettings.waveColor, 1f);

        newGradient.colorKeys = colorKeys;
        line.colorGradient = newGradient;
        
        if (waveSettings.waveType == WaveType.Radio)
        {
            light2d.enabled = false;
        }
        else
        {
            light2d.enabled = true;
        }
    }
    
    void ClearWaveSettings()
    {
        if (waveSettings is not null)
        {
            Destroy(waveSettings);
        }
    }

    public float CalculateEnergyLossDuration() => currentEnergy / waveSettings.energyLossPerSec;

    public void ConnectToAnotherParticle(LightParticle otherParticle)
    {
        if (isConnected)
        {
            return;
        }
        isConnected = true;
        
        pairParticle = otherParticle;
        particleConnection.DrawConnection();
        
        pairParticle.ranOutOfEnergy += CanBeRecycledCheck;
        pairParticle.ranOutOfEnergy += PairEnergyCheck;
    }

    public void DisconnectFromAnotherParticle()
    {
        if (isConnected == false)
        {
            return;
        }
        isConnected = false;
        
        particleConnection.DivideConnection();
        pairParticle.ranOutOfEnergy -= CanBeRecycledCheck;
        pairParticle.ranOutOfEnergy -= PairEnergyCheck;
        pairParticle = null;
    }

    void StartEnergyLoss()
    {
        energyLossTween.ChangeEndValue(0f, CalculateEnergyLossDuration())
            .Restart();
    }

    void StartFluctuation()
    {
        fluctuationTween.Kill();
        currentFluctuationPos = (waveSettings.fluctuationAmplitude + currentFluctuationAmplitudeRandomnessRange) * waveSettings.fluctuationSign;
        fluctuationTween = DOTween.To(
            () => currentFluctuationPos,
            x => currentFluctuationPos = x,
            (waveSettings.fluctuationAmplitude + currentFluctuationAmplitudeRandomnessRange) * -waveSettings.fluctuationSign,
            waveSettings.fluctuationFreq
        )
        .OnStepComplete(GenerateFluctuationAmplitudeRandomness)
        .SetEase(Ease.InOutSine)
        .SetLoops(-1, LoopType.Yoyo);
    }

    void Recycle()
    {
        WavePoolManager.Instance.lightParticlePool.Disable(recyclable);
    }
    
    void Update()
    {
        transform.position += direction * (waveSettings.lightSpeed * Time.deltaTime);
        transform.position += direction * (currentFluctuationPos * Time.deltaTime);
             
        if (pairParticle is not null)
        {
            Debug.DrawLine(transform.position, pairParticle.transform.position);
            RaycastHit2D rayHit = Physics2D.Raycast(transform.position, Util.DirectionTo(pairParticle.transform.position, transform.position), Vector2.Distance(transform.position, pairParticle.transform.position), lightReceiverLayer);

            if (rayHit)
            {
                if (rayHit.transform.TryGetComponent(out IParticleReceiver particleReceiver))
                {
                    if (particleReceiver.lastParticleIds.Contains(particleId) == false)
                    {
                        particleReceiver.ReceiveParticle(new LightContactData(
                            waveSettings.waveType, this, particleConnection, waveId, particleId
                        ));
                    }
                }
                
                if (rayHit.transform.TryGetComponent(out IWaveReceiver waveReceiver))
                {
                    if (waveReceiver.lastWaveIds.Contains(waveId) == false)
                    {
                        waveReceiver.ReceiveWave(new LightContactData(
                            waveSettings.waveType, this, particleConnection, waveId, particleId
                        ));
                    }
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 normal = collision.contacts[0].normal;
        direction = Vector2.Reflect(direction, normal.normalized);
        reflectionCount++;

        reflected?.Invoke(normal.normalized);
    }

    void GenerateFluctuationAmplitudeRandomness()
    {
        currentFluctuationAmplitudeRandomnessRange = Random.Range(-waveSettings.fluctuationAmplitudeRandomnessRange, waveSettings.fluctuationAmplitudeRandomnessRange);
    }
    
    public void ResetState()
    {
        reflectionCount = 0;
        currentEnergy = waveSettings.maxEnergy;
        direction = default(Vector2);
        fluctuationTween.Kill();
        
        waveSettings.fluctuationSign = 1f;
        currentFluctuationPos = 0f;
        currentFluctuationAmplitudeRandomnessRange = 0f;
        waveId = 0;
        particleId = 0;
        
        isHookedByAnotherParticle = false;
        isConnected = false;
        
        isHookedByAnotherParticleChanged -= CanBeRecycledCheck;
        ClearWaveSettings();
    }

    void OnDestroy()
    {
        if (energyLossTween != null && energyLossTween.IsActive())
        {
            energyLossTween.Kill();
            energyLossTween = null;
        }
    }
}

public enum WaveType
{
    NotSpecified, Light, Radio 
}

