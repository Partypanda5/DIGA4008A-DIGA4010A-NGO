using Unity.Netcode; 
using UnityEngine;   

public class NetworkProjectile : NetworkBehaviour
{
    [SerializeField] private float lifeTime = 3f; // Seconds before auto-despawn

    public override void OnNetworkSpawn()
    {
        // Only the server should despawn network objects (authoritative)
        if (IsServer)
            Invoke(nameof(Despawn), lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Only the server should decide when the projectile disappears
        if (IsServer)
            Despawn();
    }

    private void Despawn()
    {
        // Despawn removes it across the network (for everyone)
        if (NetworkObject && NetworkObject.IsSpawned)
            NetworkObject.Despawn();
    }
}