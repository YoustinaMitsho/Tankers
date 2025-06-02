using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Core;
using UnityEngine;

public class ServerSingelton : MonoBehaviour
{
    private static ServerSingelton _instance;
    public ServerGameManager GameManager { get; private set; }
    public static ServerSingelton Instance
    {
        get
        {
            if (_instance != null) return _instance;
            _instance = FindObjectOfType<ServerSingelton>();

            if (_instance == null)
            {
                Debug.LogError("No ServerSingelton");
                return null;
            }

            return _instance;
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public async Task CreateServer()
    {
        await UnityServices.InitializeAsync();

        GameManager = new ServerGameManager(
            ApplicationData.IP(),
            ApplicationData.Port(),
            ApplicationData.QPort(),
            NetworkManager.Singleton
            );
    }
    private void OnDestroy()
    {
        GameManager.Dispose();
    }
}
