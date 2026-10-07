using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;

public class Pickups : MonoBehaviour
{
    Player _player;
    private SpriteRenderer rend;

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
    }
 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && rend.color != Color.black)
            if (gameObject.name == "HealthPickup(Clone)")
            {

                if (collision.GetComponent<Player>().CurrentHealth < collision.GetComponent<Player>().StartingHealth)
                {
                    collision.GetComponent<Player>().CurrentHealth++;
                    Despawn();
                }
                //DamagePopUpGenerator.current.CreatePopUp(transform.position, "Health +1", Color.green);

            }
            else if (gameObject.name == "CoinPickup(Clone)")
            {
                collision.GetComponent<Player>().CurrentCoins++;
                Despawn();


            }
            else if (gameObject.name == "Ground Dash Unlock(Clone)")
            {
                collision.GetComponent<PlayerUnlocks>().DashUnlocked = true;
                Despawn();
                DamagePopUpGenerator.current.CreatePopUp(transform.position, "Ground Dash Unlocked", Color.white);

            }
            else if (gameObject.name == "AirDash Unlock")
            {
                collision.GetComponent<PlayerUnlocks>().AirDashUnlocked = true;
                Despawn();
                DamagePopUpGenerator.current.CreatePopUp(transform.position, gameObject.name + "ed", Color.white);
            }
            else if (gameObject.name == "DoubleJump Unlock")
            {
                collision.GetComponent<PlayerUnlocks>().DoubleJumpUnlocked = true;
                Despawn();
                DamagePopUpGenerator.current.CreatePopUp(transform.position, gameObject.name + "ed", Color.white);
            }
            else if (gameObject.name == "WallJumpSlide Unlock")
            {
                collision.GetComponent<PlayerUnlocks>().WallJumpSlideUnlocked = true;
                Despawn();
                DamagePopUpGenerator.current.CreatePopUp(transform.position, gameObject.name + "ed", Color.white);
            }
            else if (gameObject.name == "Pogo Unlock")
            {
                collision.GetComponent<PlayerUnlocks>().PogoAttackUnlocked = true;
                Despawn();
                DamagePopUpGenerator.current.CreatePopUp(transform.position, gameObject.name + "ed", Color.white);
            }else if (gameObject.name == "AirDashResetOnWall Unlock")
            {
                collision.GetComponent<PlayerUnlocks>().AirDashResetOnWallUnlocked = true;
                Despawn();
                DamagePopUpGenerator.current.CreatePopUp(transform.position, gameObject.name + "ed", Color.white);
            
            }


    }
    public void Despawn()
    {
        StartCoroutine(DespawnDelay(0.25f));

    }

    IEnumerator DespawnDelay(float duration)
    {
        //
        rend.color = Color.black;
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);



    }
}

