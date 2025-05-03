using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Cinemachine;
using Unity.Collections;
using System;

public class TankPlayer : NetworkBehaviour
{
    [Header("References:")]
    [SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;
    [field: SerializeField] public Health Health { get; private set; }
    [field: SerializeField] public CoinWallet Wallet { get; private set; }

    [Header("Settings:")]
    [SerializeField] private int _cameraPriority = 15;

    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();
    public static event Action<TankPlayer> OnPlayerSpawned;
    public static event Action<TankPlayer> OnPlayerDespawned;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            UserData userData = HostSingelton.Instance.GameManager.NetworkServer.GetUserDataByClientID(OwnerClientId);
            PlayerName.Value = userData.UserName;

            OnPlayerSpawned?.Invoke(this);
        }
        if (IsOwner)
        {
            _cinemachineVirtualCamera.Priority = _cameraPriority;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            OnPlayerDespawned?.Invoke(this);
        }
    }
}
