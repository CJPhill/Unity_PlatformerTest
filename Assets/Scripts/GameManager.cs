using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : Singleton<GameManager>
{

    private PlayerController player;
    [SerializeField]
    private InputActionReference quit;
    [SerializeField]
    private GameObject winMenu;
    [SerializeField]
    private Transform firstRespawn;
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject infoMenu;

    public RoomManager roomManager;


    private void Start()
    {
        player = GameObject.FindObjectOfType<PlayerController>();
        HideWin();
        HidePause();
        ShowInfoMenu();

    }

    public void ShowWin()
    {
        winMenu.SetActive(true);
        Time.timeScale = 0.0f;
    }

    private void HideWin()
    {
        winMenu.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void ShowPause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void HidePause()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void ShowInfoMenu()
    {
        infoMenu.SetActive(true);
        Time.timeScale = 0.0f;
        pauseMenu.SetActive(false);
    }

    public void HideInfoMenu()
    {
        infoMenu.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void Quit()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #endif
        Application.Quit();
    }

    public void OnResetBtnClick()
    {
        ResetLevel();
    }

    public void ResetLevel()
    {
        playerTransform.position = firstRespawn.position;
        HideWin() ;
    }

    public void OnResumeBtnClick()
    {
        HidePause();
    }

    public void OnQuitBtnClick()
    {
        Quit();
    }

    public void OnInfoExitBtnClick()
    {
        HideInfoMenu();
    }


}
