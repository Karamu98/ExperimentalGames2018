using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using System;
using WiimoteApi;

public class CS_WiiMoteScene
{

    public CS_WiiMote wiimote;
    private bool bDoOnce;
    private int iData;
    private float fTimer;
    public bool bStartRumble;


    public CS_NunchuckData data;
    public float xRot, yRot;
    const float fXMid = 130;
    const float fYMid = 123;
    const float fRange = 200;
    const float c_fDeadZone = 0.1f;
    bool bDoOnce1;

    bool bOnce;

    public void Init()
    {
        data = new CS_NunchuckData(wiimote);
        bDoOnce = false;
        bDoOnce1 = false;
        bOnce = true;
        fTimer = 0;
        bStartRumble = false;
    }
    public void Rumble()
    {
        if(bOnce)
        {
            wiimote.bRumbleOn = true;
            bOnce = false;
        }
        fTimer += Time.deltaTime;
        if(fTimer >= 1.0f)
        {
            fTimer = 0;
            wiimote.bRumbleOn = false;
            bStartRumble = false;
            bOnce = true;
        }
    }
    public float GetX()
    {
        if(xRot > c_fDeadZone|| xRot < -c_fDeadZone)
        {
            return xRot;
        }
        else
        {
            return 0;
        }
    }

    public float GetY()
    {
        if (yRot > c_fDeadZone || yRot < -c_fDeadZone)
        {
            return yRot;
        }
        else
        {
            return 0;
        }
    }

    public void Update ()
    {
        if (!bDoOnce)
        {
            CS_WiiMoteManager.FindWiimotes();
            bDoOnce = true;
        }

        if (!CS_WiiMoteManager.HasWiimote())
        {
            return;
        }
        if (bStartRumble)
        {
            Rumble();
        }
        wiimote = CS_WiiMoteManager.Wiimotes[0];
        wiimote.SetupIRCamera(IRDataType.BASIC);
        float fStick1 = data.stick[1];
        float fStick2 = data.stick[0];  
        //Tests if there is a wiimote and is has an extension
        if (wiimote != null && wiimote.current_ext != ExtensionController.NONE && !bDoOnce1)
        {
            data = wiimote.Nunchuck;
            bDoOnce1 = true;
        }
            xRot = (fStick1 - fXMid) / (0.5f * fRange);
            yRot = (fStick2 - fYMid) / (0.5f * fRange);
        //order to make sure it doesn't fall behind the Wiimote's update frequency
        do
        {
            iData = wiimote.ReadWiimoteData();
        } while (iData > 0);

    }

    void OnGUI()
    {
        //Will show the current extension (Nunchuck if plugged in) and the Nunchuck values as well as if a mote has been recognised.
        GUI.Box(new Rect(720,10,320,Screen.height), "");
        GUILayout.BeginArea(new Rect(720, 10, 320, Screen.height));
        GUILayout.Label("Wiimote Found: " + CS_WiiMoteManager.HasWiimote());
        //only searches for a mote at the start - can cause crashes otherwise

        if (wiimote == null)
            return;
       //Tests if there is a wiimote and is has an extension
        if (wiimote != null && wiimote.current_ext != ExtensionController.NONE)
        {
            GUIStyle bold = new GUIStyle(GUI.skin.button);

            bold.fontStyle = FontStyle.Bold;
            if (wiimote.current_ext == ExtensionController.NUNCHUCK) {
                GUILayout.Label("Nunchuck:", bold);                
                GUILayout.Label("Stick: " + xRot + ", " + yRot);
                GUILayout.Label("C: " + data.c);
                GUILayout.Label("Z: " + data.z);
            } 
        }
        GUILayout.EndArea();
    }
}
