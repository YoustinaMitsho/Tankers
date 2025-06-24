using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameHud : NetworkBehaviour
{
    [SerializeField] private GameObject startGameButton;
    /*[SerializeField] private GameObject joinCodePanel;
    [SerializeField] private TextMeshProUGUI joinCodeText;*/

    /*private void Start()
    {
        if(NetworkManager.Singleton.IsHost)
            startGameButton.SetActive(true);

        //ShowJoinCode();
    }*/

    public override void OnNetworkSpawn()
    {
        if (NetworkManager.Singleton.IsHost)
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
            player.GetComponent<Health>().Heal(100);
        }
    }

    /*public void ShowJoinCode()
    {
        if (HostSingelton.Instance.GameManager.JoinCode != null)
        {
            joinCodeText.text = $"Join Code: {HostSingelton.Instance.GameManager.JoinCode}";
            joinCodePanel.SetActive(true);
        }
    }*/
}
