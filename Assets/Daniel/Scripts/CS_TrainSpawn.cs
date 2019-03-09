using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class CS_TrainSpawn : NetworkBehaviour
{

    [SerializeField] private GameObject goTrainPrefab;
    [SerializeField] private Transform tTrainEnd;
    private Transform tTrainRef;
    [SerializeField] private float fSpeed;
    [SerializeField] private bool bTrainSpawned = true;
    [SerializeField] private float fDestroyDistance = 10.0f;

    public static bool bSpawnTrain;
    public bool bDebugSpawnTrain = true;

    [FMODUnity.EventRef]
    [SerializeField] private string sTrainSound;
	// Use this for initialization
	void Start ()
    {
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (bSpawnTrain && bTrainSpawned == false)
        {
            bDebugSpawnTrain = false;
            if(isServer)
            {
                Debug.Log("Server spawning train.");
                RpcSpawnTrain();
            }
            
        }

        if(bTrainSpawned)
        {
            Debug.Log("Updating train pos");

            tTrainRef.position = Vector3.MoveTowards(tTrainRef.position, tTrainEnd.position, fSpeed);
            if (Vector3.Distance(tTrainEnd.position, tTrainRef.position) <= fDestroyDistance)
            {
                Destroy(tTrainRef.gameObject);

                bTrainSpawned = false;
            }
        }
    }

    [ClientRpc]
    public void RpcSpawnTrain()
    {
        Debug.Log("RPC Spawning Train");
        tTrainRef = (Transform)Instantiate(goTrainPrefab, gameObject.transform).transform;
        bTrainSpawned = true;
        // CS_SoundTest.PlaySoundOnObject(tTrainRef.gameObject, sTrainSound.Remove(0,7));
    }

    [Command]
    public void CmdSpawnTrain()
    {
        Debug.Log("CMD Spawning Train");
        tTrainRef = (Transform)Instantiate(goTrainPrefab, gameObject.transform).transform;        
        bTrainSpawned = true;
       // CS_SoundTest.PlaySoundOnObject(tTrainRef.gameObject, sTrainSound.Remove(0,7));

        
    }
}
