using Unity.Netcode;          
using UnityEngine;            
using UnityEngine.InputSystem; 

public class NetworkGun : NetworkBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private NetworkObject projectilePrefab;
    [SerializeField] private Transform firePoint;          
    [SerializeField] private float projectileSpeed = 20f;   

    private PlayerInput pi;        
    private InputAction shootAction;

    public override void OnNetworkSpawn()
    {
        // Only the owning client should read the shoot input
        if (!IsOwner)
        {
            enabled = false; // disables Update() on non-owners
            return;
        }

        // Get PlayerInput and Fire action
        pi = GetComponent<PlayerInput>();
        shootAction = pi.actions["Shoot"];
        shootAction.Enable(); // allow reading input
    }

    private void Update()
    {
        if (!IsOwner) return; // safety

        // WasPressedThisFrame triggers once per button press
        if (shootAction.WasPressedThisFrame())
        {
            // Tell the server to spawn a projectile
            ShootServerRpc(firePoint.position, firePoint.forward);
        }
    }

    // Remember, ServerRpc means: called by a client, executed on the server for anti cheating
    [ServerRpc]
    private void ShootServerRpc(Vector3 pos, Vector3 forward)
    {
        // Create the projectile on the server
        var proj = Instantiate(projectilePrefab, pos, Quaternion.LookRotation(forward));

        // Spawn it as a networked object so all clients see it
        proj.Spawn();

        // Give it forward velocity (requires Rigidbody on projectile prefab)
        var rb = proj.GetComponent<Rigidbody>();
        rb.linearVelocity = forward * projectileSpeed;

        Debug.Log("shooting");
    }
}