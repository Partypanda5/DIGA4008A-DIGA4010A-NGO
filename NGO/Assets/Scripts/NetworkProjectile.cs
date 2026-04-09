using Unity.Netcode;
using UnityEngine;

public class NetworkProjectile : NetworkBehaviour
{
    [SerializeField] private int damage = 25;
    [SerializeField] private float lifeTime = 3f;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            Invoke(nameof(Despawn), lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;

        // Try find health on whatever we hit
        var health = collision.collider.GetComponentInParent<NetworkHealth>();
        if (health != null)
        {
            health.TakeDamageServerRpc(damage);
        }

        Despawn();
    }

    private void Despawn()
    {
        if (NetworkObject != null && NetworkObject.IsSpawned)
            NetworkObject.Despawn();
    }
}