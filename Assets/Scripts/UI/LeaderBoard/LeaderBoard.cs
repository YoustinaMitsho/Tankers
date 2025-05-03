using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class LeaderBoard : NetworkBehaviour
{
    [SerializeField] private Transform LeaderBoardEntityHolder;
    [SerializeField] private LeaderBoardEntityDisplay leaderBoardEntityPrefab;
    [SerializeField] private int entitiesToDisplay = 8;

    private NetworkList<LeaderBoardEntityState> leaderBoardEntities;
    private List<LeaderBoardEntityDisplay> EntityDisplays = new List<LeaderBoardEntityDisplay>();

    private void Awake()
    {
        leaderBoardEntities = new NetworkList<LeaderBoardEntityState>();
    }

    public override void OnNetworkSpawn()
    {
        if(IsClient)
        {
            leaderBoardEntities.OnListChanged += HandleLeaderBoardEntitiesChanged;
            foreach (LeaderBoardEntityState state in leaderBoardEntities)
            {
                HandleLeaderBoardEntitiesChanged(new NetworkListEvent<LeaderBoardEntityState>
                {
                    Type = NetworkListEvent<LeaderBoardEntityState>.EventType.Add,
                    Value = state
                });
            }
        }

        if (!IsServer) { return; }

        TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);
        foreach (TankPlayer player in players)
        {
            HandlePlayerSpawn(player);
        }

        TankPlayer.OnPlayerSpawned += HandlePlayerSpawn;
        TankPlayer.OnPlayerDespawned += HandlePlayerDespawn;
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            leaderBoardEntities.OnListChanged -= HandleLeaderBoardEntitiesChanged;
        }

        if (IsServer)
        {
            TankPlayer.OnPlayerSpawned -= HandlePlayerSpawn;
            TankPlayer.OnPlayerDespawned -= HandlePlayerDespawn;
        }
    }

    private void HandleLeaderBoardEntitiesChanged(NetworkListEvent<LeaderBoardEntityState> changeEvent)
    {
        switch (changeEvent.Type)
        {
            case NetworkListEvent<LeaderBoardEntityState>.EventType.Add:
                if (!EntityDisplays.Any(x => x.ClientId == changeEvent.Value.ClientId))
                {
                    LeaderBoardEntityDisplay leaderboardentity =
                        Instantiate(leaderBoardEntityPrefab, LeaderBoardEntityHolder);
                    leaderboardentity.Initialize(
                        changeEvent.Value.ClientId,
                        changeEvent.Value.PlayerName,
                        changeEvent.Value.Coins);
                    EntityDisplays.Add(leaderboardentity);
                }
                break;
            case NetworkListEvent<LeaderBoardEntityState>.EventType.Remove:
                LeaderBoardEntityDisplay entityToRemove = EntityDisplays
                    .FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);
                if (entityToRemove != null)
                {
                    entityToRemove.transform.SetParent(null);
                    Destroy(entityToRemove.gameObject);
                    EntityDisplays.Remove(entityToRemove);
                }
                break;
            case NetworkListEvent<LeaderBoardEntityState>.EventType.Value:
                LeaderBoardEntityDisplay entityToUpdate = EntityDisplays
                    .FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);
                if (entityToUpdate != null)
                {
                    entityToUpdate.UpdateCoins(changeEvent.Value.Coins);
                }
                break;
        }

        EntityDisplays.Sort((x, y) => y.Coins.CompareTo(x.Coins));
        for (int i = 0; i < EntityDisplays.Count; i++)
        {
            EntityDisplays[i].transform.SetSiblingIndex(i);
            EntityDisplays[i].UpdateText();
            EntityDisplays[i].gameObject.SetActive(i <= entitiesToDisplay - 1);
        }
        LeaderBoardEntityDisplay myEntity = EntityDisplays
            .FirstOrDefault(x => x.ClientId == NetworkManager.Singleton.LocalClientId);
        if (myEntity != null)
        {
            if (myEntity.transform.GetSiblingIndex() >= entitiesToDisplay)
            {
                LeaderBoardEntityHolder.GetChild(entitiesToDisplay - 1).gameObject.SetActive(false);
                myEntity.gameObject.SetActive(true);
            }
        }
    }

    public void HandlePlayerSpawn(TankPlayer player)
    {
        leaderBoardEntities.Add(new LeaderBoardEntityState
        {
            ClientId = player.OwnerClientId,
            PlayerName = player.PlayerName.Value,
            Coins = 0
        });

        player.Wallet.totalCoins.OnValueChanged += (oldcoins, newcoins) =>
        HandleCoinChange(player.OwnerClientId, newcoins);
    }

    public void HandlePlayerDespawn(TankPlayer player)
    {
        if (leaderBoardEntities == null) return;

        foreach (LeaderBoardEntityState state in leaderBoardEntities)
        {
            if (state.ClientId == player.OwnerClientId)
            {
                leaderBoardEntities.Remove(state);
                break;
            }
        }

        player.Wallet.totalCoins.OnValueChanged -= (oldcoins, newcoins) =>
        HandleCoinChange(player.OwnerClientId, newcoins);
    }

    private void HandleCoinChange(ulong ownerClientId, int newcoins)
    {
        for (int i = 0; i < leaderBoardEntities.Count; i++)
        {
            if (leaderBoardEntities[i].ClientId == ownerClientId)
            {
                leaderBoardEntities[i] = new LeaderBoardEntityState
                {
                    ClientId = ownerClientId,
                    PlayerName = leaderBoardEntities[i].PlayerName,
                    Coins = newcoins
                };
                break;
            }
        }
    }
}
