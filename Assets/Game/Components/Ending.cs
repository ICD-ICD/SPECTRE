using System;
using System.Collections;
using Cinemachine;
using DG.Tweening;
using R3;
using UnityEngine;

public class Ending : MonoBehaviour
{
    [SerializeField] AudioSource endMusic;
    [SerializeField] AudioSource ambience;
    [SerializeField] float fadeDuration;
    [SerializeField] Tutorial theEndMessage;
    
    // void Start()
    // {
    //     GetComponent<CameraChanger>().cameraChanged
    //         .Take(1)
    //         .Subscribe(_ => OnCameraChanged());
    // }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(EndingScript());
        }
    }

    IEnumerator EndingScript()
    {
        ambience.DOFade(0f, fadeDuration);
        Player.Instance.allowInputs = false;
        yield return new WaitForSeconds(2f);
        
        endMusic.Play();
        yield return new WaitForSeconds(5f);
        
        theEndMessage.ShowMessage("The end.");
        yield return new WaitForSeconds(5f);
        
        theEndMessage.ShowMessage("Thanks for playing!");
    }
}
