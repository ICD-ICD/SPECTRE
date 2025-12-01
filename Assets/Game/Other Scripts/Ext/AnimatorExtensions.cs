using System;
using R3;
using UnityEngine;
public static class AnimatorExtensions
{
    /// <summary>
    /// Сетап происходит только после первого вызова Update.
    /// </summary>
    public static void PlayState(this Animator animator, string name, float? normalizedTime = null, Action onSetup = null, Action onStart = null, Action onEnd = null)
    {
        float playFrom = normalizedTime.GetValueOrDefault(0f);
        animator.Play(name, 0, playFrom);
        onStart?.Invoke();

        if (onSetup != null)
        {
            Observable.EveryUpdate()
                .Take(1)
                .Subscribe(_ => onSetup.Invoke());
        }

        if (onEnd != null)
        {
            Observable.EveryUpdate()
                .SkipWhile(_ => animator.IsAnimationPlaying(name))
                .Take(1)
                .Subscribe(_ => onEnd.Invoke());
        }
    }

    public static bool IsAnimationPlaying(this Animator animator, string name)
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(name);
    }

    public static bool IsAnimationPlaying(this Animator animator, int nameHash)
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.shortNameHash == nameHash;
    }

    public static int GetAnimationNameHash(this Animator animator, string name) => Animator.StringToHash(name);
    
    public static float GetCurrentAnimationTimeNormalized(this Animator animator)
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return (stateInfo.normalizedTime >= 1f) ? 1f : stateInfo.normalizedTime;
    }

    public static int GetCurrentAnimationNameHash(this Animator animator)
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.shortNameHash;
    }

    /// <summary>
    /// Скорость анимации учитывается.
    /// </summary>
    public static float GetCurrentAnimationDuration(this Animator animator)
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return animator.GetCurrentAnimationDurationRaw() / Mathf.Abs(stateInfo.speed);
    }

    /// <summary>
    /// Скорость анимации не учитывается.
    /// </summary>
    public static float GetCurrentAnimationDurationRaw(this Animator animator)
    {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        return clipInfo[0].clip.length;
    }

    /// <summary>
    /// ВЫЗЫВАТЬ ТОЛЬКО В ONSETUP ПРИ СТАРТЕ
    /// Изменяет скорость текущего стейта чтобы достичь нужной продолжительности.
    /// </summary>
    public static void SetDuration(this Animator animator, float duration, string name)
    {
        float originalLength = animator.GetCurrentAnimationDuration();

        if (originalLength <= 0)
        {
            Debug.LogError("Не удалось определить длительность анимации!");
            return;
        }

        float requiredSpeed = originalLength / duration;
        animator.speed = Mathf.Abs(requiredSpeed);

        Observable.EveryUpdate()
            .SkipWhile(_ => animator.IsAnimationPlaying(name))
            .Take(1)
            .Subscribe(_ => animator.speed = 1);
    }
}   
