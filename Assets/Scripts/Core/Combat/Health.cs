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

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        CurrentHealth.Value = MaxHealth;
    }

    public void TakeDamage(int damage)
    {
        ModifyHealth(-damage);
    }

    public void Heal(int healAmount)
    {
        ModifyHealth(healAmount);
    }

    public void ModifyHealth(int health)
    {
        if(isDead) return;
        int newhealth = CurrentHealth.Value + health;
        CurrentHealth.Value = Mathf.Clamp(newhealth, 0, MaxHealth);
        if (CurrentHealth.Value == 0)
        {
            onDie?.Invoke(this);
            isDead = true;
        }
    }
}
