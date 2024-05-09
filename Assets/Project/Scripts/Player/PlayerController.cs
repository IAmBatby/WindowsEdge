using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterController m_charController;

    [SerializeField] float m_acceleration = 5.0f;
    [SerializeField] float m_airAcceleration = 2.5f;
    [SerializeField] float m_brakingScale = 0.6f;
    [SerializeField] float m_airBrakingScale = 0.1f;

    [SerializeField] float m_maxPlayerSpeed = 5.0f;
    [SerializeField] float m_absoluteMaxSpeed = 50.0f;

    [SerializeField] float m_jumpHeight = 2.0f;
    [SerializeField] float m_gravityScale = 1.0f;

    Vector3 m_velocity = Vector3.zero;
    float m_currentSpeed = 0.0f;
    Vector3 m_heading = Vector3.forward;

    Vector3 m_lateralVelocity = Vector3.zero;
    float m_lateralSpeed = 0.0f;
    Vector3 m_lateralHeading = Vector3.forward;

    Vector3 m_forceAccumulator = Vector3.zero;
    Vector3 m_impulseAccumulator = Vector3.zero;

    bool m_isGrounded = false;
    bool m_isJumping = false;

    [Header("GroundChecking")]
    [SerializeField] float m_groundRayLength = 0.1f;
    [SerializeField] LayerMask m_groundLayer = ~0;

    // Inputs
    [Header("Inputs")]
    Vector3 m_moveInput = Vector3.zero;

    public float braking { get { return m_brakingScale; } set {  m_brakingScale = value; } }
    public float maxPlayerSpeed { get { return m_maxPlayerSpeed; } set { m_maxPlayerSpeed = value; } }

    // Update is called once per frame
    void LateUpdate()
    {
        Process(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        //Process(Time.fixedDeltaTime);
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
        ProcessAcceleration(deltaTime);
        Friction(deltaTime);
        ProcessMovement(deltaTime);
    }

    void ProcessAcceleration(float deltaTime)
    {
        var acceleration = m_forceAccumulator;
        m_forceAccumulator = Vector3.zero;

        var impulse = m_impulseAccumulator + acceleration * deltaTime;
        m_impulseAccumulator = Vector3.zero;

        // Limit player movement if they are moving daster than their max speed.
        var playerAcceleration = m_moveInput * deltaTime;
        if(m_isGrounded)
        {
            playerAcceleration *= m_acceleration;
        }
        else
        {
            playerAcceleration *= m_airAcceleration;
        }

        playerAcceleration *= BBB.CharacterPhysics.SimpleDirectionConstraint(m_moveInput, m_lateralHeading, m_lateralSpeed, m_maxPlayerSpeed);
        impulse += playerAcceleration;

        // An Absolute maximum speed.
        impulse += BBB.CharacterPhysics.SimpleLimit(m_lateralHeading, m_lateralSpeed, m_absoluteMaxSpeed);

        // Add Gravity
        if(!m_isGrounded)
        {
            m_velocity.y += Physics.gravity.y * m_gravityScale * Time.deltaTime;
        }

        m_velocity += impulse;
        m_currentSpeed = m_velocity.magnitude;
        m_lateralVelocity = m_velocity;
        m_lateralVelocity.y = 0.0f;

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
        var collisionFlag = m_charController.Move(m_velocity * Time.deltaTime);

        bool wasGrounded = m_isGrounded;
        m_isGrounded = GroundRay(out RaycastHit hitInfo);

        if (!wasGrounded && m_isGrounded)
        {
            m_velocity.y = 0.0f;
        }

        if(!m_isGrounded && m_isJumping)
        {
            m_isJumping = false;
        }
    }

    void Friction(float deltaTime)
    {
        if (m_isGrounded)
        {
            m_impulseAccumulator += BBB.CharacterPhysics.AddDrag(m_lateralHeading, m_lateralSpeed, m_acceleration * m_brakingScale, deltaTime);
        }
        else
        {
            m_impulseAccumulator += BBB.CharacterPhysics.AddDrag(m_lateralHeading, m_lateralSpeed, m_acceleration * m_airBrakingScale, deltaTime);
        }
    }

    bool GroundRay(out RaycastHit hitInfo)
    {
        return Physics.Raycast(transform.position, Vector3.down, out hitInfo, m_groundRayLength + m_charController.skinWidth, m_groundLayer, QueryTriggerInteraction.Ignore);
    }

    public void TryJump()
    {
        if(m_isGrounded && !m_isJumping)
        {
            m_isJumping = true;
            AddImpulse(Vector3.up * BBB.CharacterPhysics.CalculateJumpForce(m_jumpHeight, Physics.gravity.y * m_gravityScale));
        }
    }

    public void SetCharacterHeight(float height)
    {
        var center = m_charController.center;
        float m_centerHeightScale = center.y / m_charController.height;
        m_charController.height = height;

        center.y = m_centerHeightScale * height;
        m_charController.center = center;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Debug.Log("Boop");
    }
}
