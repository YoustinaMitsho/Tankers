using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using Unity.VisualScripting;

public class HealingZone : NetworkBehaviour
{
    [Header("Referrences")]
    [SerializeField] private Image healPowerBar;
    [SerializeField] private AudioSource healSound;

    [Header("Settings")]
    [SerializeField] private float maxHealPower = 30f;
    [SerializeField] private float healCoolDown = 60f;
    [SerializeField] private float healTickRate = 1f;
    [SerializeField] private float coinsPerTick = 20f;
    [SerializeField] private float healthPerTick = 10f;

    private float remainingCoolDown;
    private float tickTimer;

    private List<TankPlayer> playersInZone = new List<TankPlayer>();
    private NetworkVariable<int> HealPower = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            HealPower.OnValueChanged += HandleHealPowerChange;
            HandleHealPowerChange(0, HealPower.Value);
        }

        if (IsServer)
        {
            HealPower.Value = (int)maxHealPower;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            HealPower.OnValueChanged -= HandleHealPowerChange;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!IsServer) return;
        if (collision.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player))
        {
            playersInZone.Add(player);
            healSound.Play();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsServer) return;
        if (collision.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player))
        {
            playersInZone.Remove(player);
            healSound.Stop();
        }
    }

    private void Update()
    {
        if (!IsServer) return;
        if (remainingCoolDown > 0f)
        {
            remainingCoolDown -= Time.deltaTime;
            if (remainingCoolDown <= 0f)
            {
                HealPower.Value = (int)maxHealPower;
            }
            else
            {
                return;
            }
        }

        tickTimer += Time.deltaTime;
        if (tickTimer >= 1 / healTickRate)
        {
            foreach (TankPlayer player in playersInZone)
            {
                if(HealPower.Value == 0) break;

                if(player.Health.CurrentHealth.Value == player.Health.MaxHealth) continue;

                if(player.Wallet.totalCoins.Value < coinsPerTick) continue;

                player.Wallet.SpendCoins((int)coinsPerTick);
                player.Health.Heal((int)healthPerTick);

                HealPower.Value -= 1;
                if (HealPower.Value == 0)
                {
                    remainingCoolDown = healCoolDown;
                }
            }

            tickTimer = tickTimer % (1 / healTickRate);
        }
    }

    private void HandleHealPowerChange(int oldValue, int newValue)
    {
        healPowerBar.fillAmount = (float)newValue / maxHealPower;
    }
}
