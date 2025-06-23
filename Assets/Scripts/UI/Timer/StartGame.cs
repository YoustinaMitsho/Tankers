using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class StartGame : NetworkBehaviour
{
    [SerializeField] private GameObject startGameButton;

    public override void OnNetworkSpawn()
    {
        Time.timeScale = 1f;

        if (!IsHost)
        {
            startGameButton.SetActive(false);
        }
    }

}
