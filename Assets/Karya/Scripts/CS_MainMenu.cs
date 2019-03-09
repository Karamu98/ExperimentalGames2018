using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CS_MainMenu : MonoBehaviour {
    
    [SerializeField]
    private GameObject OptionsMenu;
    [SerializeField]
    private GameObject MainMenu;
    [SerializeField] private InputField PlayerNameInput;
    [SerializeField]
    private Slider MasterSlider;
    [SerializeField]
    private Slider SFXSlider;
    [SerializeField]
    private Slider MusicSlider;
    [SerializeField]
    [FMODUnity.EventRef]
    private string sButtonClickSound;
    [SerializeField]
    [FMODUnity.EventRef]
    private string sTestSFXSound;
    
    public GameObject LobbyManager;


    private void Awake()
    {
        LobbyManager = GameObject.FindWithTag("Lobby");
        MainMenu.SetActive(true);
        OptionsMenu.SetActive(false);
        LobbyManager.SetActive(false);

        PlayerNameInput.text = CS_SessionManager.player.UserName;
        MasterSlider.value = CS_SessionManager.player.MasterVolume;
        MusicSlider.value = CS_SessionManager.player.MusicVolume;
        SFXSlider.value = CS_SessionManager.player.SFXVolume;

        MasterSlider.onValueChanged.AddListener(delegate { OnMasterVolumeChange(); });
        MusicSlider.onValueChanged.AddListener(delegate { OnMasterVolumeChange(); });
        SFXSlider.onValueChanged.AddListener(delegate { OnMasterVolumeChange(); });

        sButtonClickSound = sButtonClickSound.Remove(0, 7);
        sTestSFXSound = sTestSFXSound.Remove(0, 7);
    }

    public void OnPlayClick()
    {
        MainMenu.SetActive(false);
        LobbyManager.SetActive(true);
        CS_SoundTest.PlaySoundOnObject(gameObject, sButtonClickSound);

    }

    public void OnOptionsClick()
    {
        OptionsMenu.SetActive(true);
        MainMenu.SetActive(false);
        CS_SoundTest.PlaySoundOnObject(gameObject, sButtonClickSound);

    }

    public void OnExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnBackSerButton()
    {
        MainMenu.SetActive(true);
        //MainMenu.GetComponent<CS_MainMenu>().LobbyManager = GameObject.FindWithTag("Lobby");
        LobbyManager.SetActive(false);
        CS_SoundTest.PlaySoundOnObject(gameObject, sButtonClickSound);

    }


    public void OnBackClick()
    {

        CS_SessionManager.player.UserName = PlayerNameInput.text;
        CS_SessionManager.player.MasterVolume = CS_SoundTest.fMasterVolume;
        CS_SessionManager.player.MusicVolume = CS_SoundTest.fMusicVolume;
        CS_SessionManager.player.SFXVolume = CS_SoundTest.fSFXVolume;
        OptionsMenu.SetActive(false);
        MainMenu.SetActive(true);
        CS_SoundTest.PlaySoundOnObject(gameObject, sButtonClickSound);

    }

    public void OnMasterVolumeChange()
    {
        CS_SoundTest.SetMasterVolume(MasterSlider.value);
        CS_SoundTest.PlaySoundOnObject(gameObject, sTestSFXSound);
    }
    public void OnMusicVolumeChange()
    {
        CS_SoundTest.SetMusicVolume(MusicSlider.value);
        CS_SoundTest.PlaySoundOnObject(gameObject, sTestSFXSound);
    }
    public void OnSFXVolumeChange()
    {
        CS_SoundTest.SetSFXVolume(SFXSlider.value);
        CS_SoundTest.PlaySoundOnObject(gameObject, sTestSFXSound);
    }



}
