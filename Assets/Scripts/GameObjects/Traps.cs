using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Traps : MonoBehaviour, IDamageable
{
    public float damageAmount = 1;
    [SerailizeField] string damageType = "Normal Hit";
    public GameObject HitParticles;
    public GameObject _player;
    Vector2 playerPosition;

    [SerailizeField] float TrapDamageTimeBtw = 1f;
    private float NextTrapTime;
    // Start is called before the first frame update

    private void Awake()
    {
        NextTrapTime = 0;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!_player.GetComponent<Player>().Invunerable && !_player.GetComponent<Player>().isPogoing)
            {
                if (Time.time > NextTrapTime)
                {
                    NextTrapTime = Time.time + TrapDamageTimeBtw;


                    if (!_player.GetComponent<Player>().isDead)
                    {

                        IDamageable iDamageable = collision.gameObject.GetComponent<IDamageable>();
                        if (iDamageable != null)
                        {
                            //damage enemy
                            iDamageable.Damage(damageAmount, damageType);
                        }
                        SpawnHitParticles(HitParticles);
                    }
                }
            }
        }
        
        

        

    }

    public void Damage(float damageAmount, string damageType)
    {
        

    }
    public void SpawnHitParticles(GameObject particlesToSpawn)
    {
        playerPosition = _player.GetComponent<Player>().transform.position;
        Instantiate(particlesToSpawn, playerPosition, Quaternion.identity);
    }
}
