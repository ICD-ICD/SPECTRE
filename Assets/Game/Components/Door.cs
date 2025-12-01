using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;


public class Door : SignalReceiverBase
{
    [FormerlySerializedAs("doorDisplacement")]
    [SerializeField] Vector3 doorDisplacementRelative;
    [SerializeField] float displacementTime;
    [SerializeField] float goBackTime;
    [SerializeField] Transform antiStuckPos;
    
    [SerializeField] Ease easing;
    
    [SerializeField] AudioSource doorOpen;
    [SerializeField] AudioSource doorClose;
    
    Tweener displacementTween;
    Vector3 origPos;

    void Awake()
    {
        origPos = transform.position;
    }
    
    public override void OnSignalOpened()
    {
        base.OnSignalOpened();
        doorOpen.PlayOneShot(doorOpen.clip);
        
        Vector3 globalPos = doorDisplacementRelative + origPos;
        
        displacementTween?.Kill();
        displacementTween = transform
            .DOMove(globalPos, displacementTime)
            .SetEase(easing);
    }

    public override void OnSignalClosed()
    {
        base.OnSignalClosed();
        doorClose.PlayOneShot(doorClose.clip);
        
        displacementTween?.Kill();
        displacementTween = transform
            .DOMove(origPos, goBackTime)
            .SetEase(easing);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = antiStuckPos.position;
        }    
    }
}

