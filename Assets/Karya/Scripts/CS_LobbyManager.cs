using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CS_LobbyManager : NetworkLobbyManager
{

    public GameObject Lobby;
    public GameObject Player;
    private void Start()
    {
        Lobby.SetActive(false);
    }

    public override void OnStartHost()
    {
        base.OnStartHost();
        print("A games been created");
        Lobby.SetActive(true);
    }

    public override void OnClientConnect(NetworkConnection connection)
    {
        base.OnClientConnect(connection);
        NetworkServer.AddPlayerForConnection(connection, gamePlayerPrefab, connection.playerControllers[0].playerControllerId);
    }

}
