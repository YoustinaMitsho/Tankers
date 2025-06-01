using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameHud : MonoBehaviour
{
    public void LeaveGame()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            HostSingelton.Instance.GameManager.ShutDown();
        }

        ClientSingelton.Instance.GameManager.Disconnect();
    }
}
