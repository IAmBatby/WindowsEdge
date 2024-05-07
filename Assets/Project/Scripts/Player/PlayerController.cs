using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using UnityEngine.UIElements.Experimental;

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
    float m_currentSpeed = 0.0f;
    Vector3 m_heading = Vector3.forward;

    Vector3 m_lateralVelocity = Vector3.zero;
    float m_lateralSpeed = 0.0f;
    Vector3 m_lateralHeading = Vector3.forward;

    Vector3 m_forceAccumulator = Vector3.zero;
    Vector3 m_impulseAccumulator = Vector3.zero;

    bool m_isGrounded = false;
    bool m_isJumping = false;

    [SerializeField] float m_overSpeedScale = 1.0f;
    [SerializeField] float m_limitPullStrength = 1.0f;

    [Header("GroundChecking")]
    [SerializeField] float m_groundRayLength = 0.1f;
    [SerializeField] LayerMask m_groundLayer = ~0;

    // Inputs
    [Header("Inputs")]
    Vector3 m_moveInput = Vector3.zero;

    [System.Serializable]
    struct DebugFloat
    {
        public string name;
        public float value;

        public DebugFloat(string name, float value)
        {
            this.name = name;
            this.value = value;
        }
    }

    [System.Serializable]
    struct DebugVector3
    {
        public string name;
        public Vector3 value;

        public DebugVector3(string name, Vector3 value)
        {
            this.name = name;
            this.value = value;
        }
    }

    [SerializeField] List<DebugFloat> debug_floats = new List<DebugFloat>();
    [SerializeField] List<DebugVector3> debug_vectors = new List<DebugVector3>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        debug_floats.Clear();
        debug_vectors.Clear();
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

        // Limit player movement if they are moving daster than their max speed.
        var playerAcceleration = m_moveInput * m_acceleration * deltaTime;
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
        m_impulseAccumulator += BBB.CharacterPhysics.AddDrag(m_lateralHeading, m_lateralSpeed, m_braking, deltaTime);
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
