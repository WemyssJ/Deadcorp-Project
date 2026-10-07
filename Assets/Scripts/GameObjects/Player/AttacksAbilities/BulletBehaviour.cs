using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    [Header("General Bullet Stats")]
    [SerializeField] private LayerMask whatDestroysBullet;
    [SerializeField] private float destroyTime = 3f;

    [Header("Normal Bullet Stats")]
    [SerializeField] private float normalBulletSpeed = 15f;
    [SerializeField] private float normalBulletDamage = 1f;

    [Header("Physics Bullet Stats")]
    [SerializeField] private float physicsBulletSpeed = 17.5f;
    [SerializeField] private float physicsBulletGravity = 3f;
    [SerializeField] private float physicsBulletDamage = 2f;

    [Header("Homing Bullet Stats")]
    [SerializeField] private float HomingBulletSpeed = 15f;
    [SerializeField] private float HomingBulletDamage = 1f;
    [SerializeField] private float HomingRotationSpeed = 200f;
    [SerializeField] private float HomingDelayTime = 1.5f;
    private bool hasLineOfSight;
    Enemy closestEnemy;
    private bool shouldHome;
    

    [Header("Critical Damage")]
    [SerializeField] private float CritRate = 20f;
    [SerializeField] private float DeadlyRate = 3f;
    [SerializeField] private float CritCalculator;
    [SerializeField] private float DamageMultiplyer;
    public string damageType;

    [Header("FX")]

    //var
    private Rigidbody2D rb;
    private float damage;


    public enum BulletType
    {
        Normal,
        Physics,
        Homing
    }

    public BulletType bulletType;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        StartCoroutine(WaitBeforeHoming());
        SetDestroyTime();

        //Change rb stats based on bullet type
        SetRBStats();

        //set velocity based on bullet type
        InitialiseBulletStats();

    }

    private void FixedUpdate()
    {
        if (bulletType == BulletType.Physics)
        {
            transform.right = rb.velocity;
        }
        else if (bulletType == BulletType.Homing)
        {
            FindClosestEnemy();
            HomingBullet();
            //EnemyLineOfSight();

        }

    }

    private void InitialiseBulletStats()
    {
        SetCriticalDamage();

        if (bulletType == BulletType.Normal)
        {
            SetStraightVelocity();
            damage = normalBulletDamage * DamageMultiplyer;


        }
        else if (bulletType == BulletType.Physics)
        {
            SetPhysicstVelocity();
            damage = physicsBulletDamage * DamageMultiplyer;
        }
        else if (bulletType == BulletType.Homing)
        {
            SetHomingVelocity();
            damage = physicsBulletDamage * DamageMultiplyer;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //is the collision within the whatDestroyBullet Layer Mask
        if ((whatDestroysBullet.value & (1 << collision.gameObject.layer)) > 0)
        {
            //spawn particles
  
            //play sound  FX

            //Screen shake

            //Damage enemy
            IDamageable iDamageable = collision.gameObject.GetComponent<IDamageable>();
            if (iDamageable != null)
            {
                //damage enemy
                iDamageable.Damage(damage, damageType);
            }

            //Destroy Bullet
            Destroy(gameObject);
        }
    }
    private void SetRBStats()
    {
        if (bulletType == BulletType.Normal)
        {
            rb.gravityScale = 0f;
        }
        else if (bulletType == BulletType.Physics)
        {
            rb.gravityScale = physicsBulletGravity;
        }
    }

    private void SetDestroyTime()
    {
        Destroy(gameObject, destroyTime);
    }

    private void SetCriticalDamage()
    {

        CritCalculator = Random.Range(1, 100);

        if (CritCalculator >= 1 && CritCalculator <= DeadlyRate)
        {
            DamageMultiplyer = 3;
            damageType = "Deadly Hit";
        }
        else if (CritCalculator > DeadlyRate && CritCalculator <= CritRate)
        {
            DamageMultiplyer = 2;
            damageType = "Critical Hit";
        }
        else if (CritCalculator > CritRate)
        {
            DamageMultiplyer = 1;
            damageType = "Normal Hit";
        }
    }

    #region SetVelocity
    private void SetStraightVelocity()
    {
        rb.velocity = transform.right * normalBulletSpeed;
    }

    private void SetHomingVelocity()
    {
        rb.velocity = transform.right * HomingBulletSpeed;
    }

    private void SetPhysicstVelocity()
    {
        rb.velocity = transform.right * physicsBulletSpeed;
    }
    #endregion

    #region HomingBullet
    private void FindClosestEnemy()
    {
        float distanceToClosestEnemy = Mathf.Infinity;
        closestEnemy = null;
        Enemy[] allEnemies = GameObject.FindObjectsOfType<Enemy>();

        foreach (Enemy currentEnemy in allEnemies)
        {
            float distanceToEnemy = (currentEnemy.transform.position - this.transform.position).sqrMagnitude;
            if (distanceToEnemy < distanceToClosestEnemy)
            {
                distanceToClosestEnemy = distanceToEnemy;
                closestEnemy = currentEnemy;

            }

        }

    }
    //disabled line of sight as not working propery
    private void EnemyLineOfSight()
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position, closestEnemy.transform.position - transform.position);
        if (ray.collider != null)
        {
            hasLineOfSight = ray.collider.CompareTag("Enemy");
            print(hasLineOfSight);
            if(hasLineOfSight)
            {
                //HomingBullet();
                Debug.DrawRay(transform.position, closestEnemy.transform.position - transform.position, Color.green);
            }
            else
            {
                HomingRotationSpeed = 0;
                Debug.DrawRay(transform.position, closestEnemy.transform.position - transform.position, Color.red);
            }
        }
    }
    private void HomingBullet()
    {

        if (shouldHome == true)
        { 
            Vector2 point2Target = (Vector2)transform.position - (Vector2)closestEnemy.transform.position;
            point2Target.Normalize();
            float value = Vector3.Cross(point2Target, transform.right).z;

            if (value > 0)
            {
                rb.angularVelocity = HomingRotationSpeed;
            }
            else if (value < 0)
            {
                rb.angularVelocity = -HomingRotationSpeed;
            }
            else
                HomingRotationSpeed = 0;

            rb.velocity = transform.right * HomingBulletSpeed;
            }
        
    }
    private IEnumerator WaitBeforeHoming()
    {
        yield return new WaitForSeconds(HomingDelayTime);
        shouldHome = true;
    }


    #endregion
}
