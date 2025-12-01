using System;
using System.Linq;
using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    
    [Header("Components")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator anim;
    [SerializeField] LightSpawner waveSpawner;
    [SerializeField] InteractionController interactionController;
    [SerializeField] AudioSource lightEmitSound;
    
    [Header("Settings movement")]
    [SerializeField] float speed;
    [SerializeField] float acceleration;
    [SerializeField] float deceleration;
    [SerializeField] float velPower;
    
    [Header("Other")]
    [SerializeField] Material playerMaterial;
    [SerializeField] float materialFlashDuration;
    [SerializeField] Ease flashEase;
    [SerializeField] Light2D flashLight;
    [SerializeField] float flashMaxIntensity;
    [SerializeField] Timer waveSpawnCd;
    [SerializeField] AudioSource footstepPlayer;
    [SerializeField] AudioClip[] footstepSounds;
    [SerializeField] Timer footstepsInterval;
    AudioClip[] currentShuffled;
    
    System.Random random = new System.Random();
    int currentIdx = 0;
    
    [Header("Debugs")]
    [SerializeField] Vector2 movement;
    public bool allowInputs = true;
    
    Tweener materialLightTween;
    Tweener materialBrightnessTween;
    
    Tweener flashLightTween;
    
    void Awake()
    {
        Instance = this;
    }

    // void DoFootstep()
    // {
    //     if (footstepsInterval.IsRunning()) return;
    //     
    //     if (currentIdx >= footstepSounds.Length || currentShuffled == null)
    //     {
    //         currentShuffled = footstepSounds.OrderBy(x => random.Next()).ToArray();
    //         currentIdx = 0;
    //     }
    //     
    //     footstepPlayer.PlayOneShot(currentShuffled[currentIdx]);
    //     currentIdx++;
    //     footstepsInterval.Start();
    // }
    
    private void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (allowInputs == false)
        {
            movement = Vector2.zero;
        }

        // if (rb.velocity.magnitude > 0.05f && footstepsInterval.IsRunning() == false)
        // {
        //     DoFootstep();
        // }
        // else if (rb.velocity.magnitude < 0.05f)
        // {
        //     footstepsInterval.Reset();
        // }
        
        if (Input.GetMouseButtonDown(0) && waveSpawnCd.IsRunning() == false && allowInputs)
        {
            lightEmitSound.PlayOneShot(lightEmitSound.clip);
            waveSpawnCd.Start();
            waveSpawner.SpawnWaveInArc(WaveType.Light);
            
            materialLightTween?.Kill();
            materialLightTween = playerMaterial.DOFloat(
                1f,
                "_Saturation",
                materialFlashDuration
            )
            .SetEase(flashEase)
            .ChangeStartValue(0f);
            
            flashLightTween?.Kill();
            flashLightTween = DOTween.To(
                () => flashLight.intensity,
                x => flashLight.intensity = x,
                0f,
                materialFlashDuration
            )
            .SetEase(flashEase)
            .ChangeStartValue(flashMaxIntensity);
            
            materialBrightnessTween?.Kill();
            materialBrightnessTween = playerMaterial.DOFloat(
                1f,
                "_Brightness",
                materialFlashDuration
            )
            .SetEase(Ease.OutSine)
            .ChangeStartValue(30f);
        }
    
        // if (Input.GetMouseButtonDown(1))
        // {
        //     waveSpawner.SpawnWaveInArc(WaveType.Radio);
        // }
        
        if (Input.GetKeyDown(KeyCode.E) && allowInputs)
        {
            interactionController.InteractWithObject();
        }

        if (movement != Vector2.zero)
        {
            if (movement.y > 0f)
            {
                anim.Play("Up");
            }
            else if (movement.y < 0f)
            {
                anim.Play("Down");
            }
            else
            {
                anim.Play("Up");
            }
        }
        else
        {
            anim.Play("idle");
        }
        
        
        // if ((xinput != 0f || yinput != 0f) && !anim.IsAnimationPlaying("OverheadJump") && !anim.IsAnimationPlaying("Jump"))
        // {
        //     if (!stepInterval.IsRunning())
        //     {
        //         steps.pitch += Random.Range(-0.4f, 0.4f);
        //         steps.Play();
        //         stepInterval.Start();
        //         steps.pitch = 1f;
        //     }
        //     anim.SetBool("isRunning", true);
        // }
        // else
        // {
        //     anim.SetBool("isRunning", false);
        // }
    }
    
    void FixedUpdate()
    {
        GroundMove();
    }
    
    void GroundMove()
    {
        Vector2 targetVelocity = new Vector2(movement.x, movement.y).normalized * speed;
        
        float accelerationRateX = (Mathf.Abs(movement.x) > 0.01f) ? acceleration : deceleration;
        float accelerationRateY = (Mathf.Abs(movement.y) > 0.01f) ? acceleration : deceleration;
        
        float speedDifferenceX = targetVelocity.x - rb.velocity.x;
        float speedDifferenceY = targetVelocity.y - rb.velocity.y;
        
        float movementX = Mathf.Pow(Mathf.Abs(speedDifferenceX) * accelerationRateX, velPower) * Mathf.Sign(speedDifferenceX);
        float movementY = Mathf.Pow(Mathf.Abs(speedDifferenceY) * accelerationRateY, velPower) * Mathf.Sign(speedDifferenceY);
        
        Vector2 movementForce = new Vector2(movementX, movementY);

        rb.AddForce(movementForce, ForceMode2D.Force);
    }
}
