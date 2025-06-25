using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [field: SerializeField] public int MaxHealth { get; private set; } = 100;
    public NetworkVariable<int> CurrentHealth = new NetworkVariable<int>();

    bool isDead;
    public Action<Health> onDie;
    public Action<Health, ulong> onDieWithKiller;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        CurrentHealth.Value = MaxHealth;
    }

    public void TakeDamage(int damage)
    {
        ModifyHealth(-damage, ulong.MaxValue);
    }

    public void TakeDamage(int damage, ulong attackerClientId)
    {
        ModifyHealth(-damage, attackerClientId);
    }

    public void Heal(int healAmount)
    {
        ModifyHealth(healAmount, ulong.MaxValue);
    }

    public void ModifyHealth(int health, ulong attackerClientId)
    {
        if(isDead) return;
        int newhealth = CurrentHealth.Value + health;
        CurrentHealth.Value = Mathf.Clamp(newhealth, 0, MaxHealth);
        if (CurrentHealth.Value == 0)
        {
            onDie?.Invoke(this);
            onDieWithKiller?.Invoke(this, attackerClientId);
            isDead = true;
        }
    }
}
