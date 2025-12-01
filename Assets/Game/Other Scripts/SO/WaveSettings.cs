using UnityEngine;

[CreateAssetMenu(fileName = "Wave Settings", menuName = "SO/Wave Settings")]
public class WaveSettings : ScriptableObject
{
    [Header("Light Fluctuation")]
    public float fluctuationSign;
    public float fluctuationAmplitude;
    public float fluctuationFreq;
    public float fluctuationAmplitudeRandomnessRange;
    
    [Space(20), Header("Main Parameters")]
    public float lightSpeed;
    public WaveType waveType;
    public float maxEnergy;
    public float energyLossPerSec = 143f;

    public Color waveColor;
}
