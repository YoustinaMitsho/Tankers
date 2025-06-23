using TMPro;
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


    public void Show(string winnerName)
    {

        winnerPanel.SetActive(true);
        PostProcessVolume ppVolumn = Camera.main.gameObject.GetComponent<PostProcessVolume>();
        ppVolumn.enabled = true;
        winnerText.text = winnerName.ToString();
    }
}
