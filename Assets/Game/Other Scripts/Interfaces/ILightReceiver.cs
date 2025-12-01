using System.Collections.Generic;

public interface IParticleReceiver
{
    List<int> lastParticleIds { get; }
    void ReceiveParticle(LightContactData contactData);
}

public interface IWaveReceiver
{
    List<int> lastWaveIds { get; }
    void ReceiveWave(LightContactData contactData);
}


