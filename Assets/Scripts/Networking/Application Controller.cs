using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingelton _clientPrefab;
    [SerializeField] private HostSingelton _hostPrefab;
    [SerializeField] private ServerSingelton _serverPrefab;
    async void Start()
    {
        DontDestroyOnLoad(gameObject);

        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {
            ServerSingelton serverSingelton = Instantiate(_serverPrefab);
            await serverSingelton.CreateServer();
            await serverSingelton.GameManager.StartGameServerAsync();
        }
        else
        {
            HostSingelton hostSingelton = Instantiate(_hostPrefab);
            hostSingelton.CreateHost();

            ClientSingelton clientSingelton = Instantiate(_clientPrefab);
            bool authenticated = await clientSingelton.CreateClient();

            if(authenticated)
            {
                clientSingelton.GameManager.GoToMenu();
            }
        }
    }
}
