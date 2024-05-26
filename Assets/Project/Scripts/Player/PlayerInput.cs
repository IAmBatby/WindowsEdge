using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] PlayerController m_controller;

    [SerializeField] Transform m_camHead;
    [SerializeField] float m_cameraSens = 1.0f;

    [SerializeField] Vector2 m_camClamp = new Vector2 (-89, 89);

    [SerializeField] float m_camHeight = 1.6f;

    [SerializeField] Transform m_debug_model;

    [SerializeField] float m_climbDegrees = 15.0f;
    float m_climbDotCheck;

    [SerializeField] float m_minSpeedWallRunEngage = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_climbDotCheck = Mathf.Cos(m_climbDegrees * Mathf.Deg2Rad);
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalScale = 1.0f;
        float forwardScale = 1.0f;

        if (Input.GetAxisRaw("Fire1") != 0)
        {
            m_controller.Crouch();
        }
        else
        {
            m_controller.Stand();

            if (Input.GetAxisRaw("Fire3") != 0)
            {
                m_controller.speedScale = m_controller.settings.runSpeedScale;
                //m_controller.maxPlayerSpeed = m_playerSpeed * m_runSpeedScale;
            }
            else
            {
                m_controller.speedScale = 1.0f;
                //m_controller.maxPlayerSpeed = m_playerSpeed;
            }
        }

        Move(horizontalScale, forwardScale);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            RisingInput();
        }

        Look();

        AirborneWallInteraction();
    }

    private void LateUpdate()
    {
        m_camHead.localPosition = Vector3.up * m_controller.settings.standHeight * m_controller.smoothCrouchScale * (m_camHeight / m_controller.settings.standHeight);

        var scale = m_debug_model.localScale;
        scale.y = m_controller.smoothCrouchScale;
        m_debug_model.localScale = scale;
    }

    private void Move(float horizontalScale, float forwardScale)
    {
        var moveInput = Vector3.zero;
        moveInput.x = Input.GetAxisRaw("Horizontal") * horizontalScale;
        moveInput.z = Input.GetAxisRaw("Vertical") * forwardScale;

        moveInput = Vector3.ClampMagnitude(moveInput, 1.0f);

        m_controller.SetMoveInput(moveInput);
    }

    void Look()
    {
        var mouseInput = Vector2.zero;
        mouseInput.x = Input.GetAxisRaw("Mouse X");
        mouseInput.y = Input.GetAxisRaw("Mouse Y");

        RotateCamBody(mouseInput.x * m_cameraSens);
        RotateCamHead(mouseInput.y * m_cameraSens);
    }

    void RotateCamBody(float rot)
    {
        transform.rotation *= Quaternion.Euler(0f, rot, 0f);
    }

    void RotateCamHead(float rot)
    {
        var euler = m_camHead.eulerAngles;
        euler.x = ClampAngle(euler.x - rot, -m_camClamp.y, -m_camClamp.x);
        m_camHead.eulerAngles = euler;
    }

    public static float ClampAngle(float current, float min, float max)
    {
        float dtAngle = Mathf.Abs(((min - max) + 180) % 360 - 180);
        float hdtAngle = dtAngle * 0.5f;
        float midAngle = min + hdtAngle;

        float offset = Mathf.Abs(Mathf.DeltaAngle(current, midAngle)) - hdtAngle;
        if (offset > 0)
            current = Mathf.MoveTowardsAngle(current, midAngle, offset);
        return current;
    }

    // Will do the relevant rising input based on the parameters the player is currently in.
    // eg. no walls detected and on the ground, causes a jump input.
    // wall in front and moving towards the wall, causes a wall climb.
    void RisingInput()
    {
        Debug.Log("Rising");

        if(m_controller.frontWall)
        {
            float frontWallDot = Vector3.Dot(m_controller.worldMoveInput, -m_controller.frontWallHit.normal);
            if(frontWallDot > m_climbDotCheck)
            {
                if (m_controller.TryStartWallClimb())
                {
                    return;
                }
            }
        }
        if(m_controller.currentSpeed > m_minSpeedWallRunEngage)
        {
            if(m_controller.TryStartWallRun())
            {
                return;
            }
        }


        m_controller.TryJump();
    }

    void AirborneWallInteraction()
    {
        if (m_controller.currentState != PlayerController.PlayerState.Airborne || m_controller.totalVelocity.y < 0.0f)
        {
            return;
        }

        if (m_controller.frontWall)
        {
            float frontWallDot = Vector3.Dot(m_controller.worldMoveInput, -m_controller.frontWallHit.normal);
            if (frontWallDot > m_climbDotCheck)
            {
                if (m_controller.TryStartWallClimb())
                {
                    return;
                }
            }
        }
        if (m_controller.currentSpeed > m_minSpeedWallRunEngage)
        {
            if (m_controller.TryStartWallRun())
            {
                return;
            }
        }
    }

    private void OnValidate()
    {
        m_climbDotCheck = Mathf.Cos(m_climbDegrees * Mathf.Deg2Rad);
    }
}