using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_TrainListener : MonoBehaviour {

    public bool bTrainSpawn = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(bTrainSpawn)
        {
            bTrainSpawn = false;
            CS_TrainSpawn.bSpawnTrain = true;
        }
	}
}
