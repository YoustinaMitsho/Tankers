using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovmenet : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Transform _tankBody;
    [SerializeField] private Rigidbody2D _rigidbody;
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 4f;
    [SerializeField] private float _TurningRate = 30f;

    private Vector2 _previousMovmentInput;

    public override void OnNetworkSpawn()
    {
        if(!IsOwner) return;

        _inputReader.MoveEvent += HandleMove;
    }

    private void HandleMove(Vector2 MoveInput)
    {
        _previousMovmentInput = MoveInput;
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) return;

        float zRotation = _previousMovmentInput.x * -_TurningRate * Time.deltaTime;
        _tankBody.Rotate(0, 0, zRotation);
    }

    void FixedUpdate()
    {
        if(!IsOwner) return;

        _rigidbody.velocity = (Vector2)_tankBody.up * _previousMovmentInput.y * _moveSpeed;
    }

    public override void OnNetworkDespawn()
    {
        if(!IsOwner) return;

        _inputReader.MoveEvent -= HandleMove;
    }
    
}
