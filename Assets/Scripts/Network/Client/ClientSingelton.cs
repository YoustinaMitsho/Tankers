using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ClientSingelton : MonoBehaviour
{
    private static ClientSingelton _instance;
    public ClientGameManager GameManager {  get; private set; }
    public static ClientSingelton Instance
    {
        get 
        {
            if (_instance != null) return _instance; 
            _instance = FindObjectOfType<ClientSingelton>();

            if (_instance == null)
            {
                Debug.LogError("No ClientSingelton");
                return null;
            }

            return _instance;
        }
    }
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public async Task<bool> CreateClient()
    {
        GameManager = new ClientGameManager();

        return await GameManager.InitAsync();
    }
}
