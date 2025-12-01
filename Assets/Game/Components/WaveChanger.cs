using System;
using System.Collections.Generic;
using R3;
using UnityEngine;
using UnityEngine.Rendering.Universal;
public class WaveChanger : InteractCases, IParticleReceiver
{
    [SerializeField] WaveType changeToWaveType;
    [SerializeField] bool isStatic;
    [SerializeField] Rigidbody2D rb;
    
    [Space(20), Header("Sprites")]
    [SerializeField] SpriteRenderer sprRenderer;
    [SerializeField] Sprite changeToRadioSprite;
    [SerializeField] Sprite changeToLightSprite;
    
    [Space(20), Header("Lights")]
    [SerializeField] Light2D waveChangerLight;
    [SerializeField] Color lightColor;
    [SerializeField] Color radioColor;
    [SerializeField] Light2D waveChangerLightSupport;
    [SerializeField] Color lightColorSupport;
    [SerializeField] Color radioColorSupport;
    
    [Space(20), Header("Other")]
    [SerializeField] AudioSource pushAudio;
    [SerializeField] float pushDistanceThreshold;
    [SerializeField] Timer repeatCheckPush;
    Vector3 prevCheckPosition;
    
    // [Space(20), Header("Shake")]
    // [SerializeField] float maxAmplitude;
    // [SerializeField] float frequency;
    // [SerializeField, Range(0f, 1f)] float amplitudeCut;
    //
    // float currentDisplacement;
    // Tweener shakeTween;
    //
    // Vector3 origPos;
    
    public List<int> lastParticleIds { get; } = new List<int>();
    
    public readonly Subject<Unit> receivedParticle = new Subject<Unit>();
    
    Vector3 distanceBetweenPlayerAndThis;
    IDisposable grabSub;
    
    // void Awake()
    // {
    //     origPos = transform.position;
    //     Visualize();
    // }

    void Awake()
    {
        prevCheckPosition = transform.position;
        Visualize();
        
        if (isStatic)
        {
            rb.bodyType = RigidbodyType2D.Static;
        }
    }
    
    public void ReceiveParticle(LightContactData contactData)
    {
        lastParticleIds.Insert(0, contactData.particleId);
        if (lastParticleIds.Count > 100)
            lastParticleIds.RemoveAt(lastParticleIds.Count - 1);
        
        //if (contactData.WaveType == currentWaveType) return;
        contactData.particle.ChangeEnergy(contactData.particle.MaxEnergy);
        if (contactData.WaveType == changeToWaveType) return;
        
        WaveSettings settings = contactData.particle.WaveSettings.waveType switch {
            WaveType.Light => LightSpawner.Instance.RadioSettings,
            WaveType.Radio => LightSpawner.Instance.LightSettings,
            _ => throw new NotImplementedException()
        };
        
        contactData.particle.ChangeWaveType(settings);
        receivedParticle.OnNext(Unit.Default);
    }
    
    
    [ContextMenu("Visualize")]
    public void Visualize()
    {
        if (changeToWaveType == WaveType.Light)
        {
            waveChangerLight.color = lightColor;
            waveChangerLightSupport.color = lightColorSupport;
            sprRenderer.sprite = changeToLightSprite;
        }
        else
        {
            waveChangerLight.color = radioColor;
            waveChangerLightSupport.color = radioColorSupport;
            sprRenderer.sprite = changeToRadioSprite;
        }
    }

    void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CheckSound();
        }
    }

    void CheckSound()
    {
        if (repeatCheckPush.IsRunning() == false)
        {
            if (Vector3.Distance(transform.position, prevCheckPosition) > pushDistanceThreshold)
            {
                pushAudio.PlayOneShot(pushAudio.clip);
            }
                
            prevCheckPosition = transform.position;
            repeatCheckPush.Start();
        }
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CheckSound();
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            prevCheckPosition = transform.position;
            repeatCheckPush.Reset();
        }
    }
    
    // public override void Successful()
    // {
    //     base.Successful();
    //     
    //     //change to opposite
    //     currentWaveType = currentWaveType == WaveType.Light ? WaveType.Radio : WaveType.Light;
    //     Visualize();
    //     
    //     currentDisplacement = maxAmplitude;
    //     
    //     shakeTween?.Kill();
    //     transform.position = origPos;
    //     
    //     shakeTween = transform
    //         .DOMoveX(
    //             transform.position.x + currentDisplacement,
    //             frequency
    //         )
    //         .SetEase(Ease.Linear)
    //         .SetLoops(-1, LoopType.Yoyo)
    //         .OnStepComplete(() => {
    //             currentDisplacement = currentDisplacement * amplitudeCut * -1f;
    //
    //             if (Mathf.Abs(currentDisplacement) < 0.01f)
    //             {
    //                 shakeTween?.Kill();
    //                 transform.position = origPos;
    //             }
    //         });
    // }
}

