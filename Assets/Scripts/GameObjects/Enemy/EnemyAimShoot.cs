using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyAimShoot : MonoBehaviour
{
    [SerializeField] public GameObject gun;
    [SerializeField] private GameObject bullet;
    [SerializeField] public Transform bulletSpawnPoint;
    [SerializeField] public GameObject _player;


    [SerializeField] public float TimeBetweenShots = 0.25f;
    [SerializeField] public float NextShotTime;

    private GameObject bulletInst;

    private Vector2 worldPosition;
    private Vector2 direction;
    private float angle;


    // Update is called once per frame
    private void Update()
    {
        HandleGunRotation();
        HandleGunShooting();
    }

    private void HandleGunRotation()
    {

        //rotate gun towards player
        direction = (_player.transform.position - (Vector3)gun.transform.position).normalized;
        gun.transform.right = direction;

        //flip gun when it reached 90 degrees
        Vector3 localScale = new Vector3(1f, 1f, 1f);

        //convert rotation to angle
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //flip gun when faces opposite direction
        if (angle > 90 || angle < -90)
        {
            localScale.x = 0.1f;
            localScale.y = -0.1f;
        }
        else
        {
            localScale.x = 0.1f;
            localScale.y = 0.1f;
        }
        gun.transform.localScale = localScale;
    }
    private void HandleGunShooting()
    {
        if(DelayBetweenShots())
        {
            //spawn bullet @ spawn point @ direction of gun rotation
            bulletInst = Instantiate(bullet, bulletSpawnPoint.position, gun.transform.rotation);
        }
    }

    private bool DelayBetweenShots()
    {
        if (Time.time > NextShotTime)
        {
            NextShotTime = Time.time + TimeBetweenShots;
            return true;
        }else
        {
            return false;
        }
    }

}
    

