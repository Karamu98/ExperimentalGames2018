using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CS_Debug : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
                Debug.Log("KeyCode down: " + kcode);
        }
    }
}
