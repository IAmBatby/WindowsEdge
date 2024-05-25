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

    // Start is called before the first frame update
    void Start()
    {

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

        if (Input.GetAxisRaw("Jump") != 0)
        {
            m_controller.TryJump();
        }

        if (Input.GetMouseButtonDown(1))
        {
            m_controller.TryStartWallRun();
        }

        Look();
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
}