using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float sprintMultiplier = 1.5f;
    public float jumpForce = 5f;
    public Rigidbody rb;
    public Transform cameraTransform; // <--- Asigna tu cámara principal aquí
    private bool isGrounded;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (cameraTransform == null) cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // Movimiento relativo a la cámara
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        float currentSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift))
            currentSpeed *= sprintMultiplier;

        Vector3 move = cameraTransform.right * x + cameraTransform.forward * z;
        move.y = 0f;
        move = move.normalized * currentSpeed;

        rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);

        // Salto
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void OnCollisionStay(Collision other)
    {
        isGrounded = true;
    }

    void OnCollisionExit(Collision other)
    {
        isGrounded = false;
    }
}