using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {

    [Header("Main")]
    public Text lifeUI;
    public Text scoreUI;
    public Text moneyUI;

    [Header("Option")]
    public GameObject optionMenuUI;
    public Slider[] volumeSliders;

    [Header("GameOver")]
    public GameObject gameOverUI;
    public Text gameoverScoreUI;

    public static GameUI instance;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        Player.OnPlayerHurt += PlayerOnHurt;
        GameManager.OnGameOverStatic += GameOver;
    }

    private void OnDisable()
    {
        Player.OnPlayerHurt -= PlayerOnHurt;
        GameManager.OnGameOverStatic -= GameOver;
    }

    void Start()
    {
        UpdateVolume();
        UpdateScore();
        UpdateMoney();
    }
    
    public void Home()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Option()
    {
        AudioManager.instance.PlaySound2D("MenuClick");
        optionMenuUI.SetActive(!optionMenuUI.activeSelf);
    }

    public void PlayerOnHurt()
    {
        UpdateLife();
    }

    public void GameOver()
    {
        gameOverUI.SetActive(true);
    }

    public void UpdateVolume()
    {
        AudioSetting audioSetting = GameDataManager.instance.gameData.optionData.audioSetting;
        volumeSliders[0].value = audioSetting.masterVolumePercent;
        volumeSliders[1].value = audioSetting.musicVolumePercent;
        volumeSliders[2].value = audioSetting.sfxVolumePercent;
    }

    public void UpdateLife()
    {
        lifeUI.text = "Life : " + Player.instance.health;
    }

    public void UpdateScore()
    {
        scoreUI.text = "Score : " + GameManager.instance.score;
    }   

    public void UpdateMoney()
    {
        moneyUI.text = "x : " + GameManager.instance.money;
    }

    public void Restart()
    {
        AudioManager.instance.PlaySound2D("MenuClick");
        gameOverUI.SetActive(false);
        AudioManager.instance.Play();
        GameManager.instance.PlayerSpawn();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetMasterVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Master);
    }

    public void SetMusicVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Music);
    }

    public void SetSfxVolume(float value)
    {
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Sfx);
    }
}
