using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class TimerManger : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private NetworkVariable<int> timerValue = new NetworkVariable<int>(
    180, 
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            timerValue.OnValueChanged += HandleTimerValueChanged;
            UpdateTimerDisplay(timerValue.Value);
        }
    }

    public void StartTimer()
    {
        if (IsServer)
        {
            StartCoroutine(TimerCoroutine());
        }
    }

    private IEnumerator TimerCoroutine()
    {
        while (timerValue.Value > 0)
        {
            yield return new WaitForSeconds(1f);
            timerValue.Value--;
        }
    }

    private void HandleTimerValueChanged(int previousValue, int newValue)
    {
        UpdateTimerDisplay(newValue);
    }

    private void UpdateTimerDisplay(int value)
    {
        int minutes = value / 60;
        int seconds = value % 60;
        timerText.text = $"{minutes}:{seconds:D2}";
    }

    override public void OnNetworkDespawn()
    {
        if (IsClient)
        {
            timerValue.OnValueChanged -= HandleTimerValueChanged;
        }
    }
}
