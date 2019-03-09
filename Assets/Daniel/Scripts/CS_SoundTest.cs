using System;
using System.Runtime.InteropServices;
using System.Collections;
using UnityEngine;




public class CS_SoundTest : MonoBehaviour {


    [FMODUnity.EventRef]
    public string MenuMusicSound;

    private FMOD.Studio.EventInstance MusicInstance;

    public static float fMasterVolume = 1.0f;
    public static float fMusicVolume = 1.0f;
    public static float fSFXVolume = 1.0f;

    public bool bPlayMenuMusic = true;

    // Use this for initialization
    void Start () {
        if(bPlayMenuMusic)
        {
            MusicInstance = FMODUnity.RuntimeManager.CreateInstance(MenuMusicSound);
            MusicInstance.start();
            //MusicInstance.set
        }
    }

    // Update is called once per frame
    void Update () {
        ChangeMasterVolume(fMasterVolume);
        ChangeMusicVolume(fMusicVolume);
        ChangeSFXVolume(fSFXVolume);
    }

    void OnDestroy()
    {
        MusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public static void ChangeMasterVolume(float a_fVolume)
    {
        FMODUnity.RuntimeManager.GetVCA("vca:/Master").setVolume(a_fVolume);
    }
    public static void ChangeMusicVolume(float a_fVolume)
    {
        FMODUnity.RuntimeManager.GetVCA("vca:/Music").setVolume(a_fVolume);
    }
    public static void ChangeSFXVolume(float a_fVolume)
    {
        FMODUnity.RuntimeManager.GetVCA("vca:/SFX").setVolume(a_fVolume);
    }

    public static void SetMasterVolume(float a_fVolume)
    {
        fMasterVolume = a_fVolume;
    }
    public static void SetMusicVolume(float a_fVolume)
    {
        fMusicVolume = a_fVolume;
    }
    public static void SetSFXVolume(float a_fVolume)
    {
        fSFXVolume = a_fVolume;
    }

    // @brief	Function to play a sound and attach it to a game object so it will move with it.
    // @brief	Looped Sounds will play forever with no way to stop it.
    // @param	Vector3 a_v3Position = XYZ Vector3 to play the sound at.
    // @param	string a_sSoundEventName = Sound to Play
    // @example Play3DSoundAtPos(new Vector3(0.0f,0.0f,0.0f), "PlayerShoot");
    public static void PlaySoundOnObject(GameObject a_goGameObject, string a_sSoundEventName)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(("event:/" + a_sSoundEventName), a_goGameObject);

    }
    // @brief	Function to play a sound and attach it to a transform so it will move with it.
    // @brief	Looped Sounds will play forever with no way to stop it.
    // @param	Vector3 a_v3Position = XYZ Vector3 to play the sound at.
    // @param	string a_sSoundEventName = Sound to Play
    // @example Play3DSoundAtPos(new Vector3(0.0f,0.0f,0.0f), "PlayerShoot");
    public static void PlaySoundOnObject(Transform a_Position, string a_sSoundEventName)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(("event:/" + a_sSoundEventName), a_Position.gameObject);
    }



    // @brief	Function to play a 3D sound event at a given Vector3 Position
    // @brief	Looped Sounds will play forever with no way to stop it.
    // @param	Vector3 a_v3Position = XYZ Vector3 to play the sound at.
    // @param	string a_sSoundEventName = Sound to Play
    // @example Play3DSoundAtPos(new Vector3(0.0f,0.0f,0.0f), "PlayerShoot");
    public static void PlaySoundAtPos(Vector3 a_v3Position, string a_sSoundEventName)
    {
        FMODUnity.RuntimeManager.PlayOneShot(("event:/" + a_sSoundEventName), a_v3Position);
    }

    // @brief	Function to play a 3D sound event at a given GameObject's Position
    // @brief	Looped Sounds will play forever with no way to stop it.
    // @param	GameObject a_goGameObject = GameObject to play the sound at.
    // @param	string a_sSoundEventName = Sound to Play
    // @example Play3DSoundAtPos(gameObject, "PlayerShoot");
    public static void PlaySoundAtPos(GameObject a_goGameObject, string a_sSoundEventName)
    {
        FMODUnity.RuntimeManager.PlayOneShot(("event:/" + a_sSoundEventName), a_goGameObject.transform.position);
    }
}
