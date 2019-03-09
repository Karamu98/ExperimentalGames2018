using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;

enum POWERTYPES
{
    PAUSE,
    HEALTHUP,
    DAMAGEUP,
    SPEEDUP,
    INVINCIBILITY
}


public class CS_PowerUpCombo : NetworkBehaviour {


    [SyncVar] public bool m_bStartPowerupSequence;
    [SyncVar] public bool m_bFinished;
    [SerializeField] Text PowerUpText;
    private bool bIsTrig;
    private bool bOnce;
    public int iRandLength;
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
    private float fTimer;
    private bool bStartTimer;
    private const float fTotalRestartTime = 5;

    POWERTYPES eRandomPowerup;
    private float fPowerOldHealth;
    private float fPowerOldMaxHealth;
    private float fPowerOldSpeed;
    private float fPowerOldDamage;

    private bool bHasPowerup;
    [SerializeField] private float fPowerTime;
    private float fPowerCount;

    private int iJoyNum;

    enum Directions
    {
        LEFT,   //Joystick button 0
        DOWN,   //Joystick button 1
        UP,   //Joystick button 2
        RIGHT   //Joystick button 3
    };

    enum Joystick
    {
        JOYLEFT, //JoyStickNum
        JOYDOWN,
        JOYUP,
        JOYRIGHT
    }

    public override void OnStartClient()
    {
        if (isClient)
        {
            Debug.Log("Client initialise");
            bIsTrig = false;
            bStartTimer = false;
            iCurrentArrow = 0;
            bComplete = false;
            bOnce = true;
            fTimer = 0;
            iRandLength = 0;
            iRandDir = new int[4];
            iCheckButton = new int[5];
            bWrongButton = false;
            PowerUpText.enabled = false;
            PlayerArrows = new Image[5]; // Isnt this initialised in the player?
        }

        if (isServer)
        {
            Debug.Log("Server initialise");
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }


        if (bHasPowerup)
        {
            fPowerCount -= Time.deltaTime;
            if (fPowerCount <= 0)
            {
                TakePlayerPowerup(eRandomPowerup);
                bHasPowerup = false;
                fPowerCount = fPowerTime;
            }
        }




        if (!m_bStartPowerupSequence)
        {

        }
        if (bStartTimer)
        {
            fTimer += Time.deltaTime;
            if (fTimer >= fTotalRestartTime)
            {
                fTimer = 0;
                bStartTimer = false;
                for (int i = 0; i < iRandLength - 1; ++i)
                {
                    PlayerArrows[i].sprite = Arrows[iRandDir[i]];
                    iCheckButton[i] = 0;
                }
                iCurrentArrow = 0;
                //gameObject.GetComponent<MeshRenderer>().enabled = true;
            }
        }
        if (bComplete)
        {
            for (int i = 0; i < iRandLength - 1; ++i)
            {
                PlayerArrows[i].enabled = false;
                PlayerArrows[i].sprite = Arrows[iRandDir[i]];
                iCheckButton[i] = 0;
            }

            iCurrentArrow = 0;
            bIsTrig = false;
            //bStartTimer = true;
            //m_bStartPowerupSequence = false;
            m_bFinished = true;
            CmdFinishedPowerupSequence();


        }
        if (bIsTrig && !bComplete)
        {
            for (int i = 0; i < iRandLength - 1; ++i)
            {
                PlayerArrows[i].enabled = true;
            }
            if (bOnce)
            {
                iRandLength = UnityEngine.Random.Range(3, 5);
                for (int i = 0; i < iRandLength - 1; ++i)
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
                for (int i = 0; i < 5; ++i)
                {
                    if (kcode.ToString() != ("JoystickButton" + iRandDir[iCurrentArrow]) && kcode.ToString() != ("Joystick1Button" + iRandDir[iCurrentArrow])
                        && kcode.ToString() != ("Joystick2Button" + iRandDir[iCurrentArrow]) && kcode.ToString() != ("Joystick3Button" + iRandDir[iCurrentArrow])
                        && kcode.ToString() != ("Joystick4Button" + iRandDir[iCurrentArrow]))
                    {
                        bWrongButton = true;
                    }
                }
        }
        if (iCheckButton[1] == 1)
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
                for(int i = 0; i < 5; ++i)
                {
                    if (kcode.ToString() != ("JoystickButton" + iRandDir[iCurrentArrow]) && kcode.ToString() != ("Joystick1Button" + iRandDir[iCurrentArrow])
                        && kcode.ToString() != ("Joystick2Button" + iRandDir[iCurrentArrow]) && kcode.ToString() != ("Joystick3Button" + iRandDir[iCurrentArrow])
                        && kcode.ToString() != ("Joystick4Button" + iRandDir[iCurrentArrow]))
                    { 
                        bWrongButton = true;
                    }
                }
        }
        if (iCheckButton[2] == 1)
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
                for (int i = 0; i < 5; ++i)
                {
                    if (kcode.ToString() != ("JoystickButton" + iRandDir[iCurrentArrow]) && kcode.ToString() != ("Joystick1Button" + iRandDir[iCurrentArrow])
                        && kcode.ToString() != ("Joystick2Button" + iRandDir[iCurrentArrow]) && kcode.ToString() != ("Joystick3Button" + iRandDir[iCurrentArrow])
                        && kcode.ToString() != ("Joystick4Button" + iRandDir[iCurrentArrow]))
                    {
                        bWrongButton = true;
                    }
                }
        }
        if (iCheckButton[3] == 1)
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


