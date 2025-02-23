using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class CoinSpawner : NetworkBehaviour
{
    [SerializeField] private RespawningCoin _coinPrefab;
    [SerializeField] private int _maxCoins;
    [SerializeField] private int _coinValue = 10;
    [SerializeField] private Vector2 _xSpawnRange;
    [SerializeField] private Vector2 _ySpawnRange;
    [SerializeField] private LayerMask _layerMask;

    private float coinRaduis;
    private Collider2D[] coinBuffer = new Collider2D[1];

    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;

        coinRaduis = _coinPrefab.GetComponent<CircleCollider2D>().radius;

        for (int i = 0; i < _maxCoins; i++)
        {
            SpawnCoin();
        }
    }
    private Vector2 GetSpawnPoint()
    {
        float x = 0;
        float y = 0;
        while (true)
        {
            x = Random.Range(_xSpawnRange.x, _xSpawnRange.y);
            y = Random.Range(_ySpawnRange.x, _ySpawnRange.y);
            Vector2 SpawnPoint = new Vector2(x, y);
            int numColliders = Physics2D.OverlapCircleNonAlloc(
                SpawnPoint, coinRaduis, coinBuffer, _layerMask);
            if(numColliders == 0) return SpawnPoint;
        }
    }

    private void SpawnCoin()
    {
        RespawningCoin coinInstance =
            Instantiate(_coinPrefab, GetSpawnPoint(), Quaternion.identity);

        coinInstance.SetValue(_coinValue);
        coinInstance.GetComponent<NetworkObject>().Spawn();

        coinInstance.onCollected += HamdleCoinCollected;
    }

    private void HamdleCoinCollected(RespawningCoin coin)
    {
        coin.transform.position = GetSpawnPoint();
        coin.Reset();
    }
}
