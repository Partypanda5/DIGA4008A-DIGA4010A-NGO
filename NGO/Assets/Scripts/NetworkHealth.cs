using Unity.Netcode;
using UnityEngine;
public class NetworkHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    // Everyone can read, only server can write
    public NetworkVariable<int> Health = new NetworkVariable<int>(
        100,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]

    //RPC = Marks the method as an NGO RPC, meaning it can be invoked across the network.
    //SendTo.Server = When a client calls this method, the call is sent to the server and the methods code runs on the server (not locally on the client).
    //InvokePermission = RpcInvokePermission.Everyone = Allows any client to invoke (call) this RPC, not just the object’s owner.
    public void TakeDamageServerRpc(int amount)
    {
        Health.Value -= amount;

        if (Health.Value <= 0)
            Health.Value = maxHealth;
    }
    public float Health01
    {
        get
        {
            if (maxHealth == 0) //If maxHealth is 0 it returns 0f (to avoid dividing by zero).
            {
                return 0f;
            }
            else
            {
                return (float)Health.Value / maxHealth; //Otherwise it returns Health.Value / maxHealth as a float.
            }
        }
    }
}