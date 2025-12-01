using TMPro;
using UnityEngine;
[RequireComponent(typeof(Animator))]
public class GameMessage : MonoBehaviour
{
    Animator animator;
    [SerializeField] TMP_Text messageText;
    [SerializeField] AudioSource errorSound;
    
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ShowMessage(string msg)
    {
        messageText.text = msg;
        animator.PlayState("ShowMessage", 0f);
        
        errorSound.PlayOneShot(errorSound.clip);
    }
}
