using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class GameHud : MonoBehaviour
{
    [SerializeField] private GameObject startGameButton;

    private void Awake()
    {
        startGameButton.SetActive(true);
    }


    public void LeaveGame()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            HostSingelton.Instance.GameManager.ShutDown();
        }

        ClientSingelton.Instance.GameManager.Disconnect();
        Time.timeScale = 1f;
    }

    public async void StartGame()
    {
        if (!NetworkManager.Singleton.IsHost) return;

        FindObjectOfType<TimerManger>().StartTimer();

        await HostSingelton.Instance.GameManager.DeleteLobby();

        NetworkManager.Singleton.ConnectionApprovalCallback = (request, response) =>
        {
            response.Approved = false;
            response.Reason = "Game already started";
        };

        TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);
        foreach (TankPlayer player in players)
        {
            player.GetComponent<CoinWallet>().totalCoins.Value = 0;
        }
    }

}
