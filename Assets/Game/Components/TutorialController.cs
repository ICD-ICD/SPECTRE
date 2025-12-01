using R3;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [SerializeField] Tutorial tutorial;
    [SerializeField] WaveChanger tutorialWaveChanger;
    
    void Start()
    {
        Observable.EveryUpdate()
            .Where(_ => Input.anyKey)
            .Take(1)
            .Subscribe(_ => tutorial.ShowMessage("Press \'LMB\' to emit light."));
        
        tutorialWaveChanger.receivedParticle
            .Take(1)
            .Subscribe(_ => {
                tutorial.ShowMessage("Now you can turn light into gamma rays.");
            });
    }
}
