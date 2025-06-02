using UnityEngine;
using Mirror;

public class Move : NetworkBehaviour
{
    public Camera camera;
    public float speed = 5f;
    public float sprintMultiplier = 1.5f;
    public float jumpForce = 5f;
    public Rigidbody rb;
    public Transform cameraTransform; // <--- Asigna tu c�mara principal aqu�
    private bool isGrounded;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        camera = GetComponent<Camera>();
        //if (cameraTransform == null) cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (!isLocalPlayer) return;
        
        // Movimiento relativo a la c�mara
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        float currentSpeed = speed;
        if (Input.GetKey(KeyCode.LeftShift))
            currentSpeed *= sprintMultiplier;

        Vector3 move = cameraTransform.right * x + cameraTransform.forward * z;
        move.y = 0f;
        move = move.normalized * currentSpeed;

        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);

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

    public override void OnStartClient()
    {
        name = $"Player [{netId}| {(isLocalPlayer ? "local" : "remote")}]";
        Debug.Log("OnStartClient: " + name);
    }
    
    public override void OnStartServer()
    {
        name = $"Player[{netId}|server]";
        Debug.Log("OnStartServer: " + name);
    }
}