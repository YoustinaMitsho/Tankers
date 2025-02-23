using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : NetworkBehaviour
{
    [Header("Referances:")]
    [SerializeField] private Health _health;
    [SerializeField] private Image _healthBar;

    public override void OnNetworkSpawn()
    {
        if (!IsClient) return;
        _health.CurrentHealth.OnValueChanged += HandleHealthChange;
        HandleHealthChange(0, _health.CurrentHealth.Value);
    }

    public override void OnNetworkDespawn()
    {
        if(!IsClient) return;
        _health.CurrentHealth.OnValueChanged -= HandleHealthChange;
    }

    private void HandleHealthChange(int oldhealth, int newhealth)
    {
        _healthBar.fillAmount = (float) newhealth / _health.MaxHealth;
    }
}
