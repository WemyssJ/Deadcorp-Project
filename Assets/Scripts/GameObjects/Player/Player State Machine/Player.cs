using Cinemachine;
using System.Collections;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable
{

    #region Variables

    [Header("Camera")]
    [SerializeField] private GameObject _cameraFollowGO;
    private CameraFollowObject _cameraFollowObject;
    private float _fallSpeedYDampingChangeThreshold;


    //component vars
    [Header("References")]
    public PlayerMovementStats MoveStats;
    public Collider2D FeetColl;
    public Collider2D HeadColl;
    public Collider2D WallSlideColl;
    public Rigidbody2D RB { get; private set; }
    public Animator Anim { get; private set; }
    public GhostTrail GhostTrail { get; private set; }
    private CinemachineImpulseSource impulseSource;
    public AudioSource backgroundMusic;
    public GameObject pauseMenu;
    public Transform defaultSpawn;



    //
    [Header("FX")]
    public GameObject JumpParticles;
    public GameObject SecondJumpParticles;
    public GameObject LandParticles;
    public GameObject HitParticles;
    public Transform ParticleSpawnTransform;
    public TrailRenderer TrailRenderer;
    public ParticleSystem SpeedParticles;
    public GameObject DashParticles;
    public ParticleSystem WallSlideParticles;

    public AudioSource audiosource;
    public AudioClip SwordSwing1;
    public AudioClip SwordSwing2;
    public AudioClip Parry;
    public AudioClip ParrySuccess;
    public AudioClip Jump;
    public AudioClip Hurt;


    [Header("Height Tracker")]
    public Transform HeightTracker;

    [Header("Recieve Damage Var")]
    //move to stats later
    public SpriteRenderer rend;

    public float StartingHealth = 4;

    public float CurrentHealth;

    private float InvulnerabilityDuration = 0.5f;

    private int NumOfFlashes = 2; 

    public bool isDead;

    public bool isHurt;

    [Header("Debug")]
    public bool ShowEnteredStateDebugLog = false;

    //animation vars
    public Animator animator;
    public string currentAnimatorState;

    //state vars
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerWalkState WalkState { get; private set; }
    public PlayerRunState RunState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerInAirState InAirState { get; private set; }
    public PlayerWallSlideState WallSlideState { get; private set; }
    public PlayerWallJumpState WallJumpState { get; private set; }
    public PlayerDashState DashState { get; private set; }
    public PlayerDeathState DeathState { get; private set; }
    public PlayerGroundAttackState GroundAttackState { get; private set; }
    public PlayerAirAttackState AirAttackState { get; private set; }

    public PlayerAirPogoAttackState AirPogoAttackState { get; private set; }
    public PlayerGroundParryState GroundParryState { get; private set; }

    //collision vars
    public bool IsGrounded;

    public bool BumpedHead { get; private set; }
    public bool IsTouchingWall { get; private set; }
    private RaycastHit2D _groundHit;

    private RaycastHit2D _headHit;
    public RaycastHit2D WallHit { get; private set; }
    private RaycastHit2D _lastWallHit;

    //movement vars
    public bool IsFacingRight { get; private set; }
    public float HorizontalVelocity { get; set; }

    //jump vars
    public float VerticalVelocity { get; set; }
    public bool IsJumping { get; set; }
    public bool IsFastFalling { get; set; }
    public bool IsFalling  { get; set; }
    public float FastFallTime { get; set; }
    public float FastFallReleaseSpeed { get; set; }
    public int NumberOfJumpsUsed { get; set; }
    //apex vars
    public float ApexPoint { get; set; }
    public float TimePastApexThreshold { get; set; }
    public bool IsPastApexThreshold { get; set; }
    //jump buffer vars
    public float JumpBufferTimer { get; set; }
    public bool JumpReleasedDuringBuffer { get; set; }
    //coyote time vars
    public float CoyoteTimer { get; private set; }

    [SerializeField] public bool IsWallSliding { get; set; }
    public bool IsWallSlideFalling { get; set; }
    public bool UseWallJumpMoveStats { get; set; }
    public bool IsWallJumping { get; private set; }
    public float WallJumpTime { get; private set; }
    public bool IsWallJumpFastFalling { get; private set; }
    public bool IsWallJumpFalling { get; private set; }
    public float WallJumpFastFallTime { get; private set; }
    public float WallJumpFastFallReleaseSpeed { get; private set; }

    public float WallJumpPostBufferTimer { get; private set; }

    public float WallJumpApexPoint { get; private set; }
    public float TimePastWallJumpApexThreshold { get; private set; }
    public bool IsPastWallJumpApexThreshold { get; private set; }

    //dash vars
    public bool IsDashing { get; private set; }
    public bool IsAirDashing { get; private set; }
    public float DashTimer { get; private set; }
    public float DashOnGroundTimer { get; private set; }
    public int DashDirectionMult { get; private set; }
    public int NumberOFDashesUsed { get; private set; }
    public Vector2 DashDirection { get; private set; }
    public bool IsDashFastFalling { get; set; }
    public float DashFastFallTime { get; private set; }
    public float DashFastFallReleaseSpeed { get; private set; }

    bool IsAirDashFalling;

    //high point trackers
    public float HighestPoint { get; private set; }
    public float HeightTrackerStartingPoint { get; private set; }

    //attack var
    PlayerMelee _playerMelee;
    public float NextAttackTime { get; private set; }

    public float NextParryTime { get; private set; }

    public bool Invunerable;

    public bool IsParrying;
    public bool isPogoing;

    [Header("SFXCalc")]
    [SerializeField] private float AttackSFXCalc;


    //attack animations
    public string PogoAttackAnim { get; private set; }
    public string GroundedUpAttackAnim { get; private set; }
    public string DashAnim { get; private set; }

    public Vector3 checkpointPos;

    private float fixedDeltaTime;

    public float CurrentCoins;

    private Color startingColour;
    #endregion


    private void Awake()
    {
        GameManager.Instance.Player = this;
        //setup recieve damage

        CurrentHealth = StartingHealth;

        //start StateMachine
        StateMachine = new PlayerStateMachine();

        //initialize the individual states here
        IdleState = new PlayerIdleState(this, StateMachine);
        WalkState = new PlayerWalkState(this, StateMachine);
        RunState = new PlayerRunState(this, StateMachine);
        JumpState = new PlayerJumpState(this, StateMachine);
        InAirState = new PlayerInAirState(this, StateMachine);
        WallSlideState = new PlayerWallSlideState(this, StateMachine);
        WallJumpState = new PlayerWallJumpState(this, StateMachine);
        DashState = new PlayerDashState(this, StateMachine);
        DeathState = new PlayerDeathState(this, StateMachine);
        GroundAttackState = new PlayerGroundAttackState(this, StateMachine);
        AirAttackState = new PlayerAirAttackState(this, StateMachine);
        AirPogoAttackState = new PlayerAirPogoAttackState(this, StateMachine);
        GroundParryState = new PlayerGroundParryState(this, StateMachine);

        //initialize the direction
        IsFacingRight = true;

        PogoAttackAnim = "PogoAttack";
        GroundedUpAttackAnim = "";

        this.fixedDeltaTime = Time.fixedDeltaTime;
        MoveStats.timeInitialVelocity = MoveStats.timeInitialVelocityDefault;

        CurrentCoins = 10;

        UpdateCheckpoint(defaultSpawn.transform.position);


    }

    private void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        GhostTrail = GetComponent<GhostTrail>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        WallSlideParticles.gameObject.SetActive(false);


        startingColour = rend.color;

        //default state
        StateMachine.InitializeDefaultState(IdleState);

        if (SaveSystem.IsNewGame == false)
        {
            SaveSystem.Load();
        }

        //camera
        _cameraFollowObject = _cameraFollowGO.GetComponent<CameraFollowObject>();
        _fallSpeedYDampingChangeThreshold = CameraManager.instance._fallSpeedYDampingChangeThreshold;

    }

    private void Update()
    {
        StateMachine.CurrentState.StateUpdate();
        ResetTime();
        
        //if we are falling past a certain speed threshold
        if (RB.velocity.y < _fallSpeedYDampingChangeThreshold && !CameraManager.instance.IsLerpingYDamping && !CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(true);
        }

        //if we are standing still or moving up
        if (RB.velocity.y >= 0f && !CameraManager.instance.IsLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
        {
            //reset so it can be called again
            CameraManager.instance.LerpedFromPlayerFalling = false;

            CameraManager.instance.LerpYDamping(false);
        }
        
    }

    private void FixedUpdate()
    {
       
        StateMachine.CurrentState.StateFixedUpdate();
        
    }

    public void ApplyVelocity()
    {

        if (currentAnimatorState != "Parry")
        {
            //clamp speed
            if (!IsDashing)
            {
                ChangeVerticalVelocity(Mathf.Clamp(VerticalVelocity, -MoveStats.MaxFallSpeed, 50f));
            }
            else
            {
                ChangeVerticalVelocity(Mathf.Clamp(VerticalVelocity, -50f, 50f));
            }

            RB.velocity = new Vector2(HorizontalVelocity, VerticalVelocity);
        }
        else
        RB.velocity = new Vector2(HorizontalVelocity, 0);

    }

    public void ChangeVerticalVelocity(float changeAmount)
    {
        VerticalVelocity = changeAmount;
        //Debug.Log(VerticalVelocity);
    }

    public void IncrementVerticalVelocity(float incrementAmount)
    {
        VerticalVelocity += incrementAmount;
        //Debug.Log(VerticalVelocity);
    }

    #region Movement
    public void Move(float acceleration, float deceleration, Vector2 moveInput)
    {

        if (currentAnimatorState != "Parry")
        {
            if (!IsDashing)
            {
                float moveSpeed = 0f;
                if (InputManager.RunIsHeld)
                {
                    moveSpeed = MoveStats.MaxRunSpeed;
                }
                else { moveSpeed = MoveStats.MaxWalkSpeed; }

                if (Mathf.Abs(moveInput.x) > MoveStats.MoveThreshold)
                {
                    TurnCheck(moveInput);
                    float targetVelocity = moveInput.x * moveSpeed;
                    HorizontalVelocity = Mathf.Lerp(HorizontalVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
                }

                else
                {
                    HorizontalVelocity = Mathf.Lerp(HorizontalVelocity, 0f, deceleration * Time.deltaTime);

                }
            }
        }
        else
            HorizontalVelocity = Mathf.Lerp(HorizontalVelocity, 0f, 10 * Time.deltaTime);



    }

    private void TurnCheck(Vector2 moveInput)
    {
        if (IsFacingRight && moveInput.x < 0)
        {
            Turn(false);
        }

        else if (!IsFacingRight && moveInput.x > 0)
        {
            Turn(true);
        }
    }

    private void Turn(bool turnRight)
    {
        if (turnRight)
        {
            IsFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
            CameraManager.instance.CallCameraFaceDirection();
        }

        else
        {
            IsFacingRight = false;
            transform.Rotate(0f, -180f, 0f);
            CameraManager.instance.CallCameraFaceDirection();
        }
    }


    #endregion

    #region Landed
    public bool HasLanded()
    {
        if ((IsJumping || IsFalling || IsWallJumpFalling || IsWallJumping || IsWallSlideFalling || IsWallSliding || IsDashFastFalling ) && IsGrounded && VerticalVelocity <= 0f)
        {
            isPogoing = false;
            if (InputManager.Movement.x == 0)
            {
                HorizontalVelocity = 0;
            }


            ResetJumpValues();
            StopWallSliding();
            IsWallSlideFalling = false;
            ResetWallJumpValues();
            ResetDashes();

            ChangeVerticalVelocity(Physics2D.gravity.y);

            ReplenishJumps();
            TrailRenderer.emitting = false;

            if (IsDashFastFalling && IsGrounded)
            {
                if (!IsAirDashFalling)
                {
                    ResetDashValues();
                    return true;
                }
            }

            ResetDashValues();

            if (currentAnimatorState != "AirAttack")
            {
                ChangeAnimationState("Land");
                
            }
            

            //height tracker
            if (MoveStats.DebugShowHeightLogOnLand)
            {
                Debug.Log("Highest Point : " + (HighestPoint - HeightTrackerStartingPoint));
            }

            return true;
        }

        return false;
    }

    #endregion

    #region Jump
    #region Jump Inputs

    public void JumpInputChecks()
    {
        if (InputManager.JumpWasPressed)
        {
            //cancel if we should jump from a post wall jump buffer
            if (IsWallSlideFalling && WallJumpPostBufferTimer >= 0f)
            {
                return;
            }

            //cancel if we are wall sliding or touching a wall in the air
            else if (IsWallSliding || (IsTouchingWall && !IsGrounded))
            {
                return;
            }

            JumpWasPressed();
        }

        if (InputManager.JumpWasReleased)
        {
            JumpWasReleased();
        }
    }

    private void JumpWasPressed()
    {
        JumpBufferTimer = MoveStats.JumpBufferTime;
        JumpReleasedDuringBuffer = false;
    }

    private void JumpWasReleased()
    {
        if (JumpBufferTimer > 0f)
        {
            JumpReleasedDuringBuffer = true;
        }

        if (IsJumping && VerticalVelocity > 0f)
        {
            if (IsPastApexThreshold)
            {
                IsPastApexThreshold = false;
                IsFastFalling = true;
                FastFallTime = MoveStats.TimeForUpwardsCancel;

                //gets rid of floatiness
                ChangeVerticalVelocity(0f);

            }
            else
            {
                IsFastFalling = true;
                FastFallReleaseSpeed = VerticalVelocity;
            }
        }
    }

    #endregion

    #region Jump Checks

    public bool JumpBufferedOrCoyoteTimed()
    {
        if (JumpBufferTimer > 0f && !IsJumping && (IsGrounded || CoyoteTimer > 0f))
        {
            JumpBufferTimer = 0f;

            return true;
        }
        return false;
    }

    public bool CanJump()
    {
        //ACTUAL JUMP WITH COYOTE TIME AND JUMP BUFFER
        if (JumpBufferTimer > 0f && !IsJumping && (IsGrounded || CoyoteTimer > 0f))
        {
            if (IsDashFastFalling)
            {
                IsDashFastFalling = false;
            }

            JumpBufferTimer = 0f;
            return true;
        }
        return false;
    }

    public bool CanAirJump()
    {
        //handle double jump
        //if (JumpBufferTimer > 0f && IsJumping && NumberOfJumpsUsed < MoveStats.NumberOfJumpsAllowed)
        if (GetComponent<PlayerUnlocks>().DoubleJumpUnlocked == true)
        {
            if (JumpBufferTimer > 0f && (IsJumping || IsWallJumping || IsWallSlideFalling || IsAirDashing || IsDashFastFalling) && !IsTouchingWall && NumberOfJumpsUsed < MoveStats.NumberOfJumpsAllowed)
            {
                if (IsDashFastFalling)
                {
                    IsDashFastFalling = false;
                }

                JumpBufferTimer = 0f;
                IsFastFalling = false;
                return true;
            }
            else if ((JumpBufferTimer > 0f && IsFalling && !IsWallSlideFalling && NumberOfJumpsUsed < MoveStats.NumberOfJumpsAllowed - 1))
            {
                //NumberOfJumpsUsed++;
                JumpBufferTimer = 0f;
                IsFastFalling = false;
                return true;
            }
        }

        //handle air jump AFTER the coyote time has elapsed (subtract one extra jump here so we don't get a bonus jump)
        //else if (JumpBufferTimer > 0f && IsFalling && NumberOfJumpsUsed < MoveStats.NumberOfJumpsAllowed -1)
        else if ((JumpBufferTimer > 0f && IsFalling && !IsWallSlideFalling && NumberOfJumpsUsed < MoveStats.NumberOfJumpsAllowed - 1))
        {
            NumberOfJumpsUsed++;
            JumpBufferTimer = 0f;
            IsFastFalling = false;
            return true;
        }

        return false;
    }

    public void CheckForFalling()
    {

        //REGULAR FALLING
        if (!IsGrounded && !IsJumping && !IsFalling && !IsWallSliding && !IsWallJumping && !IsDashing && !IsDashFastFalling)
        {
            if (!IsFalling)
            {
                IsFalling = true;

                ChangeAnimationState("Fall");
               
            }
                
            StateMachine.ChangeState(InAirState);
        }
    }

    #endregion

    #region Jump Functions

    public void ResetJumpValues()
    {
        IsJumping = false;
        IsFalling = false;
        IsFastFalling = false;
        FastFallTime = 0f;
        IsPastApexThreshold = false;
    }

    public void ReplenishJumps()
    {
        NumberOfJumpsUsed = 0;
    }

    public void JumpPhysics()
    {
        if (IsJumping)
        {
            //HIT HEAD
            if (BumpedHead)
            {
                IsFastFalling = true;
            }

            if (VerticalVelocity >= 0f)
            {
                //APEX CONTROLS
                ApexPoint = Mathf.InverseLerp(MoveStats.InitialJumpVelocity, 0f, VerticalVelocity);

                if (ApexPoint > MoveStats.ApexThreshold)
                {
                    if (!IsPastApexThreshold)
                    {
                        IsPastApexThreshold = true;
                        TimePastApexThreshold = 0f;
                    }

                    if (IsPastApexThreshold)
                    {
                        TimePastApexThreshold += Time.fixedDeltaTime;
                        if (TimePastApexThreshold < MoveStats.ApexHangTime)
                        {
                            ChangeVerticalVelocity(0f);
                        }
                        else
                        {
                            //start moving downward
                            ChangeVerticalVelocity(-0.01f);
                        }
                    }
                }

                else if (!IsFastFalling)
                {
                    IncrementVerticalVelocity(MoveStats.Gravity * Time.fixedDeltaTime);

                    if (IsPastApexThreshold)
                    {
                        IsPastApexThreshold = false;
                    }
                }

            }

            else if (!IsFastFalling)
            {
                IncrementVerticalVelocity(MoveStats.Gravity * MoveStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime);
            }

            else if (VerticalVelocity < 0f)
            {
                if (!IsFalling)
                    IsFalling = true;
            }
        }

        //NORMAL FALLING (Without Jumping)
        if (IsFalling && !IsJumping && !IsGrounded)
        {
            IncrementVerticalVelocity(MoveStats.Gravity * Time.fixedDeltaTime);
        }

        //HANDLE RELEASED JUMP DECELERATION
        if (IsFastFalling)
        {
            if (FastFallTime >= MoveStats.TimeForUpwardsCancel)
            {
                IncrementVerticalVelocity(MoveStats.Gravity * MoveStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime);
            }
            else if (FastFallTime < MoveStats.TimeForUpwardsCancel)
            {
                ChangeVerticalVelocity(Mathf.Lerp(FastFallReleaseSpeed, 0f, (FastFallTime / MoveStats.TimeForUpwardsCancel)));
            }

            FastFallTime += Time.fixedDeltaTime;
        }

        if (MoveStats.DebugShowHeightLogOnLand)
        {
            if (HeightTracker.position.y > HighestPoint)
            {
                HighestPoint = HeightTracker.position.y;
            }
        }
    }
    
    public void InitiateJump()
    {

        if (isPogoing)
        {
            NumberOfJumpsUsed--;
            IsFastFalling = false;
            IsFalling = false;
            HorizontalVelocity = HorizontalVelocity * MoveStats.PogoHorizontalX;
            ChangeVerticalVelocity(MoveStats.InitialJumpVelocity * MoveStats.PogoVerticalX);
        }
        else
            ChangeVerticalVelocity(MoveStats.InitialJumpVelocity);

        ResetWallJumpValues();
        IsJumping = true;
        NumberOfJumpsUsed++;

        if (!isAnimationPlaying(animator, DashAnim))
        {
            //ChangeAnimationState("Jump");
        }

        if (!TrailRenderer.emitting)
        {
            TrailRenderer.emitting = true;
        }

        if (JumpReleasedDuringBuffer && !isPogoing)
        {
            IsFastFalling = true;
            FastFallReleaseSpeed = VerticalVelocity;
        }

        TrackHeight();
    }

    public void TrackHeight()
    {
        if (MoveStats.DebugShowHeightLogOnLand)
        {
            HeightTrackerStartingPoint = HeightTracker.position.y;
            HighestPoint = HeightTracker.position.y;
        }
    }



    #endregion
    #endregion

    #region Wall Slide

    public bool ShouldWallSlide()
    {
        if (GetComponent<PlayerUnlocks>().WallJumpSlideUnlocked == true && !InputManager.Movement.Equals(Vector2.zero))
        {
            if (IsTouchingWall && !IsGrounded && !IsDashing)
            {
                if (VerticalVelocity < 0f && !IsWallSliding)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool ShouldStopWallSliding()
    {
        if (IsWallSliding && !IsTouchingWall && !IsGrounded && !IsWallSlideFalling || InputManager.Movement.Equals(Vector2.zero))
        {
            ChangeAnimationState("Fall");
            return true;
        }

        return false;
    }

    public void StopWallSliding()
    {
        if (IsWallSliding)
        {
            NumberOfJumpsUsed++;

            IsWallSliding = false;
            IsAirDashFalling = false;

        }
    }

    #endregion

    #region Wall Jump

    public bool CanWallJumpDueToPostBufferTimer()
    {
        if (WallJumpPostBufferTimer > 0f)
        {
            return true;
        }

        return false;
    }

    public void WallJumpChecks()
    {
        if (ShouldApplyPostWallJumpBuffer())
        {
            WallJumpPostBufferTimer = MoveStats.WallJumpPostBufferTime;
        }

        //START FAST FALLING
        if (InputManager.JumpWasReleased)
        {
            WallJumpWasReleased();
        }
    }

    private void WallJumpWasReleased()
    {
        if (!IsWallSliding && !IsTouchingWall && IsWallJumping)
        {
            if (IsWallJumping && VerticalVelocity > 0f)
            {
                if (IsPastWallJumpApexThreshold)
                {
                    IsPastWallJumpApexThreshold = false;
                    IsWallJumpFastFalling = true;
                    WallJumpFastFallTime = MoveStats.TimeForUpwardsCancel;

                    //gets rid of floatiness
                    ChangeVerticalVelocity(0f);
                }
                else
                {
                    IsWallJumpFastFalling = true;
                    WallJumpFastFallReleaseSpeed = VerticalVelocity;
                }
            }
        }
    }

    public void WallJumpPhysics()
    {
        //APPLY WALL JUMP GRAVITY
        if (IsWallJumping)
        {
            //TIME TO TAKE OVER MOVEMENT CONTROLS WHILE WALL JUMPING
            WallJumpTime += Time.fixedDeltaTime;
            if (WallJumpTime >= MoveStats.TimeTillJumpApex)
            {
                UseWallJumpMoveStats = false;
            }

            //HIT HEAD
            if (BumpedHead)
            {
                IsWallJumpFastFalling = true;
                UseWallJumpMoveStats = false;
            }

            //GRAVITY IN ASCENDING
            if (VerticalVelocity >= 0f)
            {
                //APEX CONTROLS
                WallJumpApexPoint = Mathf.InverseLerp(MoveStats.WallJumpDirection.y, 0f, VerticalVelocity);

                if (WallJumpApexPoint > MoveStats.ApexThreshold)
                {
                    if (!IsPastWallJumpApexThreshold)
                    {
                        IsPastWallJumpApexThreshold = true;
                        TimePastWallJumpApexThreshold = 0f;
                    }

                    if (IsPastWallJumpApexThreshold)
                    {
                        TimePastWallJumpApexThreshold += Time.fixedDeltaTime;
                        if (TimePastWallJumpApexThreshold < MoveStats.ApexHangTime)
                        {
                            ChangeVerticalVelocity(0f);
                        }
                        else
                        {
                            ChangeVerticalVelocity(-0.01f);
                        }
                    }
                }

                //GRAVITY IN ASCENDING BUT NOT PAST APEX THRESHOLD
                else if (!IsWallJumpFastFalling)
                {
                    IncrementVerticalVelocity(MoveStats.WallJumpGravity * Time.fixedDeltaTime);

                    if (IsPastWallJumpApexThreshold)
                    {
                        IsPastWallJumpApexThreshold = false;
                    }
                }

            }

            //GRAVITY ON DESCENDING
            else if (!IsWallJumpFastFalling)
            {
                IncrementVerticalVelocity(MoveStats.WallJumpGravity * Time.fixedDeltaTime);
            }

            else if (VerticalVelocity < 0f)
            {
                if (!IsWallJumpFalling)
                    IsWallJumpFalling = true;
            }

        }

        //WALL JUMP FALLING
        if (IsWallJumpFastFalling)
        {
            if (WallJumpFastFallTime >= MoveStats.TimeForUpwardsCancel)
            {
                IncrementVerticalVelocity(MoveStats.WallJumpGravity * MoveStats.WallJumpGravityOnReleaseMultiplier * Time.fixedDeltaTime);
            }
            else if (WallJumpFastFallTime < MoveStats.TimeForUpwardsCancel)
            {
                ChangeVerticalVelocity(Mathf.Lerp(WallJumpFastFallReleaseSpeed, 0f, (WallJumpFastFallTime / MoveStats.TimeForUpwardsCancel)));
            }

            WallJumpFastFallTime += Time.fixedDeltaTime;
        }
    }

    public void InitiateWallJump(GameObject particlesToSpawn = null)
    {
        if (GetComponent<PlayerUnlocks>().WallJumpSlideUnlocked == true)
        {
            if (!IsWallJumping)
            {
                IsWallJumping = true;
                UseWallJumpMoveStats = true;
            }

            StopWallSliding();

            ResetJumpValues();
            WallJumpTime = 0f;
            ChangeVerticalVelocity(MoveStats.InitialWallJumpVelocity);

            int dirMultiplier = 0;
            Vector2 hitDir = _lastWallHit.collider.ClosestPoint(WallSlideColl.bounds.center);

            if (hitDir.x > transform.position.x)
            {
                dirMultiplier = -1;
            }
            else { dirMultiplier = 1; }

            HorizontalVelocity = ((Mathf.Abs(MoveStats.WallJumpDirection.x)) * dirMultiplier);

            //FX
            if (!isAnimationPlaying(animator, DashAnim))
            {
                //ChangeAnimationState("Jump");
            }

            TrailRenderer.emitting = true;

            //Instantiate(particlesToSpawn, _particleSpawnTransform.position, Quaternion.identity);
        }
    }

    private bool ShouldApplyPostWallJumpBuffer()
    {
        if (!IsGrounded && (IsTouchingWall || IsWallSliding))
        {
            return true;
        }
        else { return false; }
    }

    public void ResetWallJumpValues()
    {
        IsWallSlideFalling = false;
        UseWallJumpMoveStats = false;
        IsWallJumping = false;
        IsWallJumpFastFalling = false;
        IsWallJumpFalling = false;
        IsPastWallJumpApexThreshold = false;

        WallJumpFastFallTime = 0f;
        WallJumpTime = 0f;
    }

    #endregion

    #region Dash

    public void ResetDashes()
    {
        NumberOFDashesUsed = 0;
    }

    public void ResetDashValues()
    {
        IsDashFastFalling = false;
        DashOnGroundTimer = -0.01f;
        IsAirDashFalling = false;
    }

    public bool CanDash()
    {
        //ground dash
        if (IsGrounded && DashOnGroundTimer < 0 && !IsDashing)
        {
            return true;
        }
        return false;
    }


    public bool CanPogo()
    {
        if (GetComponent<PlayerUnlocks>().PogoAttackUnlocked == true && VerticalVelocity < 0f)
        {
            return true;
        }
        else
            return false;
    }


    public bool CanAirDash()
    {
        if (GetComponent<PlayerUnlocks>().AirDashUnlocked == true)
        {
            //air dash
            if (!IsGrounded && !IsDashing && NumberOFDashesUsed < MoveStats.NumberOfDashes)
            {
                IsAirDashing = true;

                //you left a wallslide but dashed within the wallJumpPostBufferTimer
                if (WallJumpPostBufferTimer > 0f)
                {
                    NumberOfJumpsUsed--;
                    if (NumberOfJumpsUsed < 0)
                    {
                        NumberOfJumpsUsed = 0;
                    }
                }
                return true;
            }
            return false;
        }
        else
            return false;
    }

    public void InitiateDash()
    {
        CheckForDashUnlock();
        if (IsJumping && GetComponent<PlayerUnlocks>().AirDashUnlocked == false)
        {
            return;
        }
        else
        {
            DashDirection = InputManager.Movement;

            Vector2 closestDirection = Vector2.zero;
            float minDistance = Vector2.Distance(DashDirection, MoveStats.DashDirections[0]);

            if (GetComponent<PlayerUnlocks>().AirDashUnlocked == true)
            {
                for (int i = 0; i < MoveStats.DashDirections.Length; i++)
                {
                    //skip if we hit it bang on
                    if (DashDirection == MoveStats.DashDirections[i])
                    {
                        closestDirection = DashDirection;
                        break;
                    }

                    float distance = Vector2.Distance(DashDirection, MoveStats.DashDirections[i]);

                    // Check if this is a diagonal direction and apply bias
                    bool isDiagonal = (Mathf.Abs(MoveStats.DashDirections[i].x) == 1 && Mathf.Abs(MoveStats.DashDirections[i].y) == 1);
                    if (isDiagonal)
                    {
                        distance -= MoveStats.DashDiagonallyBias;
                    }

                    else if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestDirection = MoveStats.DashDirections[i];
                    }
                }
            }




            //handle direction if we have no input
            if (closestDirection == Vector2.zero)
            {
                if (IsFacingRight)
                {
                    closestDirection = Vector2.right;
                }
                else { closestDirection = Vector2.left; }
            }

            DashDirectionMult = 1; //this may not be needed
            DashDirection = new Vector2(closestDirection.x * DashDirectionMult, closestDirection.y * DashDirectionMult);

            NumberOFDashesUsed++;
            IsDashing = true;
            DashTimer = 0f;
            DashOnGroundTimer = MoveStats.TimeBtwDashesOnGround;

            //FX
            Quaternion particleRot = Quaternion.FromToRotation(Vector2.right, -DashDirection);
            if (GetComponent<PlayerUnlocks>().AirDashUnlocked == true || GetComponent<PlayerUnlocks>().DashUnlocked == true)
            {
                Instantiate(DashParticles, transform.position, particleRot);
            }

            ChangeAnimationState(DashAnim);

            GhostTrail.LeaveGhostTrail(MoveStats.DashTime * 1.75f);

            ResetJumpValues();
            ResetWallJumpValues();
            StopWallSliding();
        }
    }

    public void DashPhysics()
    {

        float RollDashTime;

        if (IsDashing)
        {
            //StartCoroutine(InvulnerabilityDash());
            //stop the dash after the timer
            DashTimer += Time.fixedDeltaTime;

            if (GetComponent<PlayerUnlocks>().AirDashUnlocked == true || GetComponent<PlayerUnlocks>().DashUnlocked == true)
            {
                RollDashTime = MoveStats.DashTime;
            }
            else
            {
                RollDashTime = MoveStats.RollTime;
                if(!IsJumping)
                {
                    IsDashFastFalling = true;
                }
            }


            if (DashTimer >= RollDashTime)
            {
                if (IsGrounded)
                {
                    ResetDashes();
                }
                else { IsAirDashFalling = true; }

                IsAirDashing = false;
                IsDashing = false;



                //start the time for upwards cancel
                if (!IsJumping && !IsWallJumping)
                {
                    DashFastFallTime = 0f;
                    DashFastFallReleaseSpeed = VerticalVelocity;

                    IsDashFastFalling = true;
                    if (!IsGrounded)
                        if (!isAnimationPlaying(animator, DashAnim))
                        {
                            ChangeAnimationState("Fall");
                        }
                }

                return;
            }

            if (GetComponent<PlayerUnlocks>().DashUnlocked)
            {
                HorizontalVelocity = MoveStats.DashSpeed * DashDirection.x;

                if (GetComponent<PlayerUnlocks>().AirDashUnlocked && DashDirection.y != 0f || IsAirDashing)
                    ChangeVerticalVelocity(MoveStats.DashSpeed * DashDirection.y);
            }
            else if(GetComponent<PlayerUnlocks>().AirDashUnlocked)
            {
                HorizontalVelocity = MoveStats.DashSpeed * DashDirection.x;

                if (GetComponent<PlayerUnlocks>().AirDashUnlocked && DashDirection.y != 0f || IsAirDashing)
                    ChangeVerticalVelocity(MoveStats.DashSpeed * DashDirection.y);
            }
            else
            {
                HorizontalVelocity = MoveStats.RollSpeed * DashDirection.x;
            }
        }

        //HANDLE DASH CUT TIME
        else if (IsDashFastFalling)
        {
            //new
            if (VerticalVelocity > 0f)
            {
                if (DashFastFallTime < MoveStats.DashTimeForUpwardsCancel)
                {
                    ChangeVerticalVelocity(Mathf.Lerp(DashFastFallReleaseSpeed, 0f, (DashFastFallTime / MoveStats.DashTimeForUpwardsCancel)));
                }
                else if (DashFastFallTime >= MoveStats.DashTimeForUpwardsCancel)
                {
                    IncrementVerticalVelocity(MoveStats.Gravity * MoveStats.DashGravityOnReleaseMultiplier * Time.fixedDeltaTime);
                }

                DashFastFallTime += Time.fixedDeltaTime;
            }
            else
            {
                IncrementVerticalVelocity(MoveStats.Gravity * MoveStats.DashGravityOnReleaseMultiplier * Time.fixedDeltaTime);
            }
        }
    }

    public void DashJumpDelay()
    {

        float duration = 1f;
        StartCoroutine(WaitHoldingPosition(duration));


    }
    IEnumerator WaitHoldingPosition(float duration)
    {
        //Debug.Log($"Started at {Time.time}, waiting for {duration} seconds");
        yield return new WaitForSeconds(duration);
        //Debug.Log($"Ended at {Time.time}");
        if (CanJump())
        {
            SpawnJumpParticles(JumpParticles);

            StateMachine.ChangeState(JumpState);
        }

    }

    #endregion

    #region Collision Checks

    public void CollisionChecks()
    {
        CheckForGrounded();
        CheckForBumpedHead();
        CheckForTouchingWall();
    }

    private void CheckForGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(FeetColl.bounds.center.x, FeetColl.bounds.min.y);
        Vector2 boxCastSize = new Vector2(FeetColl.bounds.size.x, MoveStats.GroundDetectionRayLength);

        _groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, MoveStats.GroundDetectionRayLength, MoveStats.GroundLayer);
        if (_groundHit.collider != null)
        {
            IsGrounded = true;
       
        }
        else { IsGrounded = false; }

        #region Debug Visualization
        if (MoveStats.DebugShowIsGroundedBox)
        {
            Color rayColor;
            if (IsGrounded)
            {
                rayColor = Color.green;
            }
            else { rayColor = Color.red; }

            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * MoveStats.GroundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * MoveStats.GroundDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - MoveStats.GroundDetectionRayLength), Vector2.right * boxCastSize.x, rayColor);
        }
        #endregion
    }

    private void CheckForBumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(HeadColl.bounds.center.x, HeadColl.bounds.max.y);
        Vector2 boxCastSize = new Vector2(HeadColl.bounds.size.x * MoveStats.HeadWidth, MoveStats.HeadDetectionRayLength);

        _headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, MoveStats.HeadDetectionRayLength, MoveStats.GroundLayer);
        if (_headHit.collider != null)
        {
            BumpedHead = true;
        }
        else { BumpedHead = false; }

        #region Debug Visualization

        if (MoveStats.DebugShowHeadBumpBox)
        {
            float headWidth = MoveStats.HeadWidth;

            Color rayColor;
            if (BumpedHead)
            {
                rayColor = Color.green;
            }
            else { rayColor = Color.red; }

            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, boxCastOrigin.y), Vector2.up * MoveStats.HeadDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + (boxCastSize.x / 2) * headWidth, boxCastOrigin.y), Vector2.up * MoveStats.HeadDetectionRayLength, rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, boxCastOrigin.y + MoveStats.HeadDetectionRayLength), Vector2.right * boxCastSize.x * headWidth, rayColor);
        }

        #endregion
    }

    private void CheckForTouchingWall()
    {
        if (GetComponent<PlayerUnlocks>().WallJumpSlideUnlocked == true)
        {
            float originEndPoint = 0f;
            if (IsFacingRight)
            {
                originEndPoint = WallSlideColl.bounds.max.x;
            }
            else { originEndPoint = WallSlideColl.bounds.min.x; }

            float adjustedHeight = WallSlideColl.bounds.size.y * MoveStats.WallDetectionRayHeightMultiplier;
            Vector2 boxCastOrigin = new Vector2(originEndPoint, WallSlideColl.bounds.center.y);
            Vector2 boxCastSize = new Vector2(MoveStats.WallDetectionRayLength, adjustedHeight);

            WallHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, transform.right, MoveStats.WallDetectionRayLength, MoveStats.WallSlideLayer);
            if (WallHit.collider != null)
            {
                _lastWallHit = WallHit;
                IsTouchingWall = true;
            }
            else { IsTouchingWall = false; }

            #region Debug Visualization

            if (MoveStats.DebugShowWallHitBox)
            {
                Color rayColor;
                if (IsTouchingWall)
                {
                    rayColor = Color.green;
                }
                else { rayColor = Color.red; }

                Vector2 boxBottomLeft = new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - boxCastSize.y / 2);
                Vector2 boxBottomRight = new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y - boxCastSize.y / 2);
                Vector2 boxTopLeft = new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y + boxCastSize.y / 2);
                Vector2 boxTopRight = new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y + boxCastSize.y / 2);

                Debug.DrawLine(boxBottomLeft, boxBottomRight, rayColor);
                Debug.DrawLine(boxBottomRight, boxTopRight, rayColor);
                Debug.DrawLine(boxTopRight, boxTopLeft, rayColor);
                Debug.DrawLine(boxTopLeft, boxBottomLeft, rayColor);
            }

            #endregion
        }
        
    }

    #endregion

    #region Timers

    public void JumpTimers()
    {
        JumpBufferTimer -= Time.deltaTime;

        //HANDLE COYOTE TIMER
        if (!IsGrounded)
        {
            CoyoteTimer -= Time.deltaTime;
        }
        else { CoyoteTimer = MoveStats.JumpCoyoteTime; }
    }

    public void WallJumpTimers()
    {
        //HANDLE WALL JUMP BUFFER TIMER
        if (!ShouldApplyPostWallJumpBuffer())
        {
            WallJumpPostBufferTimer -= Time.deltaTime;
        }
    }

    public void DashTimers()
    {
        //HANDLE DASH TIMER
        if (IsGrounded)
        {
            DashOnGroundTimer -= Time.deltaTime;
        }
    }

    #endregion

    #region FX

    public void SpawnJumpParticles(GameObject particlesToSpawn)
    {
        Instantiate(particlesToSpawn, ParticleSpawnTransform.position, Quaternion.identity);
    }

    public void SpawnHitParticles(GameObject particlesToSpawn)
    {
        Instantiate(particlesToSpawn, this.transform.position, Quaternion.identity);
    }
    public void AttackSFX()
    {
        AttackSFXCalc = Random.Range(1, 3);

        if (AttackSFXCalc == 1)
        {
            audiosource.PlayOneShot(SwordSwing1, 5);
        }
        else if (AttackSFXCalc == 2)
        {
            audiosource.PlayOneShot(SwordSwing2, 5);
        }
    }

    #endregion

    #region Jump Visualition Tool

    private void OnDrawGizmos()
    {
        if (MoveStats.ShowWalkJumpArc)
        {
            DrawJumpArc(MoveStats.MaxWalkSpeed, Color.white);
        }

        if (MoveStats.ShowRunJumpArc)
        {
            DrawJumpArc(MoveStats.MaxRunSpeed, Color.red);
        }
    }

    private void DrawJumpArc(float moveSpeed, Color gizmoColor)
    {
        Vector2 startPosition = new Vector2(FeetColl.bounds.center.x, FeetColl.bounds.min.y);
        Vector2 previousPosition = startPosition;
        float speed = 0f;
        if (MoveStats.DrawRight)
        {
            speed = moveSpeed;
        }
        else { speed = -moveSpeed; }
        Vector2 velocity = new Vector2(speed, MoveStats.InitialJumpVelocity);

        Gizmos.color = gizmoColor;

        float timeStep = 2 * MoveStats.TimeTillJumpApex / MoveStats.ArcResolution;

        for (int i = 0; i < MoveStats.VisualizationSteps; i++)
        {
            float simulationTime = i * timeStep;
            Vector2 displacement;
            Vector2 drawPoint;
            float downTime = MoveStats.Gravity * MoveStats.GravityOnReleaseMultiplier;

            //ascending
            if (simulationTime < MoveStats.TimeTillJumpApex)
            {
                displacement = velocity * simulationTime + 0.5f * new Vector2(0, MoveStats.Gravity) * simulationTime * simulationTime;
            }

            //apex hang time
            else if (simulationTime < MoveStats.TimeTillJumpApex + MoveStats.ApexHangTime)
            {
                float apexTime = simulationTime - MoveStats.TimeTillJumpApex;
                displacement = velocity * MoveStats.TimeTillJumpApex + 0.5f * new Vector2(0, MoveStats.Gravity) * MoveStats.TimeTillJumpApex * MoveStats.TimeTillJumpApex;
                displacement += new Vector2(speed, 0) * apexTime;
            }

            //descending
            else
            {
                float descendTime = simulationTime - (MoveStats.TimeTillJumpApex + MoveStats.ApexHangTime);
                displacement = velocity * MoveStats.TimeTillJumpApex + 0.5f * new Vector2(0, MoveStats.Gravity) * MoveStats.TimeTillJumpApex * MoveStats.TimeTillJumpApex;
                displacement += new Vector2(speed, 0) * MoveStats.ApexHangTime;

                downTime *= descendTime * descendTime;


                displacement += new Vector2(speed, 0) * descendTime + 0.5f * new Vector2(0, downTime);
            }

            drawPoint = startPosition + displacement;

            if (MoveStats.StopOnCollision)
            {
                RaycastHit2D hit = Physics2D.Raycast(previousPosition, drawPoint - previousPosition, Vector2.Distance(previousPosition, drawPoint), MoveStats.GroundLayer);
                if (hit.collider != null)
                {
                    // If a hit is detected, stop drawing the arc at the hit point
                    Gizmos.DrawLine(previousPosition, hit.point);
                    break;
                }
            }

            Gizmos.DrawLine(previousPosition, drawPoint);
            previousPosition = drawPoint;
        }
    }











    #endregion

    #region Change Animation State

    public void CheckForDashUnlock()
    {
        if (GetComponent<PlayerUnlocks>().AirDashUnlocked == true && (GetComponent<PlayerUnlocks>().AirDashResetOnWallUnlocked == true))
        {
            DashAnim = "AirDash";
            MoveStats.ResetDashOnWallSlide = true;
        } else if  (GetComponent<PlayerUnlocks>().AirDashUnlocked == true)
        {
            DashAnim = "AirDash";   
        }
        else if (GetComponent<PlayerUnlocks>().DashUnlocked == true)
        {
            DashAnim = "AirDash";
            
        }else
            DashAnim = "Roll";
    }
    public void ChangeAnimationState(string newAnimationState)
    {
        

        //if Dashing
        if (currentAnimatorState == DashAnim && newAnimationState == DashAnim)
        {
            //wipe and play dash, this allows double dash to still play correct animation
            animator.Play(null);
            animator.Play(newAnimationState);

        } else if (newAnimationState == currentAnimatorState) //don't allow other animations to repeat
        {
            return;
        }



        animator.Play(newAnimationState);
        currentAnimatorState = newAnimationState;
    }
    public void MidAnimationTransitionState(string newAnimationState, int _animationLayer, float _animationTime)
    {
        animator.Play(newAnimationState, _animationLayer, _animationTime);
        currentAnimatorState = newAnimationState;
    }

    public bool isAnimationPlaying(Animator animator, string AnimationStateName)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(AnimationStateName) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime <1.0f)
        {
            return true;
        }else
        {
            return false;
        }
    }
    #endregion

    #region Combat

    #region Attack
    public bool CanAttack()
    {
        if (Time.time > NextAttackTime)
        {
            NextAttackTime = Time.time + MoveStats.TimeBtwAttacks;
            return true;

        } else
            return false;
    }

    public bool CanIdleUpAttack()
    {
        if (GetComponent<PlayerUnlocks>().IdleUpAttackUnlocked)
        {
            return true;
        }
        return false;
    }    

    #endregion

    #region Parry
    public bool CanParry()
    {
        if (Time.time > NextParryTime && GetComponent<PlayerUnlocks>().ParryUnlocked == true)
        {
            NextParryTime = Time.time + MoveStats.TimeBtwParry;
            return true;

        }
        else
            return false;
    }
    #endregion

    #region Recieve Damage
    public void Damage(float damageAmount, string damageType)
    {

 

        if (!Invunerable)
        {
            SpawnHitParticles(HitParticles);
            CameraShakemanager.instance.CameraShake(impulseSource);

            CurrentHealth = Mathf.Clamp(CurrentHealth - damageAmount, 0, StartingHealth);


            //damage pop up
            //Vector3 randomness = new Vector2(Random.Range(0f, 0.5f), Random.Range(0f, 0.5f));
            //DamagePopUpGenerator.current.CreatePopUp(transform.position + randomness, damageAmount.ToString(), Color.yellow);

            if (CurrentHealth > 0)
            {
                audiosource.PlayOneShot(Hurt, .5f);
                Slowtime(MoveStats.hurtSlowTimeAmount);
                StartCoroutine(Invulnerability());
            }
            else
            {
                Slowtime(MoveStats.deathSlowTimeAmount);
                isDead = true;
                StateMachine.InitializeDefaultState(DeathState);
            }
        }

    }
    private IEnumerator Invulnerability()
    {
        isHurt = true;
        Invunerable = true;
        Physics2D.IgnoreLayerCollision(12, 10, true);
        Physics2D.IgnoreLayerCollision(9, 10, true);
        for (int i = 0; i < NumOfFlashes; i++)
        {
            rend.color = Color.red;
            yield return new WaitForSeconds(InvulnerabilityDuration / (NumOfFlashes * 2));
            rend.color = startingColour;
            yield return new WaitForSeconds(InvulnerabilityDuration / (NumOfFlashes * 2));
        }
        Physics2D.IgnoreLayerCollision(12, 10, false);
        Physics2D.IgnoreLayerCollision(9, 10, false);
        isHurt = false;
        Invunerable = false;
    }
    private IEnumerator InvulnerabilityDash()
    {

        isHurt = true;
        Invunerable = true;
        Physics2D.IgnoreLayerCollision(12, 10, true);
        for (int i = 0; i < NumOfFlashes; i++)
        {
            rend.color = Color.cyan;
            yield return new WaitForSeconds(InvulnerabilityDuration / (NumOfFlashes * 2));
            rend.color = Color.black;
            yield return new WaitForSeconds(InvulnerabilityDuration / (NumOfFlashes * 2));
        }
        Physics2D.IgnoreLayerCollision(12, 10, false);
        isHurt = false;
        Invunerable = false;
    }


    #endregion

    #region SlowTimeFX

    

    public void Slowtime(float slowTimeAmount)
    {

        if (MoveStats.HitStop == true)
        {
            backgroundMusic.pitch = 0.3f;
            Time.timeScale = slowTimeAmount;
            Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
        }

    }

    public void ResetTime()
    {
        
        if (MoveStats.HitStop == true)
        {
            MoveStats.timeInitialVelocity = MoveStats.timeInitialVelocityDefault;
            if (Time.timeScale != 1.0f && !pauseMenu.GetComponent<PauseMenu>().GameIsPaused)
            {

                Time.timeScale = Mathf.SmoothDamp(Time.timeScale, 1.0f, ref MoveStats.timeInitialVelocity, MoveStats.smoothTime);
                backgroundMusic.pitch = Mathf.SmoothDamp(backgroundMusic.pitch, 1.0f, ref MoveStats.timeInitialVelocity, MoveStats.smoothTime);
            }
            else if (Time.timeScale >= 0.90 && Time.timeScale <= 1.00 && !pauseMenu.GetComponent<PauseMenu>().GameIsPaused)
            {
                backgroundMusic.pitch = 1;
                MoveStats.timeInitialVelocity = MoveStats.timeInitialVelocityDefault;
            }
            Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
        }
    }



    #endregion

    #region Checkpoint/respawn
    public void Die()
    {
        StartCoroutine(Respawn(1f));

    }

    IEnumerator Respawn(float duration)
    {
        
        IsDashing = false;

        //disable music
        this.GetComponent<MusicManager>().TrackPicker = 0;
        this.GetComponent<MusicManager>().ToggleChange = true;

        //disasble RB
        this.RB.simulated = false;
        

        yield return new WaitForSeconds(duration);

        //reset healt
        isDead = false;
        CurrentHealth = StartingHealth;

        //set position to last checkpoint
        this.transform.position = checkpointPos;
        //set default respawn
        UpdateCheckpoint(defaultSpawn.transform.position);


        if (!IsFacingRight)
        {
            Turn(true);
        }

        //enable RB
        this.RB.simulated = true;
        //remove any momentum
        HorizontalVelocity = 0f;
        VerticalVelocity = 0f;
        ChangeVerticalVelocity(Mathf.Clamp(VerticalVelocity, 0, 0));
        RB.velocity = new Vector2(HorizontalVelocity, VerticalVelocity);

        //set spawn state and animations
        ChangeAnimationState("Idle");
        StateMachine.ChangeState(IdleState);


    }

    public void UpdateCheckpoint(Vector2 pos)
    {
        checkpointPos = pos;
    }
    #endregion


    #endregion
    #region Save Data

    public void Save(ref PlayerSaveData data)
    {
        data.Position = transform.position;
        data.CurrentCoins = CurrentCoins;
        data.CurrentHealth = CurrentHealth;
        data.checkpointPos = checkpointPos;
    }

    public void Load(PlayerSaveData data)
    {
        transform.position = data.checkpointPos;
        CurrentCoins = data.CurrentCoins;
        CurrentHealth = data.CurrentHealth;
        checkpointPos = data.checkpointPos;
    }

    
    #endregion

}
[System.Serializable]
public struct PlayerSaveData
{
    public Vector3 Position;
    public float CurrentCoins;
    public float CurrentHealth;
    public Vector3 checkpointPos;
}


