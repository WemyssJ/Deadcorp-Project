using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

public class EnemyNOSTATE : MonoBehaviour, IDamageable
{
    BulletBehaviour bulletBehaviour;
    private Color PrintColor;
    [SerializeField] private float maxHealth = 5f;
    private float currentHealth;
    private CinemachineImpulseSource impulseSource;
    public GameObject HitParticles;


    [SerializeField] private GameObject HealthCosumable;
    private int HealthDropChance;
    Vector2 dropLocation;

    private void Start()
    {
        currentHealth = maxHealth;
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void Damage(float damageAmount, string damageType)
    {
        currentHealth -= damageAmount;


        //normal, crit or deadly colour
        if (damageType == "Deadly Hit")
        {
            PrintColor = Color.magenta;
        }
        else if (damageType == "Critical Hit")
        {
            PrintColor = Color.red;
        }
        else if (damageType == "Normal Hit")
        {
            PrintColor = Color.yellow;
        }

        //screeenshake
        CameraShakemanager.instance.CameraShake(impulseSource);
        //particles
        SpawnHitParticles(HitParticles);
        //damage pop up
        //Vector3 randomness = new Vector2(Random.Range(0f, 0.5f), Random.Range(0f, 0.5f));
        //DamagePopUpGenerator.current.CreatePopUp(transform.position + randomness, damageAmount.ToString(), PrintColor);

        //if dead, die
        if (currentHealth <= 0)
        {
            dropLocation = transform.position;
            HealthDropChance = Random.Range(1, 5);
            if (HealthDropChance == 1)
            {

                Instantiate(HealthCosumable, dropLocation, Quaternion.identity);
            }
            Destroy(gameObject);

        }
    }

    public void SpawnHitParticles(GameObject particlesToSpawn)
    {
        Instantiate(particlesToSpawn, this.transform.position, Quaternion.identity);
    }


}
