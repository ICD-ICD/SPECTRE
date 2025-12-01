using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class LightSpawner : MonoBehaviour
{
    public static LightSpawner Instance { get; private set; }
    
    [FormerlySerializedAs("bulletPrefab")]
    [Header("Spawn Settings")]
    [SerializeField] GameObject lightParticlePrefab;
    [SerializeField] WaveSettings radioSettings;
    [SerializeField] WaveSettings lightSettings;
    
    public WaveSettings RadioSettings => radioSettings;
    public WaveSettings LightSettings => lightSettings;
    public WaveSettings GetSettingsByType(WaveType waveType) => waveType switch {
        WaveType.Light => lightSettings,
        WaveType.Radio => radioSettings,
        WaveType.NotSpecified => throw new NotImplementedException(),
        _ => throw new ArgumentOutOfRangeException(nameof(waveType), waveType, null)
    };


    const int maxLightParticlesPerShot = 51;
    [SerializeField] float spawnRadius = 2f;
    
    [Header("Arc Settings")]
    [Range(0, 360)]
    [SerializeField] float arcAngle = 360f;

    Camera mainCamera;
    
    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        mainCamera = Camera.main;
    }
    
    public void SpawnWaveInArc(WaveType waveType, Vector3? position = null, int? specifiedWaveId = null, Vector3? center = null)
    {
        if (lightParticlePrefab == null || maxLightParticlesPerShot <= 0) return;
        
        // Получаем стартовый угол для спавна
        float startAngle = position == null ? GetParticleStartAngleByMouse() : GetParticleStartAngleByPosition(position.Value);
        
        // Рассчитываем угол между пулями
        float fixedAngleStep = 360f / maxLightParticlesPerShot;
        
        // Рассчитываем кол-во пуль
        int actualParticlesCount = CalculateParticlesCount(fixedAngleStep);
        actualParticlesCount = Mathf.Max(3, actualParticlesCount);
        
        //генерим айдишник волны
        int waveId = specifiedWaveId ?? GenerateId();
        
        GameObject firstParticle = null;
        LightParticle prevLight = null;
        
        int poolIdx = 0;
        bool hasInPool = true;
        
        bool fluctuateBackwards = false;
        for (int i = 0; i < actualParticlesCount; i++)
        {
            float currentAngle = startAngle + (i * fixedAngleStep);
            float angleInRadians = currentAngle * Mathf.Deg2Rad;

            Vector3 spawnPosition = center ?? CalculateSpawnPosition(angleInRadians, transform.position);

            GameObject lightParticle;
            
            IRecyclableGameObject lightFromPool = null;
            if (hasInPool)
            {
                (IRecyclableGameObject objFromPool, int idxInPool) = WavePoolManager.Instance.lightParticlePool.PickFromPool(poolIdx);
                if (objFromPool == null)
                {
                    lightParticle = Instantiate(lightParticlePrefab, spawnPosition, Quaternion.identity, WavePoolManager.Instance.transform);
                    hasInPool = false;
                }
                else
                {
                    lightParticle = objFromPool.gameObject;
                    lightParticle.transform.position = spawnPosition;
                    lightParticle.transform.rotation = Quaternion.identity;
                    poolIdx = idxInPool + 1;
                    lightFromPool = objFromPool;
                }
            }
            else
            {
                lightParticle = Instantiate(lightParticlePrefab, spawnPosition, Quaternion.identity, WavePoolManager.Instance.transform);
            }
            
            if (prevLight != null)
            {
                var particleComponent = lightParticle.GetComponentAtIndex<LightParticle>(1);
                particleComponent.ConnectToAnotherParticle(prevLight);
            }
            else
            {
                firstParticle = lightParticle;
            }
            prevLight = lightParticle.GetComponentAtIndex<LightParticle>(1);
            
            //настраиваем частичку света
            SetLightDirection(prevLight, angleInRadians);
            prevLight.WaveSettings.fluctuationSign = fluctuateBackwards ? -1f : 1f;
            prevLight.waveId = waveId;
            
            WaveSettings settings = GetSettingsByType(waveType);
            prevLight.HardChangeWaveType(settings);
            
            int particleId = Random.Range(int.MinValue, int.MaxValue);
            prevLight.particleId = particleId;
            
            fluctuateBackwards = !fluctuateBackwards;
            lightFromPool?.StartNewCycle();
        }

        if (firstParticle != null && arcAngle == 360f)
        {
            firstParticle.GetComponentAtIndex<LightParticle>(1).ConnectToAnotherParticle(prevLight);
        }
    }
    
    public static int GenerateId()
    {
        return Random.Range(int.MinValue, int.MaxValue);
    }

    float GetParticleStartAngleByMouse()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 directionToMouse = (mousePosition - transform.position).normalized;
        
        float angleToMouse = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
        float startAngle = angleToMouse - (arcAngle / 2);
        
        return startAngle;
    }
    float GetParticleStartAngleByPosition(Vector3 position)
    {
        Vector3 directionToMouse = (position - transform.position).normalized;
        
        float angleToMouse = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
        float startAngle = angleToMouse - (arcAngle / 2);
        
        return startAngle;
    }
    
    Vector3 CalculateSpawnPosition(float angleInRadians, Vector3 centerPosition)
    {
        float x = centerPosition.x + Mathf.Cos(angleInRadians) * spawnRadius;
        float y = centerPosition.y + Mathf.Sin(angleInRadians) * spawnRadius;
        
        return new Vector3(x, y, centerPosition.z);
    }
    int CalculateParticlesCount(float fixedAngleStep)
    {
        // Количество пуль = угол дуги / фиксированный шаг угла
        int count = Mathf.FloorToInt(arcAngle / fixedAngleStep);
        return count;
    }
    void SetLightDirection(LightParticle particle, float spawnAngle)
    {
        Vector2 direction = new Vector2(Mathf.Cos(spawnAngle), Mathf.Sin(spawnAngle)).normalized;
        particle.SetDirection(direction);
    }
    
    void OnDestroy()
    {
        Instance = null;
    }
}