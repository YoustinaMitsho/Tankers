using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAimming : NetworkBehaviour
{
    [Header("Referances")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Transform _transform;

    void LateUpdate()
    {
        if (!IsOwner) return;
        Vector2 aimPosition = _inputReader.AimPosition;
        aimPosition = Camera.main.ScreenToWorldPoint(aimPosition);
        _transform.up = new Vector2(
            aimPosition.x - _transform.position.x,
            aimPosition.y - _transform.position.y);
    }
}
