using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

    public void StartGame()
    {
        if (!NetworkManager.Singleton.IsHost) return;

        FindObjectOfType<TimerManger>().StartTimer();

        HostSingelton.Instance.GameManager.DeleteLobby();

        NetworkManager.Singleton.ConnectionApprovalCallback = (request, response) =>
        {
            response.Approved = false;
            response.Reason = "Game already started";
        };

    }

}
