using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(menuName = "Assets/Player/Profile")]
public class PlayerProfile : ScriptableObject
{
    [Header("Movement")]
    [SerializeField] float m_acceleration = 5.0f;
    [SerializeField] float m_airAcceleration = 2.5f;
    [SerializeField] float m_brakingScale = 0.6f;
    [SerializeField] float m_airBrakingScale = 0.1f;

    [SerializeField] float m_maxPlayerSpeed = 5.0f;
    [SerializeField] float m_absoluteMaxSpeed = 50.0f;

    public float acceleration { get { return m_acceleration; } set { m_acceleration = value; } }
    public float airAcceleration { get { return m_airAcceleration; } set { m_airAcceleration = value; } }
    public float brakingScale { get { return m_brakingScale; } set { m_brakingScale = value; } }
    public float airBrakingScale { get { return m_airBrakingScale; } set { m_airBrakingScale = value; } }
    public float maxPlayerSpeed { get { return m_maxPlayerSpeed; } set { m_maxPlayerSpeed = value; } }
    public float absoluteMaxSpeed { get { return m_absoluteMaxSpeed; } set { m_absoluteMaxSpeed = value; } }

    [Header("Speeds")]
    [SerializeField] float m_runSpeedScale = 1.5f;
    public float runSpeedScale { get { return m_runSpeedScale; } set { m_runSpeedScale = value; } }

    [Header("Crouch")]
    [SerializeField] float m_crouchAccelScale = 0.5f;
    [SerializeField] float m_crouchSpeedScale = 0.5f;
    [SerializeField] float m_crouchBrakingScale = 0.5f;
    [SerializeField] float m_standHeight = 2.0f;
    [SerializeField] float m_crouchHeightScale = 0.5f;
    [SerializeField] float m_smoothCrouchTime = 0.1f;

    public float crouchAccelScale { get { return m_crouchAccelScale; } set { m_crouchAccelScale = value; } }
    public float crouchSpeedScale { get { return m_crouchSpeedScale; } set { m_crouchSpeedScale = value; } }
    public float crouchBrakingScale { get { return m_crouchBrakingScale; } set { m_crouchBrakingScale = value; } }
    public float standHeight { get { return m_standHeight; } set { m_standHeight = value; } }
    public float crouchHeightScale { get { return m_crouchHeightScale; } set { m_crouchHeightScale = value; } }
    public float smoothCrouchTime { get { return m_smoothCrouchTime; } set { m_smoothCrouchTime = value; } }


    [Header("Jumping")]
    [SerializeField] float m_jumpHeight = 2.0f;
    [SerializeField] float m_gravityScale = 1.0f;
    [SerializeField] float m_jumpLockOutTime = 0.1f;

    public float jumpHeight { get { return m_jumpHeight; } set { m_jumpHeight = value; } }
    public float gravityScale { get { return m_gravityScale; } set { m_gravityScale = value; } }
    public float jumpLockoutTime { get { return m_jumpLockOutTime; } set { m_jumpLockOutTime = value; } }

    [Header("GroundChecking")]
    [SerializeField] float m_groundRayLength = 0.1f;
    [SerializeField] LayerMask m_groundLayer = ~0;
    [SerializeField] Vector3 m_groundNormal = Vector3.up;

    public float groundRayLength { get { return m_groundRayLength; } set { m_groundRayLength = value; } }
    public LayerMask groundLayer { get { return m_groundLayer; } set { m_groundLayer = value; } }
    public Vector3 groundNormal { get { return m_groundNormal; } }

    [Header("Wall General")]
    [SerializeField] LayerMask m_wallLayer = ~0;
    [SerializeField] float m_wallCheckHeight = 1.0f;
    [SerializeField] float m_wallCheckDistance = 1.0f;
    [SerializeField] float m_wallStickyForce = 100.0f;

    public LayerMask wallLayer { get { return m_wallLayer; } set { m_wallLayer = value; } }
    public float wallCheckHeight { get { return m_wallCheckHeight; } set { m_wallCheckHeight = value; } }
    public float wallCheckDistance { get { return m_wallCheckDistance; } set { m_wallCheckDistance = value; } }
    public float wallStickyForce { get { return m_wallStickyForce; } set { m_wallStickyForce = value; } }

    [Header("WallRunning")]
    [SerializeField] float m_wallRunForce = 5.0f;
    [SerializeField] float m_wallRunTime = 1.0f;
    [SerializeField] AnimationCurve m_wallRunGravityCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    [SerializeField] float m_initialVerticalVelocityFloor = 0.1f;
    [SerializeField] float m_initialVerticalVelocityRoof = 5.0f;
    [SerializeField] float m_wallVarienceCheck = 15.0f;

    public float wallRunForce { get { return m_wallRunForce; } set { m_wallRunForce = value; } }
    public float wallRunTime { get { return m_wallRunTime; } set { m_wallRunTime = value; } }
    public AnimationCurve wallRunGravityCurve { get { return m_wallRunGravityCurve; } }
    public float initialVerticalVelocityFloor { get { return m_initialVerticalVelocityFloor; } set { m_initialVerticalVelocityFloor = value; } }
    public float initialVerticalVelocityRoof { get { return m_initialVerticalVelocityRoof; } set { m_initialVerticalVelocityRoof = value; } }
    public float wallVarienceCheck { get { return m_wallVarienceCheck; } set { m_wallVarienceCheck = value; } }

    [Header("WallJump")]
    [SerializeField, Range(0.0f, 1.0f)] float m_wallJumpAngleStrength = 0.5f;
    [SerializeField] float m_wallJumpStrength = 1.0f;

    public float wallJumpAngleStrength { get { return m_wallJumpAngleStrength; } set { m_wallJumpAngleStrength = Mathf.Clamp(value, 0.0f, 1.0f); } }
    public float wallJumpStrength { get { return m_wallJumpStrength; } set { m_wallJumpStrength = value; } }

    [Header("Wall Climb")]
    [SerializeField] float m_climbForce = 10.0f;
    [SerializeField] float m_wallClimbTime = 1.0f;
    [SerializeField] float m_maxWallClimbSpeed = 5.0f;
    [SerializeField] float m_climbInputAcceleration = 10.0f;

    public float climbForce { get { return m_climbForce; } set { m_climbForce = value; } }
    public float wallClimbTime { get { return m_wallClimbTime; } set { m_wallClimbTime = value; } }
    public float maxWallClimbSpeed { get { return m_maxWallClimbSpeed; } set { m_maxWallClimbSpeed = value; } }
    public float climbInputAcceleration { get { return m_climbInputAcceleration; } set { m_climbInputAcceleration = value; } }
}
