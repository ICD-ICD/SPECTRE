using System;
using R3;
using UnityEngine;
using UnityEngine.UI;

public class TimerVisualizer : MonoBehaviour
{
    [SerializeField] Image img;
    [SerializeField] Image backGround;
    [SerializeField] Image outline;
    [SerializeField] bool fillInsteadOfDrain = false;
    
    [SerializeField] WaveType colorType;
    
    [SerializeField] Color lightColor;
    [SerializeField] Color radioColor;
    [SerializeField] Color neutralColor;
    
    float currentFill = 0f;
    float maxFill;
    
    Timer visualizableTimer = null;
    IDisposable subs = null;
    
    
    public void SetVisualizableTimer(Timer timer)
    {
        visualizableTimer = timer;
        maxFill = timer.Duration;
        currentFill = maxFill;
        
        var updateSub = Observable
            .EveryUpdate()
            .Where(_ => visualizableTimer.IsRunning())
            .Subscribe(_ => UpdateFill());

        var timerStartSub = timer.TimerStarted.Subscribe(_ => Show());
        var timerResetSub = timer.TimerFinished.Subscribe(_ => Hide());

        subs = Disposable.Combine(updateSub, timerResetSub, timerStartSub);
    }

    public void ChangeFillColor(WaveType waveType)
    {
        img.color = waveType switch {
            WaveType.Light => lightColor,
            WaveType.Radio => radioColor,
            _ => neutralColor
        };
    }
    
    void Start()
    {
        Hide();
        ChangeFillColor(colorType);
    }
    
    void UpdateFill()
    {
        currentFill = visualizableTimer.ElapsedTime;
        
        img.fillAmount = fillInsteadOfDrain == true ? currentFill / maxFill :  1f - Mathf.Abs(currentFill / maxFill);
    }
    
    void Show()
    {
        UpdateFill();
        backGround.enabled = true;
        img.enabled = true;
        outline.enabled = true;
    }

    void Hide()
    {
        backGround.enabled = false;
        img.enabled = false;
        outline.enabled = false;
    }
    
    void OnDestroy()
    {
        subs?.Dispose();
    }
}
