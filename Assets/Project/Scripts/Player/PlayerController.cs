using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterController m_charController;
    [SerializeField] PlayerProfile m_settings;

    [SerializeField] UnityEvent m_changeStateEvent;
    [SerializeField] UnityEvent m_onGroundedEvent;
    [SerializeField] UnityEvent m_onAirborneEvent;
    [SerializeField] UnityEvent m_onWallRunEvent;
    [SerializeField] UnityEvent m_onWallClimbEvent;

    public PlayerProfile settings { get { return m_settings; } set { m_settings = value; } }

    // Movement Var.
    Vector3 m_velocity = Vector3.zero;
    float m_currentSpeed = 0.0f;
    Vector3 m_heading = Vector3.forward;

    Vector3 m_lateralVelocity = Vector3.zero;
    float m_lateralSpeed = 0.0f;
    Vector3 m_lateralHeading = Vector3.forward;

    Vector3 m_forceAccumulator = Vector3.zero;
    Vector3 m_impulseAccumulator = Vector3.zero;

    public Vector3 velocity { get { return m_velocity; } set { SetVelocity(value); } }
    public float currentSpeed { get { return (m_currentSpeed); } }
    public Vector3 heading { get { return m_heading; } }

    public Vector3 lateralVelocity { get { return m_velocity; } }
    public float lateralSpeed { get { return (m_lateralSpeed); } }
    public Vector3 lateralHeading { get { return m_heading; } }

    public Vector3 totalVelocity { get { return (Vector3.up * m_fallingVelocity + m_velocity); } }

    // Current State
    PlayerState m_currentState = PlayerState.Grounded;
    public PlayerState currentState { get { return m_currentState; } }

    // Crouching Var
    bool m_isCrouched = false;

    float m_targetHeightSmooth = 1.0f;
    float m_smoothCrouchScale = 1.0f;
    float m_smoothCrouchVel = 0.0f;

    public bool isCrouched { get { return m_isCrouched; } }
    public float smoothCrouchScale { get { return m_smoothCrouchScale; } }

    // Jumping and Gravity Var.
    SimpleTimer m_jumpLockOutTimer = new SimpleTimer();

    float m_fallingVelocity = 0.0f;

    bool m_isJumping = false;
    bool m_touchedGrass = false;

    public float fallingVelocity { get { return m_fallingVelocity; } set { m_fallingVelocity = value; } }

    // Ground Detection Var.
    Vector3 m_groundNormal = Vector3.up;
    bool m_groundIsDetected = false;

    public Vector3 groundNormal { get { return m_groundNormal; } }
    public bool groundIsDetected { get { return m_groundIsDetected; } }
    public bool isGrounded { get { return m_currentState == PlayerState.Grounded; } }
    public bool isGroundDetected { get { return m_groundIsDetected; } }

    // Wall Interaction Var.

    // Wall Deteciton
    // Wall running
    private RaycastHit m_leftWallhit;
    private RaycastHit m_rightWallhit;
    private bool m_wallLeft;
    private bool m_wallRight;

    // Climbing
    RaycastHit m_frontWallHit;
    bool m_frontWall = false;

    public RaycastHit leftWallhit { get { return m_leftWallhit; } }
    public RaycastHit rightWallhit { get { return m_rightWallhit; } }
    public bool wallLeft { get { return m_wallLeft; } }
    public bool wallRight { get { return m_wallRight; } }

    public RaycastHit frontWallHit { get { return m_frontWallHit; } }
    public bool frontWall { get { return m_frontWall; } }

    // Wall Interaction playability
    int m_wallRunsLeft = 0;
    public int wallRunsLeft { get { return m_wallRunsLeft; } }
    SimpleTimer m_wallInteractTimer = new SimpleTimer();

    // Inputs
    [Header("Inputs")]
    [SerializeField] float m_inputScale = 1.0f;
    Vector3 m_moveInput = Vector3.zero;
    Vector3 m_worldMoveInput = Vector3.zero;

    public float inputScale { get { return m_inputScale; } set { m_inputScale = value; } }
    public Vector3 moveInput { get { return m_moveInput; } set { SetMoveInput(value); } }
    public Vector3 worldMoveInput { get { return m_worldMoveInput; } }

    public bool inputDisabled { get { return m_inputScale == 0.0f; } set { m_inputScale = System.Convert.ToSingle(!value); } }

    // Movement Var
    float m_targetBrakeScale;
    float m_targetMaxSpeed;

    float m_speedScale = 1.0f;

    public float speedScale { get { return m_speedScale; } set { m_speedScale = value; } }

    public enum PlayerState
    {
        Grounded,
        Airborne,
        Jumping,
        WallRunning,
        WallClimbing
    }

    private void Start()
    {
        m_smoothCrouchScale = 1.0f;
        Stand();
    }

    private void Update()
    {
        SmoothHeightTowards(m_targetHeightSmooth);
    }

    //public float MaxPlayerSpeed { get { return (m_maxPlayerSpeed); } set { m_maxPlayerSpeed = value; } }
    //public float CurrentPlayerSpeed { get { return (m_lateralSpeed); } set { m_lateralSpeed = value; } }
    // Update is called once per frame
    void LateUpdate()
    {
        Process(Time.deltaTime);
    }

    public void SetMoveInput(Vector3 input)
    {
        m_moveInput = input;
        m_worldMoveInput = transform.TransformDirection(input);
    }

    #region Physics
    public void AddForce(Vector3 force)
    {
        m_forceAccumulator += force;
    }

    public void AddImpulse(Vector3 impulse)
    {
        m_impulseAccumulator += impulse;
    }

    void Process(float deltaTime)
    {
        m_isJumping = !m_jumpLockOutTimer.Tick(deltaTime);
        CheckForWall();
        ProcessMovementState(m_currentState);
        ProcessMovement(deltaTime);
    }

    // This calculates an Impulse vector by using the accumulators.
    // Sets the accumulators to zero.
    Vector3 ConsumeAccumulators(float deltaTime)
    {
        var acceleration = m_forceAccumulator;
        m_forceAccumulator = Vector3.zero;

        var impulse = m_impulseAccumulator + acceleration * deltaTime;
        m_impulseAccumulator = Vector3.zero;

        return impulse;
    }

    Vector3 CalculateMoveInputAcceleration(float acceleration)
    {
        // Limit player movement if they are moving faster than their max speed.
        var moveInput = ReorientSlope(m_worldMoveInput) * m_inputScale;
        var playerAcceleration = moveInput * acceleration;
        playerAcceleration *= BBB.CharacterPhysics.SimpleDirectionConstraint(moveInput, m_lateralHeading, m_lateralSpeed, m_targetMaxSpeed * speedScale);

        return playerAcceleration;
    }

    public void SetVelocity(Vector3 velocity)
    {
        m_velocity = velocity;
        m_currentSpeed = m_velocity.magnitude;
        m_lateralVelocity = m_velocity;
        m_lateralVelocity.y = 0.0f;

        m_lateralVelocity = ReorientSlope(m_lateralVelocity);

        if (m_currentSpeed != 0.0f)
        {
            m_heading = m_velocity / m_currentSpeed;

            m_lateralSpeed = m_lateralVelocity.magnitude;
            if (m_lateralSpeed != 0.0f)
            {
                m_lateralHeading = m_lateralVelocity / m_lateralSpeed;
            }
        }
        else
        {
            m_lateralSpeed = 0.0f;
        }
    }

    void ProcessMovement(float deltaTime)
    {
        var collisionFlag = m_charController.Move(totalVelocity * Time.deltaTime);
    }

    Vector3 Gravity()
    {
        return Physics.gravity * m_settings.gravityScale * Time.deltaTime;
    }

    Vector3 BottomPos()
    {
        return transform.position + m_charController.center + Vector3.down * ((m_charController.height * 0.5f) - m_charController.radius);
    }

    bool GroundRay(out RaycastHit hitInfo)
    {
        return Physics.Raycast(transform.position, Vector3.down, out hitInfo, m_settings.groundRayLength + m_charController.skinWidth, m_settings.groundLayer, QueryTriggerInteraction.Ignore);
    }

    public void SetCharacterHeight(float height)
    {
        var center = m_charController.center;
        float m_centerHeightScale = center.y / m_charController.height;
        m_charController.height = height;

        center.y = m_centerHeightScale * height;
        m_charController.center = center;
    }

    public Vector3 ReorientSlope(Vector3 move)
    {
        return Vector3.ProjectOnPlane(move, m_groundNormal);
    }
    #endregion // Physics

    #region Actions
    public void TryJump()
    {
        if (m_isJumping)
        {
            return;
        }

        switch (m_currentState)
        {
            case PlayerState.Grounded:
                {
                    ForceJump(Vector3.up);
                    break;
                }
            case PlayerState.Airborne:
                {
                    Vector3 wallNormal;
                    if (m_frontWall)
                    {
                        wallNormal = m_frontWallHit.normal;
                    }
                    else if (m_wallRight)
                    {
                        wallNormal = m_rightWallhit.normal;
                    }
                    else if (m_wallLeft)
                    {
                        wallNormal = m_leftWallhit.normal;
                    }
                    else
                    {
                        return;
                    }

                    WallKick(wallNormal);
                    break;
                }
            case PlayerState.WallRunning:
                {
                    Vector3 wallNormal = m_wallRight ? m_rightWallhit.normal : m_leftWallhit.normal;

                    WallKick(wallNormal);

                    EnterState(PlayerState.Airborne);
                    m_wallRunsLeft++;
                    break;
                }
            case PlayerState.WallClimbing:
                {
                    Vector3 wallNormal = m_frontWallHit.normal;
                    WallKick(wallNormal);
                    EnterState(PlayerState.Airborne);
                    break;
                }
        }
    }

    public void ForceJump(Vector3 direciton)
    {
        m_jumpLockOutTimer.completeTime = m_settings.jumpLockoutTime;
        m_jumpLockOutTimer.Reset();
        direciton = direciton * BBB.CharacterPhysics.CalculateJumpForce(m_settings.jumpHeight, Physics.gravity.y * m_settings.gravityScale);
        float vertical = direciton.y;
        direciton.y = 0f;
        m_fallingVelocity += vertical;
        AddImpulse(direciton);
    }

    void WallKick(Vector3 wallNormal)
    {
        var jumpDir = Vector3.Slerp(wallNormal, Vector3.up, m_settings.wallJumpAngleStrength);
        ForceJump(jumpDir * m_settings.wallJumpStrength);
    }
    #endregion // Actions

    #region StateManagement
    void EnterState(PlayerState playerState)
    {
        if(playerState == m_currentState)
        {
            return;
        }
        ExitState(m_currentState);
        m_currentState = playerState;

        switch (playerState)
        {
            case PlayerState.Grounded:
                {
                    m_fallingVelocity = 0.0f;
                    ReorientSlope(m_velocity);

                    m_onGroundedEvent.Invoke();
                    break;
                }
            case PlayerState.Airborne:
                {
                    m_touchedGrass = false;
                    m_fallingVelocity += m_velocity.y;
                    m_velocity.y = 0.0f;
                    //m_isJumping = false;

                    m_onAirborneEvent.Invoke();
                    break;
                }
            case PlayerState.WallRunning:
                {
                    StartWallRun();
                    m_onWallRunEvent.Invoke();
                    break;
                }
            case PlayerState.WallClimbing:
                {
                    StartWallClimb();
                    m_onWallClimbEvent.Invoke();
                    break;
                }
        }
        m_changeStateEvent.Invoke();
    }

    void ExitState(PlayerState playerState)
    {
        switch (playerState)
        {
            case PlayerState.Grounded:
                {
                    break;
                }
            case PlayerState.Airborne:
                {
                    break;
                }
            case PlayerState.WallRunning:
                {
                    StopWallRun();
                    break;
                }
            case PlayerState.WallClimbing:
                {
                    StopWallClimb();
                    break;
                }
        }
    }

    void ProcessMovementState(PlayerState playerState)
    {
        switch (playerState)
        {
            case PlayerState.Grounded:
                {
                    GroundProcess(Time.deltaTime);
                    break;
                }
            case PlayerState.Airborne:
                {
                    AirProcess(Time.deltaTime);
                    break;
                }
            case PlayerState.WallRunning:
                {
                    WallRunProcess(Time.deltaTime);
                    break;
                }
            case PlayerState.WallClimbing:
                {
                    WallClimbProcess(Time.deltaTime);
                    break;
                }
        }
    }

    void GroundProcess(float deltaTime)
    {
        //SetGrounded();
        m_groundIsDetected = GroundRay(out RaycastHit hitInfo);

        if (m_groundIsDetected)
        {
            m_groundNormal = hitInfo.normal;
        }
        else
        {
            m_groundNormal = Vector3.up;
        }

        if (!m_groundIsDetected)
        {
            EnterState(PlayerState.Airborne);
        }

        //ProcessAcceleration(deltaTime);
        var impulse = ConsumeAccumulators(deltaTime);

        // Limit player movement if they are moving faster than their max speed.
        impulse += CalculateMoveInputAcceleration(m_settings.acceleration) * deltaTime;

        // An Absolute maximum speed.
        impulse += BBB.CharacterPhysics.SimpleLimit(m_lateralHeading, m_lateralSpeed, m_settings.absoluteMaxSpeed);

        SetVelocity(m_velocity + impulse);

        //Friction(deltaTime);
        m_impulseAccumulator += BBB.CharacterPhysics.AddDrag(m_heading, m_currentSpeed, m_settings.acceleration * m_targetBrakeScale, deltaTime);
    }

    void AirProcess(float deltaTime)
    {
        //SetGrounded();
        var isGrounded = TouchGrassTest();

        //ProcessAcceleration(deltaTime);
        var impulse = ConsumeAccumulators(deltaTime);

        // Limit player movement if they are moving faster than their max speed.
        impulse += CalculateMoveInputAcceleration(m_settings.airAcceleration) * deltaTime;

        // An Absolute maximum speed.
        impulse += BBB.CharacterPhysics.SimpleLimit(m_lateralHeading, m_lateralSpeed, m_settings.absoluteMaxSpeed);

        // Add Gravity
        if(!isGrounded)
        {
            m_fallingVelocity += Gravity().y;
        }

        SetVelocity(m_velocity + impulse);

        //Friction(deltaTime);
        m_impulseAccumulator += BBB.CharacterPhysics.AddDrag(m_heading, m_currentSpeed, m_settings.acceleration * m_settings.airBrakingScale, deltaTime);
    }

    void WallRunProcess(float deltaTime)
    {
        var isGrounded = TouchGrassTest();
        WallRunUpdate();

        //ProcessAcceleration(deltaTime);
        var impulse = ConsumeAccumulators(deltaTime);

        ////////// Don't need to use move input, but it would be nice to consider in the future.
        // Limit player movement if they are moving faster than their max speed.
        //impulse += CalculateMoveInputAcceleration(m_airAcceleration) * deltaTime;

        // An Absolute maximum speed.
        impulse += BBB.CharacterPhysics.SimpleLimit(m_lateralHeading, m_lateralSpeed, m_settings.absoluteMaxSpeed);

        SetVelocity(m_velocity + impulse);

        //Friction(deltaTime);
        m_impulseAccumulator += BBB.CharacterPhysics.AddDrag(m_heading, m_currentSpeed, m_settings.acceleration * m_settings.airBrakingScale, deltaTime);
    }

    void WallClimbProcess(float deltaTime)
    {
        WallClimbUpdate();

        //ProcessAcceleration(deltaTime);
        var impulse = ConsumeAccumulators(deltaTime);

        ////////// Don't need to use move input, but it would be nice to consider in the future.
        // Limit player movement if they are moving faster than their max speed.
        impulse += CalculateMoveInputAcceleration(m_settings.climbInputAcceleration) * deltaTime;

        // An Absolute maximum speed.
        impulse += BBB.CharacterPhysics.SimpleLimit(m_lateralHeading, m_lateralSpeed, m_settings.absoluteMaxSpeed);

        SetVelocity(m_velocity + impulse);

        //Friction(deltaTime);
        m_impulseAccumulator += BBB.CharacterPhysics.AddDrag(m_heading, m_currentSpeed, m_settings.acceleration * m_settings.airBrakingScale, deltaTime);
    }

    bool TouchGrassTest()
    {
        if (!m_touchedGrass)
        {
            m_touchedGrass = m_charController.isGrounded;
        }
        m_groundIsDetected = GroundRay(out RaycastHit hitInfo);

        var isGrounded = m_touchedGrass && m_groundIsDetected;

        if (m_groundIsDetected)
        {
            m_groundNormal = hitInfo.normal;
        }
        else
        {
            m_groundNormal = Vector3.up;
        }

        if (isGrounded)
        {
            EnterState(PlayerState.Grounded);
        }

        return isGrounded;
    }
    #endregion // StateManagement

    #region Crouching
    public void Crouch()
    {
        m_targetBrakeScale = m_settings.brakingScale * m_settings.crouchBrakingScale;
        m_targetMaxSpeed = m_settings.maxPlayerSpeed * m_settings.crouchSpeedScale;

        m_targetHeightSmooth = m_settings.crouchHeightScale;

        m_isCrouched = true;
    }

    public void Stand()
    {
        m_targetBrakeScale = m_settings.brakingScale;
        m_targetMaxSpeed = m_settings.maxPlayerSpeed;

        m_targetHeightSmooth = 1.0f;

        m_isCrouched = false;
    }

    void SmoothHeightTowards(float targetScale)
    {
        m_smoothCrouchScale = Mathf.SmoothDamp(m_smoothCrouchScale, targetScale, ref m_smoothCrouchVel, m_settings.smoothCrouchTime);
        SetCharacterHeight(m_settings.standHeight * m_smoothCrouchScale);
    }
    #endregion // Crouching

    #region Wallrunning
    public bool TryStartWallRun()
    {
        if (m_currentState != PlayerState.WallRunning)
        {
            if (StartWallRunCondition())
            {
                EnterState(PlayerState.WallRunning);
                return true;
            }
        }

        return false;
    }

    void WallRunUpdate()
    {
        // State 1 - Wallrunning
        if (ContinueWallRunCondition())
        {
            if(m_wallInteractTimer.Tick(Time.deltaTime))
            {
                EnterState(PlayerState.Airborne);
            }
        }
        else
        {
            EnterState(PlayerState.Airborne);
        }

        WallRunningMovement();

        m_fallingVelocity += Gravity().y * m_settings.wallRunGravityCurve.Evaluate(m_wallInteractTimer.currentTime / m_wallInteractTimer.completeTime);
    }

    private void CheckForWall()
    {
        var pos = transform.position + transform.up * m_settings.wallCheckHeight;
        m_wallRight = Physics.Raycast(pos, transform.right, out m_rightWallhit, m_settings.wallCheckDistance, m_settings.wallLayer);
        m_wallLeft = Physics.Raycast(pos, -transform.right, out m_leftWallhit, m_settings.wallCheckDistance, m_settings.wallLayer);

        m_frontWall = Physics.Raycast(pos, transform.forward, out m_frontWallHit, m_settings.wallCheckDistance, m_settings.wallLayer);
    }

    bool StartWallRunCondition()
    {
        return (m_wallLeft || m_wallRight);
    }

    bool ContinueWallRunCondition()
    {
        return (m_wallLeft || m_wallRight);// && this.fallingVelocity >= 0.0f;
    }

    private void StartWallRun()
    {
        //this.fallingVelocity = 0.0f;

        m_wallRunsLeft--;

        m_wallInteractTimer.completeTime = m_settings.wallRunTime;
        m_wallInteractTimer.Reset();
    }

    private void WallRunningMovement()
    {
        //var velocity = this.velocity;
        //velocity.y = 0.0f;
        //this.SetVelocity(velocity);

        Vector3 wallNormal = m_wallRight ? m_rightWallhit.normal : m_leftWallhit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((transform.forward - wallForward).magnitude > (transform.forward - -wallForward).magnitude)
            wallForward = -wallForward;

        // forward force
        //rb.AddForce(wallForward * wallRunForce, ForceMode.Force);
        AddForce(wallForward * m_settings.wallRunForce);

        // push to wall force
        if (!(m_wallLeft && m_moveInput.x > 0) && !(m_wallRight && m_moveInput.x < 0))
        {
            //rb.AddForce(-wallNormal * 100, ForceMode.Force);
            AddForce(-wallNormal * m_settings.wallStickyForce);
        }
    }

    private void StopWallRun()
    {
        
    }
    #endregion // WallRunning

    #region WallClimb

    public bool TryStartWallClimb ()
    {
        if (m_currentState != PlayerState.WallClimbing)
        {
            if (StartWallClimbCondition())
            {
                EnterState(PlayerState.WallClimbing);
                return true;
            }
        }

        return false;
    }

    void StartWallClimb()
    {
        m_wallInteractTimer.completeTime = m_settings.wallClimbTime;
        m_wallInteractTimer.Reset();
    }

    void StopWallClimb()
    {
        
    }

    void WallClimbUpdate()
    {
        // State 1 - Wallrunning
        if (ContinueWallClimbCondition())
        {
            if (m_wallInteractTimer.Tick(Time.deltaTime))
            {
                EnterState(PlayerState.Airborne);
            }
        }
        else
        {
            EnterState(PlayerState.Airborne);
        }

        WallClimbingMovement();
    }

    bool StartWallClimbCondition()
    {
        return m_frontWall;
    }

    bool ContinueWallClimbCondition()
    {
        return m_frontWall;
    }

    void WallClimbingMovement()
    {
        Vector3 wallNormal = m_frontWallHit.normal;

        // up force
        //AddForce(transform.up * m_climbForce * m_moveInput.z);

        //float toMax = m_maxWallClimbSpeed - m_fallingVelocity;
        //if(toMax > 0.0f)
        //{
        //    m_fallingVelocity += m_climbForce * m_moveInput.z * Time.deltaTime;
        //}

        var thing = Mathf.Min((m_settings.maxWallClimbSpeed * m_moveInput.z) - m_fallingVelocity, m_settings.maxWallClimbSpeed);
        m_fallingVelocity += thing * m_settings.climbForce * Time.deltaTime;

        // push to wall force
        if (m_moveInput.z > 0)
        {
            //rb.AddForce(-wallNormal * 100, ForceMode.Force);
            AddForce(-wallNormal * m_settings.wallStickyForce);
        }
    }

    #endregion // WallClimb

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (m_charController.collisionFlags.HasFlag(CollisionFlags.Sides))
        {
            if (Vector3.Dot(hit.normal, hit.moveDirection) < 0)
            {
                var diff = hit.normal * Vector3.Dot(hit.normal, hit.moveDirection);
                SetVelocity(m_velocity - diff);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var pos = transform.position;
        Gizmos.DrawLine(pos, pos + ReorientSlope(m_worldMoveInput));

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(pos, pos + Vector3.down * (m_settings.groundRayLength + m_charController.skinWidth));

        Gizmos.DrawCube(BottomPos(), Vector3.one * 0.2f);

        var slopeLateral = m_lateralHeading * m_lateralSpeed;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(pos, pos + slopeLateral);

        // Wall checks
        var wallCheckPos = transform.position + transform.up * m_settings.wallCheckHeight;
        // right
        Gizmos.color = Color.cyan;
        if(m_wallRight)
        {
            Gizmos.color = Color.magenta;
        }
        Gizmos.DrawLine(wallCheckPos, wallCheckPos + transform.right * m_settings.wallCheckDistance);
        // left
        Gizmos.color = Color.cyan;
        if (m_wallLeft)
        {
            Gizmos.color = Color.magenta;
        }
        Gizmos.DrawLine(wallCheckPos, wallCheckPos - transform.right * m_settings.wallCheckDistance);
        // front
        Gizmos.color = Color.cyan;
        if (m_frontWall)
        {
            Gizmos.color = Color.magenta;
        }
        Gizmos.DrawLine(wallCheckPos, wallCheckPos + transform.forward * m_settings.wallCheckDistance);
    }
}