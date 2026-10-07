using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMelee : MonoBehaviour
{
    //
    Player _player;
    PlayerUnlocks _playerUnlocks;

    [Header("Attack Stats")]
    public float attackRange;
    public float attackDelay = 0.5f;
    public Transform meleeAttackPoint;
    public Transform upMeleeAttackPoint;
    public Transform PogoAttackPoint;
    public Transform AttackPointToUse;
    public LayerMask enemyLayer;

    [Header("Critical Damage")]
    [SerializeField] private float CritRate = 20f;
    [SerializeField] private float DeadlyRate = 3f;
    [SerializeField] private float CritCalculator;
    [SerializeField] private float DamageMultiplyer;
    public string damageType;
    public string SentDamageType;
    private float BaseDamage = 1;
    private float damage;

   




    private void Awake()
    {
        _playerUnlocks = GetComponent<PlayerUnlocks>();
        _player = GameObject.FindObjectOfType(typeof(Player)) as Player;
        

    }

    public void Attack()
    {

        _player.AttackSFX();

        if (_player.isPogoing)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(AttackPointToUse.position, new Vector2(.5f, 1), 0, enemyLayer);

            foreach (Collider2D enemy in hitEnemies)
            {
                IDamageable iDamageable = enemy.gameObject.GetComponent<IDamageable>();
                if (iDamageable != null)
                {
                    //damage enemy
                    if (_playerUnlocks.EnableCritDamage && _playerUnlocks.EnableDeadlyDamage)
                    {
                        SetCritDeadlyDamage();
                    }
                    else if (_playerUnlocks.EnableCritDamage)
                    {
                        SetCritDamage();
                    }
                    else
                    {
                        DamageMultiplyer = 1;
                        damageType = "Normal Hit";
                    }

                    damage = BaseDamage * DamageMultiplyer;
                    iDamageable.Damage(damage, damageType);

                    _player.InitiateJump();

                }
            }
        } else if (!_player.isPogoing)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(AttackPointToUse.position, attackRange, enemyLayer);
            

            foreach (Collider2D enemy in hitEnemies)
            {
                IDamageable iDamageable = enemy.gameObject.GetComponent<IDamageable>();
                if (iDamageable != null)
                {
                    //damage enemy
                    if (_playerUnlocks.EnableCritDamage && _playerUnlocks.EnableDeadlyDamage)
                    {
                        SetCritDeadlyDamage();
                    }
                    else if (_playerUnlocks.EnableCritDamage)
                    {
                        SetCritDamage();
                    }
                    else
                    {
                        DamageMultiplyer = 1;
                        damageType = "Normal Hit";
                    }

                    damage = BaseDamage * DamageMultiplyer;
                    iDamageable.Damage(damage, damageType);

                }
            }
        }
        
    }
    void OnDrawGizmosSelected()
    {
        /*if (AttackPointToUse == null)
             return;

        if (_player.isPogoing)
        {
            Gizmos.DrawWireCube(AttackPointToUse.position, new Vector2(.5f, 1));
        }
        else if (!_player.isPogoing)
        {
            Gizmos.DrawWireSphere(AttackPointToUse.position, attackRange);
        }*/
    }
    private void SetCritDeadlyDamage()
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
    private void SetCritDamage()
    {

        CritCalculator = Random.Range(1, 100);

        if (CritCalculator >= 1 && CritCalculator <= CritRate)
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

   


}
    

