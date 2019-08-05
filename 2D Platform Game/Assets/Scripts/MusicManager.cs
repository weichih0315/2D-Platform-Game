using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour
{
    public SceneMusic[] sceneMusics;

    public float changeMusicFadeDuration = 1f;

    private Dictionary<string, AudioClip> groupDictionary = new Dictionary<string, AudioClip>();

    private string sceneName;

    public static MusicManager instance;

    void Awake()
    {
        instance = this;
        foreach (SceneMusic sceneMusic in sceneMusics)
        {
            groupDictionary.Add(sceneMusic.sceneName, sceneMusic.musicTheme);
        }
    }

    void Start()
    {
        OnLevelWasLoaded();
    }

    void OnLevelWasLoaded()
    {
        string newSceneName = SceneManager.GetActiveScene().name;
        if (newSceneName != sceneName)
        {
            sceneName = newSceneName;
            Invoke("PlayMusic", .2f);
        }
    }
    
    public AudioClip GetClipFromName(string name)
    {
        if (groupDictionary.ContainsKey(name))
            return groupDictionary[name];
        return null;
    }

    void PlayMusic()
    {
        AudioClip clipToPlay = GetClipFromName(sceneName);

        if (clipToPlay != null)
        {
            AudioManager.instance.PlayMusic(clipToPlay, changeMusicFadeDuration);
            //Invoke("PlayMusic", clipToPlay.length);       //改用Loop
        }
        else
            Debug.Log("this scene no music");
    }
}

[System.Serializable]
public class SceneMusic {
    public string sceneName = "";
    public AudioClip musicTheme;
}
