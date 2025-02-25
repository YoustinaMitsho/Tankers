using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HostSingelton : MonoBehaviour
{
    private static HostSingelton _instance;
    public HostGameManager GameManager { get; private set; }
    public static HostSingelton Instance
    {
        get
        {
            if (_instance != null) return _instance;
            _instance = FindObjectOfType<HostSingelton>();

            if (_instance == null)
            {
                Debug.LogError("No HostSingelton");
                return null;
            }

            return _instance;
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void CreateHost()
    {
        GameManager = new HostGameManager();
    }
    private void OnDestroy()
    {
        GameManager.Dispose();
    }
}
