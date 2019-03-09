using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class CS_HostSetup : MonoBehaviour {


    MatchInfoSnapshot match;
    [SerializeField]
    private Text HostName;

    CS_LobbyManager lobbyManager;
    GameObject LobbyParent;

	// Use this for initialization
	void Start () {
        lobbyManager = GameObject.FindGameObjectWithTag("LManager").GetComponent<CS_LobbyManager>();
        LobbyParent = GameObject.FindGameObjectWithTag("LobbyParent");
    }
	
    public void Setup(MatchInfoSnapshot a_match)
    {
        match = a_match;
        HostName.text = match.name;
    }

    public void Join()
    {
        if(lobbyManager == null)
        {
            lobbyManager = GameObject.FindGameObjectWithTag("LManager").GetComponent<CS_LobbyManager>();
        }
        var GOChild = LobbyParent.GetComponentsInChildren<Transform>(true);
        foreach(var item in GOChild)
        {
            item.gameObject.SetActive(true);
        }
        lobbyManager.matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, lobbyManager.OnMatchJoined);
    }
}
