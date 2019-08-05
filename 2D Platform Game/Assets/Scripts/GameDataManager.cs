using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class GameDataManager : MonoBehaviour {

    public GameData gameData;

    public static GameDataManager instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        gameData = new GameData();
        gameData.Load();
    }
}

[System.Serializable]
public class GameData
{
    public LevelData[] levelDatas;

    public OptionData optionData;

    public GameData()
    {
        optionData = new OptionData();
    }

    public void Save()
    {
        optionData.Save();
    }

    public void Load()
    {
        optionData.Load();
    }
}

[System.Serializable]
public class OptionData
{
    public AudioSetting audioSetting;

    public OptionData()
    {
        audioSetting = new AudioSetting();
    }

    public void Save()
    {
        audioSetting.Save();
    }

    public void Load()
    {
        audioSetting.Load();
    }
}

[System.Serializable]
public class AudioSetting
{
    public float masterVolumePercent = .5f;
    public float musicVolumePercent = .5f;
    public float sfxVolumePercent = .5f;

    public void Save()
    {
        string saveString = JsonUtility.ToJson(GameDataManager.instance.gameData.optionData.audioSetting);
        string filePath = Application.persistentDataPath + "/AudioSetting.dat";
        byte[] serializedData = Encoding.UTF8.GetBytes(saveString);
        File.WriteAllBytes(filePath, serializedData);
    }

    public bool Load()
    {
        string filePath = Application.persistentDataPath + "/AudioSetting.dat";
        byte[] serializedData;
        string jsonData = "";

        try
        {
            serializedData = File.ReadAllBytes(filePath);
            jsonData = Encoding.UTF8.GetString(serializedData);
            GameDataManager.instance.gameData.optionData.audioSetting = JsonUtility.FromJson<AudioSetting>(jsonData);
        }
        catch (System.IO.FileNotFoundException)
        {
            Debug.Log("can't find file : " + Application.persistentDataPath + "/AudioSetting.dat");
            return false;
        }

        return true;
    }
}

[System.Serializable]
public class LevelData
{
    public bool isLock = true;
    public int starCode = 0;

    public LevelData(bool isLock,int starCode)
    {
        this.isLock = isLock;
        this.starCode = starCode;
    }
}