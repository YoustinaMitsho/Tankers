using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Cinemachine;
using Unity.Collections;

public class TankPlayer : NetworkBehaviour
{
    [Header("References:")]
    [SerializeField] private CinemachineVirtualCamera _cinemachineVirtualCamera;

    [Header("Settings:")]
    [SerializeField] private int _cameraPriority = 15;

    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            UserData userData = HostSingelton.Instance.GameManager.NetworkServer.GetUserDataByClientID(OwnerClientId);
            PlayerName.Value = userData.UserName;
        }
        if (IsOwner)
        {
            _cinemachineVirtualCamera.Priority = _cameraPriority;
        }
    }
}
