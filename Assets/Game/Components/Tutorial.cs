using TMPro;
using UnityEngine;


[RequireComponent(typeof(Animator))]
public class Tutorial : MonoBehaviour
{
    Animator animator;
    [SerializeField] TMP_Text messageText;
    
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ShowMessage(string msg)
    {
        messageText.text = msg;
        animator.PlayState("ShowMessage", 0f);
    }

    public bool IsPlaying() => animator.IsAnimationPlaying("ShowMessage");
}
