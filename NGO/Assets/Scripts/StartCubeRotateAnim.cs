using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class StartCubeRotateAnim : NetworkBehaviour
{
    private NetworkAnimator netAnim;

    public override void OnNetworkSpawn()
    {
        netAnim = GetComponent<NetworkAnimator>();

        // Only the server tells everyone to start - On this machine, is the NGO server running?
        if (IsServer)
        {
            // Trigger is synchronised by NetworkAnimator
            netAnim.SetTrigger("StartRotate");
        }
    }
}