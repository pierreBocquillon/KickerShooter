using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Image fadePlane;

    public GameObject gameOverUI;
    public GameObject pauseUI;
    public GameObject winUI;
    public GameObject gameUI;
    public Text ennemiesCounter;
    public Text wavesCounter;
    public Text ammoCounter;
    public Text magCounter;
    public Text reloadInfo;
    public string reloadText;
    public Transform livesDisplay;
    Transform[] livesList;

    public Color lightLifeColor;
    public Color darkLifeColor;

    public bool gamePaused = false;

    public AudioController audioController;
    public float audioFadeRate;
    public float audioFadeDuration;
    float initialMusicVolume;

    SpawnManager spawnManager;
    Player player;
    GunController gunController;

    bool gameIsRunning = true;

    void Start()
    {
        Time.timeScale = 1;

        spawnManager = FindObjectOfType<SpawnManager>();
        player = FindObjectOfType<Player>();
        gunController = FindObjectOfType<GunController>();

        player.OnDeath += OnGameOver;

        livesList = new Transform[livesDisplay.childCount];

        for(int i = 0; i< livesDisplay.childCount; i++){
            livesList[i] = livesDisplay.GetChild(i);
        }

        initialMusicVolume = audioController.GetMusicVolume();
    }

    void Update(){
        if(gameIsRunning){
            ennemiesCounter.text = "Enemies left : " + spawnManager.enemiesRemainingAlive.ToString();
            wavesCounter.text = "Wave : " + spawnManager.GetWaveNumber();

            for(int i = 0; i < livesList.Length; i++){
                if(i <= player.health -1){
                    livesList[i].GetComponent<Image>().color = lightLifeColor;
                }else{
                    livesList[i].GetComponent<Image>().color = darkLifeColor;
                }
            }

            if(gunController.guns[gunController.equippedGunIndex].infinitMag){
                magCounter.text = "--";
            }else{
                magCounter.text = gunController.guns[gunController.equippedGunIndex].MagAmount.ToString();
            }

            if(gunController.reloading){
                reloadInfo.text = reloadText;
            }else{
                reloadInfo.text = "";
            }

            string tmpStr = "";
            for(int i = 0; i < gunController.guns[gunController.equippedGunIndex].currentMagAmmo; i++){
                tmpStr += "i";
            }
            ammoCounter.text = tmpStr;
        }

        if(Input.GetKeyDown(KeyCode.Escape)){
            if(gamePaused){
                OnUnPause();
            }else{
                OnPause();
            }
            gamePaused = !gamePaused;
        }
    }

    void OnGameOver()
    {
        gameIsRunning = false;
        gameUI.SetActive(false);
        StartCoroutine(Fade( Color.clear, Color.black, 1));
        gameOverUI.SetActive(true);
        audioController.SetMusicVolume(initialMusicVolume * audioFadeRate);
        Time.timeScale = 0;
    }

    public void OnGameEnd(){
        gameIsRunning = false;
        gameUI.SetActive(false);
        StartCoroutine(Fade( Color.clear, Color.black, 1));
        winUI.SetActive(true);
        audioController.SetMusicVolume(initialMusicVolume * audioFadeRate);
        Time.timeScale = 0;
    }

    void OnPause()
    {
        gameIsRunning = false;
        gameUI.SetActive(false);
        pauseUI.SetActive(true);
        audioController.SetMusicVolume(initialMusicVolume * audioFadeRate);
        Time.timeScale = 0;
    }

    void OnUnPause()
    {
        gameIsRunning = true;
        pauseUI.SetActive(false);
        gameUI.SetActive(true);
        audioController.SetMusicVolume(initialMusicVolume);
        Time.timeScale = 1;
    }

    IEnumerator Fade(Color from, Color to, float time){
        float speed = 1/ time;
        float percent = 0;

        while(percent < 1){
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    // UI Input
    public void StartNewGame(){
        gameIsRunning = true;
        Time.timeScale = 1;
        SceneManager.LoadScene("Game");
    }

    public void GoToMainMenu()
    {
        gameIsRunning = true;
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}
