using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityStandardAssets.Characters.FirstPerson;

public class CS_HealthBox : MonoBehaviour {

    private bool bIsTrig;
    private bool bOnce;
    private int iRandLength;
    private int[] iRandDir;
    private bool bComplete;
    private Image[] PlayerArrows; //aCTUALY GAMEOBJECT images within player hub
    [SerializeField]
    private Sprite[] Arrows; //The texture for images not correct
    [SerializeField]
    private Sprite[] ArrowCorrect; //Textures for images correct
    private int[] iCheckButton;
    private bool bWrongButton;
    private int iCurrentArrow;
    private const int iMaxHealth = 100;
    private int iCurrentHealth;
    CS_FirstPersonController FPController;
    private float fTimer;
    private bool bStartTimer;
    private const float fTotalRestartTime = 50;
    enum Directions
    {
        LEFT,   //Joystick button 0
        DOWN,   //Joystick button 1
        UP  ,   //Joystick button 2
        RIGHT   //Joystick button 3
    };

    enum Joystick
    {
        JOYLEFT, //JoyStickNum
        JOYDOWN,
        JOYUP,
        JOYRIGHT
    }


    // Use this for initialization
    void Start () {
        bIsTrig = false;
        bStartTimer = false;
        iCurrentArrow = 0;
        bComplete = false;
        bOnce = true;
        fTimer = 0;
        iRandLength = 0;
        iRandDir = new int[4];
        iCheckButton = new int[5];
        PlayerArrows = new Image[5]; // Isnt this initialised in the player?
        bWrongButton = false;
    }



	// Update is called once per frame
	void Update () {
        if(bStartTimer)
        {
            fTimer += Time.deltaTime;
            if(fTimer >= fTotalRestartTime)
            {
                fTimer = 0;
                bStartTimer = false;
                for(int i = 0; i < iRandLength; i ++)
                {
                    PlayerArrows[i].sprite = Arrows[iRandDir[i]];
                    iCheckButton[i] = 0;
                }
                iCurrentArrow = 0;
                gameObject.GetComponent<MeshRenderer>().enabled = true;
            }
        }
        if (bComplete)
        {
            for (int i = 0; i < iRandLength; i++)
            {
                PlayerArrows[i].enabled = false;
            }
            FPController.RpcAddHealth(50);
            bComplete = false;
            bIsTrig = false;
            bStartTimer = true;
        }
        if (bIsTrig && !bComplete)
        {
            for (int i = 0; i < iRandLength; i++)
            {
                PlayerArrows[i].enabled = true;
            }
            if (bOnce)
            {
                iRandLength = UnityEngine.Random.Range(3, 5);
                for (int i = 0; i < iRandLength; i++)
                {
                    int iRand = UnityEngine.Random.Range((int)Directions.LEFT, (int)Directions.RIGHT);
                    iRandDir[i] = iRand; //Saves the directions chosen into an array
                    PlayerArrows[i].sprite = Arrows[iRand];
                    PlayerArrows[i].enabled = true;
                }//Changes wont appear past here
                bOnce = false;
            }
            CheckInput(iRandLength);
        }

    }

    void CheckInput(int a_Length)
    {
        switch (a_Length)
        {
            case 3:
                CheckCase3();
                break;
            case 4:
                CheckCase4();
                break;
            case 5:
                CheckCase5();
                break;
        }
    }

