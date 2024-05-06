using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterController m_charController;

    [SerializeField] float m_acceleration = 5.0f;
    [SerializeField] float m_braking = 5.0f;

    [SerializeField] float m_maxPlayerSpeed = 5.0f;

    [SerializeField] float m_jumpHeight = 2.0f;
    [SerializeField] float m_gravityScale = 1.0f;

    Vector3 m_velocity = Vector3.zero;
    Vector3 m_forceAccumulator = Vector3.zero;
    Vector3 m_impulseAccumulator = Vector3.zero;

    float m_currentSpeed = 0.0f;
    Vector3 m_heading = Vector3.forward;

    bool m_isGrounded = false;
    bool m_isJumping = false;

    [Header("GroundChecking")]
    [SerializeField] float m_groundRayLength = 0.1f;
    [SerializeField] LayerMask m_groundLayer = ~0;

    // Inputs
    [Header("Inputs")]
    Vector3 m_moveInput = Vector3.zero;
    bool m_jumpInput = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        m_moveInput = Vector3.zero;
        m_moveInput.x = Input.GetAxisRaw("Horizontal");
        m_moveInput.z = Input.GetAxisRaw("Vertical");

        SetMoveInput(m_moveInput);

        if(Input.GetAxisRaw("Jump") != 0)
        {
            ProcessJump();
        }
    }

    public void AddForce(Vector3 force)
    {
        m_forceAccumulator += force;
    }

    public void AddImpulse(Vector3 impulse)
    {
        m_impulseAccumulator += impulse;
    }

    void SetMoveInput(Vector3 input)
    {
        m_moveInput = input;
    }

    void ProcessAcceleration(float deltaTime)
    {
        var acceleration = m_forceAccumulator;
        m_forceAccumulator = Vector3.zero;

        var impulse = m_impulseAccumulator + acceleration * deltaTime;
        m_impulseAccumulator = Vector3.zero;

        var playerAcceleration = m_moveInput * m_acceleration * deltaTime;
        impulse += playerAcceleration;

        if(!m_isGrounded)
        {
            m_velocity.y += Physics.gravity.y * m_gravityScale * Time.deltaTime * Time.deltaTime;
        }

        m_velocity += impulse * deltaTime;
        m_currentSpeed = m_velocity.magnitude;

        if(m_currentSpeed != 0.0f)
        {
            m_heading = m_velocity / m_currentSpeed;
        }
    }

    void ProcessMovement()
    {
        var collisionFlag = m_charController.Move(m_velocity);

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
        if (m_moveInput.sqrMagnitude == 0.0f)
        {
            var lateralHead = m_heading;
            lateralHead.y = 0.0f;
            AddForce(-lateralHead.normalized * m_braking);

            var lateralVel = m_velocity;
            lateralVel.y = 0.0f;

            //lateralVel = Vector3.MoveTowards(lateralVel, Vector3.zero, m_braking * deltaTime);
            //m_velocity.x = lateralVel.x;
            //m_velocity.z = lateralVel.z;
            //m_velocity = Vector3.MoveTowards(m_velocity, Vector3.zero, m_braking * deltaTime);
        }
    }

    bool GroundRay(out RaycastHit hitInfo)
    {
        return Physics.Raycast(transform.position, Vector3.down, out hitInfo, m_groundRayLength + m_charController.skinWidth, m_groundLayer);
    }

    void ProcessJump()
    {
        if(m_isGrounded && !m_isJumping)
        {
            m_isJumping = true;
            AddImpulse(Vector3.up * CalculateJumpForce(m_jumpHeight, Physics.gravity.y * m_gravityScale));
        }
    }

    float CalculateJumpForce(float height, float gravity)
    {
        // Calculate the required jump force using the formula v = sqrt(2 * g * h)
        float jumpForce = Mathf.Sqrt(2 * -gravity * height);
        return jumpForce;
    }

    private void FixedUpdate()
    {
        Friction(Time.fixedDeltaTime);

        ProcessAcceleration(Time.fixedDeltaTime);
        ProcessMovement();
    }
}