    [Command]
    public void CmdStartPowerupSequence()
    {
        if (m_bStartPowerupSequence)
        {
            RpcPowerupFailed();
        }
        else
        {
            RpcStartPowerupSequence();//yes
        }
    }

    [Command]
    public void CmdPowerupFailed()
    {
        RpcPowerupFailed();
    }



    [ClientRpc]
    public void RpcStartPowerupSequence()
    {
        if (!m_bStartPowerupSequence)
        {
            PlayerArrows = GetComponent<CS_FirstPersonController>().m_arrowImages;
            bIsTrig = true;
            m_bStartPowerupSequence = true;
            m_bFinished = false;
            bOnce = true;
            bComplete = false;
            CS_SoundTest.PlaySoundOnObject(gameObject, "SFX/Powerups/PowerDance");
            //gameObject.GetComponent<MeshRenderer>().enabled = false;
        }


    }

    [Command]
    public void CmdFinishedPowerupSequence()
    {
        m_bFinished = true;
    }

    [ClientRpc]
    public void RpcPowerupFailed()
    {
        if (m_bStartPowerupSequence && bComplete)
        {
            eRandomPowerup = (POWERTYPES)UnityEngine.Random.Range(0, 4);
            GivePlayerPowerup(eRandomPowerup);
            fPowerCount = fPowerTime;
            bHasPowerup = true;
            Debug.Log("WON IT BOI");
            CS_SoundTest.PlaySoundOnObject(gameObject, "SFX/Powerups/PowerWin");


        }
        else if (m_bStartPowerupSequence && !bComplete)
        {
            Debug.Log("LOST IT BOI");
            CS_SoundTest.PlaySoundOnObject(gameObject, "SFX/Powerups/PowerLose");


        }

        for (int i = 0; i < iRandLength - 1; ++i)
        {
            PlayerArrows[i].enabled = false;
        }

        bComplete = false;
        bIsTrig = false;
        m_bStartPowerupSequence = false;
    }

    private void GivePlayerPowerup(POWERTYPES a_ePowerType)
    {
        CS_FirstPersonController playerRef = GetComponent<CS_FirstPersonController>();

        switch (a_ePowerType)
        {
            case POWERTYPES.DAMAGEUP:
                {
                    Debug.Log("DAMAGEUP");
                    
                    fPowerOldDamage = playerRef.GetDamage();
                    playerRef.SetDamage(50);
                    playerRef.SetText("INCREASE DAMAGE");
                    break;
                }
            case POWERTYPES.HEALTHUP:
                {
                    Debug.Log("HEALTHUP");
                    playerRef.SetText("HEALTH");

                    fPowerOldHealth = playerRef.GetHealth();
                    playerRef.SetHealth(180);
                    break;
                }
            case POWERTYPES.INVINCIBILITY:
                {
                    Debug.Log("INVINCIBILITY");
                    playerRef.SetText("INVINCIBILITY");

                    fPowerOldHealth = playerRef.GetHealth();
                    fPowerOldMaxHealth = playerRef.GetMaxHealth();

                    playerRef.SetMaxHealth(1000);
                    playerRef.SetHealth(999);


                    break;
                }

            case POWERTYPES.SPEEDUP:
                {
                    Debug.Log("SPEEDUP");
                    playerRef.SetText("SPEED UP");

                    fPowerOldSpeed = playerRef.GetWalkSpeed();
                    playerRef.SetWalkSpeed(15);

                    break;
                }
        }
    }
    private void TakePlayerPowerup(POWERTYPES a_ePowerType)
    {
        CS_FirstPersonController playerRef = GetComponent<CS_FirstPersonController>();
        switch (a_ePowerType)
        {
            case POWERTYPES.DAMAGEUP:
                {
                    Debug.Log("DAMAGEUP");

                    playerRef.SetDamage((int)fPowerOldDamage);//hey bby
                    break;
                }
            case POWERTYPES.HEALTHUP:
                {
                    Debug.Log("HEALTHUP");


                    playerRef.SetHealth((int)fPowerOldHealth);
                    break;
                }
            case POWERTYPES.INVINCIBILITY:
                {
                    Debug.Log("INVINCIBILITY");
                    playerRef.SetHealth((int)fPowerOldHealth);
                    playerRef.SetMaxHealth((int)fPowerOldMaxHealth);

                   


                    break;
                }
            case POWERTYPES.SPEEDUP:
                {
                    Debug.Log("SPEEDUP");

                    
                    GetComponent<CS_FirstPersonController>().SetWalkSpeed((int)fPowerOldSpeed);

                    break;
                }
        }


    }

    private void SetText(string a_sPower)
    {
        PowerUpText.enabled = true;
        PowerUpText.text = a_sPower;
        PowerUpText.fontSize = 14;
    }

    private void BlowText()
    {
        if(PowerUpText.enabled && PowerUpText.fontSize < 130)
        {
            PowerUpText.fontSize += 1;
        }
    }

}


