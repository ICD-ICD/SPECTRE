using Cinemachine;
using R3;
using UnityEngine;

public class CameraChanger : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera cameraToChangeTo;
    CinemachineVirtualCamera changeableCamera;
    
    public readonly Subject<Unit> cameraChanged = new Subject<Unit>();
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var brain = Camera.main.GetComponent<CinemachineBrain>();
            if (brain != null)
            {
                changeableCamera = brain.ActiveVirtualCamera as CinemachineVirtualCamera;
                if (changeableCamera != null)
                {
                    changeableCamera.Priority = int.MinValue;
                }
            }

            cameraToChangeTo.Priority = int.MaxValue;
            cameraChanged.OnNext(Unit.Default);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (changeableCamera != null)
            {
                changeableCamera.Priority = int.MaxValue;
                changeableCamera = null;
            }
            cameraToChangeTo.Priority = int.MinValue;
        }
    }
}
