using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;




public class CS_LevelManager : NetworkBehaviour
{
    // Train
    [SerializeField] public GameObject[] TrainTracks;
    [SerializeField] public float[] Timings;
    private float[] counters;


    private POWERTYPES ePowerTypes;
    [SerializeField] private float fPowerCounter;
    [SerializeField] private float fPowerTimer;
    [SerializeField] private bool bSomeoneFinished;

    private void Start()
    {
        //Debug.Log("Size of TrainTracks: " + TrainTracks.Length);
        //Debug.Log("Size of Timings: " + Timings.Length);

        counters = new float[Timings.Length];
        
        for (int i = 0; i < Timings.Length; ++i)
        {
            counters[i] = Timings[i];
        }

        fPowerCounter = fPowerTimer;

    }



    // Update is called once per frame
    void Update()
    {

        PowerUpCheck();
        for (int i = 0; i < Timings.Length; ++i)
        {
            counters[i] -= Time.deltaTime;
        }


        for (int i = 0; i < Timings.Length; i++)
        {
            if (counters[i] <= 0)
            {
                if (isClient)
                {
                    TrainTracks[i].GetComponent<CS_TrainTest>().CmdStartTrain();
                    //Debug.Log("Client requesting train:  " + i);
                }
                else if (isServer)
                {
                    //Debug.Log("Server Starting train: " + i);
                    TrainTracks[i].GetComponent<CS_TrainTest>().RpcStartTrain();
                }

                counters[i] = Timings[i];
            }
        }
    }



    private void PowerUpCheck()
    {
        fPowerCounter -= Time.deltaTime;
        if (fPowerCounter <= 0)
        {
            Debug.Log("Checking...");
            fPowerCounter = fPowerTimer;
            if (isServer)
            {
                Debug.Log("Server notifying all clients");
                List<UnityStandardAssets.Characters.FirstPerson.CS_FirstPersonController> players = CS_GameManager.GetAllPlayers();

                foreach (UnityStandardAssets.Characters.FirstPerson.CS_FirstPersonController client in players)
                {
                    client.GetComponent<CS_PowerUpCombo>().CmdStartPowerupSequence();
                }

            }
        }

        List<UnityStandardAssets.Characters.FirstPerson.CS_FirstPersonController> playerlist = CS_GameManager.GetAllPlayers();

        bool bSomeonehasdoneit = false;
        foreach (UnityStandardAssets.Characters.FirstPerson.CS_FirstPersonController client in playerlist)
        {
            if(client.GetComponent<CS_PowerUpCombo>().m_bFinished == true)
            {
                bSomeonehasdoneit = true;
            }
        }
        foreach (UnityStandardAssets.Characters.FirstPerson.CS_FirstPersonController client in playerlist)
        {
            if (bSomeonehasdoneit == true)
            {
                client.GetComponent<CS_PowerUpCombo>().CmdPowerupFailed();

            }
        }




    }

    private void PowerupFinished()
    {
        bSomeoneFinished = true;
    }

}
