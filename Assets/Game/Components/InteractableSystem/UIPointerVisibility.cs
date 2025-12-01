using System;
using UnityEngine;

public class UIPointerVisibility : MonoBehaviour
{
    AudioSource hintSound;
    
    public bool isPointerVisibleOnStart = false;
    Animator animator;
    
    
    private void Start()
    {
        hintSound = GameObject.Find("Hint").GetComponent<AudioSource>();
        animator = GetComponentInParent<Animator>();
        if (!isPointerVisibleOnStart)
        {
            animator?.Play("Hidden");
        }
    }
    
    public void ShowPointer()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        animator?.Play("PopUp");
        hintSound.PlayOneShot(hintSound.clip);
    }
    
    public void HidePointer()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        animator?.Play("PopDown");
    }
    
    
}
