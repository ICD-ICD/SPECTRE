
public readonly struct LightContactData
{
    public readonly WaveType WaveType;
    public readonly LightParticle particle;
    public readonly LightParticlesConnection particlesConnection;
    public readonly int waveId;
    public readonly int particleId;
    
    public LightContactData(WaveType waveType, LightParticle particle, LightParticlesConnection particlesConnection, int waveId, int particleId)
    {
        WaveType = waveType;
        this.particle = particle;
        this.particlesConnection = particlesConnection;
        this.waveId = waveId;
        this.particleId = particleId;
    }
}
