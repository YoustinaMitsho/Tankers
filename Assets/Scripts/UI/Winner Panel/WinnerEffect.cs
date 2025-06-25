using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class WinnerEffect : MonoBehaviour
{
    public static WinnerEffect Instance { get; private set; }

    [SerializeField] private GameObject winnerPanel;
    [SerializeField] private TextMeshProUGUI winnerText;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        winnerPanel.SetActive(false);
        PostProcessVolume ppVolumn = Camera.main.gameObject.GetComponent<PostProcessVolume>();
        ppVolumn.enabled = false;
    }
    public void ShowOnAllClients(string winnerName)
    {
        ShowClientRpc(winnerName);
    }

    [ClientRpc]
    private void ShowClientRpc(string winnerName)
    {
        winnerPanel.SetActive(true);
        winnerText.text = winnerName;

        Camera.main.GetComponent<PostProcessVolume>().enabled = true;
        winnerPanel.GetComponent<AudioSource>().Play();
    }
}
