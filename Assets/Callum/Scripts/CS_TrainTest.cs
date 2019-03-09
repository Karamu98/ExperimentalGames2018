using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class CS_TrainTest : NetworkBehaviour
{
    // Set Start and end pos for map in inspector
    [SerializeField] GameObject m_StartPosition;
    [SerializeField] GameObject m_EndPosition;

    [SerializeField] GameObject m_TrainPrefab;
    [SerializeField] float m_fTrainSpeed;

    [SerializeField] float m_fTrainAcceptanceRadius;

    [SyncVar] public bool m_bTrainActive;

    // Update is called once per frame
    void Update ()
    {
        //Debug.Log("Updating");
		if(m_bTrainActive)
        {
            m_TrainPrefab.transform.position = Vector3.MoveTowards(m_TrainPrefab.transform.position, m_EndPosition.transform.position, (m_fTrainSpeed * Time.deltaTime));
            //Debug.Log("Moving");


            if(Vector3.Distance(m_TrainPrefab.transform.position, m_EndPosition.transform.position) <= m_fTrainAcceptanceRadius)
            {
                Renderer[] Models = m_TrainPrefab.GetComponentsInChildren<Renderer>();

                for(int i = 0; i < Models.Length; i++)
                {
                    Models[i].enabled = false; // Hides the train
                }
                //Debug.Log("Train is within deleting boundary. Hiding...");

                // Maybe disable a collision compont so a player can't get "hit" by it
                m_bTrainActive = false;
            }
        }
    }

    [Command]
    public void CmdStartTrain() // Call this from a level manager of some kind
    {
        //Debug.Log("Running command");
        if(m_bTrainActive)
        {
            //Debug.Log("Train is already active");
        }
        else
        {
            // Tell other clients to start train
            RpcStartTrain();
        }
    }

    [ClientRpc]
    public void RpcStartTrain()
    {
        //Debug.Log("Function called from server");
        m_bTrainActive = true;
        m_TrainPrefab.transform.position = m_StartPosition.transform.position;
        CS_SoundTest.PlaySoundOnObject(m_TrainPrefab, "Map/Train");
        Renderer[] Models = m_TrainPrefab.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < Models.Length; i++)
        {
            Models[i].enabled = true; // Unhides the train
        }
    }
}
