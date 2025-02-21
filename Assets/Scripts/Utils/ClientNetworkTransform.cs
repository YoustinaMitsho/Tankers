using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

// this gives authority to clients to do actions not just the server (not changable)
public class ClientNetworkTransform : NetworkTransform
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        CanCommitToTransform = IsOwner;
    }

    protected override void Update()
    {
        CanCommitToTransform = IsOwner;
        base.Update();
        if(NetworkManager != null)
        {
            if(NetworkManager.IsConnectedClient || NetworkManager.IsListening)
            {
                if (CanCommitToTransform)
                {
                    TryCommitTransformToServer(transform, NetworkManager.LocalTime.Time);
                }
            }
        }
    }
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
