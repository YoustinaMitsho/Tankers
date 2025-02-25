using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameSelector : MonoBehaviour
{
    [Header("References:")]
    [SerializeField] private TMP_InputField _nameField;
    [SerializeField] private Button _connectButton;
    [Header("Settings:")]
    [SerializeField] private int _minNameLength = 4;
    [SerializeField] private int _maxNameLength = 25;

    public const string PlayerNameKey = "PlayerName";

    void Start()
    {
        if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            return;
        }

        _nameField.text = PlayerPrefs.GetString(PlayerNameKey, string.Empty);
        HandleNameChange();
    }

    public void HandleNameChange()
    {
        _connectButton.interactable = 
            (_nameField.text.Length >= _minNameLength && _nameField.text.Length <= _maxNameLength);
    }

    public void Connect()
    {
        PlayerPrefs.SetString(PlayerNameKey, _nameField.text);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
