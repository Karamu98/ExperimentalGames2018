using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class CS_CameraController : NetworkBehaviour
{

	// Use this for initialization
	void Start ()
    {
		if(isLocalPlayer)
        {
            return;
        }

        GetComponentInChildren<Camera>().enabled = false;
        GetComponentInChildren<Canvas>().enabled = !true; // Dan did this
        GetComponentInChildren<FMODUnity.StudioListener>().enabled = false;
        GetComponentInChildren<AudioListener>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {

        
	}
}
