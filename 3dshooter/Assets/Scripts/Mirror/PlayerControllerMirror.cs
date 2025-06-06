using UnityEngine;
using Mirror;
public class PlayerControllerMirror : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Transform PlayerCamera;
    private CharacterController _controller;

    [Space(5)]
    [Header("Movement Settings")]
    [SerializeField] private float Speed = 5f;
    [SerializeField] private float Gravity = 9.81f;

    private float _verticalVelocity;

    [Space(5)]
    [Header("Camera Settings")]
    [Tooltip("Camera movement speed")]
    [SerializeField] private Vector2 Sensitivity;
    [Tooltip("Max range the camera can rotate")]
    [SerializeField] private float Degree = 90f;
    private Vector2 XYRotation;

    [Space(5)]
    [Header("Inputs")]
    private float HorizontalInput;
    private float VerticalInput;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();

        if (!isLocalPlayer && PlayerCamera != null)
        {
            PlayerCamera.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        InputManagement();
        Movement();
    }

    private void Movement()
    {
        GroundMovement();
        CameraRotation();
    }

    private void GroundMovement()
    {
        Vector3 move = new Vector3(HorizontalInput, 0, VerticalInput);
        move = transform.TransformDirection(move);

        move *= Speed;

        move.y = ApplyGravity();

        _controller.Move(move * Time.deltaTime);
    }

    private float ApplyGravity()
    {
        if (_controller.isGrounded)
        {
            _verticalVelocity = -1f;
        }
        else
        {
            _verticalVelocity -= Gravity * Time.deltaTime;
        }

        return _verticalVelocity;
    }

    private void InputManagement()
    {
        HorizontalInput = Input.GetAxis("Horizontal");
        VerticalInput = Input.GetAxis("Vertical");
    }

    private void CameraRotation()
    {
        Vector2 _mouseInput = new Vector2
        {
            x = Input.GetAxis("Mouse X"),
            y = Input.GetAxis("Mouse Y")
        };

        XYRotation.x -= _mouseInput.y * Sensitivity.y;
        XYRotation.y += _mouseInput.x * Sensitivity.x;

        XYRotation.x = Mathf.Clamp(XYRotation.x, -Degree, Degree);

        transform.eulerAngles = new Vector3(0f, XYRotation.y, 0f);
        PlayerCamera.localEulerAngles = new Vector3(XYRotation.x, 0f, 0f);
    }
}
