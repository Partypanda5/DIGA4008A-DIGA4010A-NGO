using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class NetworkFPSPlayer : NetworkBehaviour
{
    [Header("Player Components")]
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Animator animator;          // assign or auto-find

    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -20f;

    [Header("Player Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float maxPitch = 80f;

    [Header("Animator Params")]
    [SerializeField] private string speedParam = "Speed";

    private PlayerInput pi;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private CharacterController cc;

    private float pitch;
    private float verticalVelocity;

    public override void OnNetworkSpawn()
    {
        cc = GetComponent<CharacterController>();
        pi = GetComponent<PlayerInput>();

        if (!IsOwner)
        {
            if (playerCamera) playerCamera.enabled = false;
            if (pi) pi.enabled = false; // remote players don't read input
            return;                     // don't disable the whole script, jiust this void
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
        if (IsOwner)
        {
            // look
            Vector2 look = lookAction.ReadValue<Vector2>() * lookSensitivity;
            transform.Rotate(0f, look.x, 0f);

            pitch -= look.y;
            pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);
            cameraPivot.localEulerAngles = new Vector3(pitch, 0f, 0f);

            // move
            Vector2 m = moveAction.ReadValue<Vector2>();
            Vector3 horizontal = (transform.right * m.x + transform.forward * m.y) * moveSpeed;

            // jump/gravity
            if (cc.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
            if (cc.isGrounded && jumpAction.WasPressedThisFrame())
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

            verticalVelocity += gravity * Time.deltaTime;

            Vector3 velocity = horizontal + Vector3.up * verticalVelocity;
            cc.Move(velocity * Time.deltaTime);

            // drive animation from actual movement input
            if (animator) animator.SetFloat(speedParam, m.magnitude); // 0 = idle, >0 = walking. m.mag turns 2D input into a single number - “how much the player is trying to move”.
        }
    }

}