using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("Referances:")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private GameObject _clientProjectilePrefab;
    [SerializeField] private GameObject _serverProjectilePrefab;
    [SerializeField] private Transform _projectileSpawnPoint;
    [SerializeField] private GameObject _muzzleFlash;
    [SerializeField] private Collider2D _playerCollider;
    [Header("Projectile Settings:")]
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _muzzleFlashDuration;

    private bool fire;
    private float previousFireTime;
    private float muzzleFlashTimer;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        _inputReader.PrimaryFireEvent += HandleShoot;
    }

    public override void OnNetworkDespawn()
    {
        if(!IsOwner) return;

        _inputReader.PrimaryFireEvent -= HandleShoot;
    }
    void Update()
    {
        if (muzzleFlashTimer > 0f)
        {
            muzzleFlashTimer -= Time.deltaTime;

            if(muzzleFlashTimer <= 0f)
            {
                _muzzleFlash.SetActive(false);
            }
        }

        if (!IsOwner) return;
        if(!fire) return;

        if (Time.time < (1 / _fireRate) + previousFireTime) return;

        SpawnServerProjectileServerRpc(_projectileSpawnPoint.position, _projectileSpawnPoint.up);
        SpawnDummyProjectile(_projectileSpawnPoint.position, _projectileSpawnPoint.up);
        
        previousFireTime = Time.time;
    }

    [ServerRpc]
    private void SpawnServerProjectileServerRpc(Vector3 SpawnPos, Vector3 direction)
    {
        GameObject ServerProjectile = Instantiate(
            _serverProjectilePrefab,
            SpawnPos,
            Quaternion.identity);

        ServerProjectile.transform.up = direction;

        Physics2D.IgnoreCollision(_playerCollider, ServerProjectile.GetComponent<Collider2D>());
        if (ServerProjectile.TryGetComponent<Rigidbody2D>(out Rigidbody2D body))
        {
            body.velocity = body.transform.up * _projectileSpeed;
        }
        SpawnClientProjectileClientRpc(SpawnPos, direction);
    }
    private void SpawnDummyProjectile(Vector3 SpawnPos, Vector3 direction)
    {
        _muzzleFlash.SetActive(true);
        muzzleFlashTimer = _muzzleFlashDuration;

        GameObject ClientProjectile = Instantiate(
            _clientProjectilePrefab,
            SpawnPos,
            Quaternion.identity);

        ClientProjectile.transform.up = direction;

        Physics2D.IgnoreCollision(_playerCollider, ClientProjectile.GetComponent<Collider2D>());

        if(ClientProjectile.TryGetComponent<Rigidbody2D>(out Rigidbody2D body))
        {
            body.velocity = body.transform.up * _projectileSpeed;
        }
    }

    [ClientRpc]
    private void SpawnClientProjectileClientRpc(Vector3 SpawnPos, Vector3 direction)
    {
        if(IsOwner) return;
        SpawnDummyProjectile(SpawnPos, direction);
    }

    void HandleShoot(bool fire)
    {
        this.fire = fire;
    }
}
