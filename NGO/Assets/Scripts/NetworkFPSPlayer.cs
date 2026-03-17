using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class NetworkFPSPlayer : NetworkBehaviour
{
    [Header("Player Components")]
    [SerializeField] private Transform cameraPivot;     // empty child at head height
    [SerializeField] private Camera playerCamera;       // child camera
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -20f;

    [Header("Player Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float maxPitch = 80f;

    private PlayerInput pi;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private CharacterController cc;

    private float pitch;   // Current up/down camera rotation
    private float verticalVelocity; // Y velocity for jumping/falling

    public override void OnNetworkSpawn()
    {
        cc = GetComponent<CharacterController>();
        pi = GetComponent<PlayerInput>();

        if (!IsOwner)
        {
            // Only the owning player should have an active camera & input
            if (playerCamera) playerCamera.enabled = false;
            if (pi) pi.enabled = false;
            enabled = false;
            return;
        }

        moveAction = pi.actions["Move"];
        lookAction = pi.actions["Look"];
        jumpAction = pi.actions["Jump"];
        moveAction.Enable();
        lookAction.Enable();
        jumpAction.Enable();

        if (playerCamera) playerCamera.enabled = true;
    }

    private void Update()
    {
        // Move (X/Z)
        Vector2 m = moveAction.ReadValue<Vector2>();
        Vector3 move = transform.right * m.x + transform.forward * m.y;
        cc.Move(move * moveSpeed * Time.deltaTime);

        // Look
        Vector2 look = lookAction.ReadValue<Vector2>() * lookSensitivity;
        transform.Rotate(0f, look.x, 0f); // Yaw = rotate the whole player left/right around the Y axis

        //Jump
        if (!cc.isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f; // keeps you stuck to ground

        if (!cc.isGrounded && jumpAction.WasPressedThisFrame())
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            cc.Move(Vector3.up * jumpHeight);
            Debug.Log("jump");
        }

        verticalVelocity += gravity * Time.deltaTime;

        pitch -= look.y;   // Pitch = rotate the cameraPivot up/down (invert look.y by subtracting)
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch); // Clamp camera so player doesn't turn over
        cameraPivot.localEulerAngles = new Vector3(pitch, 0f, 0f);  // Apply pitch to the camera pivot only (keeps the body upright)
    }
}