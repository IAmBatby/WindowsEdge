using System.Collections;
using System.Collections.Generic;
using TriInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "ScriptableObjects/Settings/PlayerSettings", order = 1)]
public class PlayerSettings : ScriptableSettings
{
    [Header("Movement")]
    float m_acceleration = 5.0f;
    float m_airAcceleration = 2.5f;
    float m_brakingScale = 0.6f;
    float m_airBrakingScale = 0.1f;

    float m_maxPlayerSpeed = 5.0f;
    float m_absoluteMaxSpeed = 50.0f;

    Vector3 m_velocity = Vector3.zero;
    float m_currentSpeed = 0.0f;
    Vector3 m_heading = Vector3.forward;

    Vector3 m_lateralVelocity = Vector3.zero;
    float m_lateralSpeed = 0.0f;
    Vector3 m_lateralHeading = Vector3.forward;

    Vector3 m_forceAccumulator = Vector3.zero;
    Vector3 m_impulseAccumulator = Vector3.zero;

    [Header("Jumping")]
    float m_jumpHeight = 2.0f;
    float m_gravityScale = 1.0f;
    SimpleTimer m_jumpLockOutTimer = new SimpleTimer();

    float m_fallingVelocity = 0.0f;

    bool m_isJumping = false;
    bool m_touchedGrass = false;


    [Header("GroundChecking")]
    float m_groundRayLength = 0.1f;
    LayerMask m_groundLayer = ~0;
    Vector3 m_groundNormal = Vector3.up;

    bool m_groundIsDetected = false;



    [Header("WallRunning")]
    LayerMask m_wallLayer = ~0;
    float m_wallRunForce = 5.0f;
    SimpleTimer m_wallRunTime = new SimpleTimer();



    [Header("WallJump")]
    float m_wallJumpAngleStrength = 0.5f;
    float m_wallJumpStrength = 1.0f;

    [Header("Detection")]
    float m_wallCheckHeight = 1.0f;
    float m_wallCheckDistance = 1.0f;
    private RaycastHit m_leftWallhit;
    private RaycastHit m_rightWallhit;
    private bool m_wallLeft;
    private bool m_wallRight;

    int m_wallRunsLeft = 0;

    [Header("Inputs")]
    float m_inputScale = 1.0f;
    Vector3 m_moveInput = Vector3.zero;

    [PropertySpace]
    [Title("Walking & Running")]

    [ShowInInspector] public float PlayerMaxSpeed { get => m_maxPlayerSpeed; set => UpdateSetting(ref m_maxPlayerSpeed, value); }
    [ShowInInspector] public float AbsoluteMaxSpeed { get => m_absoluteMaxSpeed; set => UpdateSetting(ref m_absoluteMaxSpeed, value); }
    [PropertySpace(10)]
    [ShowInInspector] public float GroundedAcceleration { get => m_acceleration; set => UpdateSetting(ref m_acceleration, value); }
    [ShowInInspector] public float AirAcceleration { get => m_airAcceleration; set => UpdateSetting(ref m_airAcceleration, value); }
    [PropertySpace(10)]
    [ShowInInspector] public float GroundedBrakingScale { get => m_brakingScale; set => UpdateSetting(ref m_brakingScale, value); }
    [ShowInInspector] public float AirBrakingScale { get => m_airBrakingScale; set => UpdateSetting(ref m_airBrakingScale, value); }
    [PropertySpace(10)]
    [ShowInInspector, ShowIf(nameof(IsDebuggingEnabled)), ReadOnly] public Vector3 Velocity => m_velocity;

    [PropertySpace]
    [Title("Jumping, Falling & Landing")]

    [ShowInInspector] public float JumpHeight { get => m_jumpHeight; set => UpdateSetting(ref m_jumpHeight, value); }
    [ShowInInspector] public float GravityScale { get => m_gravityScale; set => UpdateSetting(ref m_gravityScale, value); }
    [PropertySpace(10)]
    [ShowInInspector] public float GroundRayLength { get => m_groundRayLength; set => UpdateSetting(ref m_groundRayLength, value); }
    [ShowInInspector] public LayerMask GroundMask { get => m_groundLayer; set => UpdateSetting(ref m_groundLayer, value); }
    [ShowInInspector] public Vector3 GroundNormal { get => m_groundNormal; set => UpdateSetting(ref m_groundNormal, value); }

    [PropertySpace]
    [Title("Wall Running & Jumping")]

    [ShowInInspector] public float WallRunForce { get => m_wallRunForce; set => UpdateSetting(ref m_wallRunForce, value); }
    [ShowInInspector, InlineProperty, HideReferencePicker] public SimpleTimer WallRunTime { get => m_wallRunTime; set => UpdateSetting(ref m_wallRunTime, value); }
    [ShowInInspector] public LayerMask WallMask { get => m_wallLayer; set => UpdateSetting(ref m_wallLayer, value); }
    [PropertySpace(10)]
    [ShowInInspector] public float WallJumpStrength { get => m_wallJumpStrength; set => UpdateSetting(ref m_wallJumpStrength, value); }
    [ShowInInspector] public float WallJumpAngle { get => m_wallJumpAngleStrength; set => UpdateSetting(ref m_wallJumpAngleStrength, value); }
    [PropertySpace(10)]
    [ShowInInspector] public float WallCheckHeight { get => m_wallCheckHeight; set => UpdateSetting(ref m_wallCheckHeight, value); }
    [ShowInInspector] public float WallCheckDistance { get => m_wallCheckDistance; set => UpdateSetting(ref m_wallCheckDistance, value); }

    //[ShowInInspector]
    //public float Acceleration { get => playerController.acceleration; set => UpdateSetting(ref playerController.m_acceleration, value); }

    public override void ApplySettings()
    {
        base.ApplySettings();
    }
}
