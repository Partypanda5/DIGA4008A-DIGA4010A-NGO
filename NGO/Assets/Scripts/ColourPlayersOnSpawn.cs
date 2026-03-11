using Unity.Netcode;
using UnityEngine;

public class ColourPlayersOnSpawn : MonoBehaviour
{
    private void OnEnable()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected; 
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    private void OnClientConnected(ulong clientId)
    {
        // Host/server can see spawned player objects and set their colours
        if (!NetworkManager.Singleton.IsServer) return;

        // Get the player's NetworkObject for that client
        NetworkObject playerObj = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        if (playerObj == null) return;

        // Pick a repeatable colour based on clientId (debug-friendly)
        Random.InitState((int)clientId);
        Color c = Random.ColorHSV();

        // Apply to the first Renderer found on the player
        Renderer r = playerObj.GetComponentInChildren<Renderer>();
        if (r != null) r.material.color = c;
    }
}