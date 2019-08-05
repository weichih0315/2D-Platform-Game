using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{

    public enum AudioChannel { Master, Sfx, Music };

    public float masterVolumePercent { get; private set; }
    public float sfxVolumePercent { get; private set; }
    public float musicVolumePercent { get; private set; }

    AudioSource sfx2DSource;
    AudioSource[] musicSources;
    int activeMusicSourceIndex;

    public static AudioManager instance;

    Transform audioListener;

    SoundLibrary library;

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

            library = GetComponent<SoundLibrary>();

            musicSources = new AudioSource[2];
            for (int i = 0; i < 2; i++)
            {
                GameObject newMusicSource = new GameObject("Music source " + (i + 1));
                musicSources[i] = newMusicSource.AddComponent<AudioSource>();
                musicSources[i].loop = true;
                newMusicSource.transform.parent = transform;
            }
            GameObject newSfx2Dsource = new GameObject("2D sfx source");
            sfx2DSource = newSfx2Dsource.AddComponent<AudioSource>();
            newSfx2Dsource.transform.parent = transform;

            audioListener = FindObjectOfType<AudioListener>().transform;
        }
    }

    private void Start()
    {
        AudioSetting audioSetting = GameDataManager.instance.gameData.optionData.audioSetting;
        masterVolumePercent = audioSetting.masterVolumePercent;
        musicVolumePercent = audioSetting.musicVolumePercent;
        sfxVolumePercent = audioSetting.sfxVolumePercent;
    }

    void Update()
    {
        /*if (FindObjectOfType<Player>() != null)
        {
            audioListener.position = FindObjectOfType<Player>().transform.position;
        }*/
    }

    public void SetVolume(float volumePercent, AudioChannel channel)
    {
        AudioSetting audioSetting = GameDataManager.instance.gameData.optionData.audioSetting;

        if (channel == AudioChannel.Master)
        {
            masterVolumePercent = volumePercent;
            audioSetting.masterVolumePercent = masterVolumePercent;
        }
        else if (channel == AudioChannel.Music)
        {
            musicVolumePercent = volumePercent;
            audioSetting.musicVolumePercent = musicVolumePercent;
        }
        else if (channel == AudioChannel.Sfx)
        {
            sfxVolumePercent = volumePercent;
            audioSetting.sfxVolumePercent = sfxVolumePercent;
        }

        musicSources[0].volume = musicVolumePercent * masterVolumePercent;
        musicSources[1].volume = musicVolumePercent * masterVolumePercent;        

        audioSetting.Save();
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 1)
    {
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        musicSources[activeMusicSourceIndex].clip = clip;
        musicSources[activeMusicSourceIndex].Play();

        StartCoroutine(AnimateMusicCrossfade(fadeDuration));
    }

    public void Play()
    {
        for (int index = 0; index < 2; index++)
            musicSources[index].Play();
    }

    public void Pause()
    {
        for(int index = 0; index < 2; index++)
            musicSources[index].Pause();
    }

    public void Stop()
    {
        for (int index = 0; index < 2; index++)
            musicSources[index].Stop();
    }

    public void PlaySound(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent * masterVolumePercent);
        }
    }

    public void PlaySound(string soundName, Vector3 pos)
    {
        PlaySound(library.GetClipFromName(soundName), pos);
    }

    public void PlaySound2D(string soundName)
    {
        sfx2DSource.PlayOneShot(library.GetClipFromName(soundName), sfxVolumePercent * masterVolumePercent);
    }

    public void PlaySound2D(AudioClip audioClip)
    {
        sfx2DSource.PlayOneShot(audioClip, sfxVolumePercent * masterVolumePercent);
    }

    IEnumerator AnimateMusicCrossfade(float duration)
    {
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / duration;
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVolumePercent * masterVolumePercent, percent);
            musicSources[1 - activeMusicSourceIndex].volume = Mathf.Lerp(musicVolumePercent * masterVolumePercent, 0, percent);
            yield return null;
        }
        musicSources[1 - activeMusicSourceIndex].clip = null;
    }

    private void OnEnable()
    {
        GameManager.OnGameOverStatic += GameOver;
    }

    private void OnDisable()
    {
        GameManager.OnGameOverStatic -= GameOver;
    }

    private void GameOver()
    {
        Stop();
    }
}
