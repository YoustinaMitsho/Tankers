using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingelton _clientPrefab;
    [SerializeField] private HostSingelton _hostPrefab;
    async void Start()
    {
        DontDestroyOnLoad(gameObject);

        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {

        }
        else
        {
            ClientSingelton clientSingelton = Instantiate(_clientPrefab);
            bool authenticated = await clientSingelton.CreateClient();

            HostSingelton hostSingelton = Instantiate(_hostPrefab);
            hostSingelton.CreateHost();

            if(authenticated)
            {
                clientSingelton.GameManager.GoToMenu();
            }
        }
    }
}
