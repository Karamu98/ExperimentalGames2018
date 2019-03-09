using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using System.Collections.Generic;       //Allows us to use Lists. 

public class CS_SessionManager : MonoBehaviour
{
    public static CS_SessionManager instance = null; //Static instance of GameManager which allows it to be accessed by any other script.
    public static PlayerData player;


    private void OnApplicationQuit()
    {
        SaveSession();
    }

    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
        {
            //if not, set instance to this
            instance = this;
            player = new PlayerData(); // Create an isntance of PlayerData in this singleton
            LoadSession();
        }
        else if (instance != this)
        {
            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);
        }

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }

    public void SaveSession()
    {
        if (!File.Exists(Application.dataPath + "/saves/SaveData.dat"))
        {
            if (!Directory.Exists(Application.dataPath + "/saves"))
            {
                Directory.CreateDirectory(Application.dataPath + "/saves");
            }

            FileStream file = File.Create(Application.dataPath + "/saves/SaveData.dat");
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(file, player);
            file.Close();

            Debug.Log("Saved: " + player.UserName);
        }
        else
        {
            FileStream file = File.Open(Application.dataPath + "/saves/SaveData.dat", FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(file, player);
            file.Close();

            Debug.Log("Overwrite: " + player.UserName);
        }
    }

    public void LoadSession()
    {
        if (File.Exists(Application.dataPath + "/saves/SaveData.dat"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.dataPath + "/saves/SaveData.dat", FileMode.Open);
            player = (PlayerData)formatter.Deserialize(file);

            file.Close();

            Debug.Log("Loaded Profile: " + player.UserName);
        }
    }
}

[Serializable]
public class PlayerData
{
    public PlayerData()
    {
        UserName = "Jamie";
        MasterVolume = 1;
        MusicVolume = 1;
        SFXVolume = 1;
    }

    private string m_ssUserName;
    public string UserName
    {
        get { return m_ssUserName; }
        set { m_ssUserName = value; }
    }

    private float m_iMasterVolume;
    public float MasterVolume
    {
        get { return m_iMasterVolume; }
        set { m_iMasterVolume = value; }
    }

    private float m_iMusicVolume;
    public float MusicVolume
    {
        get { return m_iMusicVolume; }
        set { m_iMusicVolume = value; }
    }

    private float m_iSFXVolume;
    public float SFXVolume
    {
        get { return m_iSFXVolume; }
        set { m_iSFXVolume = value; }
    }


}
