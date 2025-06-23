using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class TimerManger : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private NetworkVariable<int> timerValue = new NetworkVariable<int>(
    20, 
    NetworkVariableReadPermission.Everyone,
    NetworkVariableWritePermission.Server);

    private bool hasEnded;
    private bool isPulsing = false;
    private Vector3 originalScale;


    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            originalScale = timerText.transform.localScale;
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

        if (value <= 30)
        {
            timerText.color = Color.red;

            if (!isPulsing)
            {
                StartCoroutine(PulseTimerText());
                isPulsing = true;
            }
        }
        else
        {
            timerText.color = Color.white;

            if (isPulsing)
            {
                StopCoroutine(PulseTimerText());
                timerText.transform.localScale = originalScale;
                isPulsing = false;
            }
        }

        if (value == 0 && IsServer && !hasEnded)
        {
            Time.timeScale = 0f;
            hasEnded = true;
            string winner = FindObjectOfType<LeaderBoard>().GetTopPlayerName();
            ShowGameOverUIClientRpc(winner);
        }

        if(value == 0 && IsClient && !hasEnded)
        {
            Time.timeScale = 0f;
        }

    }

    [ClientRpc]
    private void ShowGameOverUIClientRpc(string winnerName)
    {
        WinnerEffect.Instance.Show(winnerName);
    }

    private IEnumerator PulseTimerText()
    {
        float pulseDuration = 0.5f;
        float scaleAmount = 1.2f;

        while (true)
        {
            timerText.transform.localScale = originalScale * scaleAmount;
            yield return new WaitForSeconds(pulseDuration / 2f);

            timerText.transform.localScale = originalScale;
            yield return new WaitForSeconds(pulseDuration / 2f);
        }
    }


    override public void OnNetworkDespawn()
    {
        if (IsClient)
        {
            timerValue.OnValueChanged -= HandleTimerValueChanged;
        }
    }
}
