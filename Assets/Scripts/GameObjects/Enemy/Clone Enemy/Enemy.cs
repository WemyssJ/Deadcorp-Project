using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    #region Variables
    //
    //component vars
    [Header("References")]
    public EnemyStats EnemyStats;
    public Rigidbody2D RB { get; private set; }
    public Animator animator;
    public Collider2D FeetColl;
    public Collider2D WallSlideColl;
    public BoxCollider2D EnemyLineOfSight;
    public GameObject HitParticles;
    public GameObject ParriedParticles;
    public Transform meleeAttackPoint;
    public SpriteRenderer rend;
    private HealthBar _healthBar;

    private CinemachineImpulseSource impulseSource;
    //public Player _player;

    [Header("Shooting")]
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


    [Header("FX")]
    public AudioSource audiosource;
    public Color startingColour;


    [Header("Timers")]
    public float NextAttackTime;

    [Header("Follow Player")]
    public GameObject player;
    private float distance;

    [Header("Patrol Points")]
    [SerializeField] private Transform PatrolPointA;
    [SerializeField] private Transform PatrolPointB;

    [Header("Movement Controls")]
    public bool Patroling;
    public bool isHoldingPosition;
    public bool IsFacingRight { get; private set; }
    public bool hasLineOfSight;
    public bool hasLostLineOfSight;
    public bool closeToEnemy;
    public bool inAttackRange;
    public bool isAttacking;
    public bool isChasing;
    public bool isDead;
    public float xDistance;
    public float yDistance;

    //collision vars
    public bool IsGrounded { get; private set; }
    private RaycastHit2D _groundHit;
    public RaycastHit2D WallHit { get; private set; }
    public bool IsTouchingWall;
    private RaycastHit2D _lastWallHit;

    //movement
    private Transform currentPoint;

    //animator
    public string currentAnimatorState;

    //damage
    private Color PrintColor;
    public float currentHealth;
    [SerializeField] private GameObject HealthCosumable;
    [SerializeField] private GameObject Coin;
    private int HealthDropChance;
    Vector2 dropLocation;

    public LayerMask enemyLayer;

    //jump vars
    public float VerticalVelocity { get; set; }
    public float HorizontalVelocity { get; set; }
    public bool IsJumping;
    public bool IsFalling;
    public float ApexPoint { get; set; }
    public float TimePastApexThreshold { get; set; }
    public bool IsPastApexThreshold { get; set; }

    //state vars
    public EnemyStateMachine StateMachine { get; private set; }
    public EnemyIdleState IdleState { get; private set; }
    public EnemyPatrolState PatrolState { get; private set; }
    public EnemyChaseState ChaseState { get; private set; }
    public EnemyRunState RunState { get; private set; }
    public EnemyGroundAttackState GroundAttackState { get; private set; }
    public EnemyDeathState DeathState { get; private set; }
    public EnemyInAirState InAirState { get; private set; }
    public EnemyGroundDashState GroundDashState { get; private set; }
    public EnemyParriedState ParriedState { get; private set; }
    public EnemyLostTargetState LostTargetState { get; private set; }
    public EnemyInvisibleState InvisibleState { get; private set; }

    public EnemyShootState ShootState { get; private set; }
    public EnemyCobaltGoliathAttackState CobaltGoliathAttackState { get; private set; }



    public bool ShowEnteredStateDebugLog = false;
    #endregion

    private float fixedDeltaTime;

    private float NextPopUpTime;
    private float PopUpDelay;

    //miniboss
    public float standardAttackNo;
    private int NumOfFlashes = 4;
    private float dashChargeTime = 2;
    public bool isDashing;
    public bool isTouchingPlayer;
    public bool isInvulnerable;

    Vector3 DashDirection;

    private void Awake()
    {
        if (PatrolPointA == null || PatrolPointB == null)
        {
            PatrolPointA = this.transform;
            PatrolPointB = this.transform;


        }
        //start StateMachine
        StateMachine = new EnemyStateMachine();

        //initialize the individual states here
        IdleState = new EnemyIdleState(this, StateMachine);
        PatrolState = new EnemyPatrolState(this, StateMachine);
        ChaseState = new EnemyChaseState(this, StateMachine);
        GroundAttackState = new EnemyGroundAttackState(this, StateMachine);
        DeathState = new EnemyDeathState(this, StateMachine);
        InAirState = new EnemyInAirState(this, StateMachine);
        ShootState = new EnemyShootState(this, StateMachine);
        LostTargetState = new EnemyLostTargetState(this, StateMachine);
        ParriedState = new EnemyParriedState(this, StateMachine);
        GroundDashState = new EnemyGroundDashState(this, StateMachine);
        InvisibleState = new EnemyInvisibleState(this, StateMachine);
        CobaltGoliathAttackState = new EnemyCobaltGoliathAttackState(this, StateMachine);
        RunState = new EnemyRunState(this, StateMachine);

        //initalize spawn point
        currentPoint = PatrolPointB.transform;
        //isHoldingPosition = false;

        //set health
        currentHealth = EnemyStats.maxHealth;

        if (EnemyStats.isBoss)
        {
            _healthBar = GetComponentInChildren<HealthBar>();
        }
        PopUpDelay = 1f;
        isDead = false;

        this.fixedDeltaTime = Time.fixedDeltaTime;
        EnemyStats.timeInitialVelocity = EnemyStats.timeInitialVelocityDefault;
        standardAttackNo = 1;


    }

    private void Start()
    {
        StateMachine.InitializeDefaultState(IdleState);
        RB = GetComponent<Rigidbody2D>();
        startingColour = rend.color;
        impulseSource = GetComponent<CinemachineImpulseSource>();

        

    }

    private void Update()
    {

        StateMachine.CurrentState.StateUpdate();

        ResetTime();


    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.StateFixedUpdate();
        playerIsDead();
    }

    #region Movement
    public void Move(float Speed)
    {

        if (!isDead && !isDashing && EnemyStats.BossName != "OnyxShooter")
        {
            if (!hasLostLineOfSight)
            {
                if (!hasLineOfSight)
                {
                    //RB.gravityScale = EnemyStats.Gravity;
                    Vector2 point = currentPoint.position - transform.position;

                    if (currentPoint == PatrolPointB.transform && isHoldingPosition == false && !isChasing)
                    {
                        Patroling = true;
                        transform.localScale = new Vector3(1, 1, 1);
                        RB.velocity = new Vector2(Speed, 0);
                    }

                    else if (currentPoint == PatrolPointA.transform && isHoldingPosition == false && !isChasing)
                    {
                        Patroling = true;
                        transform.localScale = new Vector3(-1, 1, 1);
                        RB.velocity = new Vector2(-Speed, 0);

                    }

                    if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == PatrolPointB.transform)
                    {
                        HoldPosition();
                        currentPoint = PatrolPointA.transform;
                    }
                    else if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == PatrolPointA.transform)
                    {
                        HoldPosition();
                        currentPoint = PatrolPointB.transform;

                    }
                }
                else if (hasLineOfSight)
                {
                    Patroling = false;
                    xDistance = Mathf.Abs(transform.position.x - player.transform.position.x);
                    yDistance = Mathf.Abs(transform.position.y - player.transform.position.y);
                    if (xDistance < EnemyStats.DistanceToAttack)
                    {
                        if (transform.position.x > player.transform.position.x)
                        {
                            transform.localScale = new Vector3(-1, 1, 1);
                        }
                        else if (transform.position.x < player.transform.position.x)
                        {
                            transform.localScale = new Vector3(1, 1, 1);
                        }

                        if (yDistance < EnemyStats.DistanceToAttack)
                        {
                            closeToEnemy = true;
                        }
                    }
                    else
                    {
                        closeToEnemy = false;
                        if (transform.position.x > player.transform.position.x)
                        {
                            transform.localScale = new Vector3(-1, 1, 1);
                            transform.position += Vector3.left * Speed * Time.deltaTime;
                        }
                        else if (transform.position.x < player.transform.position.x)
                        {
                            transform.localScale = new Vector3(1, 1, 1);
                            transform.position += Vector3.right * Speed * Time.deltaTime;
                        }
                    }

                }
            }

        }
    }


    public void HoldPosition()
    {

        float duration = 2f;
        if (!EnemyStats.isCashCow)
        {
            StartCoroutine(WaitHoldingPosition(duration));
        }


    }
    IEnumerator WaitHoldingPosition(float duration)
    {
        isHoldingPosition = true;
        Patroling = false;
        //Debug.Log($"Started at {Time.time}, waiting for {duration} seconds");
        yield return new WaitForSeconds(duration);
        //Debug.Log($"Ended at {Time.time}");
        isHoldingPosition = false;
        Patroling = true;

    }

    #endregion
    #region Line of Sight
    public void enemyLineofSight()
    {


        if(!EnemyStats.isCashCow)
        {
            if (!EnemyStats.isBoss)
            {
                RaycastHit2D seeEnemy =
                Physics2D.BoxCast(EnemyLineOfSight.bounds.center + transform.right * EnemyStats.lineOfSightRangeX * transform.localScale.x * EnemyStats.lineOfSightColliderDist,
                new Vector3(EnemyLineOfSight.bounds.size.x * EnemyStats.lineOfSightRangeX, EnemyLineOfSight.bounds.size.x * EnemyStats.lineOfSightRangeY, EnemyLineOfSight.bounds.size.z)
                ,
                0, Vector2.left, 0, enemyLayer);



                if (!isDead && seeEnemy.collider != null && !player.GetComponent<Player>().isDead)
                {
                    if (hasLineOfSight == false && CanPopUp())
                    {
                        DamagePopUpGenerator.current.CreatePopUp(transform.position, "!", Color.red);
                    }
                    hasLineOfSight = true;

                }
                else
                {
                    hasLineOfSight = false;
                }
            }
            else
                hasLineOfSight = true;
        }
        else
            hasLineOfSight = false;



    }

    public void LostTarget()
    {
        if (!isDead)
        {

            hasLostLineOfSight = true;
            StartCoroutine(LostTargetLooking(2));
        }

    }
    IEnumerator LostTargetLooking(float duration)
    {
        float durationSplit;
        durationSplit = duration / 2;

        
        transform.localScale = new Vector3(1, 1, 1);
        yield return new WaitForSeconds(durationSplit);
        transform.localScale = new Vector3(-1, 1, 1);
        yield return new WaitForSeconds(durationSplit);
        transform.localScale = new Vector3(1, 1, 1);
        yield return new WaitForSeconds(durationSplit);
        transform.localScale = new Vector3(-1, 1, 1);
        yield return new WaitForSeconds(durationSplit);
        transform.localScale = new Vector3(1, 1, 1);
        hasLostLineOfSight = false;

        if (CanPopUp())
        {
            DamagePopUpGenerator.current.CreatePopUp(transform.position, "?", Color.green);
        }
    }

    #endregion
    #region CollisionChecks

    public void CollisionChecks()
    {
        CheckForGrounded();
        CheckForTouchingWall();
    }
    private void CheckForGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(FeetColl.bounds.center.x, FeetColl.bounds.min.y);
        Vector2 boxCastSize = new Vector2(FeetColl.bounds.size.x, EnemyStats.GroundDetectionRayLength);

        _groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, EnemyStats.GroundDetectionRayLength, EnemyStats.GroundLayer);
        if (_groundHit.collider != null)
        {
            IsGrounded = true;

        }
        else { IsGrounded = false; }

        #region Debug Visualization
        if (EnemyStats.DebugShowIsGroundedBox)
        {
            Color rayColor;
            if (IsGrounded)
            {
                rayColor = Color.green;
            }
            else { rayColor = Color.red; }

            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * EnemyStats.GroundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * EnemyStats.GroundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - EnemyStats.GroundDetectionRayLength), Vector2.right * boxCastSize.x, rayColor);
        }
        #endregion
    }
    #endregion
    #region Change Animation State
    public void ChangeAnimationState(string newAnimationState)
    {
        animator.Play(newAnimationState);
        currentAnimatorState = newAnimationState;
    }
    public void MidAnimationTransitionState(string newAnimationState, int _animationLayer, float _animationTime)
    {
        animator.Play(newAnimationState, _animationLayer, _animationTime);
        currentAnimatorState = newAnimationState;
    }

    public void AnimatorApplyDamage(Animator animator, string AnimationStateName)
    {
        if (isAttacking == true)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= (EnemyStats.AnimFrameDealDamage - EnemyStats.AnimFrameDealDamageTolerance) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= (EnemyStats.AnimFrameDealDamage + EnemyStats.AnimFrameDealDamageTolerance))
            {
                isAttacking = false;
                bool isPlayerParrying = player.GetComponent<Player>().IsParrying;
                bool PlayerFacingRight = player.GetComponent<Player>().IsFacingRight;

                AudioClip PlayerParrySuccess = player.GetComponent<Player>().ParrySuccess;


                if (transform.position.x > player.transform.position.x && xDistance < 2f && PlayerFacingRight)
                {//enemy is right of player & closer than 1.5 && player is facing left
                    if (!isPlayerParrying)
                    {
                        Attack();
                        

                    }
                    else if (isPlayerParrying)
                    {
                        audiosource.PlayOneShot(PlayerParrySuccess,2.5f);
                        SpawnHitParticles(ParriedParticles);
                        StateMachine.InitializeDefaultState(ParriedState);
                    }
                } else if (transform.position.x < player.transform.position.x && xDistance < 2 && !PlayerFacingRight)
                { //enemy is left of player & closer than 1.5 && player is facing right
                    if (!isPlayerParrying)
                    {
                        Attack();

                    }
                    else if (isPlayerParrying)
                    {
                        audiosource.PlayOneShot(PlayerParrySuccess,2.5f);
                        SpawnHitParticles(ParriedParticles);
                        StateMachine.InitializeDefaultState(ParriedState);
                    }
                }
                else
                {
                    Attack();
                }

                
            }
        }

    }

    public bool isAnimationPlaying(Animator animator, string AnimationStateName)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(AnimationStateName) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion
    #region Gizmos
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(PatrolPointA.transform.position, 0.5f);
        Gizmos.DrawWireSphere(PatrolPointB.transform.position, 0.5f);
        Gizmos.DrawLine(PatrolPointA.transform.position, PatrolPointB.transform.position);

        Gizmos.DrawWireSphere(meleeAttackPoint.position, EnemyStats.attackRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(EnemyLineOfSight.bounds.center + transform.right * EnemyStats.lineOfSightRangeX * transform.localScale.x * EnemyStats.lineOfSightColliderDist, new Vector3(EnemyLineOfSight.bounds.size.x * EnemyStats.lineOfSightRangeX, EnemyLineOfSight.bounds.size.x * EnemyStats.lineOfSightRangeY, EnemyLineOfSight.bounds.size.z));
    }

    #endregion
    #region FX
    public void SpawnHitParticles(GameObject particlesToSpawn)
    {
        Instantiate(particlesToSpawn, this.transform.position, Quaternion.identity);
    }
        #endregion
    #region damage
        public void Damage(float damageAmount, string damageType)
    {

        if (!isInvulnerable)
        {
            currentHealth -= damageAmount;

            if(EnemyStats.BossName == "OnyxShooter" && standardAttackNo == 1)
            {
                standardAttackNo++;
                RB.bodyType = RigidbodyType2D.Dynamic;
            }
        }
        
        if (EnemyStats.isBoss)
        {
            _healthBar.UpdateHealthBar(EnemyStats.maxHealth, currentHealth);
        }





        //Slowtime(EnemyStats.hurtSlowTimeAmount);



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

        if (!isDead && !isInvulnerable)
        {
            
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
                Slowtime(EnemyStats.deathSlowTimeAmount);
                isDead = true;
                StateMachine.InitializeDefaultState(DeathState);
                DropOnDeath();
            }else
            {
                if (EnemyStats.isCashCow)
                {
                    dropLocation = transform.position;
                    Instantiate(Coin, dropLocation, Quaternion.identity);
                }
                Slowtime(EnemyStats.hurtSlowTimeAmount);
            }

        }

    }

    private void DropOnDeath()
    {
        dropLocation = transform.position;
            HealthDropChance = Random.Range(1, 5);
        if (HealthDropChance == 1 && isDead && !EnemyStats.isBoss)
        {
            Instantiate(HealthCosumable, dropLocation, Quaternion.identity);
        }else if (isDead)
        {
            Instantiate(Coin, dropLocation, Quaternion.identity);
        }
    }
    #endregion

    public void Despawn()
    {

        Destroy(gameObject);
    }

    public void DisableCollider()
    {
        RB.gravityScale = 0f;
        foreach (var c in this.GetComponentsInChildren<Collider2D>())
        {
            c.enabled = false;
        }

    }
    #region Attack
    public bool CanAttack()
    {
        
        if (Time.time > NextAttackTime)
        {

            NextAttackTime = Time.time + EnemyStats.attackCooldown;
            return true;

        }
        else
            return false;
    }

    public void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(meleeAttackPoint.position, EnemyStats.attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            IDamageable iDamageable = enemy.gameObject.GetComponent<IDamageable>();
            if (iDamageable != null)
            {
                //damage enemy
                iDamageable.Damage(EnemyStats.damage, EnemyStats.damageType);
                
                //memeStatement();
            }
        }

    }

    public void AttackDelay()
    {
        CanAttack();
        StartCoroutine(WaitBeforeAttacking(EnemyStats.AttackWarning));


    }
    IEnumerator WaitBeforeAttacking(float duration)
    {
        //DamagePopUpGenerator.current.CreatePopUp(transform.position , "!", Color.red);
        //Debug.Log($"Started at {Time.time}, waiting for {duration} seconds");
        rend.color = Color.magenta;
        yield return new WaitForSeconds(duration);
        //Debug.Log($"Ended at {Time.time}");
        rend.color = startingColour;
        isAttacking = true;
        ChangeAnimationState("RunAttack");


    }

    public void DashChargeUp()
    {
         
        StartCoroutine(WaitBeforeDashing(EnemyStats.AttackWarning));
        
        


    }
    IEnumerator WaitBeforeDashing(float duration)
    {

        isDashing = true;
        ChangeAnimationState(null);
        ChangeAnimationState("AirDash");

        for (int i = 0; i < NumOfFlashes; i++)
        {
            rend.color = Color.red;
            yield return new WaitForSeconds(dashChargeTime / (NumOfFlashes * 2));
            rend.color = Color.black;
            yield return new WaitForSeconds(dashChargeTime / (NumOfFlashes * 2));
        }
        rend.color = Color.red;


        isAttacking = true;
        StartCoroutine(DashDuration(0.5f));


        



    }
    IEnumerator DashDuration(float duration)
    {
        RB.gravityScale = 0;
        if (transform.position.x > player.transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            DashDirection = new Vector3(-1, 0, 0);
        }
        else if (transform.position.x < player.transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
            DashDirection = new Vector3(1, 0, 0);
        }
        
        yield return new WaitForSeconds(duration);
        RB.gravityScale = 9.81f;
        isAttacking = false;
        isDashing = false;
        rend.color = startingColour;



    }

    public void Dash()
    {
        if(isDead)
        { StateMachine.ChangeState(DeathState); 
            return; }
        if (IsTouchingWall)
        {
            transform.position += DashDirection * 0 * Time.deltaTime;

        }
        if (!IsTouchingWall)
        {
            transform.position += DashDirection * EnemyStats.DashSpeed * Time.deltaTime;
        }


    }



    public IEnumerator FadeOut()
    {
        ChangeAnimationState("Run");
        yield return new WaitForSeconds(0.1f);
        rend.color = new Color(0, 0, 0, 0.75f);
        yield return new WaitForSeconds(0.1f);
        rend.color = new Color(0, 0, 0, 0.5f);
        yield return new WaitForSeconds(0.1f);
        rend.color = new Color(0, 0, 0, 0.25f);
        yield return new WaitForSeconds(0.1f);
        rend.color = new Color(0, 0, 0, 0f);
        yield return new WaitForSeconds(2f);
        StartCoroutine(FadeIn());

    }
    public IEnumerator FadeIn()
    {
        
        yield return new WaitForSeconds(0.1f);
        rend.color = new Color(0, 0, 0, 0.25f);
        yield return new WaitForSeconds(0.1f);
        rend.color = new Color(0, 0, 0, 0.5f);
        StateMachine.ChangeState(GroundAttackState);
        yield return new WaitForSeconds(0.1f);
        rend.color = new Color(0, 0, 0, 0.75f);
        yield return new WaitForSeconds(0.1f);
        rend.color = startingColour;
        isInvulnerable = false;


    }


    #endregion

    public void Slowtime(float slowTimeAmount)
    {
        if (EnemyStats.HitStop == true)
        {
            Time.timeScale = slowTimeAmount;
            Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
        }
        
    }

    public void ResetTime()
    {
        if (EnemyStats.HitStop == true)
        {

            if (Time.timeScale != 1.0f)
            {

                Time.timeScale = Mathf.SmoothDamp(Time.timeScale, 1.0f, ref EnemyStats.timeInitialVelocity, EnemyStats.smoothTime);
            }
            else if (Time.timeScale == 1)
            {
                EnemyStats.timeInitialVelocity = EnemyStats.timeInitialVelocityDefault;
            }
            Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
        }
    }

    public bool CanPopUp()
    {
        if (!isDead)
        {
            if (Time.time > NextPopUpTime)
            {
                NextPopUpTime = Time.time + PopUpDelay;
                return true;
            }
            else
                return false;
        }else
            return false;
        
        
    }

    public bool playerIsDead()
    {
        if (player.GetComponent<Player>().isDead && EnemyStats.isBoss)
        {
            StartCoroutine(FadeIn());
            StateMachine.ChangeState(IdleState);
            rend.color = startingColour;
            hasLineOfSight = false;
            currentHealth = EnemyStats.maxHealth;
            _healthBar.UpdateHealthBar(EnemyStats.maxHealth, currentHealth);
            return true;


        }
        return false;
    }

    private void CheckForTouchingWall()
    {
        float originEndPoint = 0f;
        if (IsFacingRight)
        {
            originEndPoint = WallSlideColl.bounds.max.x;
        }
        else { originEndPoint = WallSlideColl.bounds.min.x; }

        float adjustedHeight = WallSlideColl.bounds.size.y * 0.9f;
        Vector2 boxCastOrigin = new Vector2(originEndPoint, WallSlideColl.bounds.center.y);
        Vector2 boxCastSize = new Vector2(0.125f, adjustedHeight);

        WallHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, transform.right, 0.125f, EnemyStats.WallLayer);
        if (WallHit.collider != null)
        {
            _lastWallHit = WallHit;
            IsTouchingWall = true;
        }
        else { IsTouchingWall = false; }
    }

    public void HandleGunRotation()
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
    public void HandleGunShooting()
    {
        if (DelayBetweenShots())
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
        }
        else
        {
            return false;
        }
    }
}



