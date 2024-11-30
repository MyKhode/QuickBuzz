using UnityEngine;
using Unity.Netcode;

public class ClientDisconnectEvents : NetworkBehaviour
{
    // Called when the client disconnects
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        // Check if this is the local player
        if (IsOwner) 
        {
            // This will run when the current client disconnects
            Debug.Log($"Client with ID {NetworkManager.Singleton.LocalClientId} has disconnected.");

            // You can log more player-specific info if needed, for example:
            // Debug.Log($"Player {NetworkManager.Singleton.LocalClientId} disconnected at {Time.time}");
        }
    }

    // This method can be used to monitor when a client disconnects on the server side.
    private void OnClientDisconnect(ulong clientId)
    {
        Debug.Log($"Client with ID {clientId} has disconnected.");
    }

    void Start()
    {
        // Add the callback for when a client disconnects
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the callback when the object is destroyed
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
    }
}
