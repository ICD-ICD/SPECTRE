using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightParticlesConnection : MonoBehaviour
{
    [SerializeField] LightParticle thisParticle;
    [SerializeField] Light2D lightEmitter;
    
    [SerializeField] LayerMask obstacleLayers;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] float connectionDivideThreshold;
    
    [SerializeField] float reflectionCountCheckTime;
    [SerializeField] float receiveLightCheckTime;
    [SerializeField] float connectionMaxWidth;
    
    [SerializeField] float normalsMaxAngle;
    
    [SerializeField] float lightEmitterMaxIntensity;
    
    bool isDrawingConnection = false;
    
    Vector2 thisParticleLastReflectionNormal;
    Vector2 pairParticleLastReflectionNormal;
    
    bool divided = false;
    bool cycleStarted = false;
    
    public float ConnectionLength
    {
        get => Vector3.Distance(
            lineRenderer.GetPosition(0),
            lineRenderer.GetPosition(1));
    }
    
    public void OnEnable()
    {
        lineRenderer.enabled = false;
        thisParticle.reflected += OnThisParticleReflected;
        thisParticle.waveTypeChanged += OnThisParticleWaveTypeChanged;
    }

    void OnPairParticleWaveTypeChanged(WaveType waveType)
    {
        StartCoroutine(NeighboursWaveTypeChangedCheckTimer());
        return;
        
        IEnumerator NeighboursWaveTypeChangedCheckTimer()
        {
            yield return new WaitForSeconds(reflectionCountCheckTime);

            if (thisParticle.PairParticle is not null)
            {
                if (thisParticle.PairParticle.WaveSettings.waveType != waveType)
                {
                    thisParticle.PairParticle.ChangeWaveType(LightSpawner.Instance.GetSettingsByType(waveType));
                    thisParticle.PairParticle.DisconnectFromAnotherParticle();
                }
            }
            else
            {
                thisParticle.DisconnectFromAnotherParticle();
            }
            
            
            if (thisParticle.hooker is not null)
            {
                if (thisParticle.hooker.WaveSettings.waveType != waveType)
                {
                    thisParticle.hooker.ChangeWaveType(LightSpawner.Instance.GetSettingsByType(waveType));
                    thisParticle.hooker.hooker.DisconnectFromAnotherParticle();
                }
            }
            else
            {
                thisParticle.DisconnectFromAnotherParticle();
            }
        }
    }
    void OnThisParticleWaveTypeChanged(WaveType waveType)
    {
        StartCoroutine(NeighboursWaveTypeChangedCheckTimer());
        return;
        
        IEnumerator NeighboursWaveTypeChangedCheckTimer()
        {
            yield return new WaitForSeconds(reflectionCountCheckTime);

            if (thisParticle.PairParticle is not null)
            {
                if (thisParticle.PairParticle.WaveSettings.waveType != waveType)
                {
                    thisParticle.DisconnectFromAnotherParticle();
                }
            }
            else
            {
                thisParticle.DisconnectFromAnotherParticle();
            }
            
            
            if (thisParticle.hooker is not null)
            {
                if (thisParticle.hooker.WaveSettings.waveType != waveType)
                {
                    thisParticle.DisconnectFromAnotherParticle();
                }
            }
            else
            {
                thisParticle.DisconnectFromAnotherParticle();
            }
        }
    }
    
    void OnThisParticleReflected(Vector2 normal)
    {
        thisParticleLastReflectionNormal = normal;
        StartCoroutine(ReflectionCheckTimer());
        return;
        
        IEnumerator ReflectionCheckTimer()
        {
            yield return new WaitForSeconds(reflectionCountCheckTime);
            
            if (thisParticle.PairParticle is not null)
            {
                ReflectionDivideCheck();
            }
            else
            {
                thisParticle.DisconnectFromAnotherParticle();
            }
        }
    }
    void OnPairParticleReflected(Vector2 normal)
    {
        pairParticleLastReflectionNormal = normal;
        
        if (gameObject.activeSelf) StartCoroutine(ReflectionCheckTimer());
        return;

        IEnumerator ReflectionCheckTimer()
        {
            yield return new WaitForSeconds(reflectionCountCheckTime);
            
            if (thisParticle.PairParticle is not null)
            {
                ReflectionDivideCheck();
            }
            else
            {
                thisParticle.DisconnectFromAnotherParticle();
            }
        }
    }
    
    public void DrawConnection()
    {
        cycleStarted = true;
        isDrawingConnection = true;
        lineRenderer.enabled = true;
        
        thisParticle.PairParticle.IsHookedByAnotherParticle = true;
        thisParticle.PairParticle.hooker = thisParticle;
        thisParticle.PairParticle.reflected += OnPairParticleReflected;
        thisParticle.PairParticle.waveTypeChanged += OnThisParticleWaveTypeChanged;
    }

    public void DivideConnection()
    {
        if (divided)
        {
            return;
        }
        
        isDrawingConnection = false;
        lineRenderer.enabled = false;
        
        thisParticle.PairParticle.reflected -= OnPairParticleReflected;
        thisParticle.PairParticle.waveTypeChanged -= OnThisParticleWaveTypeChanged;
        thisParticle.PairParticle.IsHookedByAnotherParticle = false;
        thisParticle.PairParticle.hooker = null;
        divided = true;
    }

    void ReflectionDivideCheck()
    {
        if (thisParticle.reflectionCount != thisParticle.PairParticle.reflectionCount)
        {
            thisParticle.DisconnectFromAnotherParticle();
        }
        
        float angle = GetAngleBetweenNormals(thisParticleLastReflectionNormal, pairParticleLastReflectionNormal);
        if (angle > normalsMaxAngle)
        {
            thisParticle.DisconnectFromAnotherParticle();
        }
        
        // RaycastHit2D hit = Physics2D.Raycast(thisParticle.transform.position, Util.DirectionTo(pairParticle.transform.position, thisParticle.transform.position), Vector2.Distance(thisParticle.transform.position, pairParticle.transform.position), obstacleLayers);
        // if (hit)
        // {
        //     thisParticle.DisconnectFromAnotherParticle(;
        // }
    }
    void Update()
    {
        if (thisParticle.IsConnected == true || thisParticle.IsHookedByAnotherParticle == true)
        {
            lightEmitter.intensity = Functions.LinearFunc(
                thisParticle.CurrentEnergy,
                0f,
                lightEmitterMaxIntensity,
                0f,
                thisParticle.MaxEnergy
            );
        }
        else
        {
            lightEmitter.intensity = 0f;
        }
    }
    
    
    void LateUpdate()
    {
        if (isDrawingConnection)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, thisParticle.PairParticle.transform.position);

            float connectionStartWidth = 0f;
            
            if (thisParticle.IsHookedByAnotherParticle)
            {
                connectionStartWidth = Functions.LinearFunc(
                    thisParticle.CurrentEnergy,
                    0f,
                    connectionMaxWidth,
                    0f,
                    thisParticle.MaxEnergy
                );
            }
            
            float connectionEndWidth = 0f;
                
            if (thisParticle.PairParticle.IsConnected)
            {
                connectionEndWidth = Functions.LinearFunc(
                    thisParticle.PairParticle.CurrentEnergy,
                    0f,
                    connectionMaxWidth,
                    0f,
                    thisParticle.PairParticle.MaxEnergy
                );
            }

            if (thisParticle.PairParticle.IsConnected == false &&
                thisParticle.IsHookedByAnotherParticle == false &&
                thisParticle.IsConnected == true)
            {
                connectionStartWidth = Functions.LinearFunc(
                    thisParticle.CurrentEnergy,
                    0f,
                    connectionMaxWidth,
                    0f,
                    thisParticle.MaxEnergy
                );
                
                connectionEndWidth = Functions.LinearFunc(
                    thisParticle.PairParticle.CurrentEnergy,
                    0f,
                    connectionMaxWidth,
                    0f,
                    thisParticle.PairParticle.MaxEnergy
                );
            }
            
            lineRenderer.startWidth = connectionStartWidth;
            lineRenderer.endWidth = connectionEndWidth;
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    public void ResetState()
    {
        if (!cycleStarted) return;
        
        thisParticle.DisconnectFromAnotherParticle();
        divided = false;
        cycleStarted = false;
        StopAllCoroutines();
        
        thisParticle.reflected -= OnThisParticleReflected;
        thisParticle.waveTypeChanged -= OnThisParticleWaveTypeChanged;
    }

    float GetAngleBetweenNormals(Vector2 normal1, Vector2 normal2)
    {
        Vector2 n1 = normal1.normalized;
        Vector2 n2 = normal2.normalized;
        float dot = Vector2.Dot(n1, n2);
        
        dot = Mathf.Clamp(dot, -1f, 1f);
    
        return Mathf.Acos(dot) * Mathf.Rad2Deg;
    }
}
