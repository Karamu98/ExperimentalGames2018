using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.Match;

public class CS_JoinRoom : MonoBehaviour {

    CS_LobbyManager LobbyManager;
    public GameObject PrefabForHost;
    public GameObject ParentForHost;

	// Use this for initialization
	void Start () {
        LobbyManager = GameObject.FindGameObjectWithTag("LManager").GetComponent<CS_LobbyManager>();
	}

    public void RefreshRoom()
    {
        if(LobbyManager == null)
        {
            LobbyManager = GameObject.FindGameObjectWithTag("LManager").GetComponent<CS_LobbyManager>();
            //Fallback
        }
        if(LobbyManager.matchMaker == null)
        {
            LobbyManager.StartMatchMaker();
        }

        LobbyManager.matchMaker.ListMatches(0, 2, "", true, 0, 0, OnMatchList);
    }

    private void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> MatchList)
    {
        if(!success)
        {
            //throw new NotImplementedException();
            Debug.Log("Please refresh");
        }

        foreach(MatchInfoSnapshot match in MatchList)
        {
            GameObject ListGO = Instantiate(PrefabForHost);
            ListGO.transform.SetParent(ParentForHost.transform);
            CS_HostSetup hostSetup = ListGO.GetComponent<CS_HostSetup>();
            hostSetup.Setup(match);
        }
    }
}
