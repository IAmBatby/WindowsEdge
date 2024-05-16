using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterController m_charController;

    [Header("Movement")]
    [SerializeField] float m_acceleration = 5.0f;
    [SerializeField] float m_airAcceleration = 2.5f;
    [SerializeField] float m_brakingScale = 0.6f;
    [SerializeField] float m_airBrakingScale = 0.1f;

    [SerializeField] float m_maxPlayerSpeed = 5.0f;
    [SerializeField] float m_absoluteMaxSpeed = 50.0f;

    Vector3 m_velocity = Vector3.zero;
    float m_currentSpeed = 0.0f;
    Vector3 m_heading = Vector3.forward;

    Vector3 m_lateralVelocity = Vector3.zero;
    float m_lateralSpeed = 0.0f;
    Vector3 m_lateralHeading = Vector3.forward;

    Vector3 m_forceAccumulator = Vector3.zero;
    Vector3 m_impulseAccumulator = Vector3.zero;

    [Header("Jumping")]
    [SerializeField] float m_jumpHeight = 2.0f;
    [SerializeField] float m_gravityScale = 1.0f;
    [SerializeField] SimpleTimer m_jumpLockOutTimer = new SimpleTimer();

    float m_fallingVelocity = 0.0f;

    bool m_isJumping = false;
    bool m_touchedGrass = false;

    [Header("GroundChecking")]
    [SerializeField] float m_groundRayLength = 0.1f;
    [SerializeField] LayerMask m_groundLayer = ~0;
    [SerializeField] Vector3 m_groundNormal = Vector3.up;

    bool m_groundIsDetected = false;

    [Header("WallRunning")]
    [SerializeField] LayerMask m_wallLayer = ~0;
    [SerializeField] float m_wallRunForce = 5.0f;
    [SerializeField] SimpleTimer m_wallRunTime = new SimpleTimer();

    [Header("WallJump")]
    [SerializeField, Range(0.0f, 1.0f)] float m_wallJumpAngleStrength = 0.5f;
    [SerializeField] float m_wallJumpStrength = 1.0f;

    [Header("Detection")]
    [SerializeField] float m_wallCheckHeight = 1.0f;
    [SerializeField] float m_wallCheckDistance = 1.0f;
    private RaycastHit m_leftWallhit;
    private RaycastHit m_rightWallhit;
    private bool m_wallLeft;
    private bool m_wallRight;

    int m_wallRunsLeft = 0;

    // Inputs
    [Header("Inputs")]
    [SerializeField] float m_inputScale = 1.0f;
    Vector3 m_moveInput = Vector3.zero;

    public float braking { get { return m_brakingScale; } set {  m_brakingScale = value; } }
    public float maxPlayerSpeed { get { return m_maxPlayerSpeed; } set { m_maxPlayerSpeed = value; } }
    public float gravityScale { get { return m_gravityScale; } set { m_gravityScale = value; } }
    public float fallingVelocity { get { return m_fallingVelocity; } set { m_fallingVelocity = value; } }

    public bool inputDisabled { get { return m_inputScale == 0.0f; } set { m_inputScale = System.Convert.ToSingle(!value); } }

    public Vector3 velocity { get { return m_velocity; } set { SetVelocity(value); } }

    public bool isGrounded { get { return m_currentState == PlayerState.Grounded; } }
    public bool isGroundDetected { get { return m_groundIsDetected;} }

    public float MaxPlayerSpeed { get { return (m_maxPlayerSpeed); } set { m_maxPlayerSpeed = value; } }
    public float CurrentPlayerSpeed { get { return (m_lateralSpeed); } set { m_lateralSpeed = value; } }
    // Update is called once per frame
    void LateUpdate()
    {
        Process(Time.deltaTime);
    }

    public void AddForce(Vector3 force)
    {
        m_forceAccumulator += force;
    }

    public void AddImpulse(Vector3 impulse)
    {
        m_impulseAccumulator += impulse;
    }

    public void SetMoveInput(Vector3 input)
    {
        m_moveInput = input;
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
        var moveInput = ReorientSlope(m_moveInput) * m_inputScale;
        var playerAcceleration = moveInput * acceleration;
        playerAcceleration *= BBB.CharacterPhysics.SimpleDirectionConstraint(moveInput, m_lateralHeading, m_lateralSpeed, m_maxPlayerSpeed);

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
        var collisionFlag = m_charController.Move((Vector3.up * m_fallingVelocity + m_velocity) * Time.deltaTime);
    }

    Vector3 Gravity()
    {
        return Physics.gravity * m_gravityScale * Time.deltaTime;
    }

    Vector3 BottomPos()
    {
        return transform.position + m_charController.center + Vector3.down * ((m_charController.height * 0.5f) - m_charController.radius);
    }

    bool GroundRay(out RaycastHit hitInfo)
    {
        return Physics.Raycast(transform.position, Vector3.down, out hitInfo, m_groundRayLength + m_charController.skinWidth, m_groundLayer, QueryTriggerInteraction.Ignore);
    }

    public void TryJump()
    {
        if(m_isJumping)
        {
            return;
        }

        switch(m_currentState)
        {
            case PlayerState.Grounded:
                {
                    ForceJump(Vector3.up);
                    break;
                }
            case PlayerState.Airborne:
                {
                    Vector3 wallNormal;
                    if(m_wallRight)
                    {
                        wallNormal = m_rightWallhit.normal;
                    }
                    else if(m_wallLeft)
                    {
                        wallNormal = m_leftWallhit.normal;
                    }
                    else
                    {
                        return;
                    }

                    var jumpDir = Vector3.Slerp(wallNormal, Vector3.up, m_wallJumpAngleStrength);

                    ForceJump(jumpDir * m_wallJumpStrength);
                    break;
                }
            case PlayerState.WallRunning:
                {
                    Vector3 wallNormal = m_wallRight ? m_rightWallhit.normal : m_leftWallhit.normal;
                    var jumpDir = Vector3.Slerp(wallNormal, Vector3.up, m_wallJumpAngleStrength);

                    EnterState(PlayerState.Airborne);
                    m_wallRunsLeft++;
                    ForceJump(jumpDir * m_wallJumpStrength);
                    break;
                }
        }
    }

    public void ForceJump(Vector3 direciton)
    {
        m_jumpLockOutTimer.Reset();
        AddImpulse(direciton * BBB.CharacterPhysics.CalculateJumpForce(m_jumpHeight, Physics.gravity.y * m_gravityScale));
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

    enum PlayerState
    {
        Grounded,
        Airborne,
        Jumping,
        WallRunning
    }

    PlayerState m_currentState = PlayerState.Grounded;

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
                    Debug.Log("Grounded");
                    m_fallingVelocity = 0.0f;
                    ReorientSlope(m_velocity);
                    break;
                }
            case PlayerState.Airborne:
                {
                    Debug.Log("Air");
                    m_touchedGrass = false;
                    m_fallingVelocity = m_velocity.y;
                    m_velocity.y = 0.0f;
                    //m_isJumping = false;
                    break;
                }
            case PlayerState.WallRunning:
                {
                    StartWallRun();
                    break;
                }
        }
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
        impulse += CalculateMoveInputAcceleration(m_acceleration) * deltaTime;

        // An Absolute maximum speed.
        impulse += BBB.CharacterPhysics.SimpleLimit(m_lateralHeading, m_lateralSpeed, m_absoluteMaxSpeed);

        SetVelocity(m_velocity + impulse);

        //Friction(deltaTime);
        m_impulseAccumulator += BBB.CharacterPhysics.AddDrag(m_heading, m_currentSpeed, m_acceleration * m_brakingScale, deltaTime);
    }

    void AirProcess(float deltaTime)
    {
        //SetGrounded();
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

        //ProcessAcceleration(deltaTime);
        var impulse = ConsumeAccumulators(deltaTime);

        // Limit player movement if they are moving faster than their max speed.
        impulse += CalculateMoveInputAcceleration(m_airAcceleration) * deltaTime;

        // An Absolute maximum speed.
        impulse += BBB.CharacterPhysics.SimpleLimit(m_lateralHeading, m_lateralSpeed, m_absoluteMaxSpeed);

        // Add Gravity
        if(!isGrounded)
        {
            m_fallingVelocity += Gravity().y;
        }

        SetVelocity(m_velocity + impulse);

        //Friction(deltaTime);
        m_impulseAccumulator += BBB.CharacterPhysics.AddDrag(m_heading, m_currentSpeed, m_acceleration * m_airBrakingScale, deltaTime);
    }

    void WallRunProcess(float deltaTime)
    {
        WallRunUpdate();

        //ProcessAcceleration(deltaTime);
        var impulse = ConsumeAccumulators(deltaTime);

        ////////// Don't need to use move input, but it would be nice to consider in the future.
        // Limit player movement if they are moving faster than their max speed.
        //impulse += CalculateMoveInputAcceleration(m_airAcceleration) * deltaTime;

        // An Absolute maximum speed.
        impulse += BBB.CharacterPhysics.SimpleLimit(m_lateralHeading, m_lateralSpeed, m_absoluteMaxSpeed);

        SetVelocity(m_velocity + impulse);

        //Friction(deltaTime);
        m_impulseAccumulator += BBB.CharacterPhysics.AddDrag(m_heading, m_currentSpeed, m_acceleration * m_airBrakingScale, deltaTime);
    }

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
            if(m_wallRunTime.Tick(Time.deltaTime))
            {
                EnterState(PlayerState.Airborne);
            }
        }
        else
        {
            EnterState(PlayerState.Airborne);
        }

        WallRunningMovement();
    }

    private void CheckForWall()
    {
        var pos = transform.position + transform.up * m_wallCheckHeight;
        m_wallRight = Physics.Raycast(pos, transform.right, out m_rightWallhit, m_wallCheckDistance, m_wallLayer);
        m_wallLeft = Physics.Raycast(pos, -transform.right, out m_leftWallhit, m_wallCheckDistance, m_wallLayer);
    }

    private bool AboveGround()
    {
        return !isGroundDetected;
    }

    bool StartWallRunCondition()
    {
        return (m_wallLeft || m_wallRight) && AboveGround();
    }

    bool ContinueWallRunCondition()
    {
        return (m_wallLeft || m_wallRight) && AboveGround() && this.fallingVelocity <= 0.0f;
    }

    private void StartWallRun()
    {
        Debug.Log("Start Wall Run");
        this.fallingVelocity = 0.0f;

        m_wallRunsLeft--;

        m_wallRunTime.Reset();
    }

    private void WallRunningMovement()
    {
        var velocity = this.velocity;
        velocity.y = 0.0f;
        this.SetVelocity(velocity);

        Vector3 wallNormal = m_wallRight ? m_rightWallhit.normal : m_leftWallhit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((transform.forward - wallForward).magnitude > (transform.forward - -wallForward).magnitude)
            wallForward = -wallForward;

        // forward force
        //rb.AddForce(wallForward * wallRunForce, ForceMode.Force);
        AddForce(wallForward * m_wallRunForce);

        // push to wall force
        if (!(m_wallLeft && m_moveInput.x > 0) && !(m_wallRight && m_moveInput.x < 0))
        {
            //rb.AddForce(-wallNormal * 100, ForceMode.Force);
            AddForce(-wallNormal * 100);
        }
    }

    private void StopWallRun()
    {
        Debug.Log("Stop Wall Run");
    }
    #endregion

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
        Gizmos.DrawLine(pos, pos + ReorientSlope(m_moveInput));

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(pos, pos + Vector3.down * (m_groundRayLength + m_charController.skinWidth));

        Gizmos.DrawCube(BottomPos(), Vector3.one * 0.2f);

        var slopeLateral = m_lateralHeading * m_lateralSpeed;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(pos, pos + slopeLateral);

        Gizmos.color = Color.cyan;
        var wallCheckPos = transform.position + transform.up * m_wallCheckHeight;
        Gizmos.DrawLine(wallCheckPos, wallCheckPos + transform.right * m_wallCheckDistance);
        Gizmos.DrawLine(wallCheckPos, wallCheckPos - transform.right * m_wallCheckDistance);
    }
}