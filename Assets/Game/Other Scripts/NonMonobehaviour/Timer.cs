using UnityEngine;
using System;
using R3;

[Serializable]
public class Timer
{
    bool isRunning = false;
    [SerializeField] float duration;
    [SerializeField] float elapsedTime = 0f;

    public float Duration { get => duration; set { if (!IsRunning()) duration = value; } }
    public float ElapsedTime => elapsedTime;

    public Observable<bool> TimerFinished => timerFinished;
    public Observable<Unit> TimerStarted => timerStarted;

    readonly Subject<bool> timerFinished = new();
    readonly Subject<Unit> timerStarted = new();

    IDisposable updateSubscription;

    public void Start()
    {
        if (IsRunning())
            Reset();

        if (Duration == 0f) Debug.LogWarning("Продолжительность таймера - 0 секунд, забыл настроить");
        isRunning = true;

        updateSubscription = Observable
            .EveryUpdate()
            .TakeWhile(_ => IsRunning())
            .Subscribe(_ => Tick());

        timerStarted.OnNext(Unit.Default);
    }

    void Tick()
    {
        elapsedTime += Time.deltaTime;
        if (ElapsedTime >= Duration)
        {
            Finished();
        }
    }

    void Finished()
    {
        isRunning = false;
        elapsedTime = 0f;
        updateSubscription?.Dispose();
        timerFinished.OnNext(true);
    }

    public void Reset()
    {
        isRunning = false;
        elapsedTime = 0f;
        updateSubscription?.Dispose();
        timerFinished.OnNext(false);
    }

    public bool IsRunning() => isRunning;
}