    void CheckCase3()
    {
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
                if (kcode.ToString() != ("JoystickButton" + iRandDir[iCurrentArrow]) && kcode.ToString() != ("Joystick1Button" + iRandDir[iCurrentArrow]))
                {
                    Debug.Log(kcode.ToString());
                    bWrongButton = true;
                }
        }
        if (Input.GetKeyDown("joystick button " + iRandDir[iCurrentArrow]) && iCheckButton[1] == 1)
        {
            PlayerArrows[2].sprite = ArrowCorrect[iRandDir[2]];
            bComplete = true;
            iCurrentArrow = 3;
        }
        else if (Input.GetKeyDown("joystick button " + iRandDir[iCurrentArrow]) && iCheckButton[0] == 1)
        {
            PlayerArrows[1].sprite = ArrowCorrect[iRandDir[1]];
            iCheckButton[1] = 1;
            iCurrentArrow = 2;
        }
        else if (Input.GetKeyDown("joystick button " + iRandDir[iCurrentArrow]))
        {
            iCheckButton[0] = 1;
            iCurrentArrow = 1;
            PlayerArrows[0].sprite = ArrowCorrect[iRandDir[0]];
            bWrongButton = false;
        }
        if (bWrongButton)
        {
            PlayerArrows[0].sprite = Arrows[iRandDir[0]];
            PlayerArrows[1].sprite = Arrows[iRandDir[1]];
            PlayerArrows[2].sprite = Arrows[iRandDir[2]];
            iCheckButton[0] = 0;
            iCheckButton[1] = 0;
            iCurrentArrow = 0;
        }

    }

    void CheckCase4()
    {
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
                if (kcode.ToString() != ("JoystickButton" + iRandDir[iCurrentArrow]) && kcode.ToString() != ("Joystick1Button" + iRandDir[iCurrentArrow]))
                {
                    Debug.Log(kcode.ToString());
                    bWrongButton = true;
                }
        }
        if (Input.GetKeyDown("joystick button " + iRandDir[iCurrentArrow]) && iCheckButton[2] == 1)
        {
            PlayerArrows[3].sprite = ArrowCorrect[iRandDir[3]];
            bComplete = true;
            iCurrentArrow = 4;
        }
        else if (Input.GetKeyDown("joystick button " + iRandDir[iCurrentArrow]) && iCheckButton[1] == 1)
        {
            PlayerArrows[2].sprite = ArrowCorrect[iRandDir[2]];
            iCheckButton[2] = 1;
            iCurrentArrow = 3;
        }
        else if (Input.GetKeyDown("joystick button " + iRandDir[iCurrentArrow]) && iCheckButton[0] == 1)
        {
            iCheckButton[1] = 1;
            iCurrentArrow = 2;
            PlayerArrows[1].sprite = ArrowCorrect[iRandDir[1]];
        }
        else if (Input.GetKeyDown("joystick button " + iRandDir[iCurrentArrow]))
        {
            iCheckButton[0] = 1;
            iCurrentArrow = 1;
            PlayerArrows[0].sprite = ArrowCorrect[iRandDir[0]];
            bWrongButton = false;
        }
        if (bWrongButton)
        {
            PlayerArrows[0].sprite = Arrows[iRandDir[0]];
            PlayerArrows[1].sprite = Arrows[iRandDir[1]];
            PlayerArrows[2].sprite = Arrows[iRandDir[2]];
            PlayerArrows[3].sprite = Arrows[iRandDir[3]];
            iCheckButton[0] = 0;
            iCheckButton[1] = 0;
            iCheckButton[2] = 0;
            iCurrentArrow = 0;
        }
    }

    void CheckCase5()
    {
        Debug.Log(bWrongButton);
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
                if (kcode.ToString() != ("JoystickButton" + iRandDir[iCurrentArrow]) && kcode.ToString() != ("Joystick1Button" + iRandDir[iCurrentArrow]))
                {
                    Debug.Log(kcode.ToString());
                    bWrongButton = true;
                }
        }
        if (Input.GetKeyDown("joystick button " + iRandDir[iCurrentArrow]) && iCheckButton[3] == 1)
        {
            PlayerArrows[4].sprite = ArrowCorrect[iRandDir[4]];
            bComplete = true;
            iCurrentArrow = 5;
        }
        else if (Input.GetKeyDown("joystick button " + iRandDir[iCurrentArrow]) && iCheckButton[2] == 1)
        {
            PlayerArrows[3].sprite = ArrowCorrect[iRandDir[3]];
            iCheckButton[3] = 1;
            iCurrentArrow = 4;
        }
        else if (Input.GetKeyDown("joystick button " + iRandDir[iCurrentArrow]) && iCheckButton[1] == 1)
        {
            iCheckButton[2] = 1;
            iCurrentArrow = 3;
            PlayerArrows[2].sprite = ArrowCorrect[iRandDir[2]];
        }
        else if (Input.GetKeyDown("joystick button " + iRandDir[iCurrentArrow]) && iCheckButton[0] == 1)
        {
            iCheckButton[1] = 1;
            iCurrentArrow = 2;
            PlayerArrows[1].sprite = ArrowCorrect[iRandDir[1]];
            bWrongButton = false;
        }
        else if (Input.GetKeyDown("joystick button " + iRandDir[iCurrentArrow]))
        {
            iCheckButton[0] = 1;
            iCurrentArrow = 1;
            PlayerArrows[0].sprite = ArrowCorrect[iRandDir[0]];
            bWrongButton = false;
        }
        if (bWrongButton)
        {
            PlayerArrows[0].sprite = Arrows[iRandDir[0]];
            PlayerArrows[1].sprite = Arrows[iRandDir[1]];
            PlayerArrows[2].sprite = Arrows[iRandDir[2]];
            PlayerArrows[3].sprite = Arrows[iRandDir[3]];
            PlayerArrows[4].sprite = Arrows[iRandDir[4]];
            iCheckButton[0] = 0;
            iCheckButton[1] = 0;
            iCheckButton[2] = 0;
            iCheckButton[3] = 0;
            iCurrentArrow = 0;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if(!bStartTimer)
            {
                PlayerArrows = other.GetComponent<CS_FirstPersonController>().m_arrowImages;
                FPController = other.GetComponent<CS_FirstPersonController>();
                bIsTrig = true;
                gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }
}
