using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private TankPlayer playerPrefab;
    [SerializeField] private float KeptCoinPercent;
    [SerializeField] private AudioSource audioSource;

    private Dictionary<ulong, Action<Health, ulong>> _deathHandlers = new();

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);
        foreach (TankPlayer player in players)
        {
            HandlePlayerSpawned(player);
        }

        TankPlayer.OnPlayerSpawned += HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawned += HandlePlayerDespawned;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) { return; }

        TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
    }

    private void HandlePlayerSpawned(TankPlayer player)
    {
        //player.Health.onDie += (health) => HandlePlayerDie(player);
        Action<Health, ulong> handler = (health, killerId) => HandlePlayerDie(player, killerId); 
        _deathHandlers[player.OwnerClientId] = handler;
        player.Health.onDieWithKiller += handler;
    }

    private void HandlePlayerDespawned(TankPlayer player)
    {
        //player.Health.onDie -= (health) => HandlePlayerDie(player);
        if (_deathHandlers.TryGetValue(player.OwnerClientId, out var handler))
        {
            player.Health.onDieWithKiller -= handler;
            _deathHandlers.Remove(player.OwnerClientId);
        }
    }

    private void HandlePlayerDie(TankPlayer player, ulong killerClientId)
    {
        int keptMoney = (int)(player.Wallet.totalCoins.Value * (KeptCoinPercent / 100));

        ClientRpcParams rpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new List<ulong> { player.OwnerClientId, killerClientId }
            }
        };
        PlayDeathSoundClientRpc(rpcParams);

        Destroy(player.gameObject);

        StartCoroutine(RespawnPlayer(player.OwnerClientId, keptMoney));
    }

    [ClientRpc]
    private void PlayDeathSoundClientRpc(ClientRpcParams clientRpcParams = default)
    {
        audioSource.Play();
    }

    private IEnumerator RespawnPlayer(ulong ownerClientId, int keptMoney)
    {
        yield return null;

        TankPlayer playerInstance = Instantiate(
            playerPrefab, SpawnPoints.GetRandomSpawnPoint(), Quaternion.identity);
        playerInstance.NetworkObject.SpawnAsPlayerObject(ownerClientId);
        playerInstance.Wallet.totalCoins.Value += keptMoney;
    }
}
