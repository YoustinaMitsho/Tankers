using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text;
using System.Net;
using Unity.Services.Authentication;

public class ClientGameManager : IDisposable
{
    private const string MenuSceneName = "Menu";
    private JoinAllocation joinAllocation;
    private NetworkClient _networkClient;
    public async Task<bool> InitAsync()
    {
        await UnityServices.InitializeAsync();

        _networkClient = new NetworkClient(NetworkManager.Singleton);

        AuthenticationState state = await AuthenticationHandler.DoAuth();

        if (state == AuthenticationState.Authenticated) return true;

        return false;
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(MenuSceneName);
    }

    public async Task StartClientAsync(string JoinCode)
    {
        try
        {
            joinAllocation = await Relay.Instance.JoinAllocationAsync(JoinCode);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        RelayServerData relayServerData = new RelayServerData(joinAllocation, "udp");
        transport.SetRelayServerData(relayServerData);

        UserData userData = new UserData
        {
            UserName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name"),
            AuthId = AuthenticationService.Instance.PlayerId
        };
        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;

        NetworkManager.Singleton.StartClient();
    }
    public void Disconnect()
    {
        _networkClient.Disconnect();
    }

    public void Dispose()
    {
        _networkClient?.Dispose();
    }

}
