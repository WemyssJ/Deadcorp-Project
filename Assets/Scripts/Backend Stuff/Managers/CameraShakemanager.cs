using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShakemanager : MonoBehaviour
{
    public static CameraShakemanager instance;


    [SerializeField] private float globalShakeForce = 1f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;        }
    }

    public void CameraShake(CinemachineImpulseSource impulseSource)
    {
        impulseSource.GenerateImpulseWithForce(globalShakeForce);
    }
}
    