using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField _joinCodeField;
    [SerializeField] private Button _hostButton;

    private void Start()
    {
        _hostButton.interactable = true;
    }
    public async void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback = null;
        await HostSingelton.Instance.GameManager.StartHostAsync();
        _hostButton.interactable = false;
    }

    public async void StartClient()
    {
        await ClientSingelton.Instance.GameManager.StartClientAsync(_joinCodeField.text);
    }
}
