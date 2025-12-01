using UnityEngine;

public class LightParticleRecyclable : MonoBehaviour, IRecyclableGameObject
{
    [SerializeField] LightParticle particle;
    [SerializeField] LightParticlesConnection connection;
    
    public bool IsActive { get; private set; }
    
    public void ResetState()
    {
        gameObject.SetActive(false);
        particle.ResetState();
        connection.ResetState();
        IsActive = false;
    }
    
    public void StartNewCycle()
    {
        gameObject.SetActive(true);
        IsActive = true;
    }
}
