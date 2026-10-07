
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy Stats")]
public class EnemyStats : ScriptableObject
{
    [Header("Movement")]
    [Range(0f, 100f)] public float MaxRunSpeed = 20f;
    [Range(0f, 100f)] public float MaxWalkSpeed = 10f;
    public float Gravity { get; private set; }
    


    [Header("Collisions Checks")]
    public LayerMask GroundLayer;
    public LayerMask enemyLayer;
    public float GroundDetectionRayLength = 0.02f;


    [Header("Attack")]
    [Range(0f, 5f)] public float attackCooldown;
    public string damageType = "Normal Hit";
    public float damage = 1f;
    [Range(-5f, 5f)] public float AttackParticlesOffset;
    [Range(0f, 5f)] public float AttackWarning;
    [Range(0f, 1f)] public float AnimFrameDealDamage;
    [Range(0f, 0.25f)] public float AnimFrameDealDamageTolerance;
    [Range(0f, 5f)] public float DistanceToAttack;


    [Range(0.5f, 1f)] public float ApexThreshold = 0.97f;
    [Range(0.01f, 1f)] public float ApexHangTime = 0.075f;



    [Header("Attack & Line of Sight Range")]
    [Range(0f, 5f)] public float attackRange;
    [Range(0f, 50f)] public float lineOfSightRangeX;
    [Range(0f, 25f)] public float lineOfSightRangeY;
    public float lineOfSightColliderDist = 1f;

    [Header("Health")]
    [Range(0f, 100f)] public float maxHealth;
    [Range(0f, 100f)] public float TimeUntiDespawn;


    [Header("Debug")]
    public bool DebugShowIsGroundedBox = false;

    [Header("HitStop")]
    [Range(0f, 1f)] public float hurtSlowTimeAmount;
    [Range(0f, 1f)] public float deathSlowTimeAmount;
    [Range(0f, 5f)] public float smoothTime;
    [Range(0f, 100f)] public float timeInitialVelocityDefault;
    public float timeInitialVelocity;
    public bool HitStop;

    [Header("EnemyDifferentiator")]
    public bool isCashCow;
    public bool isBoss;
    public string BossName;
    [Range(0f, 50f)] public float DashSpeed;
    public LayerMask WallLayer;



}


