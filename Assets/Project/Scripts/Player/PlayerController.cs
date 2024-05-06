using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterController m_charController;

    [SerializeField] float m_acceleration = 5.0f;
    [SerializeField] float m_braking = 5.0f;

    [SerializeField] float m_maxPlayerSpeed = 5.0f;
    [SerializeField] float m_absoluteMaxSpeed = 50.0f;

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

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

        if(m_moveInput.sqrMagnitude > 0.0f)
        {
            var lateralVel = m_velocity;
            lateralVel.y = 0.0f;
            var playerAcceleration = m_moveInput * m_acceleration * deltaTime;
            playerAcceleration += BBB.CharacterPhysics.LimitAcceleration(lateralVel, m_moveInput, m_moveInput.magnitude, m_acceleration, deltaTime, m_maxPlayerSpeed, m_absoluteMaxSpeed);
            impulse += playerAcceleration;
        }

        if(!m_isGrounded)
        {
            m_velocity.y += Physics.gravity.y * m_gravityScale * Time.deltaTime;
        }

        m_velocity += impulse;
        m_currentSpeed = m_velocity.magnitude;

        if(m_currentSpeed != 0.0f)
        {
            m_heading = m_velocity / m_currentSpeed;
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
        //if (m_moveInput.sqrMagnitude == 0.0f)
        //{
        //    var lateralHead = m_heading;
        //    lateralHead.y = 0.0f;
        //    AddForce(-lateralHead.normalized * m_braking);
        //
        //    var lateralVel = m_velocity;
        //    lateralVel.y = 0.0f;
        //
        //    //lateralVel = Vector3.MoveTowards(lateralVel, Vector3.zero, m_braking * deltaTime);
        //    //m_velocity.x = lateralVel.x;
        //    //m_velocity.z = lateralVel.z;
        //    //m_velocity = Vector3.MoveTowards(m_velocity, Vector3.zero, m_braking * deltaTime);
        //}

        m_impulseAccumulator += BBB.CharacterPhysics.AddDrag(m_heading, m_currentSpeed, m_braking, deltaTime);
    }

    bool GroundRay(out RaycastHit hitInfo)
    {
        return Physics.Raycast(transform.position, Vector3.down, out hitInfo, m_groundRayLength + m_charController.skinWidth, m_groundLayer);
    }

    public void TryJump()
    {
        if(m_isGrounded && !m_isJumping)
        {
            m_isJumping = true;
            AddImpulse(Vector3.up * BBB.CharacterPhysics.CalculateJumpForce(m_jumpHeight, Physics.gravity.y * m_gravityScale));
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("Boop");
    }
}
