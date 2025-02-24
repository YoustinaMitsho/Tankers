using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField _joinCodeField;
    public async void StartHost()
    {
        await HostSingelton.Instance.GameManager.StartHostAsync();
    }

    public async void StartClient()
    {
        await ClientSingelton.Instance.GameManager.StartClientAsync(_joinCodeField.text);
    }
}
