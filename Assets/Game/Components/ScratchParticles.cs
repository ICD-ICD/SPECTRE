using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScratchParticles : MonoBehaviour
{
    void Update()
    {
        Vector3 direction = Player.Instance.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up) * Quaternion.Euler(0, -90, -90);
    }
}
