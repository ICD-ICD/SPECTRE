using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LightModulator : MonoBehaviour, IParticleReceiver
{
    [SerializeField] Color iconRadioColor;
    [SerializeField] Color iconLightColor;
    [SerializeField] Color defaultColor;
    
    [SerializeField] SpriteRenderer iconRenderer;
    [SerializeField] float flashDuration;
    
    [Space(20), Header("Other")]
    [SerializeField] AudioSource pushAudio;
    [SerializeField] float pushDistanceThreshold;
    [SerializeField] Timer repeatCheckPush;
    Vector3 prevCheckPosition;
    
    public List<int> lastParticleIds { get; } = new List<int>();
    Tweener iconColorTween;

    void Start()
    {
        iconRenderer.color = defaultColor;
    }
    
    public void ReceiveParticle(LightContactData contactData)
    {
        lastParticleIds.Insert(0, contactData.waveId);
        if (lastParticleIds.Count > 100)
            lastParticleIds.RemoveAt(lastParticleIds.Count - 1);
        
        contactData.particle.ChangeEnergy(contactData.particle.MaxEnergy * 2f);
        
        Color targetColor = contactData.WaveType switch {
            WaveType.Light => iconLightColor,
            _ => iconRadioColor
        };
        
        iconColorTween?.Kill();
        iconColorTween = iconRenderer
            .DOColor(defaultColor, flashDuration)
            .ChangeStartValue(targetColor)
            .SetEase(Ease.Flash);
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
}

