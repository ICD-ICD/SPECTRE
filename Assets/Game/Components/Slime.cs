using System;
using R3;
using UnityEngine;
using Random = UnityEngine.Random;

public class Slime : MonoBehaviour
{
    [SerializeField] Animator anim;
    
    void Start()
    {
        anim.Play("Idle");
        
        Observable.Timer(TimeSpan.FromSeconds(Random.Range(0f, 10f)))
            .Take(1)
            .Subscribe(_ => {
                anim.Play("Down");
            });
        
        
    }
}
