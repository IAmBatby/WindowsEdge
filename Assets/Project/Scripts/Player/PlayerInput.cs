using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] PlayerController m_controller;

    [SerializeField] Transform m_camHead;
    [SerializeField] float m_cameraSens = 1.0f;

    [SerializeField] Vector2 m_camClamp = new Vector2 (-89, 89);

    [SerializeField] float m_standBrake = 0.6f;
    [SerializeField] float m_crouchBrake = 0.1f;

    [SerializeField] float m_playerSpeed = 5.0f;
    [SerializeField] float m_crouchSpeedScale = 0.5f;
    [SerializeField] float m_runSpeedScale = 1.5f;

    [SerializeField] float m_standHeight = 2.0f;
    [SerializeField] float m_crouchHeightScale = 0.5f;
    [SerializeField] float m_camHeight = 1.6f;

    [SerializeField] Transform m_debug_model;

    float m_smoothCrouch = 1.0f;
    float m_smoothCrouchVel = 0.0f;
    [SerializeField] float m_smoothCrouchTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_smoothCrouch = 1.0f;
        m_controller.braking = m_standBrake;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalScale = 1.0f;
        float forwardScale = 1.0f;

        if (Input.GetAxisRaw("Fire1") != 0)
        {
            horizontalScale *= m_crouchSpeedScale;
            forwardScale *= m_crouchSpeedScale;

            Crouch();
        }
        else
        {
            Stand();

            if (Input.GetAxisRaw("Fire3") != 0)
            {
                m_controller.maxPlayerSpeed = m_playerSpeed * m_runSpeedScale;
            }
            else
            {
                m_controller.maxPlayerSpeed = m_playerSpeed;
            }
        }

        Move(horizontalScale, forwardScale);

        if (Input.GetAxisRaw("Jump") != 0)
        {
            m_controller.TryJump();
        }

        Look();
    }

    private void Move(float horizontalScale, float forwardScale)
    {
        var moveInput = Vector3.zero;
        moveInput.x = Input.GetAxisRaw("Horizontal") * horizontalScale;
        moveInput.z = Input.GetAxisRaw("Vertical") * forwardScale;

        moveInput = transform.TransformDirection(Vector3.ClampMagnitude(moveInput, 1.0f));

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

    void SmoothCrouch(float targetScale)
    {
        m_smoothCrouch = Mathf.SmoothDamp(m_smoothCrouch, targetScale, ref m_smoothCrouchVel, m_smoothCrouchTime);

        m_controller.SetCharacterHeight(m_standHeight * m_smoothCrouch);

        m_camHead.localPosition = Vector3.up * m_standHeight * m_smoothCrouch;

        var scale = m_debug_model.localScale;
        scale.y = m_smoothCrouch;
        m_debug_model.localScale = scale;
    }

    void Crouch()
    {
        m_controller.braking = m_crouchBrake;
        m_controller.maxPlayerSpeed = m_playerSpeed * m_crouchSpeedScale;

        SmoothCrouch(m_crouchHeightScale);
    }

    void Stand()
    {
        m_controller.braking = m_standBrake;
        m_controller.maxPlayerSpeed = m_playerSpeed;

        SmoothCrouch(1.0f);
    }
}