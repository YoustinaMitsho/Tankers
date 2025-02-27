using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager _networkManager;
    private Dictionary<ulong, string> clientIdToAuth = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> authIdToUserData = new Dictionary<string, UserData>();
    
    public NetworkServer(NetworkManager networkManager)
    {
        _networkManager = networkManager;
        _networkManager.ConnectionApprovalCallback += ApprovalCheck;

        networkManager.OnServerStarted += OnNetworkReady;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
        UserData userData = JsonUtility.FromJson<UserData>(payload);

        clientIdToAuth[request.ClientNetworkId] = userData.AuthId;
        authIdToUserData[userData.AuthId] = userData;


        response.Approved = true;
        response.Position = SpawnPoints.GetRandomSpawnPoint();
        response.Rotation = quaternion.identity;
        response.CreatePlayerObject = true;
    }

    private void OnNetworkReady()
    {
        _networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (clientIdToAuth.TryGetValue(clientId, out string authId))
        {
            clientIdToAuth.Remove(clientId);
            authIdToUserData.Remove(authId);
        }
    }

    public UserData GetUserDataByClientID(ulong clientId)
    {
        if (clientIdToAuth.TryGetValue(clientId, out string authId))
        {
            if (authIdToUserData.TryGetValue(authId, out UserData user))
            {
                return user;
            }

            return null;
        }
        return null;
    }

    public void Dispose()
    {
        if (_networkManager == null) { return; }

        _networkManager.ConnectionApprovalCallback -= ApprovalCheck;
        _networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        _networkManager.OnServerStarted -= OnNetworkReady;

        if (_networkManager.IsListening)
        {
            _networkManager.Shutdown();
        }

    }
}
