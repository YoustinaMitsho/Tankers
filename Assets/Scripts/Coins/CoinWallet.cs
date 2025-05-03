using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    [Header("Referrences")]
    [SerializeField] private BountyCoin coinPrefab;
    [SerializeField] private Health health;

    [Header("Settings")]
    [SerializeField] private float coinSpread = 3f;
    [SerializeField] private float bountyPercent = 50f;
    [SerializeField] private int bountyCoinCount = 10;
    [SerializeField] private int minBountyCoinsValue = 5;
    [SerializeField] private LayerMask layerMask;
    private Collider2D[] coinBuffer = new Collider2D[1];
    private float coinRadius;

    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;
        coinRadius = coinPrefab.GetComponent<CircleCollider2D>().radius;
        health.onDie += HandleDie;
    }

    public override void OnNetworkDespawn()
    {
        if(!IsServer) return;
        health.onDie -= HandleDie;
    }
    private void HandleDie(Health health)
    {
        int bountyValue = (int)(totalCoins.Value * (bountyPercent / 100f));
        int bountyCoinValue = bountyValue / bountyCoinCount;
        if (bountyCoinValue < minBountyCoinsValue) return;

        for (int i = 0; i < bountyCoinCount; i++)
        {
            Vector2 spawnPoint = GetSpawnPoint();
            BountyCoin coinInstance = Instantiate(coinPrefab, spawnPoint, Quaternion.identity);
            coinInstance.SetValue(bountyCoinValue);
            coinInstance.NetworkObject.Spawn();
        }
    }

    public NetworkVariable<int> totalCoins = new NetworkVariable<int>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<Coin>(out Coin coin)) return;
        
        int coinvalue = coin.Collect();

        if(!IsServer) return;

        totalCoins.Value += coinvalue;
        
    }

    public void SpendCoins(int amount)
    {
        totalCoins.Value -= amount;
    }

    private Vector2 GetSpawnPoint()
    {
        while (true)
        {
            Vector2 SpawnPoint = (Vector2) transform.position + UnityEngine.Random.insideUnitCircle * coinSpread;
            int numColliders = Physics2D.OverlapCircleNonAlloc(
                SpawnPoint, coinRadius, coinBuffer, layerMask);
            if (numColliders == 0) return SpawnPoint;
        }
    }
}
