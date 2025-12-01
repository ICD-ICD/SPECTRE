using System.Linq;
using UnityEngine;

public class FootstepsPlayer : MonoBehaviour
{
    [SerializeField] AudioClip[] footstepSounds;
    [SerializeField] AudioSource footstepPlayer;

    public bool isLocked = false;
    
    AudioClip[] currentShuffled;
    System.Random random = new System.Random();
    int currentIdx = 0;
    
    public void Play()
    {
        if (isLocked) return;
        
        if (currentIdx >= footstepSounds.Length || currentShuffled == null)
        {
            currentShuffled = footstepSounds.OrderBy(x => random.Next()).ToArray();
            currentIdx = 0;
        }
        
        footstepPlayer.PlayOneShot(currentShuffled[currentIdx]);
        currentIdx++;
    }
}
