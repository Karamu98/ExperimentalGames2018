using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class  CS_GameManager : NetworkBehaviour
{
    static int iMaxKills = 10;

    private static Dictionary<string, UnityStandardAssets.Characters.FirstPerson.CS_FirstPersonController> players = new Dictionary<string, UnityStandardAssets.Characters.FirstPerson.CS_FirstPersonController>();

    private const string ssPlayerPrefix = "Player ";

    public static void RegisterPlayer(string a_netID, UnityStandardAssets.Characters.FirstPerson.CS_FirstPersonController a_player)
    {
        string playerID = ssPlayerPrefix + a_netID;
        players.Add(playerID, a_player);
        a_player.transform.name = playerID;
    }

    public static void RemovePlayer(string a_playerID)
    {
        players.Remove(a_playerID);
    }

    public static UnityStandardAssets.Characters.FirstPerson.CS_FirstPersonController GetPlayer(string a_playerID)
    {
        return players[a_playerID];
    }

    public static List<UnityStandardAssets.Characters.FirstPerson.CS_FirstPersonController> GetAllPlayers()
    {
        List<UnityStandardAssets.Characters.FirstPerson.CS_FirstPersonController> playerObjects = new List<UnityStandardAssets.Characters.FirstPerson.CS_FirstPersonController>();

        foreach(string key in players.Keys)
        {
            playerObjects.Add(players[key]);
        }

        return playerObjects;
    }

    public static void CheckForEnd()
    {
        List<UnityStandardAssets.Characters.FirstPerson.CS_FirstPersonController> players = GetAllPlayers();

        for (int i = 0; i < players.Count; i++)
        {
            if(players[i].iKills >= iMaxKills)
            {
                // Do a command to end game
                players[i].EndGame(players[i].gameObject.name);
            }
        }
    }

    public static Dictionary<string, UnityStandardAssets.Characters.FirstPerson.CS_FirstPersonController> GetPlayerDictionary()
    {
        return players;
    }

    public string GetPlayerName(string a_playerID)
    {
        return players[a_playerID].GetName();
    }

    /*
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(200, 200, 200, 500));
        GUILayout.BeginVertical();

        foreach (string playerID in players.Keys)
        {
            GUILayout.Label(playerID);
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
    */
    
}
