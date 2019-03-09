using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CS_LobbyPlayer : NetworkLobbyPlayer {

    [SerializeField]
    private GameObject ParentPref;
    [SerializeField]
    private Button JoinButton;
    [SerializeField]
    private Text PlayerNameText;
    [SerializeField]
    private Text ButtonText;
    private GameObject Canvas;

    public void OnClickJoinButton()
    {
        SendReadyToBeginMessage();
    }

    public override void OnClientEnterLobby()
    {
        base.OnClientEnterLobby();
        ParentPref = GameObject.FindGameObjectWithTag("Parent");
        gameObject.transform.SetParent(ParentPref.transform);
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        if(isLocalPlayer)
        {
            SetUp();
        }
        else
        {
            SetUpOtherPlayers();
        }
    }

    private void SetUp()
    {
            PlayerNameText.text = "MYPlayer";
            JoinButton.enabled = true;
            ButtonText.text = "Join";
    }

    private void SetUpOtherPlayers()
    {
            //If not local player
            PlayerNameText.text = "NotMyPlayer";
            JoinButton.enabled = false;
            ButtonText.text = "Waiting";
    }

}
